using UnityEngine;
using UnityEngine.UI;

    public class TutorialGunScript : MonoBehaviour
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
        [SerializeField] private float maxCurrentAmmo = 10f; //proiettili massimi in una carica
        [SerializeField] private float maxLeftAmmo = 100f; //proiettili massimi ancora da caricare
        private float currentAmmo = 10f; //proiettili nella carica
        private float leftAmmo = 100f; //proiettili ancora da caricare
        private Text bulletText;    


        [Header("Reload")]
        [SerializeField] KeyCode reloadKey = KeyCode.R;
        [SerializeField] private float timeToReload = 1.75f; //tempo totale per ricaricare
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

        private void Awake()
        {
            bulletText = GameObject.Find("/Canvas/HUDTutorial/Texts/BulletText").GetComponent<Text>();
        }

        void Start()
        {
            leftAmmo = maxLeftAmmo;
            currentAmmo = maxCurrentAmmo;
            reloadingTimeRemaining = timeToReload;
        }

        void Update() 
        { 

            if (!isAutomatic)
            {
                if (Input.GetMouseButtonDown(0) && !isReloading && Time.time >= nextTimeToFire && currentAmmo > 0 && !TutorialPauseMenu.GameIsPaused)
                { //solo quando si clicca, non quando si tiene premuto
                    Shoot();
                    nextTimeToFire = Time.time + (1f / fireRate);
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && !isReloading && Time.time >= nextTimeToFire && currentAmmo > 0 && !TutorialPauseMenu.GameIsPaused)
                { //se il tasto è premuto
                    Shoot();
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
        public void Shoot()
        {
            muzzleFlash.Play(); //effetto sparo dalla canna della pistola

            FindObjectOfType<AudioManager>().Play("GunShot"); //suono sparo

            int layerMask = ~LayerMask.GetMask("Player"); //?LayerMask che rappresenta tutti le layerMasks eccetto Player 

            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, layerMask)) //se colpisce qualcosa che non sia il giocatore stesso
            {

                TutorialHealthSystem enemy = hit.transform.GetComponent<TutorialHealthSystem>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)); //crea l'effetto del colpo del proiettile
                impactGO.layer = 0; //imposta il layer dell'oggetto come diverso da quello dell'arma facendo cosi' in modo che non compaia spostato più a destra rispetto a dove sparato
                Destroy(impactGO, 0.5f);
            }


            currentAmmo--;


            ShootBullet();
            bulletText.text = currentAmmo + " / " + leftAmmo;
        }

        //
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
                bulletText.text = currentAmmo + " / " + leftAmmo;
            }
        }


    }
