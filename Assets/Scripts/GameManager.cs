using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all save data throughout scenes and between sessions
/// </summary>
public class GameManager : MonoBehaviour
{
    private const float INIT_SPAWNPOINT_X = -17.16f;
    private const float INIT_SPAWNPOINT_Y = 1.93f;

    // instance - SINGLETON
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            // create instance if one doesn't already exist
            if (_instance == null)
            {
                GameObject manager = new GameObject("[Game Manager]");
                manager.AddComponent<GameManager>();
            }

            return _instance;
        }
    }

    // save data
    [System.Serializable]
    private class Data
    {
        public bool[] levers = new bool[4];
        public Vector2 spawnpoint;
        public bool hasWon;
    }
    private Data _data;

    // components
    private AudioSource _audioSource;

    #region UNITY METHODS
    private void Awake() // called each time a scene is loaded/reloaded
    {
        // setup SavePointManager as a singleton class
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // initialize default data
            _data = new Data();
            for(int i = 0; i < _data.levers.Length; i++)
                _data.levers[i] = false;
#if UNITY_EDITOR
            if (FindAnyObjectByType<SetSpawn>())
                _data.spawnpoint = new Vector2(FindAnyObjectByType<SetSpawn>().spawn.x, FindAnyObjectByType<SetSpawn>().spawn.y);
            else
                _data.spawnpoint = new Vector2(INIT_SPAWNPOINT_X, INIT_SPAWNPOINT_Y);
#else
            _data.spawnpoint = new Vector2(INIT_SPAWNPOINT_X, INIT_SPAWNPOINT_Y);
#endif
            _data.hasWon = false;


            // components
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
#endregion

#region DATA MODIFIERS

    /// <summary>
    /// true = lever flipped; false = lever not flipped
    /// </summary>
    public bool GetLever(int index) 
    {
        // error checking for invalid input
        if (index < 0 || index >= _data.levers.Length)
            Debug.LogError("GetLever(index): Invalid lever index input");

        return _data.levers[index];
    }

    /// <summary>
    /// set index lever to true (flipped)
    /// </summary>
    public void FlipLever(int index)
    {
        // error checking for invalid input
        if (index < 0 || index >= _data.levers.Length)
            Debug.LogError("FlipLever(index): Invalid lever index input");

        _data.levers[index] = true;
    }

    public Vector2 GetSpawnpoint()
    {
        return _data.spawnpoint;
    }
    
    /// <summary>
    /// should be called whenever a new side room is entered so the appropriate player spawn location in next scene is set
    /// </summary>
    public void SetSpawnpoint(Vector2 newSpawnpoint)
    {
        _data.spawnpoint = newSpawnpoint;
    }

    public bool HasWon()
    {
        return _data.hasWon;
    }

    public void SetWon()
    {
        _data.hasWon = true;
    }
#endregion

#region SCENE MANAGEMENT
    public void TransitionScene(string sceneName)
    {
        // load new scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void TransitionScene(int sceneIndex)
    {
        // load new scene
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }
#endregion

#region AUDIO
    public void PlaySound(AudioClip clip, int volume)
    {
        _audioSource.PlayOneShot(clip, volume);
    }
#endregion
}