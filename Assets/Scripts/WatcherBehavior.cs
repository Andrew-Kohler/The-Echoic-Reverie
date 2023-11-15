using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherBehavior : MonoBehaviour
{
    [SerializeField] Sprite leftFacing;
    [SerializeField] Sprite centerFacing;
    [SerializeField] Sprite rightFacing;
    private SpriteRenderer sr;
    private GameObject player;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(this.gameObject.transform.position, player.gameObject.transform.position) > 1.4f){
            if ((this.gameObject.transform.position.x > player.gameObject.transform.position.x))
            {
                sr.sprite = leftFacing;
            }
            else
            {
                sr.sprite = rightFacing;
            }
        }
        else
        {
            sr.sprite=centerFacing;
        }
        
    }
}
