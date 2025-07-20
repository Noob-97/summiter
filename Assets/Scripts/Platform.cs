using DG.Tweening;
using System.Collections;
using UnityEngine;
public enum PlatformType
{
    Moving,
    Falling,
    Timed,
}
public class Platform : MonoBehaviour
{
    public PlatformType type;
    public SpriteRenderer highlight;
    public float Delay;
    public float Activation = 0.75f;
    [Header("Moving Platform")]
    public Transform Destination;
    public bool FlipSprite;
    LineRenderer line;
    public float MovingSec = 0.5f;
    Vector3 Origin;
    Vector3 dest;
    [Header("Falling Platform")]
    public Vector2 Direction;
    public float Velocity;
    public float SpawnPoint;
    public float Threshold;
    [Header("Timed Platform")]
    public float TimeOffset;
    Rigidbody2D rb;

    void Start()
    {
        if (type == PlatformType.Moving)
        {
            Origin = transform.position;
            dest = Destination.position;
            StartCoroutine(MovingLoop());
            line = GetComponent<LineRenderer>();
            line.SetPosition(0, Origin);
            line.SetPosition(1, dest);
        }
        if (type == PlatformType.Falling)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (type == PlatformType.Timed)
        {
            StartCoroutine(TimedLoop());
        }
    }

    IEnumerator MovingLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Delay);
            highlight.color = Color.white;
            highlight.DOFade(0, Activation / 3 * 2);
            yield return new WaitForSeconds(Activation);
            transform.DOMove(dest, MovingSec);
            if (FlipSprite) { GetComponent<SpriteRenderer>().flipX = false; }
            yield return new WaitForSeconds(MovingSec);

            yield return new WaitForSeconds(Delay);
            highlight.color = Color.white;
            highlight.DOFade(0, Activation / 3 * 2);
            yield return new WaitForSeconds(Activation);
            transform.DOMove(Origin, MovingSec);
            if (FlipSprite) { GetComponent<SpriteRenderer>().flipX = true; }
            yield return new WaitForSeconds(MovingSec);
        }
    }

    IEnumerator TimedLoop()
    {
        yield return new WaitForSeconds(TimeOffset);

        while (true)
        {
            yield return new WaitForSeconds(Delay);
            GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().DOFade(1, Activation / 6);
            highlight.color = Color.white;
            highlight.DOFade(0, Activation / 6);
            yield return new WaitForSeconds(Activation / 6 * 4);
            highlight.color = Color.white;
            highlight.DOFade(0, Activation / 6);
            yield return new WaitForSeconds(Activation / 6 * 2);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().DOFade(0.25f, Activation / 6);
            highlight.color = Color.white;
            highlight.DOFade(0, Activation / 6);
        }
    }

    bool highlightflag;
    private void Update()
    {
        if (type == PlatformType.Falling)
        {
            rb.MovePosition(rb.position + (Direction * Velocity * Time.deltaTime));
            if (Direction == Vector2.down)
            {
                if (transform.localPosition.y < Threshold)
                {
                    transform.localPosition = new Vector2(transform.localPosition.x, SpawnPoint);
                    highlight.color = Color.white;
                    highlight.DOFade(0, Activation / 3 * 2);
                    highlightflag = false;
                }
                if (transform.localPosition.y < Threshold + 2 && highlightflag == false)
                {
                    highlight.color = Color.white;
                    highlight.DOFade(0, Activation / 3 * 2);
                    highlightflag = true;
                }
            }
            if (Direction == Vector2.up)
            {
                if (transform.localPosition.y > Threshold)
                {
                    transform.localPosition = new Vector2(transform.localPosition.x, SpawnPoint);
                    highlight.color = Color.white;
                    highlight.DOFade(0, Activation / 3 * 2);
                    highlightflag = false;
                }
                if (transform.localPosition.y > Threshold + 2 && highlightflag == false)
                {
                    highlight.color = Color.white;
                    highlight.DOFade(0, Activation / 3 * 2);
                    highlightflag = true;
                }
            }
        }
    }
}
