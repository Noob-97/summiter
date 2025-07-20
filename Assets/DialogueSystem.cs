using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Dialogue
{
    public Sprite face;
    public string sentence;
    public bool DontAnimate;
    public bool AutoEnter;
    public float SecTillNext = 0f;
    public float TextSpeed = 0.02f;
}
public class DialogueSystem : MonoBehaviour
{
    public bool ExecuteOnStart;
    public bool DialogComplete;
    public Dialogue[] Dialogues;
    TextMeshProUGUI text;
    Image faceimage;
    AudioSource audiosource;
    Animator animator;
    private bool DialogDone;
    private bool PressedNextSentence;
    private bool PressedNextWhileType;
    public UnityEvent ConversationFinished;
    public UnityEvent<int> FinishedDialogue;
    public void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        faceimage = transform.Find("Portrait").GetComponent<Image>();
        animator = GetComponentInChildren<Animator>();
        audiosource = GetComponent<AudioSource>();
        if (ExecuteOnStart)
        {
            RunConversation();
        }
    }
    public void RunConversation()
    {
        StartCoroutine(WaitForEnter());
        DialogComplete = false;
    }
    public IEnumerator WaitForEnter()
    {
        //Debug.Log(TEST.name);
        DialogDone = false;
        GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        for (int i = 0; i < Dialogues.Length; i++)
        {
            PressedNextSentence = false;
            PressedNextWhileType = false;

            StartCoroutine(Type(Dialogues[i].sentence, Dialogues[i].TextSpeed, Dialogues[i].DontAnimate));

            faceimage.sprite = Dialogues[i].face;
            if (!Dialogues[i].DontAnimate) { animator.Play("taling"); }

            yield return new WaitUntil(() => Continue(Dialogues[i].AutoEnter) && text.text == Dialogues[i].sentence);
            
            yield return new WaitForSeconds(Dialogues[i].SecTillNext);
            text.text = "";
            FinishedDialogue.Invoke(i);
            if (i == Dialogues.Length - 1)
            {
                GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                DialogComplete = true;
                ConversationFinished.Invoke();
            }
        }
    }
    public void GetNextDialogue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (DialogDone)
            {
                PressedNextSentence = true;
            }
            if (!DialogDone)
            {
                PressedNextWhileType = true;
            }
        }
    }
    public bool Continue(bool Auto)
    {
        if (PressedNextSentence)
        {
            return true;
        }
        else if (Auto)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public IEnumerator Type(string sentence, float WriteSpeed, bool DontAnimate)
    {
        foreach (char letter in sentence.ToCharArray())
        {
            text.text += letter;
            audiosource.pitch = Random.Range(-2f, 2f);
            audiosource.Play();
            yield return new WaitForSeconds(WriteSpeed);
            if (text.text == sentence)
            {
                DialogDone = true;
                if (!DontAnimate) { animator.Play("talingNL"); }
            }
            else
            {
                DialogDone = false;
            }
            if (PressedNextWhileType)
            {
                WriteSpeed = 0.0025f;
            }
        }
    }
}