using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;

    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MapManager>();
            }

            return instance;
        }
    }

    public Transform winPosMiddle;
    public Transform closeChest;
    public Transform openChest;
    public Transform openChestPlace;
}
