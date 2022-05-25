using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    public Stack<GameObject> brickStack ;
    public Transform brickStackTransform;
    public Transform map;
    public Transform playerModel;
    public Transform tf;
    public GameObject brickPrefab;
    // private Transform winPosMiddle;
    // private Transform closeChest;
    // private Transform openChest;
    // private Transform openChestPlace;
    private Vector3 playerOriginPos;
    public Animator playerAnimator;

    private Vector3 mouseDownPos;
    private Vector3 mouseUpPos;
    private bool didClick = false;
    private Direction currentDirection = Direction.None;
    private float cellSize = 1f;
    private float brickHeight = 0.3f;
    [SerializeField] private float speed = 50f;
    private bool isMoving = false;
    private bool didWin = false;
    private int winPhase = 0;
    private Vector3 targetCell;
    private int brickCount;

    private static PlayerController instance;

    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerController>();
            }

            return instance;
        }
    }

    private void Awake() {
        tf = transform;
    }

    // Start is called before the first frame update
    void Start()
    {   
        playerOriginPos = tf.position;
        brickStack = new Stack<GameObject>();
        //Init();
    }

    //Không cache những biến đã bị destroy cùng gameobject được nên tạm thời comment lại?
    // void Init(){
    //     Debug.Log("eohehl:"+MapManager.Instance);
    //     //Debug.Log("Win pos middle 0:"+winPosMiddle);
    //     winPosMiddle = null;
    //     //Debug.Log("Win pos middle 1:"+winPosMiddle);
    //     winPosMiddle = MapManager.Instance.winPosMiddle;
    //     //Debug.Log("Map manager:"+MapManager.Instance.winPosMiddle);
    //     //Debug.Log("Win pos middle 2:"+winPosMiddle);
    //     closeChest = null;
    //     closeChest = MapManager.Instance.closeChest;
    //     openChest = null;
    //     openChest = MapManager.Instance.openChest;
    //     Debug.Log("open chest place 0:"+openChestPlace);
    //     openChestPlace = null;
    //     Debug.Log("open chest place 1:"+openChestPlace);
    //     openChestPlace = MapManager.Instance.openChestPlace;
    //     Debug.Log("manager chest :"+MapManager.Instance.openChestPlace);
    //     Debug.Log("open chest place 2:"+openChestPlace);
    // }

    // Update is called once per frame
    void Update()
    {
        GetMoveDirection();
        HandleMovement();
        HandleWin();
    }

    private void GetMoveDirection(){
        if(isMoving)    return;
        if(Input.GetMouseButtonDown (0)){
            mouseDownPos = Input.mousePosition;
            didClick = true;
        }
        if(Input.GetMouseButtonUp(0)){
            if(!didClick)   return;
            didClick = false;
            mouseUpPos = Input.mousePosition;
            Vector2 xAxis = new Vector2(1,0);
            Vector2 directVector = new Vector2(mouseUpPos.x - mouseDownPos.x,mouseUpPos.y - mouseDownPos.y);
            if(directVector.sqrMagnitude == 0) return;
            float angleDiff = Vector2.Angle(xAxis,directVector);
            currentDirection = GetDirectionFromAngle(angleDiff);
        }
    }

    private Direction GetDirectionFromAngle(float angle){

        Direction direction = Direction.None;

        if(angle >= 135){
            direction = Direction.Left;
        }else if(angle <= 45 ){
            direction = Direction.Right;
        }
        if(direction != Direction.None){
            return direction;
        }

        float heightDiff = mouseUpPos.y - mouseDownPos.y;
        if(heightDiff > 0){
            direction = Direction.Up;
        }else if(heightDiff < 0){
            direction = Direction.Down;
        }

        return direction;
    }

    private void HandleMovement(){
        if(currentDirection == Direction.None)  return;
        if(isMoving){
            tf.position = Vector3.MoveTowards(tf.position , targetCell , speed * Time.deltaTime);
            //Check close to target
            float offset = 0.1f;
            if((tf.position-targetCell).sqrMagnitude < offset * offset){
                tf.position = targetCell;
                FindNextCell();
            }
        }else{
            FindNextCell();
        }
    }

    private int[] DirectionToOffset(Direction direction){

        int[] arrayDir = null;

        if(direction == Direction.Up){
            arrayDir = new int[]{0,1};
        }else if(direction == Direction.Right){
            arrayDir = new int[]{1,0};
        }else if(direction == Direction.Down){
            arrayDir = new int[]{0,-1};
        }else if(direction == Direction.Left){
            arrayDir = new int[]{-1,0};
        }
        
        return arrayDir;
    }

    private void CheckNewCell(Transform hit , Vector3 newPos){
        if( hit.CompareTag(TagUtility.TAG_UNWALKABLE)){
            isMoving = false;
            currentDirection = Direction.None;
        }else if( hit.CompareTag(TagUtility.TAG_BRICK)){
            isMoving = true;
            targetCell = newPos;
            Destroy(hit.gameObject);
            GetNewBrick();
        }else if( hit.CompareTag(TagUtility.TAG_WALKABLE)){
            isMoving = true;
            targetCell = newPos;
        }else if( hit.CompareTag(TagUtility.TAG_BRIDGE)){
            if(brickStack.Count > 0){
                isMoving = true;
                targetCell = newPos;
                UseBrick(newPos);
            }else{
                isMoving = false;
                currentDirection = Direction.None;
            }
        }else if( hit.CompareTag(TagUtility.TAG_WINPOS)){
            didWin = true;
            currentDirection = Direction.None;
            isMoving = false;
        }
    }

    private void FindNextCell(){
        Vector3 newPos = GetNewPos();
        Vector3 shootingRaycastPos = newPos;
        shootingRaycastPos.y = 100f;
        RaycastHit hit;
        Physics.Raycast( shootingRaycastPos , -Vector3.up , out hit , Mathf.Infinity);
        CheckNewCell(hit.transform,newPos);
    }

    private void AdjustBrickChange(){
        playerModel.localPosition = new Vector3(playerModel.localPosition.x,0.3f * brickStack.Count,playerModel.localPosition.z);
    }

    private void GetNewBrick(){
        GameObject brick = Instantiate(brickPrefab,brickStackTransform);
        brickStack.Push(brick);
        brick.transform.localPosition = new Vector3(0,(brickStack.Count-1) * brickHeight,0);
        AdjustBrickChange();
    }

    private void UseBrick(Vector3 newPos){
        GameObject brick = brickStack.Pop();
        brick.transform.parent = map;
        brick.transform.position = new Vector3(newPos.x , tf.position.y - brickHeight  ,newPos.z);
        brick.tag = TagUtility.TAG_WALKABLE;
        AdjustBrickChange();
    }

    private Vector3 GetNewPos(){
        int[] tempDirectionArray = DirectionToOffset(currentDirection);
        Vector3 newPos = tf.position;
        newPos.x += tempDirectionArray[0];
        newPos.z += tempDirectionArray[1];
        return newPos;
    }

    private void HandleWin(){
        if(!didWin) return;

        Transform openChestPlace = MapManager.Instance.openChestPlace;
        Transform winPosMiddle = MapManager.Instance.winPosMiddle;
        Transform closeChest = MapManager.Instance.closeChest;
        Transform openChest = MapManager.Instance.openChest;

        //Debug.Log(openChestPlace+"----"+MapManager.Instance.openChestPlace);
        Vector3 destination2 = new Vector3(openChestPlace.position.x,tf.position.y,openChestPlace.position.z);
        Vector3 destination1 = new Vector3(winPosMiddle.position.x,tf.position.y,winPosMiddle.position.z);
        tf.position = Vector3.MoveTowards(tf.position , destination2 , speed/2 * Time.deltaTime);
        if(winPhase == 0 &(tf.position-destination1).sqrMagnitude < 0.1f * 0.1f){
            winPhase ++;
            brickCount = brickStack.Count;
        }
        if(winPhase == 1 & (tf.position-destination2).sqrMagnitude < 0.1f * 0.1f){
            winPhase ++;
        }
        if(winPhase == 1){
            //Change anim and hide brick ,shoot particle
            while(brickStack.Count > 0){
                brickStack.Pop().SetActive(false);

            }

            AdjustBrickChange();
            playerModel.transform.rotation = Quaternion.Euler(0,0,0);
            playerAnimator.SetBool(TagUtility.TAG_ANIM_DID_WIN,true);

        }
        if(winPhase == 2){
            //Change chest model
            closeChest.gameObject.SetActive(false);
            openChest.gameObject.SetActive(true); 
            UIManager.Instance.OpenUI(UIName.GamePlay);
            CanvasGameplay.Instance.scoreTxt.text = brickCount.ToString();
        }
    }

    public void ResetPlayerState(){
        //Init();
        didClick = false;
        currentDirection = Direction.None;
        isMoving = false;
        didWin = false;
        winPhase = 0;
        tf.position = playerOriginPos;
        playerAnimator.SetBool(TagUtility.TAG_ANIM_DID_WIN,false);
        playerModel.transform.rotation = Quaternion.Euler(0,180,0);
    }
}

public enum Direction{
    Up,
    Down,
    Left,
    Right,
    None
}