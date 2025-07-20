using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public enum ControlMode
{
    None,
    PlayerInput,
    Simulated,
    Follower
}
[Serializable]
public class MovementAction
{
    public Vector2 direction;
    public float time;
}
public class FollowData
{
    public Vector2 Position;
    public Quaternion Rotation;
    public string Animation;
}

public class CharacterController2D : MonoBehaviour
{
    [Header("General")]
    Vector2 dir;
    Rigidbody2D rb;
    Animator anim;
    public float vel = 7;
    public static int health;
    public static int maxhealth = 30;
    public bool DiveAllowed;
    public ControlMode Mode;
    public Sprite HurtSprite;
    public Sprite DeadSprite;
    public float WorldBoundaryY = -15;
    public int BoundaryDamage = 10;
    public Vector2 SpawnPoint;
    public float FallMultiplier;
    public GameObject healsfx;
    public GameObject hurtsfx;
    public GameObject watersfx;
    public GameObject warnsfx;
    public GameObject deathsfx;
    public GameObject doorsfx;
    [Header("Progression")]
    public bool JumpEnabled;
    public bool RunEnabled;
    public float JumpForce = 15;
    public Transform OnGroundRef;
    public LayerMask CheckLayers;
    [Header("Follower")]
    public Transform FollowerTarget;
    public int QueuePosition;
    public int latencyBetween;
    public float YOffset;
    private Queue<FollowData> playerpositions = new Queue<FollowData>();
    private Vector2 lastplayerpos;
    private bool CanMove;
    private FollowData CurrentFollowData;
    [Header("DEBUG")]
    public bool FlyEnabled;
    public MovementAction[] Actions;
    public UnityEvent FinishedActions;
    public UnityEvent OnCrouch;
    public UnityEvent StopCrouch;
    bool heal;
    bool lerpvig;
    float t;
    bool respawning;
    private void Start()
    {   
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 4:
                maxhealth = 120;
                break;
        }

        health = maxhealth;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (Mode == ControlMode.Simulated)
        {
            StartCoroutine(ExecuteActions());
        }
        if (Mode == ControlMode.Follower)
        {
            ConfigureFollower();
        }
        PlayerPrefs.SetString("lastlvl", SceneManager.GetActiveScene().name);

