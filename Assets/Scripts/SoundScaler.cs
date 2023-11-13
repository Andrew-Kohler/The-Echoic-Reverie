using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class SoundScaler : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject player;

    [SerializeField] private float maxDistance = 12f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioSource.volume = math.remap(maxDistance, 0, .05f, .7f, Vector2.Distance(this.gameObject.transform.position, player.gameObject.transform.position));
        audioSource.Play();
    }
}
