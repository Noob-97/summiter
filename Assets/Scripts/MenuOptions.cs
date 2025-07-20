using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class MenuOptions : MonoBehaviour
{
    public PlayableDirector cinematic;
    public GameObject txtbox;
    public GameObject nosavefile;

    public void StartNew()
    {
        StartCoroutine(Transition("start"));
    }
    public void Continue()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("lastlvl")))
        {
            nosavefile.SetActive(true);
        }
        else
        {
            StartCoroutine(Transition("continue"));
        }
    }
    public void Exit()
    {
        StartCoroutine(Transition("exit"));
    }
    IEnumerator Transition(string operation)
    {
        GameObject.FindGameObjectWithTag("black").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        if (operation == "start")
        {
            PlayerPrefs.DeleteKey("spawnX");
            PlayerPrefs.DeleteKey("spawnY");
            SceneManager.LoadScene("Level1");
        }
        else if (operation == "continue")
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("lastlvl"));
        }
        else if (operation == "exit")
        {
            Application.Quit();
        }
    }
    public void GetSkip(CallbackContext context)
    {
        if (context.performed)
        {
            SkipIntro();
        }
    }

    public void SkipIntro()
    {
        if (cinematic.time < 73.1f)
        {
            cinematic.time = 73.1f;
            cinematic.Play();
            txtbox.SetActive(false);
        }
    }
}
