using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class SoundScaler : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject player;

    [SerializeField] private float maxDistance = 12f;
    private float maxVolume = .6f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        if (Vector2.Distance(this.gameObject.transform.position, player.gameObject.transform.position) > maxDistance / 3f)
        {
            audioSource.volume = math.remap(maxDistance, 0, .02f, maxVolume/4f, Vector2.Distance(this.gameObject.transform.position, player.gameObject.transform.position));
        }
        else
        {
            audioSource.volume = math.remap(maxDistance, 0, .05f, maxVolume/1.7f, Vector2.Distance(this.gameObject.transform.position, player.gameObject.transform.position));
        }
        
        audioSource.Play();
    }
}
