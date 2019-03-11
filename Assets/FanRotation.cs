using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanRotation : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, .3f, 0);
    }
}
