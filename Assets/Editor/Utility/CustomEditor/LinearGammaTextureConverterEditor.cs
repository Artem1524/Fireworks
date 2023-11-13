using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LinearGammaTextureConverter))]
// [CanEditMultipleObjects]
public class LinearGammaTextureConverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LinearGammaTextureConverter myScript = (LinearGammaTextureConverter)target;

        if (GUILayout.Button("Convert"))
            myScript.ConvertTextureAndSaveToFile();
    }
}
