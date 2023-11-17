using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerrisWheelMotor : MonoBehaviour
{
    // Start is called before the first frame update
    public bool turning;
    [SerializeField] private float rotationSpeed = .1f;
    void Start()
    {
        turning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (turning)
        {
            this.gameObject.transform.Rotate(new Vector3(0, 0, rotationSpeed));
        }
    }
}
