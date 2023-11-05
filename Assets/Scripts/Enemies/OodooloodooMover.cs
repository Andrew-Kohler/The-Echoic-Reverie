using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OodooloodooMover : MonoBehaviour
{
    [Header("End Points")]
    [SerializeField] private Vector3 _startPos, _endPos;

    [Header("Movement Parameters")]
    [SerializeField] private float _goalSpeed = 1f;
    [SerializeField, Tooltip("rate at which it achieves goal speed from current")] private float _speedSharpness = 1f;
    [SerializeField] private float _minDuration = 0.5f, _maxDuration = 2f;

    private bool _isForwardDirection = true; // forward = from start to end
    private bool _isMoving = true; // start in move cycle
    private float _durationTimer = -1; // initialized in first update frame
    private float currSpeed = 0;

    // Start is called before the first frame update
    void Start() 
    {
        transform.position = _startPos;
    }

    // Update is called once per frame
    void Update()
    {
        if(_durationTimer < 0)
        {
            _durationTimer = Random.Range(_minDuration, _maxDuration);
            // toggle moving state
            _isMoving = !_isMoving;

            if(_isMoving) // just started moving
            {
                // walk animation
            }
            else // just stopped moving
            {
                // start oodooloodooling animation + sound + particles
            }
        }

        if(_isMoving) // is movin along
        {
            currSpeed = Mathf.Lerp(currSpeed, _goalSpeed, 1 - Mathf.Exp(-_speedSharpness * Time.deltaTime));

            // non-normalized
            Vector3 travelDir = _isForwardDirection ? _endPos - transform.position : _startPos - transform.position;
            
            // check for snapping to end
            if (travelDir.magnitude < currSpeed * Time.deltaTime) // THIS IS THE ISSUE PROBABLY - DEBUG HERE
            {
                // snap to end
                transform.position = _isForwardDirection ? _endPos : _startPos;
                // flip direction
                _isForwardDirection = !_isForwardDirection;
                // stop moving
                _isMoving = !_isMoving;
            }
            else // standard movement
            {
                transform.position += travelDir.normalized * Time.deltaTime;
            }
        }

        // update timer
        _durationTimer -= Time.deltaTime;
    }
}
