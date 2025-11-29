using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public class ParallaxClass
{
    [Header("動かす画像のオブジェクト"), SerializeField] GameObject imageObject;
    [Header("他の画像のサイズと合わないときに設定"), SerializeField] GameObject sizeObject;
    [Tooltip("カメラへの追従しにくさ（0で動かない、1でカメラと同じ動き）"), SerializeField] Vector2 parallaxEffect = new Vector2(1f, 0f);
    [Header("ループするかどうか"), SerializeField] bool canLoop = true;
    float length;
    Vector3 startPos;
    float timeElapsed;

    public GameObject ImageObject
    {
        get { return imageObject; }
    }
    public GameObject SizeObject
    {
        get { return sizeObject; }
    }
    public Vector2 ParallaxEffect
    {
        get { return parallaxEffect; }
    }
    public float Length
    {
        get { return length; }
        set { length = value; }
    }
    public Vector3 StartPos
    {
        get { return startPos; }
        set { startPos = value; }
    }
    public bool CanLoop
    {
        get { return canLoop; }
    }
    public float TimeElapsed
    {
        get { return timeElapsed; }
        set { timeElapsed = value; }
    }
    public bool IsSizeObjectAttached() { return sizeObject != null; }// sizeObjectがnullでない(=アタッチされている)かどうかを返す
}
