using UnityEngine;
using Random = UnityEngine.Random;
using TarodevController;

/// <summary>
/// This is a pretty filthy script. I was just arbitrarily adding to it as I went.
/// You won't find any programming prowess here.
/// This is a supplementary script to help with effects and animation. Basically a juice factory.
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator _anim;
    [SerializeField] private AudioSource _source;
    private PlayerController _player;
    SpriteRenderer sr;

    [Header("Audio")]
    [SerializeField] private AudioClip[] _footstepClips;
    [SerializeField] private float _stepVolume;
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private float _jumpVolume;
    [SerializeField] private AudioClip _clingClip;
    [SerializeField] private float _clingVolume;

    [Header("Particles")]
    //[SerializeField] private ParticleSystem _jumpParticles, _launchParticles;

    [Header("Walk Effects")]
    [SerializeField, Tooltip("frequency of walk effects while walking")] private float _walkEffectFrequency = 1f;
    private float _walkEffectTimer = 0f;
    private Vector2 _movement; // not currently used (could be helpful?)

    void Awake() => _player = GetComponentInParent<PlayerController>();

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_player == null) return;

        // Flip the sprite
        if (_player.Input.X != 0 && !_player.IsStomped)
        {
            //transform.localScale = new Vector3(_player.Input.X > 0 ? _baseScale.x : -_baseScale.x, _baseScale.y, _baseScale.z);
            if(_player.Input.X < 0)
            {
                sr.flipX = true;
            }
            else if(_player.Input.X > 0)
            {
                sr.flipX = false;
            }
        }

        // Set animation variables
        if(_player.Velocity.x != 0) 
        {
            _anim.SetBool("isMoving", true);
        }
        else
        {
            _anim.SetBool("isMoving", false);
        }
        _anim.SetBool("isGrounded", _player.Grounded);
        _anim.SetFloat("yVelocity", _player.Velocity.y);

        _anim.SetBool("isClinging", _player.CurrentlyClinging);


        // Landing
        if (_player.LandingThisFrame)
        {
            // landing step sound
            _source.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length)], _stepVolume);
            _walkEffectTimer = _walkEffectFrequency; // restart timer - prevent double audio

            // particle effect (at feet)
        }

        // movement effects
        if (_player.Grounded && _player.Velocity.x != 0)
        {
            // do walk effects on timer delay
            if(_walkEffectTimer < 0)
            {
                // walking sound
                _source.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length)], _stepVolume);

                // walking particles (at feet)

                // restart timer
                _walkEffectTimer = _walkEffectFrequency;
            }
            else
            {
                _walkEffectTimer -= Time.deltaTime;
            }
        }

        // Bonking (head hits ceiling)
        if (_player.BonkingThisFrame)
        {
            // bonking audio (same as steps)
            _source.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length)], _stepVolume); // might remove and just do timer in grounded

            // particle effects (at head)
        }

        // Jump effects
        if (_player.JumpingThisFrame)
        {
            // jumping sound
            _source.PlayOneShot(_jumpClip, _jumpVolume);

            // particle effect (at feet)
        }

        if(_player.ClingingThisFrame)
        {
            // cling audio
            _source.PlayOneShot(_clingClip, _clingVolume);

            // particle effect (at side)
        }

        _movement = _player.RawMovement; // Previous frame movement is more valuable
    }
}