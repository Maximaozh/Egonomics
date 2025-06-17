using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconRecipeCountUI : MonoBehaviour
{
    [Header("Поля UI")]

    public TMP_Text countTMP;
    public Image Ico;

    public void Configure(int text, Sprite image)
    {
        countTMP.text = text.ToString();
        Ico.sprite = image;
    }
}
