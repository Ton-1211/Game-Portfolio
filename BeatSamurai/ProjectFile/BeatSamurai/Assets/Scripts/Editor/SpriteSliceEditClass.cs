using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

public class SpriteSliceEditClass
{
    /// <summary>
    /// TextureImporterのspritesheetの中にあるスプライトすべてにpivotを設定します。
    /// </summary>
    public void SetPivotsAll(string path, TextureImporter textureImporter, float x, float y)
    {
        textureImporter.isReadable = true;// 読み込み・書き込みを有効化
        for (int i = 0; i < textureImporter.spritesheet.Length; i++)
        {
            textureImporter.spritesheet[i].alignment = (int)SpriteAlignment.Custom;
            textureImporter.spritesheet[i].pivot = new Vector2(x, y);
        }
        /*編集した内容をMetaファイルを反映*/
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    /// <summary>
    /// TextureImporterのspritesheetの中にあるスプライトすべてを配置したAnimationClipを作成します。
    /// </summary>
    public void SpritesToAnimationAll(SpriteMetaData[] spriteSheet, Animator animator, float intervalTime)
    {
        AnimationClip animationClip = new AnimationClip();

        // 以下にスプライトのファイルパスを取得してキーを打ったアニメーションカーブを作成、そのカーブをクリップに加える予定
    }
}
