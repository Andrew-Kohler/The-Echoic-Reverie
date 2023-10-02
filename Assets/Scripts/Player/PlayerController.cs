using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TarodevController;

/// <summary>
/// Hey!
/// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
/// Right now it only contains movement and jumping, but it should be pretty easy to expand... I may even do it myself
/// if there's enough interest. You can play and compete for best times here: https://tarodev.itch.io/
/// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/GqeHHnhHpz
/// </summary>
public class PlayerController : MonoBehaviour, IPlayerController
{
    // constants - for readability
    private const float NO_COL = -1;

    // Public for external hooks
    public Vector3 Velocity { get; private set; }
    public FrameInput Input { get; private set; }
    public bool JumpingThisFrame { get; private set; }
    public bool LandingThisFrame { get; private set; }
    public Vector3 RawMovement { get; private set; }
    public bool Grounded => _colDown != NO_COL;
    public bool ClingingThisFrame { get; private set; }
    public bool CurrentlyClinging { get; private set; }

    private Vector3 _lastPosition;
    private float _currentHorizontalSpeed, _currentVerticalSpeed;

    // This is horrible, but for some reason colliders are not fully established when update starts...
    private bool _active;
    void Awake() => Invoke(nameof(Activate), 0.5f);
    void Activate() => _active = true;

