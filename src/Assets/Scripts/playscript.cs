using UnityEngine;
using UnityEngine.SceneManagement;

public class playscript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Quit()
    {
        Application.Quit();
    }

    public void LoadScene()
    {
        Debug.Log("Button clicked!");
        SceneManager.LoadScene("intro");
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs have been cleared.");
    ;
    }
    public void LoadIntro()
    {
        Debug.Log("Button clicked!");
        SceneManager.LoadScene("intro");
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs have been cleared.");
    ;
    }

}
