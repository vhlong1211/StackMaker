using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

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
    
    //Building prefabs
    public Transform prefabBC;
    public Transform prefabUnwalkable;
    public Transform prefabWalkable;
    public Transform prefabBridge;
    public Transform prefabWinpos;

    //Handle End Prefab
    [HideInInspector]
    public Transform winPosMiddle;
    [HideInInspector]
    public Transform closeChest;
    [HideInInspector]
    public Transform openChest;
    [HideInInspector]
    public Transform openChestPlace;

    public Transform newPrefabLevel;
    //public List<GameObject> prefabLevelList;
    public List<String> levelDataPathList;
    [HideInInspector]
    public int selectedLevel = 0;

    public void SaveLevel(){
        if(newPrefabLevel == null) return;
        Debug.Log("Saved");

        //string localPrefabPath = "Assets/Gameplay/Resources/Levels/" + "Level" + ".prefab";
        //localPrefabPath = AssetDatabase.GenerateUniqueAssetPath(localPrefabPath);
        //PrefabUtility.SaveAsPrefabAsset(newPrefabLevel.gameObject, localPrefabPath);

        string locaDatalPath = "Assets/Gameplay/Resources/LevelData/" + "Level" + ".txt";
        locaDatalPath = AssetDatabase.GenerateUniqueAssetPath(locaDatalPath);
        if(!File.Exists(locaDatalPath)){
            levelDataPathList.Add(locaDatalPath);
            File.WriteAllText(locaDatalPath,"");
        }else{
            File.WriteAllText(locaDatalPath,"");
        }
        for(int i = 0 ; i < newPrefabLevel.childCount; i++){
            Transform tempOb = newPrefabLevel.GetChild(i);
            string content = tempOb.tag + "/" + tempOb.position.x + "/" + tempOb.position.y +"/" + tempOb.position.z +"/"+tempOb.localRotation.eulerAngles.x+"/"+tempOb.localRotation.eulerAngles.y+"/"+tempOb.localRotation.eulerAngles.z +"\n";
            File.AppendAllText(locaDatalPath,content);
            Debug.Log(newPrefabLevel.GetChild(i).name);
        }        
    }

    public void LoadMapLevel(){
        Transform levelHolder = GameManager.Instance.levelHolder;
        int count = levelHolder.transform.childCount;
        if (count != 0)
        {
            for (int i = 0; i < count; i++)
            {
                Transform item = levelHolder.transform.GetChild(0);
                if(item!=null) DestroyImmediate(item.gameObject);

            }
        }
        try
        {
            // TODO: fix about string level
            // GameObject currentLevel = Instantiate(prefabLevelList[selectedLevel]);
            // currentLevel.transform.parent = levelHolder.transform;
            // Debug.Log("Generate levelsuccess");
            string filePath = levelDataPathList[selectedLevel];
            List<string> fileLines = File.ReadAllLines(filePath).ToList();
            foreach(string line in fileLines){
                string[] spliterArr = line.Split('/');
                if(TagToPrefabType(spliterArr[0]) != null){
                    Transform currentPiece = Instantiate(TagToPrefabType(spliterArr[0]),levelHolder);
                    if(spliterArr[0] == TagUtility.TAG_WINPOS){
                        winPosMiddle = currentPiece.Find(TagUtility.NAME_MID_WINPOS).transform;
                        closeChest = currentPiece.Find(TagUtility.NAME_CLOSE_CHEST).transform;
                        openChest = currentPiece.Find(TagUtility.NAME_OPEN_CHEST).transform;
                        openChestPlace = currentPiece.Find(TagUtility.NAME_OPEN_CHEST_PLACE).transform;
                    }
                    currentPiece.position = new Vector3(float.Parse(spliterArr[1]),float.Parse(spliterArr[2]),float.Parse(spliterArr[3]));
                    currentPiece.eulerAngles = new Vector3(float.Parse(spliterArr[4]),float.Parse(spliterArr[5]),float.Parse(spliterArr[6]));
                }
            }
        }
        catch (System.Exception)
        {
            Debug.Log("Can't generate level ");
        }
    }

    public void EditLevel() {
        string locaDatalPath = "Assets/Gameplay/Resources/LevelData/Level " + selectedLevel + ".txt";
        File.WriteAllText(locaDatalPath, "");
        for (int i = 0; i < newPrefabLevel.childCount; i++)
        {
            Transform tempOb = newPrefabLevel.GetChild(i);
            string content = tempOb.tag + "/" + tempOb.position.x + "/" + tempOb.position.y + "/" + tempOb.position.z + "/" + tempOb.localRotation.eulerAngles.x + "/" + tempOb.localRotation.eulerAngles.y + "/" + tempOb.localRotation.eulerAngles.z + "\n";
            File.AppendAllText(locaDatalPath, content);
            Debug.Log(newPrefabLevel.GetChild(i).name);
        }
    }

    public void DeleteLevel() {
        string localDatalPath = "Assets/Gameplay/Resources/LevelData/Level " + selectedLevel + ".txt";
        File.Delete(localDatalPath);
        levelDataPathList.RemoveAt(selectedLevel);
    }

    public void ResetLevel(){
        DestroyImmediate(newPrefabLevel.gameObject);
        newPrefabLevel = null;
    }

    public Transform TagToPrefabType(string tag){

        Transform returnType = null;
        if(tag == TagUtility.TAG_UNWALKABLE){
            returnType = prefabUnwalkable;
        }else if(tag == TagUtility.TAG_WALKABLE){
            returnType = prefabWalkable;
        }else if(tag == TagUtility.TAG_BRIDGE){
            returnType = prefabBridge;
        }else if(tag == TagUtility.TAG_BC){
            returnType = prefabBC;
        }else if(tag == TagUtility.TAG_WINPOS){
            returnType = prefabWinpos;
        }

        return returnType;
    } 
}
