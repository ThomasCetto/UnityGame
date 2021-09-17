using UnityEngine;

public class TutorialAnimationScript : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {

        rb = gameObject.GetComponentInParent(typeof(Rigidbody)) as Rigidbody;
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.z) + Mathf.Abs(rb.velocity.x));
    }
}