        if (PlayerPrefs.HasKey("spawnX"))
        {
            SpawnPoint = new Vector2(PlayerPrefs.GetFloat("spawnX"), PlayerPrefs.GetFloat("spawnY"));
            transform.position = SpawnPoint;
        }
    }
    public void ConfigureFollower()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        transform.Find("BODY").Find("referbody_0").GetComponent<SpriteRenderer>().DOFade(0.2f, 0.25f);
        transform.Find("LHAND").Find("referlh_0").GetComponent<SpriteRenderer>().DOFade(0.2f, 0.25f);
        transform.Find("RHAND").Find("referrh_0").GetComponent<SpriteRenderer>().DOFade(0.2f, 0.25f);
        transform.Find("LFOOT").Find("referlf_0").GetComponent<SpriteRenderer>().DOFade(0.2f, 0.25f);
        transform.Find("RFOOT").Find("referrf_0").GetComponent<SpriteRenderer>().DOFade(0.2f, 0.25f);
    }

    IEnumerator ExecuteActions()
    {
        for (int i = 0; i < Actions.Length; i++)
        {
            MovementAction action = Actions[i];
            dir = action.direction;
            UpdateAnim();
            yield return new WaitForSeconds(action.time);
        }
        dir = Vector2.zero;
        UpdateAnim();
        FinishedActions.Invoke();
    }

    public void Flip()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z);
    }

    public void GetMovement(CallbackContext context)
    {
        if (Mode == ControlMode.PlayerInput || fromcrouch)
        {
            if (context.performed)
            {
                dir = new Vector2(context.ReadValue<float>(), dir.y);
            }
            else if (context.canceled)
            {
                dir = new Vector2(0, dir.y);
            }
            if (fromcrouch)
            {
                UpdateAnim("crouch");
            }
            else
            {
                UpdateAnim();
            }
        }
    }

    public void GetDive(CallbackContext context)
    {
        if (DiveAllowed && Mode == ControlMode.PlayerInput)
        {
            if (context.performed)
            {
                dir = new Vector2(dir.x, context.ReadValue<float>());
            }
            else if (context.canceled)
            {
                dir = new Vector2(dir.x, 0);
            }
            UpdateAnim();
        }
        else
        {
            dir = new Vector2(dir.x, 0);
        }

        if (FlyEnabled && flying)
        {
            dir = new Vector2(dir.x, context.ReadValue<float>());
        }
    }

    public void Dive()
    {
        if (rb != null)
        {
            rb.freezeRotation = false;
        }
        DiveAllowed = true;
    }

    bool fromcrouch;
    public void GetCrouch(CallbackContext context)
    {
        if (context.performed && Mode == ControlMode.PlayerInput)
        {
            OnCrouch.Invoke();
            ChangeMode("None");
            UpdateAnim("crouch");
            fromcrouch = true;
            vel = 7;
        }
        else if (context.canceled && Mode == ControlMode.None && fromcrouch)
        {
            StopCrouch.Invoke();
            ChangeMode("PlayerInput");
            UpdateAnim("idle");
            fromcrouch = false;
            vel = 4;
        }
    }

    bool DiveDirection;
    public void GetDiveDirection(CallbackContext context)
    {
        if (context.performed && DiveAllowed)
        {
            rb.freezeRotation = true;
            DiveDirection = true;


        }
        else if (context.canceled && DiveAllowed)
        {
            rb.freezeRotation = false;
            DiveDirection = false;
        }
    }

    bool dying;
    public void Death()
    {
        ChangeMode("None");
        rb.freezeRotation = false;
        rb.AddForce(UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(5f, 15f), ForceMode2D.Impulse);

        transform.Find("BODY").Find("protbody_0").GetComponent<SpriteRenderer>().sprite = DeadSprite;
        transform.Find("BODY").Find("protbody_0 (1)").GetComponent<SpriteRenderer>().sprite = DeadSprite;

        Instantiate(deathsfx);
        GameObject.FindGameObjectWithTag("music").GetComponent<AudioSource>().DOFade(0, 0.25f);
        StartCoroutine(ReSpawn(true));
        //GameObject.FindGameObjectWithTag("death").transform.DOScaleY(1, 0.25f).SetDelay(7);
    }

    public void Hit(int damage)
    {
        health = health - damage;
        GameObject.FindGameObjectWithTag("hp").GetComponent<TextMeshProUGUI>().text = health.ToString();
        StartCoroutine(ShowHurtSprite());
        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOKill();
        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(3);
        GameObject.FindGameObjectWithTag("vol").GetComponent<Volume>().profile.TryGet(out Vignette vig);
        vig.intensity.value = 0.35f;
        vig.color.value = new Color(1, 0, 0.02794027f);
        lerpvig = true;
        t = 0;
        float value = (float)health / (float)maxhealth;
        GameObject.FindGameObjectWithTag("uifill").GetComponent<Image>().DOFillAmount(value, 0.25f);
        GameObject.FindGameObjectWithTag("mini").GetComponent<Minihealth>().UpdateHealth();
        Instantiate(hurtsfx);
    }

    public void Heal(int HPperSec)
    {
        heal = true;
        StartCoroutine(HealLoop(HPperSec));
    }

    public void StopHeal()
    {
        heal = false;
    }

    public void SingleHeal(int HPPlus)
    {
        GameObject.FindGameObjectWithTag("hp").GetComponent<TextMeshProUGUI>().text = health.ToString();
        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOKill();
        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(3);

        if (health == maxhealth)
        {
            heal = false;
        }

        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOKill();
        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(3);

        if (health + HPPlus > maxhealth)
        {
            health = maxhealth;
        }
        else
        {
            health += HPPlus;
        }

        float value = (float)health / (float)maxhealth;
        GameObject.FindGameObjectWithTag("uifill").GetComponent<Image>().DOFillAmount(value, 0.25f);
        GameObject.FindGameObjectWithTag("mini").GetComponent<Minihealth>().UpdateHealth();

        GameObject.FindGameObjectWithTag("vol").GetComponent<Volume>().profile.TryGet(out Vignette vig);
        vig.intensity.value = 0.35f;
        vig.color.value = new Color(0, 1, 0.5f);
        lerpvig = true;
        t = 0;

        Instantiate(healsfx);

    }
    IEnumerator HealLoop(int intervals)
    {

        while (heal)
        {
            if (health + intervals > maxhealth)
            {
                health = maxhealth;
            }
            else
            {
                health += intervals;
            }

            GameObject.FindGameObjectWithTag("hp").GetComponent<TextMeshProUGUI>().text = health.ToString();
            print(health);
            print(GameObject.FindGameObjectWithTag("hp").GetComponent<TextMeshProUGUI>().text);

            float value = (float)health / (float)maxhealth;
            GameObject.FindGameObjectWithTag("uifill").GetComponent<Image>().DOFillAmount(value, 0.25f);
            GameObject.FindGameObjectWithTag("mini").GetComponent<Minihealth>().UpdateHealth();

            GameObject.FindGameObjectWithTag("vol").GetComponent<Volume>().profile.TryGet(out Vignette vig);
            vig.intensity.value = 0.35f;
            vig.color.value = new Color(0, 1, 0.5f);
            lerpvig = true;
            t = 0;

            GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOKill();
            GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(3);

            Instantiate(healsfx);

            yield return new WaitForSeconds(1);

            if (health == maxhealth)
            {
                heal = false;
            }

            GameObject.FindGameObjectWithTag("hp").GetComponent<TextMeshProUGUI>().text = health.ToString();
            GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOKill();
        }
        GameObject.FindGameObjectWithTag("hpalpha").GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetDelay(2);
    }

    IEnumerator ShowHurtSprite()
    {
        Sprite previous = transform.Find("BODY").Find("protbody_0").GetComponent<SpriteRenderer>().sprite;
        transform.Find("BODY").Find("protbody_0").GetComponent<SpriteRenderer>().sprite = HurtSprite;
        transform.Find("BODY").Find("protbody_0 (1)").GetComponent<SpriteRenderer>().sprite = HurtSprite;

        yield return new WaitForSeconds(3);

        transform.Find("BODY").Find("protbody_0").GetComponent<SpriteRenderer>().sprite = previous;
        transform.Find("BODY").Find("protbody_0 (1)").GetComponent<SpriteRenderer>().sprite = previous;
    }

    public void WaterReposition()
    {
        rb.rotation = 0;
        rb.freezeRotation = true;
        DiveAllowed = false;
        dir = new Vector2(dir.x, 0);
    }

    public void WaterSFX()
    {
        Instantiate(watersfx);
    }

    public void SetCheckpoint(Vector2 spawnpoint)
    {
        PlayerPrefs.SetFloat("spawnX", spawnpoint.x);
        PlayerPrefs.SetFloat("spawnY", spawnpoint.y);
    }

    public void ChangeMode(string mode)
    {
        Mode = (ControlMode)Enum.Parse(typeof(ControlMode), mode);
        if (Mode == ControlMode.Simulated)
        {
            StartCoroutine(ExecuteActions());
        }
        if (Mode == ControlMode.Follower)
        {
            ConfigureFollower();
        }
        if (Mode == ControlMode.None)
        {
            dir = Vector2.zero;
        }
    }

    void UpdateAnim(string overrideAnim = "none")
    {
        if (overrideAnim == "none")
        {
            if (dir == Vector2.zero)
            {
                anim.Play("idle");
            }
            else if (dir == Vector2.right)
            {
                anim.Play("walking");
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
            }
            else if (dir == Vector2.left)
            {
                anim.Play("walking");
                transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);
            }
        }
        else
        {
            anim.Play(overrideAnim);
        }
    }

    IEnumerator ReSpawn(bool wait7 = false)
    {
        if (wait7)
        {
            yield return new WaitForSeconds(7);
        }
        GameObject.FindGameObjectWithTag("black").GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void CatchGrab()
    {
        isgrabbing = true;
    }

    public void CatchStopGrab()
    {
        isgrabbing = false;
    }

    public void GetJump(CallbackContext context)
    {
        if (context.performed)
        {
            jumpflag = true;
        }
        else
        {
            jumpflag = false;
        }
    }

    public void EnableJump()
    {
        JumpEnabled = true;
    }

    bool runflag;
    public void GetRun(CallbackContext context)
    {
        if (context.performed && RunEnabled)
        {
            runflag = true;
            vel = vel * 2;
        }
        else if (runflag)
        {
            runflag = false;
            vel = vel / 2;
        }
    }

    public void EnableRun()
    {
        RunEnabled = true;
    }

    public void SetVelocity(int velocity)
    {
        vel = velocity;
    }

    bool isgrabbing;
    bool jumpflag;
    bool flying;
    void FixedUpdate()
    {
        if (isgrabbing)
        {
            rb.AddForce(new Vector2(dir.x * vel, rb.linearVelocity.y));
        }
        else if (fromcrouch)
        {
            rb.AddForce(new Vector2(dir.x * vel * 2, 0));
        }
        else if (Mode == ControlMode.Follower)
        {
            if (FollowerTarget.position != (Vector3)lastplayerpos)
            {
                FollowData followdata = new FollowData();
                followdata.Position = FollowerTarget.position + new Vector3(0, YOffset);
                followdata.Rotation = FollowerTarget.rotation;
                followdata.Animation = FollowerTarget.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name;
                playerpositions.Enqueue(followdata);
                lastplayerpos = FollowerTarget.position;

                if (playerpositions.Count >= latencyBetween * QueuePosition)
                {
                    CanMove = true;
                }
                if (CanMove)
                {
                    CurrentFollowData = playerpositions.Dequeue();
                    transform.position = CurrentFollowData.Position;
                    transform.rotation = CurrentFollowData.Rotation;
                    if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != CurrentFollowData.Animation)
                    {
                        anim.Play(CurrentFollowData.Animation);
                    }
                }
            }
        }
        else
        {
            Collider2D INFO = Physics2D.OverlapCapsule(OnGroundRef.position, new Vector2(1.1f, 0.2f), CapsuleDirection2D.Horizontal, 0, CheckLayers);

            if (jumpflag && JumpEnabled && INFO)
            {
                rb.linearVelocity = new Vector2(dir.x * vel, JumpForce);
                if (transform.parent.CompareTag("plat"))
                {
                    transform.SetParent(null);
                }
            }
            else if (DiveAllowed)
            {
                rb.linearVelocity = new Vector2(dir.x * vel, dir.y * (vel / 8) + rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(dir.x * vel, rb.linearVelocity.y);
                if (INFO)
                {
                    if (INFO.CompareTag("plat"))
                    {
                        transform.SetParent(INFO.transform);
                    }
                }
            }
        }

        //rb.MovePosition(rb.position + (dir * vel * Time.deltaTime));
        // Boundary
        if (transform.position.y < WorldBoundaryY && Mode == ControlMode.PlayerInput)
        {
            Hit(BoundaryDamage);
            StartCoroutine(ReSpawn());
        }
        // Lerp Vig
        if (lerpvig)
        {
            t += Time.deltaTime;
            float percentage = t / 2;
            GameObject.FindGameObjectWithTag("vol").GetComponent<Volume>().profile.TryGet(out Vignette vig);
            vig.intensity.value = Mathf.Lerp(0.35f, 0, percentage);
            if (vig.intensity.value == 0)
            {
                lerpvig = false;
            }
        }
        // Dive Direction
        if (DiveDirection)
        {
            Vector3 MousePos = Input.mousePosition;
            MousePos = Camera.main.ScreenToWorldPoint(MousePos);
            Vector2 Direction = new Vector2(MousePos.x - transform.position.x, MousePos.y - transform.position.y);
            transform.up = Direction;
        }
        // Gravity Increase
        if (rb.linearVelocityY < 0)
        {
            rb.linearVelocityY -= -Physics2D.gravity.y * FallMultiplier * Time.deltaTime;
        }
        // Debug Flying
        if (Input.GetKey(KeyCode.P))
        {
            flying = true;
            rb.MovePosition(rb.position + dir * vel * 2 * Time.deltaTime);
        }
        if (health < 0 && dying == false)
        {
            dying = true;
            Death();
        }
    }

    public void GetReset(CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(ReSpawn());
        }
    }
    
    public void Door()
    {
        Instantiate(doorsfx);
    }

    public void NextScene()
    {
        PlayerPrefs.DeleteKey("spawnX");
        PlayerPrefs.DeleteKey("spawnY");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
