using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParallaxScript : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [Header("動かす背景たち"), SerializeField] ParallaxClass[] parallaxClasses;
    // Start is called before the first frame update
    void Start()
    {
        foreach(ParallaxClass parallaxClass in parallaxClasses)// 登録されている背景オブジェクトすべて
        {
            // 開始時点の位置と画像の長さ(x軸方向)を設定
            parallaxClass.StartPos = parallaxClass.ImageObject.transform.position;// 開始時点のオブジェクトの位置を取得
            GameObject lengthObject = parallaxClass.IsSizeObjectAttached() == true ? parallaxClass.SizeObject : parallaxClass.ImageObject;// サイズの基準にするオブジェクトを設定
                                                                                                                                                       
            parallaxClass.Length = lengthObject.GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }

    private void FixedUpdate()
    {
        foreach(var parallaxClass in parallaxClasses)
        {
            float loopDistanceX = mainCamera.transform.position.x * (1 - parallaxClass.ParallaxEffect.x);// ループ判定用
            float distanceX = mainCamera.transform.position.x * parallaxClass.ParallaxEffect.x;// 視差効果用
            float distanceY = mainCamera.transform.position.y * parallaxClass.ParallaxEffect.y;

            parallaxClass.ImageObject.transform.position = new Vector3(parallaxClass.StartPos.x + distanceX,
                parallaxClass.StartPos.y + distanceY, parallaxClass.StartPos.z);// 背景座標をdistanceの分移動させる

            if (parallaxClass.CanLoop)// ループするタイプのとき
            {
                // 背景が画面外に行ったら背景を移動させる(無限ループの処理)
                if (loopDistanceX > parallaxClass.StartPos.x + parallaxClass.Length)
                {
                    parallaxClass.StartPos += new Vector3(parallaxClass.Length, 0f, 0f);
                }
                else if (loopDistanceX < parallaxClass.StartPos.x - parallaxClass.Length)
                {
                    parallaxClass.StartPos -= new Vector3(parallaxClass.Length, 0f, 0f);
                }
            }
        }
    }
}
