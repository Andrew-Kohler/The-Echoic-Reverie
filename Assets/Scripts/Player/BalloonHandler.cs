using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonHandler : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField, Tooltip("used to detect if stomped - pop balloon")] private PlayerController _player;
    [SerializeField, Tooltip("enabled and disabled to show player balloon state")] private GameObject _balloonObject;
    [SerializeField, Tooltip("used to swap animation state of kid")] private Animator _kidAnim;
    [SerializeField, Tooltip("gives the kid a balloon")] private GameObject _kidBalloon;
    [SerializeField, Tooltip("silence, child")] private AudioSource _kidAudio;

    [Header("Colliders")]
    [SerializeField, Tooltip("used to detect collision with timmy or balloon cart")] private Collider2D _playerCollider;
    [SerializeField, Tooltip("used to check for player deposit of balloon")] private Collider2D _timmyCollider;
    [SerializeField, Tooltip("used to check for player pickup of balloon")] private Collider2D _balloonCartCollider;
    [SerializeField, Tooltip("disabled when balloon has been delivered")] private Collider2D _ringmasterCollider;

    private bool _hasBalloon = false;

    // Update is called once per frame
    void Update()
    {
        if (_playerCollider.IsTouching(_balloonCartCollider)) // pick up balloon
        {
            _hasBalloon = true;
        }

        if (_player.IsStomped && _hasBalloon) // balloon pop
        {
            _hasBalloon = false;
        }

        if(_playerCollider.IsTouching(_timmyCollider) && _hasBalloon) // deliver balloon
        {
            _ringmasterCollider.enabled = false; // disable enemy collider on ringmaster
            _hasBalloon = false; // no longer has balloon
            _kidBalloon.SetActive(true);
            _kidAnim.Play("ChildIdle");
            _kidAudio.enabled = false;
        }

        // show balloon object if player has balloon
        _balloonObject.SetActive(_hasBalloon);
    }

    IEnumerator PlayCry()
    {
        yield return null;
    }
}
