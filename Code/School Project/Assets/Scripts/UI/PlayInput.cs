using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;

public class PlayInput : MonoBehaviour
{
    public Button confirm;
    public InputField inputPlayerOne;
    public InputField inputPlayerTwo;

    public string playerOneUsername;
    public string playerTwoUsername;

    // Start is called before the first frame update
    void Start()
    {
        confirm.onClick.AddListener(GetInputOnClickHandler);
    }

    // Update is called once per frame
    public void GetInputOnClickHandler()
    {
        //Names contain only letters, numbers and underscore, has length 4-16 characters, and not equal
        if(Regex.IsMatch(inputPlayerOne.text, @"^[a-zA-Z0-9_]+$") && Regex.IsMatch(inputPlayerTwo.text, @"^[a-zA-Z0-9_]+$")
            && inputPlayerOne.text.Length <= 16 && inputPlayerTwo.text.Length <= 16 && inputPlayerOne.text.Length >=4 && inputPlayerTwo.text.Length >= 4
            && inputPlayerOne.text != inputPlayerTwo.text)
        {
            //Set names, and show a button to start the game
            playerOneUsername = inputPlayerOne.text;
            playerTwoUsername = inputPlayerTwo.text;
            GameObject.FindGameObjectWithTag("StartGame").transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            //Minimise the start game button
            GameObject.FindGameObjectWithTag("StartGame").transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
