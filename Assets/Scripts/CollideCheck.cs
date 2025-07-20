using UnityEngine;
using UnityEngine.Events;

public class CollideCheck : MonoBehaviour
{
    public UnityEvent<Vector2> HitByPlayerOnce;
    public UnityEvent HitByPlayer;
    public UnityEvent HitByCharacter;
    public UnityEvent ExitedByCharacter;
    public UnityEvent StayByCharacter;
    public UnityEvent OnGlacier;
    [Header("only for glacier")]
    public GameObject PlayerRef;
    [Header("only for checkpoint")]
    public Vector2 ckspot;
    bool checkglaciercrouch;
    bool playeronce;
    Animator anim;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterController2D>() != null)
        {
            HitByCharacter.Invoke();
            if (collision.name == "PlayerComposite")
            {
                HitByPlayer.Invoke();
                if (!playeronce)
                {
                    HitByPlayerOnce.Invoke(ckspot);
                    playeronce = true;
                    if (ckspot != Vector2.zero)
                    {
                        anim = GetComponent<Animator>();
                        anim.Play("raiseflag");
                    }
                }
            }
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterController2D>() != null)
        {
            ExitedByCharacter.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterController2D>() != null)
        {
            StayByCharacter.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        checkglaciercrouch = true;
        OnGlacier.Invoke();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        checkglaciercrouch = false;
    }

    public void CheckGlacier()
    {
        if (checkglaciercrouch)
        {
            transform.SetParent(PlayerRef.transform);
        }
    }
    public void StopGlacier()
    {
        if (checkglaciercrouch)
        {
            transform.SetParent(null);
        }
    }
}
