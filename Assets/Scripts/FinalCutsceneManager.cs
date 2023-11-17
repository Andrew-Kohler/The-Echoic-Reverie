using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject[] charaList = new GameObject[8];
    [SerializeField] private Animator cameraAnim;
    [SerializeField] private FerrisWheelMotor wheelMotor;

    [SerializeField] private ParticleSystem _particleEffect;
    [SerializeField] private GameObject _leverBall;
    [SerializeField] private float _leverBallGrowFactor;

    private AudioSource audioSource;

    [SerializeField] AudioClip ferrisWheelMusic;
    [SerializeField] AudioClip theEnd;
    [SerializeField] private GameObject thankYou;
    [SerializeField] private GameObject mainMenu;

    private void OnEnable()
    {
        BellRinger.onCompleteBellRing += FinalCutscene;
    }

    private void OnDisable()
    {
        BellRinger.onCompleteBellRing -= FinalCutscene;
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < charaList.Length; i++)
        {
            charaList[i].SetActive(false);
        }
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinalCutscene()
    {
        StartCoroutine(DoFinalCutscene());
    }

    IEnumerator DoFinalCutscene()
    {
        // Do a special lever effect out from the center of the ferris wheel that perma-enables everything
        _particleEffect.Play();
        StartCoroutine(DoGrowCutsceneBall());
        // Wait for a given amt of time
        yield return new WaitForSeconds(1f);
        // Enable all the friends
        for (int i = 0; i < charaList.Length; i++)
        {
            charaList[i].SetActive(true);
        }
        // Wait for the effect to finish
        yield return new WaitForSeconds(3f);
        // Start the wheel
        wheelMotor.turning = true;
        audioSource.PlayOneShot(ferrisWheelMusic);
        // Maybe play a sound effect for this?
        // Let the player take it in for a little
        yield return new WaitForSeconds(5f);
        // Pan the camera up to a title card
        cameraAnim.Play("FinalCamMove");
        audioSource.volume = .25f;
        audioSource.PlayOneShot(theEnd);
        // There should be a "return to title" button up there that gets faded in
        yield return new WaitForSeconds(7.264f);
        thankYou.SetActive(true);
        mainMenu.SetActive(true);
    }

    private IEnumerator DoGrowCutsceneBall()
    {
        _leverBall.SetActive(true);

        while (_particleEffect.isPlaying)
        {
            _leverBall.transform.localScale += Vector3.one * (_leverBallGrowFactor * Time.fixedDeltaTime); // apply grow factor

            yield return new WaitForFixedUpdate();
        }

        Destroy(_leverBall);
    }
}
