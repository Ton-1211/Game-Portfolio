using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MusicalScoreEditor : EditorWindow
{
    [MenuItem("MyTool/譜面エディター")]// それぞれのメニュー名
    static void Open()
    {
        EditorWindow.GetWindow<MusicalScoreEditor>("譜面エディター");// エディタ拡張ウィンドウのタイトル
    }

    Object midiFile;
    string path;

    void OnGUI()
    {
        GUIStyle descriptionStyle = new GUIStyle(EditorStyles.label);
        descriptionStyle.fontSize = 16;
        descriptionStyle.fontStyle = FontStyle.Bold;

        using(new GUILayout.VerticalScope())
        {
            EditorGUILayout.LabelField("譜面作成画面", descriptionStyle);
            EditorGUILayout.Space();

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                midiFile = EditorGUILayout.ObjectField("Midiファイル", midiFile, typeof(Object), true);
                if(change.changed)
                {
                    path = AssetDatabase.GetAssetPath(midiFile);
                }
            }

            if(GUILayout.Button("Midiファイルを読み込み"))
            {
                MidiLoader loader = new MidiLoader();
                loader.LoadMIDI(path);
            }
        }
    }
}
