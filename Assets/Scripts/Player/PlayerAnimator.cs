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
    [SerializeField] private Animator _anim;
    [SerializeField] private AudioSource _source;
    [SerializeField] private LayerMask _groundMask;
    SpriteRenderer sr;
    //[SerializeField] private ParticleSystem _jumpParticles, _launchParticles;
    
    //[SerializeField] private AudioClip[] _footsteps;
    [SerializeField] private float _maxTilt = .1f;
    [SerializeField] private float _tiltSpeed = 1;
    //[SerializeField, Range(1f, 3f)] private float _maxIdleSpeed = 2;
    //[SerializeField] private float _maxParticleFallSpeed = -40;

    private PlayerController _player;
    private bool _playerGrounded;
    private ParticleSystem.MinMaxGradient _currentGradient;
    private Vector2 _movement;
    private Vector3 _baseScale;

    void Awake() => _player = GetComponentInParent<PlayerController>();

    private void Start()
    {
        _baseScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_player == null) return;

        // Flip the sprite
        if (_player.Input.X != 0)
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
        _anim.SetBool("isGrounded", _playerGrounded);
        _anim.SetFloat("yVelocity", _player.Velocity.y);

        _anim.SetBool("isClinging", _player.CurrentlyClinging);

        

        // Lean while running
        var targetRotVector = new Vector3(0, 0, Mathf.Lerp(-_maxTilt, _maxTilt, Mathf.InverseLerp(-1, 1, _player.Input.X)));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotVector), _tiltSpeed * Time.deltaTime);

        // Speed up idle while running
        // _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, Mathf.Abs(_player.Input.X)));

        // Splat
        if (_player.LandingThisFrame)
        {
            //_anim.SetTrigger(GroundedKey);
            //_source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
        }

        // Jump effects
        if (_player.JumpingThisFrame)
        {
            //_anim.SetTrigger(JumpKey);
            //_anim.ResetTrigger(GroundedKey);

            // Only play particles when grounded (avoid coyote)
            if (_player.Grounded)
            {
                //SetColor(_jumpParticles);
                //SetColor(_launchParticles);
                //_jumpParticles.Play();
            }
        }

        // Play landing effects and begin ground movement effects
        if (!_playerGrounded && _player.Grounded)
        {
            _playerGrounded = true;
            /*if(_player.Velocity.x != 0)
            {
                _moveParticles1.Play();
                _moveParticles2.Play();
            }
            else
            {
                _moveParticles1.Stop();
                _moveParticles2.Stop();
            }*/
            
            //_landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, _maxParticleFallSpeed, _movement.y);
            //SetColor(_landParticles);
            //_landParticles.Play();
        }
        else if (_playerGrounded && !_player.Grounded) // leaving ground
        {
            _playerGrounded = false;
            /*_moveParticles1.Stop();
            _moveParticles2.Stop();*/
        }

        // Detect ground color - particles are color of ground?
        /*var groundHit = Physics2D.Raycast(transform.position, Vector3.down, 2, _groundMask);
        if (groundHit && groundHit.transform.TryGetComponent(out SpriteRenderer r))
        {
            _currentGradient = new ParticleSystem.MinMaxGradient(r.color * 0.9f, r.color * 1.2f);
            SetColor(_moveParticles);
        }*/

        _movement = _player.RawMovement; // Previous frame movement is more valuable
    }

    private void OnDisable()
    {
        //_moveParticles.Stop();
    }

    private void OnEnable()
    {
        //_moveParticles.Play();
    }

    void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = _currentGradient;
    }

    #region Animation Keys

    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
    private static readonly int JumpKey = Animator.StringToHash("Jump");

    #endregion
}