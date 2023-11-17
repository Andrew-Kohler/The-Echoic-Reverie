using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalButton : MonoBehaviour
{
    public void EndGame()
    {
        GameManager.Instance.SetWon();
        SceneManager.LoadScene(0);
    }
}
