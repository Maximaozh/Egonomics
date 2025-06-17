using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowScreen : MonoBehaviour
{
    public Button turnEndButton;
    public Image panel;
    public float duration = 0.5f;
    public float wait = 0.5f;
    public float targetAlpha = 0.5f;

    public void Start()
    {
        turnEndButton.onClick.AddListener(StartDarkenEffect);
    }

    public void StartDarkenEffect()
    {
        StartCoroutine(DarkenCoroutine());
    }

    private System.Collections.IEnumerator DarkenCoroutine()
    {
        panel.color = new Color(0, 0, 0, 0);
        panel.gameObject.SetActive(true);


        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, targetAlpha, t / duration);
            panel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(wait);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(targetAlpha, 0, t / duration);
            panel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        panel.gameObject.SetActive(false);
    }
}
