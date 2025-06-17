using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationSlider : MonoBehaviour
{
    public Slider slider;
    public TMP_Text text;
    void Start()
    {
        slider = this.gameObject.GetComponent<Slider>();
        slider.minValue = 1;
        slider.maxValue = 99;
        slider.onValueChanged.AddListener(x => HandleSliderChanged(x));

        Simulation.simCount = 1;
        
    }

    public void HandleSliderChanged(float value)
    {
        text.text = Mathf.FloorToInt(value).ToString("0");
        Simulation.simCount = Mathf.FloorToInt(value);
    }
}
