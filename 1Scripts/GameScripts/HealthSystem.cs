using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

namespace NoNameGame
{
    public class HealthSystem : MonoBehaviourPunCallbacks
    {
        [SerializeField] private float maxHealth = 100f;
        public float currentHealth = 100f;
        private bool isDead = false;
        private float timer = 0, maxTimer = 5f;

        
 
        private Manager manager;
        private Transform healthBar;
        private Text hpRatio;
        private Text respawnText;
        private Text usernameTextUI;
        [SerializeField] private TextMeshPro playerUsername;

        [HideInInspector] public ProfileData playerProfile;

        [SerializeField] private GameObject youssef;
        [SerializeField] private GameObject youssefRagdoll;
        [SerializeField] private GameObject hat;

        [SerializeField] private Transform mainCamera;

        //private GameObject bloodScreenImage;
        //private float r, g, b, a;

        private void Awake()
        {
            if (!photonView.IsMine) return;

            manager = GameObject.Find("Manager").GetComponent<Manager>();
            healthBar = GameObject.Find("Bar").transform;
            //bloodScreenImage = GameObject.Find("BloodScreen");
            hpRatio = GameObject.Find("/UI/Canvas/HUD/Texts/HPRatioText").GetComponent<Text>();
            respawnText = GameObject.Find("/UI/Canvas/HUD/Texts/RespawnText").GetComponent<Text>();
            respawnText.gameObject.SetActive(false);
            usernameTextUI = GameObject.Find("/UI/Canvas/HUD/Texts/UsernameText").GetComponent<Text>();

        }

        private void Start()
        {

            if (!photonView.IsMine) return;

            currentHealth = maxHealth;
            
            RefreshHealthBar();
            hpRatio.text = currentHealth + " / " + maxHealth;

            usernameTextUI.text = CreateAndJoinRooms.myProfile.username;
            photonView.RPC("SyncProfile", RpcTarget.All, CreateAndJoinRooms.myProfile.username, CreateAndJoinRooms.myProfile.level, CreateAndJoinRooms.myProfile.xp, CreateAndJoinRooms.myProfile.kills, CreateAndJoinRooms.myProfile.deaths);

            /* r = bloodScreenImage.GetComponent<Image>().color.r; 
             g = bloodScreenImage.GetComponent<Image>().color.g;
             b = bloodScreenImage.GetComponent<Image>().color.b;
             a = bloodScreenImage.GetComponent<Image>().color.a;*/

            //CalculateAlpha();
            //AdjustColor();
        }

        private void Update()
        {
            if (!photonView.IsMine) return;

            if (isDead)
            {
                timer += Time.deltaTime;

                respawnText.text = "Respawn in " + (int)(maxTimer - timer) + "...";
            }

            if (Input.GetKeyDown(KeyCode.U))
                TakeDamage(35, -1);



            if (timer >= maxTimer)
                photonView.RPC("Die", RpcTarget.All);

            

            RefreshHealthBar();
            hpRatio.text = currentHealth + " / " + maxHealth;
        }


        public void TakeDamage(float amount, int actor)
        {
            if (!photonView.IsMine) return;

            currentHealth -= amount;
            print("current health: " + currentHealth);
            if (currentHealth < 0)
                currentHealth = 0;
            //CalculateAlpha();


            RefreshHealthBar();
            hpRatio.text = currentHealth + " / " + maxHealth;
            //AdjustColor();

            if (currentHealth <= 0)
            {   

                photonView.RPC("TurnIntoRagdoll", RpcTarget.All);

                if (actor >= 0 && !isDead)//solo se muore da un altro giocatore
                { 
                    manager.ChangeStat_S(actor, 0, 1); //kill
                    
                }

                if (!isDead) //se è il colpo che uccide incrementa il contatore delle morti
                {
                    manager.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1); //morte
                    AddDeath();
                    
                }
                isDead = true;
                respawnText.gameObject.SetActive(true);
            }
        }

        [PunRPC]
        private void Die()
        {
            if (!photonView.IsMine) return;

            if (manager != null) //prima spawna e poi distrugge perchè altrimenti il codice verrebbe distrutto prima di essere eseguito
                manager.Spawn();
            
            PhotonNetwork.Destroy(gameObject);

            respawnText.gameObject.SetActive(false);

        }

        private void RefreshHealthBar()
        {
            if (healthBar != null)
            {
                float ratio = currentHealth / maxHealth;
                
                healthBar.localScale = Vector3.Lerp(healthBar.localScale, new Vector3(ratio, 1f, 1f), Time.deltaTime * 6f); //fa un effetto omogeneo quando diminuiscono gli hp
            }

        }

        private void AdjustColor()
        {
            //Color c = new Color(r, g, b, a);
            //bloodScreenImage.GetComponent<Image>().color = c;
        }

        private void CalculateAlpha()
        {
            //a = 1f - (currentHealth/100f);
        }

        [PunRPC]
        public void TurnIntoRagdoll()
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<PlayerLook>().enabled = false;
            GetComponent<GunScript>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;

            youssef.SetActive(false);
            youssefRagdoll.SetActive(true);
            
            if(hat.GetComponent<Rigidbody>() == null)
                hat.AddComponent<Rigidbody>();
        }

        [PunRPC]
        private void SyncProfile(string u, int l, int x, short k, short d)
        {
            playerProfile = new ProfileData(u, l, x, k, d);
            playerUsername.text = playerProfile.username;
        }

        private void AddDeath()
        {
            /*ProfileData data = Data.LoadProfile();
            data.deaths++;
            Data.SaveProfile(data);*/

        }

        public float GetHealth()
        {
            return currentHealth;
        }

    }

    
}
