using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipLever : MonoBehaviour
{
    [SerializeField] private int _leverIndex = 0;

    private SpriteRenderer _renderer;

    private bool _flipped;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();

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

            // TODO: play flip sound and activate big particle effect!! - only activates once on first flip
        }
    }
}
