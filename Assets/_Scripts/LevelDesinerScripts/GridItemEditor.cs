#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridItem))]
public class GridItemEditor : Editor
{
    private string letter = "";
    

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GridItem gridItem = (GridItem)target;

        letter = EditorGUILayout.TextField("Letter", letter);
        if (GUILayout.Button("Set Letter"))
        {
            gridItem.SetLetter(letter);
            EditorUtility.SetDirty(gridItem);
            letter = "";
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif