using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Grab : MonoBehaviour
{
    private bool hold;
    public int rotspeed = 300;
    public UnityEvent OnGrab;
    public UnityEvent StopGrab;

    // Update is called once per frame
    void Update()
    {
        // INPUT
        if (Input.GetKey(KeyCode.Mouse0))
        {
            hold = true;
        }
        else
        {
            hold = false;
            Destroy(GetComponent<HingeJoint2D>());
            GetComponent<Collider2D>().isTrigger = false;
            StopGrab.Invoke();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hold && collision.gameObject.tag == "Grab")
        {
            Rigidbody2D rb = collision.transform.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                HingeJoint2D fj = transform.gameObject.AddComponent<HingeJoint2D>();
                fj.connectedBody = rb;
                fj.autoConfigureConnectedAnchor = false;

            }
            else
            {
                HingeJoint2D fj = transform.gameObject.AddComponent<HingeJoint2D>();
                fj.enableCollision = true;
            }
            GetComponent<Collider2D>().isTrigger = true;
            OnGrab.Invoke();
        }
    }
}
