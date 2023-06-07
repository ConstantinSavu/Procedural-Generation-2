using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool gameIsPaused = false;
    [SerializeField] World world;
    [SerializeField] GameObject gamePauseCanvas;

    private float originalTimeScale;

    void Update()
    {
        if(!world.IsWorldCreated){
                return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)){

            if(gameIsPaused){
                Resume();
            }
            else{
                Pause();
            }
            
        }
    }

    public void Resume(){
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = originalTimeScale;
        gameIsPaused = false;
        gamePauseCanvas.SetActive(false);
    }

    public void Pause(){
        Cursor.lockState = CursorLockMode.None;
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        gameIsPaused = true;
        gamePauseCanvas.SetActive(true);
    }
}
