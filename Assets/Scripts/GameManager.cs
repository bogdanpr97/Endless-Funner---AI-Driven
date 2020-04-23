using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public Transform platformGenerator;
	private Vector3 platformStartPoint;

	public PlayerController thePlayer;
    public RobotController theRobot;
	private Vector3 playerStartPoint;

	private PlatformDestroyer[] platformList;
	private ScoreManager theScoreManager;

    public DeathMenu theResetScreen;

    public bool powerupReset;
    // Start is called before the first frame update
    void Start()
    {
        	platformStartPoint = platformGenerator.position;
        	playerStartPoint = thePlayer.transform.position;
        	theScoreManager = FindObjectOfType<ScoreManager>();
    }

    public void RestartGame() 
    {
        theScoreManager.scoreIncreasing = false;
        thePlayer.gameObject.SetActive(false);
        theResetScreen.gameObject.SetActive(true);
    	// StartCoroutine("RestartGameCo");
    }

    // this is for death menu
    public void Reset()
    {
        theResetScreen.gameObject.SetActive(false);
        platformList = FindObjectsOfType<PlatformDestroyer>();
        for(int i = 0; i < platformList.Length; i++)
        {
            platformList[i].gameObject.SetActive(false);
        }
        // thePlayer.transform.position = playerStartPoint;
        theRobot.transform.position = playerStartPoint + new Vector3(1.0f,0f,0f);
        platformGenerator.transform.position = platformStartPoint;
        // thePlayer.gameObject.SetActive(true);
        theRobot.gameObject.SetActive(true);

        theScoreManager.scoreCount = 0;
        theScoreManager.scoreIncreasing = true;
        powerupReset = true;
    }

    // public IEnumerator RestartGameCo()
    // {
    // 	theScoreManager.scoreIncreasing = false;

    // 	thePlayer.gameObject.SetActive(false);
    // 	yield return new WaitForSeconds(0.5f);
    // 	platformList = FindObjectsOfType<PlatformDestroyer>();
    // 	for(int i = 0; i < platformList.Length; i++)
    // 	{
    // 		platformList[i].gameObject.SetActive(false);
    // 	}
    // 	thePlayer.transform.position = playerStartPoint;
    // 	platformGenerator.transform.position = platformStartPoint;
    // 	thePlayer.gameObject.SetActive(true);

    // 	theScoreManager.scoreCount = 0;
    // 	theScoreManager.scoreIncreasing = true;

    // }
}
