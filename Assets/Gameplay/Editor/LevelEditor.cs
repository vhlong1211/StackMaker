using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LevelManager)), CanEditMultipleObjects]
public class LevelEditor : Editor
{   
   
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LevelManager levelManager = (LevelManager)target;
        if(GUILayout.Button("New Level")){
            levelManager.ResetLevel();
        }
        if(GUILayout.Button("Save Level")){
            //PrefabUtility.SaveAsPrefabAsset(prefabLevel, localPath, out prefabSuccess);
            levelManager.SaveLevel();
        }
        if (GUILayout.Button("Load Level"))
        {
            //PrefabUtility.SaveAsPrefabAsset(prefabLevel, localPath, out prefabSuccess);
            LevelManager.Instance.LoadMapLevel();
        }
        if (GUILayout.Button("Edit Level"))
        {
            //PrefabUtility.SaveAsPrefabAsset(prefabLevel, localPath, out prefabSuccess);
            LevelManager.Instance.EditLevel();
        }
        GUIContent levelLabel = new GUIContent("Selected Level");
        List<string> levelNameList = new List<string>();
        foreach (string level in levelManager.levelDataPathList)
        {
            string[] strPart = level.Split('/');
            levelNameList.Add(strPart[strPart.Length-1]);
        }
        levelManager.selectedLevel = EditorGUILayout.Popup(levelLabel,levelManager.selectedLevel,levelNameList.ToArray());
    }
}
