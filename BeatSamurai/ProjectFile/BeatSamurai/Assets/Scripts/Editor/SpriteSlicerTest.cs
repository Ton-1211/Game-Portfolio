using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpriteSlicerTest : EditorWindow
{
    [MenuItem("MyTool/コマ割りスライス")]// それぞれのメニュー名
    static void Open()
    {
        EditorWindow.GetWindow<SpriteSlicerTest>("コマ割りスライス");// エディタ拡張ウィンドウのタイトル
    }

    float elementMaxWidth = 480; 

    Texture2D texture = null;
    bool sliceSizeOptionOpen = true;
    bool textureSizeOptionOpen = true;
    bool pivotPositionOptionOpen = true;
    bool sliceNumOptionOpen = false;

    int horizontalSliceNum = 0;
    int verticalSliceNum = 0;

    int sliceWidth = 0;
    int sliceHeight = 0;

    int textureWidth = 0;
    int textureHeight = 0;

    float zoomNum = 1f;
    float zoomMin = 0.1f;
    float zoomMax = 2f;
    Vector2 sliceSize = Vector2.zero;
    Vector2 textureStartPosition = Vector2.zero;// 画像のどの位置から表示するか(0～1で指定)
    Vector2 textureAspectRate = Vector2.one;// //画像をどれぐらい表示するか(0～1で指定)
    Vector2 previewAdjustment = new Vector2(1.5f, 1f);// プレビューの位置調整用

    Vector2 pivotPosition = new Vector2(0.5f, 0f);

    Vector2 scrollPosition = Vector2.zero;

    /*ウィンドウ表示*/
    private void OnGUI()
    {
        // 他のラベルに影響させないためにコピーしてから変更する
        GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
        titleStyle.fontSize = 16;
        titleStyle.fontStyle = FontStyle.Bold;

        using (new GUILayout.VerticalScope())
        {
            EditorGUILayout.LabelField("テクスチャのインポート設定とスライスをします", titleStyle);// 説明文
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);// スクロール開始
            using (var change = new EditorGUI.ChangeCheckScope())// このスコープ内で値が変更されたか調べる
            {
                texture = (Texture2D)EditorGUILayout.ObjectField("Texture", texture, typeof(Texture2D), true);
                if (change.changed && texture != null)// 値が変更されたとき（nullチェック付き）
                {
                    textureWidth = texture.width;
                    textureHeight = texture.height;
                }
            }

            sliceSizeOptionOpen = EditorGUILayout.Foldout(sliceSizeOptionOpen, "テクスチャのスライスサイズ設定");// 折りたたみ
            if (sliceSizeOptionOpen)// 開いているとき
            {
                using (new EditorGUI.IndentLevelScope(1))// 字下げ
                {
                    sliceNumOptionOpen = EditorGUILayout.Foldout(sliceNumOptionOpen, "分割数から自動で設定");
                    if (sliceNumOptionOpen)
                    {
                        using (new EditorGUI.IndentLevelScope(1))// 字下げ
                        {
                            using (new GUILayout.VerticalScope("Box"))
                            {
                                using (var change = new EditorGUI.ChangeCheckScope())
                                {
                                    EditorGUILayout.LabelField("値を変更するたびに自動でスライス設定を更新");
                                    horizontalSliceNum = EditorGUILayout.IntField("横の分割数", horizontalSliceNum, GUILayout.MaxWidth(elementMaxWidth));
                                    verticalSliceNum = EditorGUILayout.IntField("縦の分割数", verticalSliceNum, GUILayout.MaxWidth(elementMaxWidth));
                                    if (change.changed && horizontalSliceNum != 0 && verticalSliceNum != 0)// 値が変更されたとき（0で割らないようにしている）
                                    {
                                        sliceWidth = textureWidth / horizontalSliceNum;
                                        sliceHeight = textureHeight / verticalSliceNum;
                                    }
                                }
                            }
                        }
                    }
                }

                using (new EditorGUI.IndentLevelScope())// 字下げ
                {
                    using (new GUILayout.VerticalScope("Box"))
                    {
                        sliceWidth = EditorGUILayout.IntField("スライスの幅", sliceWidth, GUILayout.MaxWidth(elementMaxWidth));
                        sliceHeight = EditorGUILayout.IntField("スライスの高さ", sliceHeight, GUILayout.MaxWidth(elementMaxWidth));
                    }
                }
            }

            textureSizeOptionOpen = EditorGUILayout.Foldout(textureSizeOptionOpen, "テクスチャ自体のサイズ設定");// デフォルトではスプライトサイズが最大2048なので手動で再設定
            if (textureSizeOptionOpen)
            {
                using (new EditorGUI.IndentLevelScope(1))// 字下げ
                {
                    using (new GUILayout.VerticalScope("Box"))
                    {
                        textureWidth = EditorGUILayout.IntField("テクスチャの幅", textureWidth, GUILayout.MaxWidth(elementMaxWidth));
                        textureHeight = EditorGUILayout.IntField("テクスチャの高さ", textureHeight, GUILayout.MaxWidth(elementMaxWidth));
                    }
                }
            }

            pivotPositionOptionOpen = EditorGUILayout.Foldout(pivotPositionOptionOpen, "pivotの位置設定");
            if (pivotPositionOptionOpen)
            {
                using (new EditorGUI.IndentLevelScope(1))// 字下げ
                {
                    using (new GUILayout.VerticalScope("Box"))
                    {
                        pivotPosition.x = EditorGUILayout.Slider("x", pivotPosition.x, 0f, 1f, GUILayout.MaxWidth(elementMaxWidth));// 0～1の範囲に制限
                        pivotPosition.y = EditorGUILayout.Slider("y", pivotPosition.y, 0f, 1f, GUILayout.MaxWidth(elementMaxWidth));
                    }
                }
            }
            GUILayout.FlexibleSpace();

            // スライス後のプレビュー
            using (new GUILayout.VerticalScope("Box"))
            {
                GUILayout.Label("プレビュー", GUILayout.MaxWidth(elementMaxWidth));
                Rect labelRect = GUILayoutUtility.GetLastRect();// 「プレビュー」の文のRectを取得（位置の情報がほしい）
                if (texture != null && (sliceWidth != 0 && sliceHeight != 0))
                {
                    sliceSize = new Vector2(sliceWidth, sliceHeight);
                    GUILayout.Space(sliceSize.y);

                    Vector2 previewStartPoint = new Vector2(labelRect.x + sliceSize.x * (previewAdjustment.x - zoomNum / 2), labelRect.y + sliceSize.y * (previewAdjustment.y - zoomNum / 2));
                    Rect previewRect = new Rect(previewStartPoint, sliceSize * zoomNum);                                    // 描画開始位置と描画サイズ
                    textureAspectRate = new Vector2(sliceSize.x / textureWidth, sliceSize.y / textureHeight);               // スライス後の一枚の画像サイズを設定
                    textureStartPosition = Vector2.zero;                                                                    // プレビューするスライスを切り替える仕様にするときに設定()
                    GUI.DrawTextureWithTexCoords(previewRect, texture, new Rect(textureStartPosition, textureAspectRate));  // プレビューを描画

                    Vector3 previewPivot = new Vector3(previewRect.x + previewRect.width * pivotPosition.x, previewRect.y
                        + previewRect.height * (1 - pivotPosition.y), 0f);// y座標の基準がSprite Editorと異なる
                    float radius = 2f;
                    Handles.color = new Color32(80, 160, 240, 200);
                    Handles.DrawSolidDisc(previewPivot, Vector3.forward, radius);

                    GUILayout.Space(sliceSize.y);
                }
                GUILayout.Space(20f);
                using (var change = new EditorGUI.ChangeCheckScope())
                {
                    zoomNum = EditorGUILayout.Slider("拡大率", zoomNum, zoomMin, zoomMax, GUILayout.MaxWidth(elementMaxWidth));
                    if (change.changed)
                    {
                        if (zoomNum < zoomMin) zoomNum = zoomMin;// 直接入力で想定した値の範囲から外れないように
                        if (zoomNum > zoomMax) zoomNum = zoomMax;
                    }
                }
            }
            EditorGUILayout.EndScrollView();// ここまでがスクロールに含まれる

            if (GUILayout.Button("設定を適用してスライス！"))
            {
                ImportSettingAndSlice(texture);
            }
            EditorGUILayout.Space();
        }
    }

    void ImportSettingAndSlice(Texture2D texture)
    {
        /*引数で受け取ったテクスチャオブジェクトのパスと、テクスチャインポート設定を取得*/
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

        /*テクスチャインポートプロパティを設定*/
        ti.isReadable = true;// 読み込み・書き込みを有効化
        ti.textureType = TextureImporterType.Sprite;                        // テクスチャタイプをスプライトに設定
        ti.spriteImportMode = SpriteImportMode.Multiple;                    // スプライトインポートモードを複数に設定
        ti.maxTextureSize = 8192;                                           // 画像の最大サイズを8192に設定
        ti.textureCompression = TextureImporterCompression.Uncompressed;    // 画像の圧縮をオフ

        /*SpriteEditorのスライス操作*/
        List<SpriteMetaData> spriteMetaDatas = new List<SpriteMetaData>();
        int fileNo = 0;
        for (int h = textureHeight; h > 0; h -= sliceHeight)
        {
            for (int w = 0; w < textureWidth; w += sliceWidth)
            {
                SpriteMetaData spriteMetaData = new SpriteMetaData();
                spriteMetaData.pivot = pivotPosition;// 今は直接入力しているが、エディターウィンドウで設定できるようにするかも
                spriteMetaData.alignment = (int)SpriteAlignment.Custom;
                spriteMetaData.name = texture.name.ToString() + '_' + fileNo.ToString();
                spriteMetaData.rect = new Rect(w, h - sliceHeight, sliceWidth, sliceHeight);// ここで範囲を指定してスライス
                spriteMetaDatas.Add(spriteMetaData);
                fileNo++;
            }
        }
        Debug.Log(spriteMetaDatas.Count + "個のスプライトにスライスしました");
        ti.spritesheet = spriteMetaDatas.ToArray();

        /*編集した内容をMetaファイルに反映*/
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
}
