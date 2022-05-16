using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIName {GamePlay}

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }

            return instance;
        }
    }


    public Transform parent;
    public Dictionary<UIName, UICanvas> canvasPrefabs = new Dictionary<UIName, UICanvas>();
    public Dictionary<UIName, UICanvas> canvasManagers = new Dictionary<UIName, UICanvas>();

    private void Awake()
    {

        UICanvas[] canvas = Resources.LoadAll<UICanvas>("UI/");

        for (int i = 0; i < canvas.Length; i++)
        {
            canvasPrefabs.Add(canvas[i].nameUI, canvas[i]);
        }

        Debug.Log(canvas.Length);
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
    }

    public UICanvas OpenUI(UIName name)
    {
        //check neu khong co trong list thi phai khoi tao
        //neu co trong list
        //1.OnInit
        //2.OnOpen

        UICanvas canvas = null;

        if (!canvasManagers.ContainsKey(name) || canvasManagers[name] == null)
        {
            canvas = Instantiate(canvasPrefabs[name], parent);
            canvasManagers.Add(name, canvas);
        }
        else
        {
            canvas = canvasPrefabs[name];
        }

        canvas.OnInit();
        canvas.OnOpen();

        return canvas;
    }

    public void CloseUI(UIName name)
    {
        //check neu khong co trong list thi thoi
        //neu co trong list
        //2.OnClose
    }

    public bool IsOpened(UIName name)
    {
        //check neu khong co trong list thi false
        //neu co trong list
        //check xem no co dang bat hay khong

        return true;
    }

    public UICanvas GetUI(UIName name)
    {
        //check trong list khong co thi tra ve null
        //neu co thi tra ve UI
        return null;
    }

}
