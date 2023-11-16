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
    private float _currSpeed = 0;

    private Animator anim;
    private SpriteRenderer sr;
    private SoundScaler scaler;
    private bool activeCoroutine;

    [Header("Particle Parameters")]
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] GameObject _projectilePrefab;

    // Start is called before the first frame update
    void Start() 
    {
        transform.position = _startPos;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        scaler = GetComponent<SoundScaler>();
    }

    // Update is called once per frame
    void Update()
    {
        // cycles between states
        if(_durationTimer < 0)
        {
            _durationTimer = Random.Range(_minDuration, _maxDuration);
            // toggle moving state
            _isMoving = !_isMoving;

            if(_isMoving) // just started moving
            {
                anim.SetBool("isMoving", true);
            }
            else // just stopped moving
            {
                anim.SetBool("isMoving", false);
                sr.flipX = !sr.flipX;
                if(!activeCoroutine)
                    StartCoroutine(DoYell());
                // start oodooloodooling animation + sound + particles
            }
        }

        if(_isMoving) // is movin along
        {
            // lerp to goal speed
            _currSpeed = Mathf.Lerp(_currSpeed, _goalSpeed, 1 - Mathf.Exp(-_speedSharpness * Time.deltaTime)); 
        }
        else // is not movin along 
        {
            // lerp to stopped speed
            _currSpeed = Mathf.Lerp(_currSpeed, 0, 1 - Mathf.Exp(-_speedSharpness * Time.deltaTime));
        }

        // UPDATE POSITION
        // non-normalized travel direction
        Vector3 travelDir = _isForwardDirection ? _endPos - transform.position : _startPos - transform.position;
        // check for snapping to end
        if (travelDir.magnitude < _currSpeed * Time.deltaTime)
        {
            // snap to end
            transform.position = _isForwardDirection ? _endPos : _startPos;
            // flip direction
            _isForwardDirection = !_isForwardDirection;
            // stop moving
            _isMoving = false;
            _durationTimer = Random.Range(_minDuration, _maxDuration);
            // snap currspeed to 0 to prevent bounce back effect when slowing down
            _currSpeed = 0;
        }
        else // standard movement
        {
            transform.position += travelDir.normalized * _currSpeed * Time.deltaTime;
        }

        // update timer
        _durationTimer -= Time.deltaTime;
    }

    IEnumerator DoYell()
    {
        ParticleSystem tempSys;

        activeCoroutine = true;
        anim.Play("Yell", 0, 0);
        scaler.PlaySound();

        tempSys = Instantiate(_particleSystem, new Vector3(this.transform.position.x, this.transform.position.y + .7f, 0f), new Quaternion(0, 0, 0, 0));
        tempSys.gameObject.transform.Rotate(new Vector3(0, 0, 45));

        // calculate velocity
        Vector2 stepperPos = new Vector2(this.transform.position.x, this.transform.position.y + .7f);
        Vector2 velocity = new Vector2(0, 3);

        // create projectile with velocity
        Instantiate(_projectilePrefab, stepperPos, _projectilePrefab.transform.rotation).GetComponent<Rigidbody2D>().velocity = velocity;

        yield return new WaitForSeconds(.916f);
        anim.Play("Idle", 0, 0);
        activeCoroutine = false;
        Destroy(tempSys.gameObject);

        yield return null;  
    }
}
