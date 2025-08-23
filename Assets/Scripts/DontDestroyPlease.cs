using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyPlease : MonoBehaviour
{
    public string Tag = "music";
    void Start()
    {
        if (GameObject.FindGameObjectWithTag(Tag) == gameObject)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().name == "FinalScene")
        {
            Destroy(gameObject);
        }
    }
}
