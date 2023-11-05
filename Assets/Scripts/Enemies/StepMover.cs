using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepperMover : MonoBehaviour
{
    [Header("End Points")]
    [SerializeField] private float _startX, _endX;

    [Header("Movement Parameters")]
    [SerializeField, Tooltip("rate at which it achieves goal speed from current")] private float _speedSharpness = 1f;
    [SerializeField] private float _steppingInterval = 0.5f, _steppingSpeed = 5f;
    [SerializeField, Min(0)] private float _jumpSpeed = 1f, _fallSpeed = 4f;
    [SerializeField] private float _pauseInterval = 3f;

    private bool _isForwardDirection = true; // forward = from start to end
    private bool _isMoving = false; // start in stopped cycle
    private float _durationTimer = -1; // initialized in first update frame
    private Vector2 _currSpeed = Vector2.zero;
    private float _initHeight, _maxHeight;

    // Start is called before the first frame update
    void Start() 
    {
        _initHeight = transform.position.y;

        transform.position = new Vector3(_startX, transform.position.y, transform.position.z);

        _durationTimer = _pauseInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // cycles between states
        if(_durationTimer < 0)
        {
            // toggle moving state
            _isMoving = !_isMoving;

            if(_isMoving) // just started moving
            {
                // set timer to time per step
                _durationTimer = _steppingInterval;

                // walk animation
            }
            else // just stopped moving
            {
                // set timer to time per stop
                _durationTimer = _pauseInterval;

                // start step sound + particles (set animation to stationary?)
            }
        }

        if (_isMoving) // is movin along
        {
            // lerp to goal move speed
            _currSpeed.x = Mathf.Lerp(_currSpeed.x, _steppingSpeed, 1 - Mathf.Exp(-_speedSharpness * Time.deltaTime));

            // lerp to goal jump speed
            _currSpeed.y = Mathf.Lerp(_currSpeed.y, _durationTimer > _steppingInterval / 2f ? _jumpSpeed : -_fallSpeed,
                1 - Mathf.Exp(-_speedSharpness * Time.deltaTime));
            
        }
        else // is not movin along 
        {
            // lerp to stopped speed - prevents instantaneous stopping
            _currSpeed.x = Mathf.Lerp(_currSpeed.x, 0, 1 - Mathf.Exp(-_speedSharpness * Time.deltaTime));

            // continue falling
            _currSpeed.y = Mathf.Lerp(_currSpeed.y, -_fallSpeed, 1 - Mathf.Exp(-_speedSharpness * Time.deltaTime));
        }

        // UPDATE HORIZONTAL POSITION
        // non-normalized travel direction
        float travelDir = _isForwardDirection ? _endX - transform.position.x : _startX - transform.position.x;
        // check for snapping to end
        if (Mathf.Abs(travelDir) < _currSpeed.x * Time.deltaTime)
        {
            // snap to end
            transform.position = new Vector3(_isForwardDirection ? _endX : _startX, transform.position.y, transform.position.z);
            // flip direction
            _isForwardDirection = !_isForwardDirection;
            // stop moving - and start stop cycle
            _isMoving = false;
            _durationTimer = _pauseInterval;
            // snap currspeed to 0 to prevent instant velocity swap and teleport effect
            _currSpeed.x = 0;
        }
        else // standard movement
        {
            transform.position += new Vector3((travelDir < 0 ? -1 : 1) * _currSpeed.x * Time.deltaTime, 0, 0);
        }

        // UPDATE VERTICAL POSITION
        if(_currSpeed.y < 0 && Mathf.Abs(_currSpeed.y * Time.deltaTime) > transform.position.y - _initHeight) // if going to pass the ground this frame
        {
            // snap to ground
            transform.position = new Vector3(transform.position.x, _initHeight, transform.position.z);
            _currSpeed.y = 0;
        }
        else // standard movement
        {
            transform.position += new Vector3(0, _currSpeed.y * Time.deltaTime, 0);
        }

        // update timer
        _durationTimer -= Time.deltaTime;
    }
}
