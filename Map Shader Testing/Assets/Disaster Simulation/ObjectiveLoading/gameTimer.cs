using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameTimer : MonoBehaviour
{
    public Text sceneTimer;
    public float curTime;
    public float timeLimit = 600;

    public enum GameState { Paused, Completed, Running };
    public GameState gameState = GameState.Running;

    // Start is called before the first frame update
    void Start()
    {
        curTime = timeLimit;
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
