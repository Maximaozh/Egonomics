using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnlyIfChild : MonoBehaviour
{
    void Update()
    {
        if (transform.childCount == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
