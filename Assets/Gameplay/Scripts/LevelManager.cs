using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{  
    private static LevelManager instance;

    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelManager>();
            }

            return instance;
        }
    }

    public Transform newPrefabLevel;
    public List<GameObject> prefabLevelList;
    [HideInInspector]
    public int selectedLevel = 0;
    // Start is called before the first frame update
    public void SaveLevel(){
        if(newPrefabLevel == null) return;
        Debug.Log("Saved");
        string localPath = "Assets/Resources/Levels/" + "Level" + ".prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        PrefabUtility.SaveAsPrefabAsset(newPrefabLevel.gameObject, localPath);
    }

    public void ResetLevel(){
        DestroyImmediate(newPrefabLevel.gameObject);
        newPrefabLevel = null;
    }
}
