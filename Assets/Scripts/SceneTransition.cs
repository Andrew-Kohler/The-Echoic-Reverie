using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] int sceneIndex = 0;
    [SerializeField] Vector2 spawnpoint = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager.Instance.SetSpawnpoint(spawnpoint);
            GameManager.Instance.TransitionScene(sceneIndex);
        }
    }
}
