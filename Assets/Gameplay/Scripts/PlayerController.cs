using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    public Stack<GameObject> brickStack ;
    public Transform brickStackTransform;
    public Transform map;
    public Transform playerModel;
    public GameObject brickPrefab;
    public Transform winPosMiddle;
    public Transform closeChest;
    public Transform openChest;
    public Transform openChestPlace;
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

    // Start is called before the first frame update
    void Start()
    {
        brickStack = new Stack<GameObject>();
    }

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
            if(directVector.magnitude == 0) return;
            float angleDiff = Vector2.Angle(xAxis,directVector);
            currentDirection = GetDirectionFromAngle(angleDiff);
        }
    }

    private Direction GetDirectionFromAngle(float angle){
        if(angle >= 135){
            return Direction.Left;
        }else if(angle <= 45 ){
            return Direction.Right;
        }
        float heightDiff = mouseUpPos.y - mouseDownPos.y;
        if(heightDiff > 0){
            return Direction.Up;
        }else if(heightDiff < 0){
            return Direction.Down;
        }else{
            return Direction.None;
        }
    }

    private void HandleMovement(){
        if(currentDirection == Direction.None)  return;
        if(isMoving){
            transform.position = Vector3.MoveTowards(transform.position , targetCell , speed * Time.deltaTime);
            //Check close to target
            float offset = 0.1f;
            if(Vector3.Distance(transform.position,targetCell) < offset){
                transform.position = targetCell;
                FindNextCell();
            }
        }else{
            FindNextCell();
        }
    }

    private int[] DirectionToOffset(Direction direction){
        if(direction == Direction.Up){
            int[] arrayDir = new int[]{0,1};
            return arrayDir;
        }else if(direction == Direction.Right){
            int[] arrayDir = new int[]{1,0};
            return arrayDir;
        }else if(direction == Direction.Down){
            int[] arrayDir = new int[]{0,-1};
            return arrayDir;
        }else if(direction == Direction.Left){
            int[] arrayDir = new int[]{-1,0};
            return arrayDir;
        }
        return null;
    }

    private void CheckNewCell(Transform hit , Vector3 newPos){
        if( hit.CompareTag("Unwalkable")){
            isMoving = false;
            currentDirection = Direction.None;
        }else if( hit.CompareTag("Brick")){
            isMoving = true;
            targetCell = newPos;
            Destroy(hit.gameObject);
            GetNewBrick();
        }else if( hit.CompareTag("Walkable")){
            isMoving = true;
            targetCell = newPos;
        }else if( hit.CompareTag("Bridge")){
            if(brickStack.Count > 0){
                isMoving = true;
                targetCell = newPos;
                UseBrick(newPos);
            }else{
                isMoving = false;
                currentDirection = Direction.None;
            }
        }else if( hit.CompareTag("Winpos")){
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
        brick.transform.position = new Vector3(newPos.x , transform.position.y - brickHeight  ,newPos.z);
        brick.tag = "Walkable";
        AdjustBrickChange();
    }

    private Vector3 GetNewPos(){
        int[] tempDirectionArray = DirectionToOffset(currentDirection);
        Vector3 newPos = transform.position;
        newPos.x += tempDirectionArray[0];
        newPos.z += tempDirectionArray[1];
        return newPos;
    }

    private void HandleWin(){
        if(!didWin) return;
        Vector3 destination2 = new Vector3(openChestPlace.position.x,transform.position.y,openChestPlace.position.z);
        Vector3 destination1 = new Vector3(winPosMiddle.position.x,transform.position.y,winPosMiddle.position.z);
        transform.position = Vector3.MoveTowards(transform.position , destination2 , speed/2 * Time.deltaTime);
        if(winPhase == 0 & Vector3.Distance(transform.position,destination1) < 0.1f){
            winPhase ++;
        }
        if(winPhase == 1 & Vector3.Distance(transform.position,destination2) < 0.1f){
            winPhase ++;
        }
        if(winPhase == 1){
            //Change anim and hide brick ,shoot particle
            brickCount = brickStack.Count;
            while(brickStack.Count > 0){
                brickStack.Pop().SetActive(false);

            }

            AdjustBrickChange();
            playerModel.transform.rotation = Quaternion.Euler(0,0,0);
            playerAnimator.SetBool("didWin",true);

        }
        if(winPhase == 2){
            //Change chest model
            closeChest.gameObject.SetActive(false);
            openChest.gameObject.SetActive(true);
            UIManager.Instance.OpenUI(UIName.GamePlay);
        }
    }
}

public enum Direction{
    Up,
    Down,
    Left,
    Right,
    None
}