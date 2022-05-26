using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public TextMeshProUGUI scoreTxt;

    public void ResetLevel(){
        OnClose();
        LevelManager.Instance.LoadMapLevel();
        PlayerController.Instance.ResetPlayerState();
    }

    public void NextLevel(){
        OnClose();
        int nextLevel = LevelManager.Instance.selectedLevel + 1;
        LevelManager.Instance.selectedLevel = nextLevel > LevelManager.Instance.prefabLevelList.Count-1 ? 0 : nextLevel;
        LevelManager.Instance.LoadMapLevel();
        PlayerController.Instance.ResetPlayerState();
    }
}
