using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Minihealth : MonoBehaviour
{
    public Image FillUI;
    public TextMeshProUGUI text;
    public bool Paused;
    public bool StartPaused;
    public TextMeshProUGUI textHP;
    public GameObject warningSFX;
    void Start()
    {
        if (PlayerPrefs.HasKey("health"))
        {
            text.text = PlayerPrefs.GetInt("health").ToString();
        }
        else
        {
            text.text = CharacterController2D.maxhealth.ToString();
        }

        if (StartPaused)
        {
            Pause(true);
        }
        StartCoroutine(Loop());
    }

    public void UpdateHealth()
    {
        float value = (float)CharacterController2D.health / (float)CharacterController2D.maxhealth;
        FillUI.DOFillAmount(value, 0.25f);
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (!Paused)
            {
                CharacterController2D.health--;
            }
            float value = (float)CharacterController2D.health / (float)CharacterController2D.maxhealth;
            FillUI.DOFillAmount(value, 0.25f);
            text.text = CharacterController2D.health.ToString();

            if (CharacterController2D.health < 5 && CharacterController2D.health >= 0)
            {
                GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().alpha = 1;
                GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOFade(0, 1);
                Instantiate(warningSFX);
            }
            string val = CharacterController2D.health.ToString();
            textHP.text = val;
        }
    }

    public void Pause(bool hidetext = false)
    {
        Paused = true;
        if (!hidetext)
        {
            text.text = "PAUSED";
        }
    }
    public void Resume()
    {
        Paused = false;
        text.text = CharacterController2D.health.ToString();
    }
    public void Restart()
    {
        print(3);
        PlayerPrefs.DeleteKey("health");
        SceneManager.LoadScene("IntroScene");
    }
}
