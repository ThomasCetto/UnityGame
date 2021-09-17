/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoNameGame
{
    public class HitBoxDetection : MonoBehaviour
    {
        public enum collisionType { head, body, legs }
        public collisionType damageType;

        [SerializeField] private Transform player;
        



        public void Hit(float damage)
        {
            HealthSystem healthSystem = player.GetComponent<HealthSystem>();
            try
            {
                healthSystem.TakeDamage(damage);
            }
            catch
            {
                print("This object does not contain a HealthSystem script");
            }
        }
    }
}*/
