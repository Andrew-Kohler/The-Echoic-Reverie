using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompedStateHandler : MonoBehaviour
{
    private PlayerController _player;

    [Header("Time")]
    [SerializeField] private float _stompedDuration = 0.5f;
    private float _timer = -1;

    private bool _isStomperGone = true;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_isStomperGone) // only update timer if stomper has departed
        {
            // update timer
            if (_timer > 0)
                _timer -= Time.deltaTime;
            else
                _player.IsStomped = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stomp Box"))
        {
            // place player in stomped state
            _player.IsStomped = true;
            _isStomperGone = false;

            // TODO: send signal to player animator to enter stomped animation state
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Stomp Box"))
        {
            // start timer for returning player control
            _timer = _stompedDuration;
            _isStomperGone = true;

            // TODO: send signal to player animator to enter stomped animation state
        }
    }
}
