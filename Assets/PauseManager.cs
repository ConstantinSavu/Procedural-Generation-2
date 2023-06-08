using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] public static bool gameIsPaused = false;
    [SerializeField] World world;
    [SerializeField] GameObject gamePauseCanvas;

    private float originalTimeScale;

    void Awake()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        gamePauseCanvas.SetActive(false);
    }

    void Start(){
        gameIsPaused = false;
        Time.timeScale = 1f;
        gamePauseCanvas.SetActive(false);
    }

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
        Time.timeScale = 1f;
        gameIsPaused = false;
        gamePauseCanvas.SetActive(false);
    }

    public void Pause(){
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        gameIsPaused = true;
        gamePauseCanvas.SetActive(true);
    }
}
