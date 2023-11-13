using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is essentially just another type of PlayerAnimator, but for clarity of purpose I'm keeping them separate
//  -Andrew

public class PlayerNoiseVisuals : MonoBehaviour
{
    //[SerializeField] private ParticleSystem _jumpParticles, _launchParticles;
    //[SerializeField] private ParticleSystem _moveParticles, _landParticles;
    //[SerializeField] private AudioClip[] _footsteps;
    //[SerializeField, Range(1f, 3f)] private float _maxIdleSpeed = 2;
    //[SerializeField] private float _maxParticleFallSpeed = -40;

    [SerializeField] private ParticleSystem _moveParticles1, _moveParticles2, _moveParticles3, _clingParticles1, _clingParticles2;

    private PlayerController _player;
    private bool _playerGrounded;
    private ParticleSystem.MinMaxGradient _currentGradient;
    private Vector2 _movement;
    private Vector3 _baseScale;

    void Awake() => _player = GetComponentInParent<PlayerController>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_playerGrounded && _player.Velocity.x != 0) // If we are walking on the ground
        {
            _moveParticles1.Play();
            _moveParticles2.Play();
            _moveParticles3.Play();
        }
        else
        {
            _moveParticles1.Stop();
            _moveParticles2.Stop();
            _moveParticles3.Stop();
        }

        if (!_playerGrounded && _player.Grounded)
        {
            _playerGrounded = true;
        }
        else if (_playerGrounded && !_player.Grounded) // leaving ground
        {
            _playerGrounded = false;
        }

        if (_player.ClingingThisFrame)
        {

        }


    }
}
