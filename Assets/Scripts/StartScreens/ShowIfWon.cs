using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIfWon : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] _sprites = new SpriteRenderer[8];

    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.Instance.HasWon())
        {
            foreach (SpriteRenderer sprite in _sprites)
                sprite.enabled = true;
        }
    }
}
