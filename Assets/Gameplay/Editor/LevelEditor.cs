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
        GUIContent levelLabel = new GUIContent("Selected Level");
        List<string> levelNameList = new List<string>();
        foreach (GameObject level in levelManager.prefabLevelList)
        {
            levelNameList.Add(level.name);
        }
        levelManager.selectedLevel = EditorGUILayout.Popup(levelLabel,levelManager.selectedLevel,levelNameList.ToArray());
    }
}
