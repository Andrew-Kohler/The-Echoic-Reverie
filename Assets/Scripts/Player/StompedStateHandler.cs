using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompedStateHandler : MonoBehaviour
{
    private PlayerController _player;

    [Header("Time")]
    [SerializeField] private float _stompedDuration = 4f;
    private float _timer = -1;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // update timer
        if (_timer > 0)
            _timer -= Time.deltaTime;
        else
            _player.IsStomped = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stomp Box") && _player.Grounded)
        {
            // disable player controller and start timer
            _player.IsStomped = true;
            _timer = _stompedDuration;
            
            // TODO: send signal to player animator to enter stomped animation state
        }
    }
}