    private void Update()
    {
        if (!_active) return;
        // Calculate velocity
        Velocity = (transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = transform.position;

        GatherInput();
        RunCollisionChecks();

        CalculateWalk(); // Horizontal movement
        CalculateJumpApex(); // Affects fall speed, so calculate before gravity
        CalculateGravity(); // Vertical movement
        CalculateJump(); // Possibly overrides vertical
        CalculateWallCling(); // Overrides horizontal and vertical if necessary

        MoveCharacter(); // Actually perform the axis movement

        ProjectileAbility(); // Handle projectile spawning and cooldowns
    }


    #region Gather Input

    private void GatherInput()
    {
        Input = new FrameInput
        {
            JumpDown = UnityEngine.Input.GetButtonDown("Jump"),
            JumpUp = UnityEngine.Input.GetButtonUp("Jump"),
            X = UnityEngine.Input.GetAxisRaw("Horizontal"),
            MouseDown = UnityEngine.Input.GetMouseButtonDown(0),
            MousePos = UnityEngine.Input.mousePosition
        };
        if (Input.JumpDown)
        {
            _lastJumpPressed = Time.time;
        }
    }

    #endregion

    #region Collisions

    [Header("COLLISION")] [SerializeField] private Bounds _characterBounds;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _detectorCount = 3;
    [SerializeField] private float _detectionRayLength = 0.1f;
    [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground
    [Tooltip("consistent spacing between player collider and colliders")]
    [SerializeField] private float _colGap = 0.1f;

    private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
    private float _colUp, _colRight, _colDown, _colLeft; // IMPORTANT: collision distance ; -1 = no collision

    private float _timeLeftGrounded; // only used for coyote jump timing (i.e. time since fell off ground not time since jump)
    private float _timeClingStart = float.MinValue;

    // We use these raycast checks for pre-collision information
    private void RunCollisionChecks()
    {
        // Generate ray ranges. 
        CalculateRayRanged();

        // Ground - Grounded state
        LandingThisFrame = false;
        float groundedCheck = RunDetection(_raysDown);
        if (_colDown!=NO_COL && groundedCheck==NO_COL) _timeLeftGrounded = Time.time; // Only trigger when first leaving
        else if (_colDown==NO_COL && groundedCheck!=NO_COL)
        {
            _coyoteUsable = true; // Only trigger when first touching
            LandingThisFrame = true;
        }

        _colDown = groundedCheck;

        // Walls - Clinging state
        ClingingThisFrame = false;
        float clingingCheckLeft = RunDetection(_raysLeft);
        float clingingCheckRight = RunDetection(_raysRight);
        if((_colLeft==NO_COL && clingingCheckLeft!=NO_COL) || (_colRight==NO_COL && clingingCheckRight!=NO_COL)) // just started a cling
        {
            ClingingThisFrame = true;
            _timeClingStart = Time.time;
            _leftCling = clingingCheckLeft!=NO_COL; // set direction to know which way to wall jump later
        }

        _colLeft = clingingCheckLeft;
        _colRight = clingingCheckRight;

        // Ceiling
        _colUp = RunDetection(_raysUp);

        float RunDetection(RayRange range)
        {
            IEnumerable<Vector2> vectors = EvaluateRayPositions(range);
            float colDist = -1; // default -1 indicates no collision

            // check for nearest collision distance
            foreach(Vector2 vector in vectors)
            {
                RaycastHit2D raycast = Physics2D.Raycast(vector, range.Dir, _detectionRayLength, _groundLayer);
                if (raycast && (raycast.distance < colDist || colDist == -1))
                    colDist = raycast.distance;
            }

            return colDist;
        }
    }

    private void CalculateRayRanged()
    {
        // This is crying out for some kind of refactor. 
        var b = new Bounds(transform.position + _characterBounds.center, _characterBounds.size);

        _raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
        _raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
        _raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
        _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
    }


    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
    {
        for (var i = 0; i < _detectorCount; i++)
        {
            var t = (float)i / (_detectorCount - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    private void OnDrawGizmos()
    {
        // Bounds - drawn in and out of play mode
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

        // Rays - only drawn when not in play mode
        if (!Application.isPlaying)
        {
            CalculateRayRanged();
            Gizmos.color = Color.blue;
            foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft })
            {
                foreach (var point in EvaluateRayPositions(range))
                {
                    Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
                }
            }
        }

        if (!Application.isPlaying) return;

        // Draw the future position. Handy for visualizing gravity - only drawn in play mode
        Gizmos.color = Color.red;
        var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center + move, _characterBounds.size);
    }

    #endregion

    #region Walk

    [Header("WALKING")] [SerializeField] private float _acceleration = 90;
    [SerializeField] private float _moveClamp = 13;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _apexBonus = 2;

    private void CalculateWalk()
    {
        if (Input.X != 0)
        {
            // Set horizontal move speed
            _currentHorizontalSpeed += Input.X * _acceleration * Time.deltaTime;

            // Apply bonus at the apex of a jump
            var apexBonus = Mathf.Sign(Input.X) * _apexBonus * _apexPoint;
            _currentHorizontalSpeed += apexBonus * Time.deltaTime;
        }
        else
        {
            // No input. Let's slow the character down
            _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
        }

        // clamped by max frame movement
        _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

        if (_currentHorizontalSpeed > 0 && _colRight!=NO_COL) 
        {
            // Don't walk through walls
            _currentHorizontalSpeed = 0;

            // snap to right wall
            transform.position += new Vector3(_colRight - _colGap, 0, 0);
        }
        if(_currentHorizontalSpeed < 0 && _colLeft != NO_COL)
        {
            // Don't walk through walls
            _currentHorizontalSpeed = 0;

            // snap to left wall
            transform.position += new Vector3(_colGap -_colLeft, 0, 0);
        }
    }

    #endregion

    #region Gravity

    [Header("GRAVITY")] [SerializeField] private float _fallClamp = -40f;
    [SerializeField, Tooltip("gravity while moving up")] private float _minFallAccel = 80f;
    [SerializeField, Tooltip("gravity while falling")] private float _maxFallAccel = 120f;
    private float _fallAccel;

    private void CalculateGravity()
    {
        if (_colDown!=NO_COL)
        {
            if (_currentVerticalSpeed < 0)
            {
                // Don't fall through floor (it is solid)
                _currentVerticalSpeed = 0;

                // snap to ground (consistent height/gap)
                transform.position += new Vector3(0, _colGap - _colDown, 0);
            }
        }
        else
        {
            // Add downward force while ascending if we ended the jump early
            var fallAccel = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallAccel * _jumpEndEarlyGravityModifier : _fallAccel;

            // Fall
            _currentVerticalSpeed -= fallAccel * Time.deltaTime;

            // Clamp
            if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
        }
    }

    #endregion

    #region Jump

    [Header("JUMPING")] 
    [SerializeField] private float _jumpVelocity = 30;
    [SerializeField] private Vector2 _wallJumpVelocity = new Vector2(5f, 10f);
    [SerializeField, Tooltip("lower value = smaller window of apex bonus control; also effects fall speeds somehow")] private float _jumpApexThreshold = 10f;
    [SerializeField] private float _coyoteTimeThreshold = 0.1f;
    [SerializeField] private float _jumpBuffer = 0.1f;
    [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
    private bool _coyoteUsable;
    private bool _endedJumpEarly = true;
    private float _apexPoint; // Becomes 1 at the apex of a jump
    private float _lastJumpPressed;
    private bool CanUseCoyote => _coyoteUsable && _colDown==NO_COL && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
    private bool HasBufferedJump => _colDown!=NO_COL && _lastJumpPressed + _jumpBuffer > Time.time;

    private void CalculateJumpApex()
    {
        
        if (_colDown==NO_COL)
        {
            // Gets stronger the closer to the top of the jump
            _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
            _fallAccel = Mathf.Lerp(_minFallAccel, _maxFallAccel, _apexPoint);
        }
        else
        {
            _apexPoint = 0;
        }
    }

    private void CalculateJump()
    {
        // Jump if: grounded or within coyote threshold || sufficient jump buffer && not wall clinging
        if ((Input.JumpDown && CanUseCoyote || HasBufferedJump) && _timeClingStart + _clingDuration <= Time.time)
        {
            _currentVerticalSpeed = _jumpVelocity;
            _endedJumpEarly = false;
            _coyoteUsable = false;
            _timeLeftGrounded = float.MinValue; // smallest value so that time difference from current time is never small
            JumpingThisFrame = true;
        }
        else if(Input.JumpDown && _timeClingStart + _clingDuration > Time.time) // wall jump
        {
            _currentHorizontalSpeed = (_leftCling ? 1 : -1) * _wallJumpVelocity.x;
            _currentVerticalSpeed = _wallJumpVelocity.y;
            _timeClingStart = float.MinValue; // end cling state
            _timeLeftGrounded = float.MinValue;
            _endedJumpEarly = false;
            JumpingThisFrame = true;
        }
        else
        {
            JumpingThisFrame = false;
        }

        // End the jump early if button released
        if (_colDown==NO_COL && Input.JumpUp && !_endedJumpEarly && Velocity.y > 0 && _timeClingStart + _clingDuration <= Time.time)
        {
            // _currentVerticalSpeed = 0;
            _endedJumpEarly = true;
        }

        if (_colUp!=NO_COL)
        {
            if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0; // bonk
        }
    }

    #endregion

    #region Wall Cling

    [Header("WALL CLING")]
    [SerializeField] private float _clingDuration = 1f;
    private bool _leftCling;

    private void CalculateWallCling()
    {
        if (_timeClingStart + _clingDuration > Time.time) // still clinging
        {
            // set speeds to 0 as you are clung to wall
            CurrentlyClinging = true;
            _currentHorizontalSpeed = 0;
            _currentVerticalSpeed = 0;
        }
        else
        {
            CurrentlyClinging = false;
            _timeClingStart = float.MinValue;
        }
    }

    #endregion

    #region Move

    [Header("MOVE")]
    [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
    private int _freeColliderIterations = 10;

    // We cast our bounds before moving to avoid future collisions
    private void MoveCharacter()
    {
        var pos = transform.position + _characterBounds.center;
        RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
        var move = RawMovement * Time.deltaTime;
        var furthestPoint = pos + move;

        // check furthest movement. If nothing hit, move and don't do extra checks
        var hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
        if (!hit)
        {
            transform.position += move;
            return;
        }

        // otherwise increment away from current pos; see what closest position we can move to
        var positionToMoveTo = transform.position;
        for (int i = 1; i < _freeColliderIterations; i++)
        {
            // increment to check all but furthestPoint - we did that already
            var t = (float)i / _freeColliderIterations;
            var posToTry = Vector2.Lerp(pos, furthestPoint, t);

            if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer))
            {
                transform.position = positionToMoveTo;

                // We've landed on a corner or hit our head on a ledge. Nudge the player gently - *not entirely sure how this works
                if (i == 1)
                {
                    if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                    var dir = transform.position - hit.transform.position;
                    transform.position += dir.normalized * move.magnitude;
                }

                return;
            }

            positionToMoveTo = posToTry;
        }
    }

    #endregion

    #region PROJECTILE
    [Header("Projectile Ability")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _cooldown = 5f;

    private float _cooldownTimer = 0f;

    public void ProjectileAbility()
    {
        // fire projectile
        if (Input.MouseDown && _cooldownTimer <= 0)
        {
            // calculate velocity
            Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 velocity = (Input.MousePos - playerPos).normalized * _projectileSpeed;

            // create projectile with velocity
            Instantiate(_projectilePrefab, transform.position, _projectilePrefab.transform.rotation).GetComponent<Rigidbody2D>().velocity = velocity;

            _cooldownTimer = _cooldown; // start cooldown
        }
        else if(_cooldownTimer > 0) // timer
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }
    #endregion
}