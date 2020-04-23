using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
	public Text scoreText;
	public Text highscoreText;

	public float scoreCount;
	public float highscoreCount;

	public float pointsPerSecond;

	public bool scoreIncreasing;
    // Start is called before the first frame update
    void Start()
    {
    	scoreIncreasing = true;
        if(PlayerPrefs.HasKey("HighScore"))
        {
        	highscoreCount = PlayerPrefs.GetFloat("HighScore");
        }
    }

    // Update is called once per frame
    void Update()
    {
    	if(scoreIncreasing)
    	{
    	scoreCount += pointsPerSecond * Time.deltaTime;
    	}

    	if(scoreCount > highscoreCount) 
    	{
    		highscoreCount = scoreCount;
    		PlayerPrefs.SetFloat("HighScore", highscoreCount);
    	}
        scoreText.text = "Score: " + Mathf.Round(scoreCount);
        highscoreText.text = "Highscore: " + Mathf.Round(highscoreCount);

    }

    public void AddScore(int pointsToAdd)
    {
    	scoreCount += pointsToAdd;
    }
}
