using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;

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
    [SerializeField] private float _landVolume;
    [SerializeField] private AudioClip[] _jumpClips;
    [SerializeField] private float _jumpVolume;
    [SerializeField] private AudioClip _knockbackClip;
    [SerializeField] private float _knockbackVolume;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _moveParticlesLeft;
    [SerializeField] private ParticleSystem _moveParticlesRight;
    [SerializeField] private ParticleSystem _moveParticlesDown;
    [SerializeField] private ParticleSystem _LeftWallJumpBurst, _leftWallParticle;
    [SerializeField] private ParticleSystem _rightWallJumpBurst, _rightWallParticle;

    [Header("Walk Effects")]
    [SerializeField, Tooltip("frequency of walk effects while walking")] private float _walkEffectFrequency = 1f;
    private float _walkEffectTimer = 0f;

    private bool _prevIsInControl = true;
    private bool _prevIsBouncing = false;

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

        _anim.SetBool("isStomped", _player.IsStomped);


        // Landing
        if (_player.LandingThisFrame)
        {
            // landing step sound
            _source.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length)], _landVolume);

            // particle effect (at feet)
            StartCoroutine(DoGroundParticles());
        }

        // movement effects
        if (_player.Grounded && _player.Velocity.x != 0 && _player.Velocity.y == 0)
        {
            // do walk effects on timer delay
            if(_walkEffectTimer < 0)
            {
                // walking sound
                _source.PlayOneShot(_footstepClips[Random.Range(0, _footstepClips.Length)], _stepVolume);

                // walking particles (at feet)
                StartCoroutine(DoWalkParticles());

                // restart timer
                _walkEffectTimer = _walkEffectFrequency;
            }
            else
            {
                _walkEffectTimer -= Time.deltaTime;
            }
        }
        else
            _walkEffectTimer = _walkEffectFrequency; // restart timer if not grounded and walking dd- prevent double audio

        // Bonking (head hits ceiling)
        if (_player.BonkingThisFrame)
        {
            // no audio needed - sounds worse with it

            // no particles needed
        }

        // Jump effects (ground jump, or enemy bounce)
        if (_player.JumpingThisFrame || (_player.IsBouncing && !_prevIsBouncing))
        {
            // jumping sound
            _source.PlayOneShot(_jumpClips[Random.Range(0, _jumpClips.Length)], _jumpVolume);

            // particle effect (at feet)
            StartCoroutine(DoGroundParticles());
        }

        // Left wall jump effects
        if (_player.LeftWallJumpingThisFrame)
        {
            // jumping sound
            _source.PlayOneShot(_jumpClips[Random.Range(0, _jumpClips.Length)], _jumpVolume);

            // particle effect (on wall)
            StartCoroutine(DoLeftWallJumpParticles());
        }

        // Right Wall jump effects
        if (_player.RightWallJumpingThisFrame)
        {
            // jumping sound
            _source.PlayOneShot(_jumpClips[Random.Range(0, _jumpClips.Length)], _jumpVolume);

            // particle effect (on wall)
            StartCoroutine(DoRightWallJumpParticles());
        }

        if(!_player.IsInControl && _prevIsInControl) // knocked by enemy this frame
        {
            // knockback sound
            _source.PlayOneShot(_knockbackClip, _knockbackVolume);
        }

        _movement = _player.RawMovement; // Previous frame movement is more valuable

        _prevIsInControl = _player.IsInControl;
        _prevIsBouncing = _player.IsBouncing;
    }

    #region PARTICLE COROUTINES
    private IEnumerator DoGroundParticles()
    {
        ParticleSystem system1 = Instantiate(_moveParticlesLeft, _moveParticlesLeft.transform.position, _moveParticlesLeft.transform.rotation);
        ParticleSystem system2 = Instantiate(_moveParticlesRight, _moveParticlesRight.transform.position, _moveParticlesRight.transform.rotation);
        ParticleSystem system3 = Instantiate(_moveParticlesDown, _moveParticlesDown.transform.position, _moveParticlesDown.transform.rotation);

        yield return new WaitForSeconds(system1.main.startLifetime.constant);

        Destroy(system1.gameObject);
        Destroy(system2.gameObject);
        Destroy(system3.gameObject);
    }

    private IEnumerator DoWalkParticles()
    {
        ParticleSystem system1;
        if (_movement.x > 0)
            system1 = Instantiate(_moveParticlesLeft, _moveParticlesLeft.transform.position, _moveParticlesLeft.transform.rotation);
        else
            system1 = Instantiate(_moveParticlesRight, _moveParticlesRight.transform.position, _moveParticlesRight.transform.rotation);
        ParticleSystem system2 = Instantiate(_moveParticlesDown, _moveParticlesDown.transform.position, _moveParticlesDown.transform.rotation);

        yield return new WaitForSeconds(system1.main.startLifetime.constant);

        Destroy(system1.gameObject);
        Destroy(system2.gameObject);
    }

    private IEnumerator DoLeftWallJumpParticles()
    {
        ParticleSystem system1 = Instantiate(_LeftWallJumpBurst, _LeftWallJumpBurst.transform.position, _LeftWallJumpBurst.transform.rotation);
        ParticleSystem system2 = Instantiate(_leftWallParticle, _leftWallParticle.transform.position, _leftWallParticle.transform.rotation);

        yield return new WaitForSeconds(system1.main.startLifetime.constant);

        Destroy(system1.gameObject);
        Destroy(system2.gameObject);
    }

    private IEnumerator DoRightWallJumpParticles()
    {
        ParticleSystem system1 = Instantiate(_rightWallJumpBurst, _rightWallJumpBurst.transform.position, _rightWallJumpBurst.transform.rotation);
        ParticleSystem system2 = Instantiate(_rightWallParticle, _rightWallParticle.transform.position, _rightWallParticle.transform.rotation);

        yield return new WaitForSeconds(system1.main.startLifetime.constant);

        Destroy(system1.gameObject);
        Destroy(system2.gameObject);
    }
    #endregion
}