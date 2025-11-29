using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum JudgeType
{
    great = 0,
    good = 1,
    bad = 2,
    miss = 3
}

[System.Serializable] public class NumberSprite
{
    [SerializeField] List<Sprite> sprites;

    public List<Sprite> Sprites
    {
        get { return sprites; }
    }
}

[System.Serializable]
public class ComboPanel
{
    [Header("ケタ等をまとめてるオブジェクト"), SerializeField] GameObject parent;
    [SerializeField] Image digit100;
    [SerializeField] Image digit10;
    [SerializeField] Image digit1;

    public GameObject Parent => parent;
    public Image Digit100 => digit100;
    public Image Digit10 => digit10;
    public Image Digit1 => digit1;
}
public class ScoreScript : MonoBehaviour
{
    [SerializeField] NumberSprite numberSprites;
    [SerializeField] ComboPanel comboPanel;
    [SerializeField] ComboPanel maxComboPanel;
    [SerializeField] Image comboText;

    int combo;
    int maxCombo;
    bool playing;

    void Start()
    {
        if(!comboPanel.Parent.activeSelf)
        {
            comboPanel.Parent.SetActive(true);
        }
        if(maxComboPanel.Parent.activeSelf)
        {
            maxComboPanel.Parent.SetActive(false);
        }
        combo = 0;
        playing = true;
    }

    void FixedUpdate()
    {
        if (playing)
        {
            ShowCombo(comboPanel);
        }
    }

    public void DisplayMaxCombo()
    {
        playing = false;
        comboPanel.Parent.SetActive(false);
        // maxComboを参照して文字にする
        maxComboPanel.Parent.SetActive(true);
        ShowCombo(maxComboPanel, true);
    }

    public void AddCombo()
    {
        combo++;
    }

    public void ResetCombo()
    {
        if(maxCombo == 0 || maxCombo < combo) maxCombo = combo;
        combo = 0;
    }

    void ShowCombo(ComboPanel comboPanel, bool showMaxCombo = false)
    {
        int showNum = showMaxCombo ? maxCombo : combo;
        comboPanel.Digit100.sprite = numberSprites.Sprites[showNum / 100];
        comboPanel.Digit10.sprite = numberSprites.Sprites[(showNum % 100) / 10];
        comboPanel.Digit1.sprite = numberSprites.Sprites[showNum % 10];
    }

    void CorrectDigitsDisplay(Image image, int digitNum, int digitCount, int totalCount)
    {
        if(digitCount >= totalCount && digitNum == 0)// コンボの桁数以上の桁でかつ0のとき
        {
            image.sprite = null;
        }
    }
}
