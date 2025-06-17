using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemRecipeUI : MonoBehaviour
{
    [Header("Ёлементы UI")]
    public TMP_Text nameTMP;
    public TMP_Text countTMP;
    public Image Ico;


    public void Configure(string name, int count, Sprite image)
    {
        nameTMP.text = name;
        countTMP.text = count.ToString();
        Ico.sprite = image;
    }
}
