using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellRinger : MonoBehaviour
{
    [Header("Times")]
    [SerializeField] private float _initialWaitTime;
    [SerializeField] private float _timeBetweenBells;

    [Header("Visual Effects")]
    [SerializeField] private Vector3 _from = new Vector3(0.0F, 0.0F, 10.0F);
    [SerializeField] private Vector3 _to = new Vector3(0.0F, 0.0F, -10.0F);
    [SerializeField] private float _frequency = 4F;
    [SerializeField] private float rateOfFade = .7f;

    [Header("Sounds")]
    [SerializeField] private AudioClip _bellSound0;
    [SerializeField] private AudioClip _bellSound1;
    [SerializeField] private AudioClip _bellSound2;
    [SerializeField] private AudioClip _bellSound3;
    [SerializeField] private float _bellVolume = 1.0f;

    [Header("Bells")]
    [SerializeField] private SpriteRenderer _bell0;
    [SerializeField] private SpriteRenderer _bell1;
    [SerializeField] private SpriteRenderer _bell2;
    [SerializeField] private SpriteRenderer _bell3;

    private AudioSource _audioSource;
    private bool _prevLever0;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _prevLever0 = GameManager.Instance.GetLever(0);

        _bell0.color = new Color(_bell0.color.r, _bell0.color.g, _bell0.color.b, 0f);
        _bell1.color = new Color(_bell1.color.r, _bell1.color.g, _bell1.color.b, 0f);
        _bell2.color = new Color(_bell2.color.r, _bell2.color.g, _bell2.color.b, 0f);
        _bell3.color = new Color(_bell3.color.r, _bell3.color.g, _bell3.color.b, 0f);

        StartCoroutine(DoRingBells());
    }

    // Update is called once per frame
    void Update()
    {
        // if player flip lever and no active coroutine then ring the bells - necessary for initial bells ring
        if (_prevLever0 == false && GameManager.Instance.GetLever(0))
            StartCoroutine(DoRingBells());

        _prevLever0 = GameManager.Instance.GetLever(0);
    }

    private IEnumerator DoRingBells()
    {

        yield return new WaitForSeconds(_initialWaitTime);

        if(GameManager.Instance.GetLever(0))
        {
            StartCoroutine(DoFadeEffect(_bell0));
            StartCoroutine(DoShakeEffect(_bell0));
            _audioSource.PlayOneShot(_bellSound0, _bellVolume);
        }

        yield return new WaitForSeconds(_timeBetweenBells);

        if (GameManager.Instance.GetLever(1))
        {
            StartCoroutine(DoFadeEffect(_bell1));
            StartCoroutine(DoShakeEffect(_bell1));
            _audioSource.PlayOneShot(_bellSound1, _bellVolume);
        }

        yield return new WaitForSeconds(_timeBetweenBells);

        if (GameManager.Instance.GetLever(2))
        {
            StartCoroutine(DoFadeEffect(_bell2));
            StartCoroutine(DoShakeEffect(_bell2));
            _audioSource.PlayOneShot(_bellSound2, _bellVolume);
        }

        yield return new WaitForSeconds(_timeBetweenBells);

        if (GameManager.Instance.GetLever(3))
        {
            StartCoroutine(DoFadeEffect(_bell3));
            StartCoroutine(DoShakeEffect(_bell3));
            _audioSource.PlayOneShot(_bellSound3, _bellVolume);
        }
    }

    IEnumerator DoFadeEffect(SpriteRenderer sr)
    {
        //sr.color = regularColor;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        while (sr.color.a > 0)
        {
            float alpha = sr.color.a - rateOfFade * Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }
    }

    IEnumerator DoShakeEffect(SpriteRenderer sr)
    {
        Vector3 n_from = _from;
        Vector3 n_to = _to;
        Quaternion from;
        Quaternion to;

        while (n_from.z > 0 && n_to.z < 0)
        {
            from = Quaternion.Euler(n_from);
            to = Quaternion.Euler(n_to);
            float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * _frequency));
            sr.transform.localRotation = Quaternion.Lerp(from, to, lerp);

            n_from = new Vector3(n_from.x, n_from.y, n_from.z - 7f * Time.deltaTime);
            n_to = new Vector3(n_to.x, n_to.y, n_to.z + 7f * Time.deltaTime);
            yield return null;
        }

    }
}
