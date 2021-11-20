using UnityEngine;
using UnityEngine.SceneManagement;

public class EndUI : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void changeScenes(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void restart()
    {
        SceneManager.LoadScene("SolarSystem");
    }

    public void quit()
    {
        Application.Quit();
    }
}