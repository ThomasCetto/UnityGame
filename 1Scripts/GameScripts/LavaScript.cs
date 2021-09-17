using UnityEngine;

namespace NoNameGame
{
    public class LavaScript : MonoBehaviour
    {
        private RaycastHit hit;

        private float lavaDamage = 8f;

        private void OnCollisionStay(Collision coll)
        {

            if(coll.gameObject.GetComponent<HealthSystem>() != null)
                coll.gameObject.GetComponent<HealthSystem>().TakeDamage(lavaDamage, -1);
        }


    }
}
