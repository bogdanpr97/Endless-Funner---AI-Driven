using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public string mainMenuLevel;

	public GameObject pauseMenu;

	public void PauseGame()
	{
		Time.timeScale = 0f;
		pauseMenu.SetActive(true);
	}

	public void ResumeGame()
	{
		Time.timeScale = 1f;
		pauseMenu.SetActive(false);
	}

	public void RestartGame()
	{
		pauseMenu.SetActive(false);
		Time.timeScale = 1f;
		FindObjectOfType<GameManager>().Reset();
	}

	public void QuitToMain()
	{
		pauseMenu.SetActive(false);
		Time.timeScale = 1f;
		SceneManager.LoadScene(mainMenuLevel);
	}
}
