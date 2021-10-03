using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
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

    public void play()
    {
        SceneManager.LoadScene("SolarSystem");
    }

    public void quit()
    {
        Application.Quit();
    }
}

//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class UIManager : MonoBehaviour
//{
//    public GameObject cube; //prefab
//    public GameObject capsule; //prefab

//    public Transform SpawnLoc; //spawner location
//    public GameObject plane;
//    public Slider slider;

//    public void SpawnCube()
//    {
//        //random spawn
//        Vector3 randomSpawn = new Vector3(SpawnLoc.position.x + Random.Range(-10, 10), SpawnLoc.position.y, SpawnLoc.position.z + Random.Range(-10, 10));
//        Instantiate(cube, randomSpawn, SpawnLoc.rotation); //clones of cube prefab
//        //soawn with random scaling
//        cube.transform.localScale = Vector3.one * Random.Range(0.1f, 2.0f);
//    }

//    public void SpawnCap()
//    {
//        //random spawn
//        Vector3 randomSpawn = new Vector3(SpawnLoc.position.x + Random.Range(-5, 10), SpawnLoc.position.y, SpawnLoc.position.z + Random.Range(-5, 1));
//        Instantiate(capsule, randomSpawn, SpawnLoc.rotation); //clones of cube prefab
//        //soawn with random scaling
//        capsule.transform.localScale = Vector3.one * Random.Range(0.1f, 1.0f);

//    }

//    public void changeScenes(string scene)
//    {
//        SceneManager.LoadScene(scene);
//    }

//    public void movePlane(float pos)
//    {
//        plane.transform.position = new Vector3(pos, plane.transform.position.y, plane.transform.position.z);
//    }

//}