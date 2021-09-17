using UnityEngine;
using Photon.Pun;

public class AnimationScript : MonoBehaviourPunCallbacks
{
    Rigidbody rb;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine) return;

        rb = gameObject.GetComponentInParent(typeof(Rigidbody)) as Rigidbody;
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.z) + Mathf.Abs(rb.velocity.x)); 
    }
}
