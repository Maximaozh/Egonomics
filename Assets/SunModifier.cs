using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SunModifier : MonoBehaviour
{
    public Light lightC;
    public bool increase = true;
    public float min = 0.0f;
    public float max = 0.0f;
    public float speed = 1f;

    public void Start()
    {
        lightC = this.gameObject.GetComponent<Light>();
    }

    public void Update()
    {
        if(increase)
        {
            lightC.intensity += Time.deltaTime * speed;
            if (lightC.intensity > max)
                increase = !increase;
        }
        else
        {
            lightC.intensity -= Time.deltaTime * speed;
            if (lightC.intensity < min)
                increase = !increase;
        }
    }
}
