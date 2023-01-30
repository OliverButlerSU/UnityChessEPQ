using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private GameObject controller;
    private bool paused = true;
    private float currentTime = 900; //15 minutes in seconds
    public Text timeText;
    
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!paused && controller.GetComponent<Main>().canMove)
        {
            if (currentTime > 0)
            {
                //calculate change in time per frame
                currentTime -= Time.deltaTime;
                convertToText();
            }
            else
            {  
                //user runs out of time
                currentTime = 0f;
                convertToText();
                //ends the game
                Main endGame = controller.GetComponent<Main>();
                endGame.gameEnd(endGame.currentPlayer);
            }
        }
    }

    public void cyclePause()
    {
        paused = !paused;
    }

    public void convertToText()
    {
        //converting float to text in time format of minute : second : milliseconds
        timeText.text = string.Format("{0:00}:{1:00}:{2:000}", Mathf.FloorToInt(currentTime / 60), Mathf.FloorToInt(currentTime % 60), Mathf.FloorToInt(currentTime % 1 * 1000));
    }

    public void stopTimer()
    {
        paused = true;
    }
}
