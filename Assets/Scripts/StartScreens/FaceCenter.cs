using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCenter : MonoBehaviour
{
    private SpriteRenderer _sr;

    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _sr.flipX = transform.position.x > 0;
    }
}
