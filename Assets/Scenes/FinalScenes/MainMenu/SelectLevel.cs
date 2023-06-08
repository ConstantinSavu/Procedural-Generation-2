using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{
    public void LoadMainMenu(){
        
        SceneManager.LoadScene(0);
    }
    public void LoadPlains(){
        SceneManager.LoadScene(1);
    }

    public void LoadDesert(){
        SceneManager.LoadScene(2);
    }

    public void LoadIsland(){
        SceneManager.LoadScene(3);
    }

    public void LoadCave(){
        SceneManager.LoadScene(4);
    }

    public void LoadNextLevel(){
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}
