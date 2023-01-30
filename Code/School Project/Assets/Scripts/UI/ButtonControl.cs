using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour
{
    //these functions are used to allow functions to occur when a button is pressed
    public void offlineButton()
    {
        //loads the next scene in the build manager section
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void quitButton()
    {
        //Quits the application
        Application.Quit();
    }
}
