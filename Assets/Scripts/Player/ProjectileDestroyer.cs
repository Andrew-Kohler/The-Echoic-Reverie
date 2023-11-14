using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDestroyer : MonoBehaviour
{
    [SerializeField] private float _timeToStop = 5f;
    private float _timeToDestroy;
    private float _timer;

    private bool _isStopped = false;

    // components
    private Rigidbody2D _rb;
    private ParticleSystem _particles;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _particles = GetComponent<ParticleSystem>();

        _timeToDestroy = _particles.main.startLifetime.constant;

        _timer = _timeToStop; // start timer
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer < 0)
        {
            if(!_isStopped)
            {
                _rb.velocity = Vector2.zero;
                _particles.Stop();

                _timer = _timeToDestroy;
                _isStopped = true;
            }
            else // already stopped
            {
                Destroy(gameObject);
            }
        }

        _timer -= Time.deltaTime;
    }
}
