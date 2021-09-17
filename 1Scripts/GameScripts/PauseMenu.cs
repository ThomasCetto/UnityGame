using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace NoNameGame
{
    public class PauseMenu : MonoBehaviourPunCallbacks
    {

        public static bool GameIsPaused = false;

        public GameObject pauseMenuUI;

        [SerializeField] private Transform manager;


        public void Start()
        {
            GameIsPaused = false;
        }

        public void Update()
        {

            if (Input.GetKeyDown(KeyCode.Escape) && !SettingsMenu.InSettings)
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            GameIsPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Pause()
        {
            pauseMenuUI.SetActive(true);
            GameIsPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void QuitGame()
        {
            SaveStats(); //salva le statistiche prima di quittare
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Menu");





        }

        public void SaveStats()
        {
            ProfileData data = Data.LoadProfile();

            data.deaths += manager.GetComponent<Manager>().playerInfo[manager.GetComponent<Manager>().myind].deaths;
            data.kills += manager.GetComponent<Manager>().playerInfo[manager.GetComponent<Manager>().myind].kills;

            Data.SaveProfile(data);

        }
    }
}
