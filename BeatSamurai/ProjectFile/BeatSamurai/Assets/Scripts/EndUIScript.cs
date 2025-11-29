using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndUIScript : MonoBehaviour
{
    [SerializeField] Button firstSelectedButton;

    void OnEnable()
    {
        firstSelectedButton.Select();
    }
}
