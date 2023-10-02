using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float _timeToExpire = 5f;
    private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        _timer = _timeToExpire;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer < 0)
            Destroy(gameObject);

        _timer -= Time.deltaTime;
    }
}
