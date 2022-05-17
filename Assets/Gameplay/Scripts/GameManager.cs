using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    public GameObject levelHolder;

    private void Start() {
        LoadMapLevel();
    }

    public void LoadMapLevel()
    {   
        int count = levelHolder.transform.childCount;
        if (count != 0)
        {
            for(int i = 0; i < count; i++)
            {
                Transform item = levelHolder.transform.GetChild(i);
                GameObject.Destroy(item.gameObject);
            }
        }
        try
        {
            // TODO: fix about string level
            GameObject currentLevel = Instantiate(LevelManager.Instance.prefabLevelList[LevelManager.Instance.selectedLevel]);
            currentLevel.transform.parent = levelHolder.transform;
            Debug.Log("Generate levelsuccess");
        }
        catch (System.Exception)
        {
            Debug.Log("Can't generate level ");
        }
    }
}
