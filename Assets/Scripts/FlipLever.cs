using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipLever : MonoBehaviour
{
    [SerializeField] private int _leverIndex = 0;

    [SerializeField] private AudioClip _leverSound;
    [SerializeField] private float _leverVolume = 1.0f;
    [SerializeField] private ParticleSystem _particleEffect;
    [SerializeField] private GameObject _leverBall;
    [SerializeField] private float _leverBallGrowFactor;

    private SpriteRenderer _renderer;
    private AudioSource _audioSource;

    private bool _flipped;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        // ensure it starts flipped if flipped when in the scene previously
        bool state = GameManager.Instance.GetLever(_leverIndex);
        _flipped = state; // initialize state to prevent re-flip
        _renderer.flipX = state; // flip it visually if necessary
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") & !_flipped)
        {
            _flipped = true; // set to flipped state

            _renderer.flipX = true; // flip it visually

            GameManager.Instance.FlipLever(_leverIndex); // set data to flipped

            // play flip sound and activate big particle effect!! - only activates once on first flip
            _audioSource.PlayOneShot(_leverSound, _leverVolume);
            _particleEffect.Play();
            StartCoroutine(DoGrowLeverBall());

            // TODO: set off bells if in 0_MainGrounds (delay to ensure it does not conflict with shock wave effect) - coroutinned delay??
        }
    }

    private IEnumerator DoGrowLeverBall()
    {
        _leverBall.SetActive(true);

        while(_particleEffect.isPlaying)
        {
            _leverBall.transform.localScale += Vector3.one * (_leverBallGrowFactor * Time.fixedDeltaTime); // apply grow factor

            yield return new WaitForFixedUpdate();
        }

        Destroy(_leverBall);
    }
}
