using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
	private Controls _controls;
    public GameObject PauseMenu; 

	private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }    

    public void ResumeGame()
    {
		Cursor.lockState = CursorLockMode.Locked;
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void PauseGame()
    {
		Cursor.lockState = CursorLockMode.None;
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }    

    public void Menu(string menuScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);
    }

	public void Update(){
		if(_controls.Player.Pause.IsPressed()){
			PauseGame();
        }
	}
}
