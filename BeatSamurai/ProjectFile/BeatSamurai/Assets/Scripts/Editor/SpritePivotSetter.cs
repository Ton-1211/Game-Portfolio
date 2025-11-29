using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpritePivotSetter : EditorWindow
{
    //[MenuItem("Butadon'sTool/一括pivot設定")]
    static void Open()
    {
        EditorWindow.GetWindow<SpritePivotSetter>("一括pivot設定");
    }

    Texture2D texture = null;
    float x = 0f;
    float y = 0f;
    SpriteSliceEditClass spriteSliceEdit = new SpriteSliceEditClass();

    private void OnGUI()
    {
        EditorGUILayout.LabelField("指定した場所にpivotをスプライトのスライスすべてに一括で設定します");

        texture = (Texture2D)EditorGUILayout.ObjectField("Texture", texture, typeof(Texture2D), true);

        EditorGUILayout.LabelField("pivotの位置設定");
        x = EditorGUILayout.Slider("x", x, 0f, 1f);// 0～1の範囲に制限
        y = EditorGUILayout.Slider("y", y, 0f, 1f);

        if (GUILayout.Button("pivotを設定"))
        {
            SettingPivot(texture);
        }
    }

    private void SettingPivot(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

        spriteSliceEdit.SetPivotsAll(path, ti, x, y);
    }
}
