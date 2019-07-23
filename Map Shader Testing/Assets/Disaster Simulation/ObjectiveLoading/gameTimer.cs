using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameTimer : MonoBehaviour
{
    public Text sceneTimer;
    public Text scoreText;

    public float curTime;
    public float timeLimit = 600;

    private MapController mapControl;
    private GameObject gameOverPanel;

    public enum GameState { Paused, Completed, Running };
    public GameState gameState = GameState.Running;

    // Start is called before the first frame update
    void Start()
    {
        curTime = timeLimit;
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
            if (curTime > 0.0f)
            {
                curTime -= Time.fixedDeltaTime;
                int minutes = Mathf.FloorToInt(curTime / 60.0f);
                int seconds = Mathf.FloorToInt(curTime - (minutes * 60));
                sceneTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            if (curTime <= 0.0f)
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
