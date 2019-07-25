using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Paused, Completed, Running };

public class gameTimer : MonoBehaviour
{
    public Text sceneTimer;
    public Text scoreText;

    public float currentTime;
    public float timeLimit = 600;

    private MapController mapControl;
    private GameObject gameOverPanel;

    public GameState gameState = GameState.Running;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = timeLimit;
        mapControl = GameObject.Find("Main Camera").GetComponent<MapController>();

        gameOverPanel = GameObject.Find("gameOver");
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (gameState == GameState.Running)
        {
            if (currentTime > 0.0f)
            {
                currentTime -= Time.fixedDeltaTime;
                int minutes = Mathf.FloorToInt(currentTime / 60.0f);
                int seconds = Mathf.FloorToInt(currentTime - (minutes * 60));
                sceneTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            if (currentTime <= 0.0f)
            {
                gameState = GameState.Completed;
                gameOverPanel.SetActive(true);
                scoreText.text = "Final Score" + "\n" + Mathf.FloorToInt(mapControl.score);
            }
        }
        
        else if (gameState == GameState.Paused)
        {

        }

        else if (gameState == GameState.Completed)
        {
            
        }
    }
}
