using UnityEngine;

public class DontDestroyPlease : MonoBehaviour
{
    public string Tag = "music";
    void Start()
    {
        if (GameObject.FindGameObjectWithTag(Tag) == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
