using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerrisWheelCar : MonoBehaviour
{
    // Start is called before the first frame update
    float parentZ;
    float z;
    void Start()
    {
        parentZ = GetComponentInParent<Transform>().rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        parentZ = GetComponentInParent<Transform>().rotation.z;
        this.transform.rotation = Quaternion.Euler(0, 0, -parentZ);
    }
}
