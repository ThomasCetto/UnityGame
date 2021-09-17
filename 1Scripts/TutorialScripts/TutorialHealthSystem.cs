using UnityEngine;

    public class TutorialHealthSystem : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        private Transform healthBar;


        private void Start()
        {


            currentHealth = maxHealth;

        }

        public void TakeDamage(float amount)
        {
             currentHealth -= amount;

             if (currentHealth <= 0)
             {
                 Die();
             }
        }

        void Die()
        {
            Destroy(gameObject);
        }

    }
