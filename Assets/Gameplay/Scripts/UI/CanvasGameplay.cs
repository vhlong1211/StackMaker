using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGameplay : UICanvas
{
    private static CanvasGameplay instance;

    public static CanvasGameplay Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CanvasGameplay>();
            }

            return instance;
        }
    }
}
