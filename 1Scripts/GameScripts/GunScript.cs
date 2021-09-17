using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

namespace NoNameGame
{
    public class GunScript : MonoBehaviourPunCallbacks
    {
        [Header("Shoot info")]
        [SerializeField] private float damage = 40f;
        [SerializeField] private float range = 100f;
        [SerializeField] private float fireRate = 15f;
        [SerializeField] private bool isAutomatic = false;

        [SerializeField] private Camera fpsCam;
        [SerializeField] private Camera weaponCam;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private GameObject impactEffect;
        [SerializeField] private Transform pistol;

        private float nextTimeToFire = 0f;

        [Header("Ammos")]
        [SerializeField] private float maxCurrentAmmo = 12f; //proiettili massimi in una carica
        [SerializeField] private float maxLeftAmmo = 96f; //proiettili massimi ancora da caricare
        private float currentAmmo = 12f; //proiettili nella carica
        private float leftAmmo = 96f; //proiettili ancora da caricare
        private Text ammoText;

        [Header("Reload")]
        [SerializeField] KeyCode reloadKey = KeyCode.R;
        [SerializeField] private float timeToReload = 1.25f; //tempo totale per ricaricare
        [SerializeField] private float gunSpins = 2f;
        private float reloadingTimeRemaining; //tempo rimanente alla fine della ricarica
        private bool isReloading = false;
        [SerializeField] private Vector3 rotationAfterReloading;

        [Header("Bullet")]
        [SerializeField] private Transform gunTip;
        [SerializeField] private GameObject bullet;
        [SerializeField] private float bulletForce = 500000f;
        [SerializeField] private int bulletLayer;

        private RaycastHit hit;

        void Awake()
        {
            ammoText = GameObject.Find("/UI/Canvas/HUD/Texts/BulletText").GetComponent<Text>();
            ammoText.text = 12 + " / " + 96;
        }

        void Start()
        {
            if (!photonView.IsMine) return;

            leftAmmo = maxLeftAmmo;
            currentAmmo = maxCurrentAmmo;
            reloadingTimeRemaining = timeToReload;
        }

        void Update()
        {
            if (!photonView.IsMine) return;


            if (!isAutomatic)
            {
                if (Input.GetMouseButtonDown(0) && !isReloading && Time.time >= nextTimeToFire && currentAmmo > 0 && !PauseMenu.GameIsPaused && photonView.IsMine)
                { //solo quando si clicca, non quando si tiene premuto
                    photonView.RPC("Shoot", RpcTarget.All);
                    nextTimeToFire = Time.time + (1f / fireRate);
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && !isReloading && Time.time >= nextTimeToFire && currentAmmo > 0 && !PauseMenu.GameIsPaused && photonView.IsMine)
                { //se il tasto è premuto
                    photonView.RPC("Shoot", RpcTarget.All);
                    nextTimeToFire = Time.time + (1f / fireRate);
                }
            }

            if (Input.GetKeyDown(reloadKey) && currentAmmo < maxCurrentAmmo && leftAmmo > 0 && !isReloading)
            {
                ReloadGun();
            }

            if (isReloading)
                ReloadMechanics();
        }


        //IL RAYCAST PARTE DALLA CAM PRINCIPALE, E VA VERSO LA TRAIETTORIA DELLA CAM DELL'ARMA
        [PunRPC]
        public void Shoot()
        {
            muzzleFlash.Play(); //effetto sparo dalla canna della pistola

            FindObjectOfType<AudioManager>().Play("GunShot"); //suono sparo

            int layerMask = ~LayerMask.GetMask("Player"); //LayerMask che rappresenta tutti le layerMasks eccetto Player 

            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, layerMask)) //se colpisce qualcosa che non sia il giocatore stesso
            {
                if (photonView.IsMine)
                {
                    if (hit.collider.gameObject.layer == 10) //se colpisce un nemico
                    {
                        if (hit.collider.gameObject.name.Equals("Head")) //se colpisce la testa
                        {
                            hit.collider.transform.root.gameObject.GetPhotonView().RPC("TakeGunDamage", RpcTarget.All, damage * 2, PhotonNetwork.LocalPlayer.ActorNumber);
                        
                            
                        
                        }
                        else
                        {
                            hit.collider.transform.root.gameObject.GetPhotonView().RPC("TakeGunDamage", RpcTarget.All, damage, PhotonNetwork.LocalPlayer.ActorNumber);




                        }

                    }
                }

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); //crea l'effetto del colpo del proiettile
                impactGO.layer = 0; //imposta il layer dell'oggetto come diverso da quello dell'arma facendo cosi' in modo che non compaia spostato più a sinistra rispetto a dove sparato
                Destroy(impactGO, 0.5f);
            }

            photonView.RPC("ShootBullet", RpcTarget.All);

            if (photonView.IsMine)
            {
                currentAmmo--;
                ammoText.text = currentAmmo + " / " + leftAmmo;
            }
        }

        [PunRPC]
        private void ShootBullet()
        {
            GameObject temporaryBullet = Instantiate(bullet, gunTip.position, gunTip.rotation);

            temporaryBullet.layer = bulletLayer; //8 è il layer delle armi, 7 quello del player

            Rigidbody rb;
            rb = temporaryBullet.GetComponent<Rigidbody>();

            rb.AddForce(fpsCam.transform.forward * bulletForce);

            Destroy(temporaryBullet, 1.0f);
        }

        private void ShootMechanics()
        {
            if (!isAutomatic)
            {
                if (Input.GetMouseButtonDown(0) && !isReloading && Time.time >= nextTimeToFire && currentAmmo > 0)
                { //solo quando si clicca, non quando si tiene premuto
                    Shoot();
                    nextTimeToFire = Time.time + (1f / fireRate);
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && !isReloading && Time.time >= nextTimeToFire && currentAmmo > 0)
                { //se il tasto è premuto
                    Shoot();
                    nextTimeToFire = Time.time + (1f / fireRate);
                }
            }
        }

        //modifica i valori dei proiettili
        private void ReloadGun()
        {
            isReloading = true;

            if (leftAmmo + currentAmmo >= maxCurrentAmmo)
            {
                leftAmmo -= maxCurrentAmmo - currentAmmo;
                currentAmmo = maxCurrentAmmo;
            }
            else
            {
                currentAmmo += leftAmmo;
                leftAmmo = 0;
            }


        }

        //Tutte le azioni che fa ad ogni aggiornamento durante il periodo di ricarica
        private void ReloadMechanics()
        {
            reloadingTimeRemaining -= Time.deltaTime;
            pistol.localRotation *= Quaternion.Euler((gunSpins * 360) / (timeToReload / Time.deltaTime), 0, 0);

            if (reloadingTimeRemaining <= 0)
            {
                reloadingTimeRemaining = timeToReload;
                isReloading = false;
                pistol.localRotation = Quaternion.Euler(rotationAfterReloading);

                ammoText.text = currentAmmo + " / " + leftAmmo;
            }
        }


        [PunRPC]
        private void TakeGunDamage(float damage, int actor)
        {
            GetComponent<HealthSystem>().TakeDamage(damage, actor);
        }

        [PunRPC]
        private float GetPlayerHealth()
        {
            return GetComponent<HealthSystem>().GetHealth();
        }




    }
}
