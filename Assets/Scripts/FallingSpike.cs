using DG.Tweening;
using System.Collections;
using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    Vector2 spawnpos;
    Quaternion spawnrot;
    public float delay;
    public float respawnafter = 6;
    public SpriteRenderer highlight;
    [Header("not recommended to change")]
    public int warns = 3;
    void Start()
    {
        spawnpos = transform.position;
        spawnrot = transform.rotation;
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            for (int i = 0; i < warns; i++)
            {
                yield return new WaitForSeconds(0.4f);
                highlight.color = Color.white;
                highlight.DOFade(0, 0.4f);
            }
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            yield return new WaitForSeconds(respawnafter);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            transform.position = spawnpos;
            transform.rotation = spawnrot;
        }
    }
}
