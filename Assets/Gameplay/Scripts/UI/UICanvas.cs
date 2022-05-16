using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    public UIName nameUI;


    public virtual void OnInit()
    {

    }

    public virtual void OnOpen()
    {
        gameObject.SetActive(true);
    }


    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }

}
