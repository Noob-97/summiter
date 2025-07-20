using DG.Tweening;
using System.Collections;
using UnityEngine;

public class WaterCollectable : MonoBehaviour
{
    public int HPPlus = 5;
    public SpriteRenderer sprite1;
    public SpriteRenderer sprite2;

    public void Start()
    {
        StartCoroutine(Anim());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "PlayerComposite")
        {
            GameObject.Find("PlayerComposite").GetComponent<CharacterController2D>().SingleHeal(HPPlus);
            Destroy(gameObject);
        }
    }

    IEnumerator Anim()
    {
        while (true)
        {
            sprite1.color = new Color(sprite1.color.r, sprite1.color.g, sprite1.color.b, 1);
            sprite2.color = new Color(sprite2.color.r, sprite2.color.g, sprite2.color.b, 1);
            sprite1.DOFade(0, 0.5f);
            sprite2.DOFade(0, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
