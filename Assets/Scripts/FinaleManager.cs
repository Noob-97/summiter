using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinaleManager : MonoBehaviour
{
    public GameObject logo;
    public GameObject madeby;
    public GameObject forthe;
    public GameObject audioassets;
    public GameObject musicby;
    public GameObject madein;
    public GameObject specialthx;
    public GameObject andyou;
    public GameObject thxforplaying;
    public void GetEndedDialogue(int dialogueIndex)
    {
        switch (dialogueIndex)
        {
            case 0:
                logo.SetActive(true);
                logo.transform.Find("text").GetComponent<TextMeshProUGUI>().DOFade(1, 0.25f);
                break;
            case 1:
                madeby.SetActive(true);
                madeby.GetComponent<TextMeshProUGUI>().DOFade(1, 0.25f);
                break;
            case 2:
                madeby.SetActive(false);
                forthe.SetActive(true);
                forthe.GetComponent<TextMeshProUGUI>().DOFade(1, 0.25f);
                break;
            case 3:
                forthe.SetActive(false);
                audioassets.SetActive(true);
                audioassets.GetComponent<TextMeshProUGUI>().DOFade(1, 0.25f);
                break;
            case 4:
                audioassets.SetActive(false);
                musicby.SetActive(true);
                musicby.GetComponent<TextMeshProUGUI>().DOFade(1, 0.25f);
                break;
            case 5:
                musicby.SetActive(false);
                madein.SetActive(true);
                madein.GetComponent<TextMeshProUGUI>().DOFade(1, 0.25f);
                break;
            case 6:
                madein.SetActive(false);
                specialthx.SetActive(true);
                specialthx.GetComponent<TextMeshProUGUI>().DOFade(1, 0.25f);
                break;
            case 7:
                specialthx.SetActive(false);
                andyou.SetActive(true);
                andyou.GetComponent<TextMeshProUGUI>().DOFade(1, 0.25f);
                break;
            case 8:
                andyou.SetActive(false);
                thxforplaying.SetActive(true);
                thxforplaying.GetComponent<TextMeshProUGUI>().DOFade(1, 0.25f);
                break;
        }
    }

    public void GameFinished()
    {
        StartCoroutine(ByeBye());
    }

    IEnumerator ByeBye()
    {
        yield return new WaitForSeconds(3f);
        GameObject.FindGameObjectWithTag("black").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        PlayerPrefs.DeleteKey("spawnX");
        PlayerPrefs.DeleteKey("spawnY");
        PlayerPrefs.DeleteKey("lastlvl");
        SceneManager.LoadScene("IntroScene");
    }
}
