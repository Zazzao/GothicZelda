using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    SerializedProperty itemNameProp;
    SerializedProperty iconProp;
    SerializedProperty descriptionProp;
    SerializedProperty isStackableProp;
    //SerializedProperty valueProp;
    //SerializedProperty stackableProp;

    private void OnEnable()
    {
        itemNameProp = serializedObject.FindProperty("itemName");
        iconProp = serializedObject.FindProperty("icon");
        descriptionProp = serializedObject.FindProperty("description");
        isStackableProp = serializedObject.FindProperty("isStackable");
        
        
        //valueProp = serializedObject.FindProperty("value");
        //stackableProp = serializedObject.FindProperty("stackable");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(itemNameProp);
        EditorGUILayout.PropertyField(iconProp);
        EditorGUILayout.PropertyField (descriptionProp);
        EditorGUILayout.PropertyField(isStackableProp);
        //EditorGUILayout.PropertyField(valueProp);
        //EditorGUILayout.PropertyField(stackableProp);

        GUILayout.Space(10);

        Sprite sprite = iconProp.objectReferenceValue as Sprite;

        if (sprite != null)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(sprite);

            if (texture != null)
            {
                GUILayout.Label(texture,
                    GUILayout.Width(64),
                    GUILayout.Height(64));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
