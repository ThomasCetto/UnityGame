using UnityEngine;

    public class TutorialPlayerLook : MonoBehaviour
    {
        [SerializeField] private float sensX = 150f;
        [SerializeField] private float sensY = 150f;

        [SerializeField] Transform cam;
        [SerializeField] Transform orientation;
        [SerializeField] Transform upperArmR;
        [SerializeField] Transform upperArmL;
        [SerializeField] Transform head;
        [SerializeField] Transform hat;

        float mouseX;
        float mouseY;
        float multiplier = 0.01f;
        float xRotation;
        float yRotation;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Update is called once per frame
        void Update()
        {

            if (TutorialPauseMenu.GameIsPaused) return;

            MyInput();
        }

        private void LateUpdate()
        {

            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
            transform.rotation = Quaternion.Euler(0, yRotation, 0);

            upperArmL.transform.rotation *= Quaternion.Euler(0, xRotation / 5, xRotation); //ruota verso l'alto le braccia del giocatore
            upperArmR.transform.rotation *= Quaternion.Euler(0, -xRotation / 5, -xRotation);
            head.transform.rotation *= Quaternion.Euler(xRotation, 0, 0); //e la testa
            hat.transform.rotation *= Quaternion.Euler(xRotation, 0, 0); //e il cappello

            hat.transform.localPosition -= new Vector3(0, 0, xRotation / 700); //fa stare attaccato il cappello alla testa


        }

        private void MyInput()
        {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");

            yRotation += mouseX * sensX * multiplier; //destra e sinistra
            xRotation -= mouseY * sensY * multiplier; //su e giù

            xRotation = Mathf.Clamp(xRotation, -70f, 70f); //non fa andare oltre i -70 e 70 gradi in su e giù
        }

    }

