using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

namespace NoNameGame
{
    [System.Serializable]
    public class ProfileData
    {
        public string username;
        public int level;
        public int xp;
        public short kills;
        public short deaths;

        public ProfileData() //costruttore vuoto
        {
            this.username = "";
            this.level = 0;
            this.xp = 0;
            this.kills = 0;
            this.deaths = 0;
        }

        public ProfileData(string u, int l, int x, short k, short d) //costruttore
        {
            this.username = u;
            this.level = l;
            this.xp = x;
            this.kills = k;
            this.deaths = d;
        }

    }


    public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField createInput;
        [SerializeField] private TMP_InputField joinInput;

        [SerializeField] private TMP_InputField usernameField;
        [SerializeField] private TMP_Text deathsText;
        [SerializeField] private TMP_Text killsText;
        public static ProfileData myProfile = new ProfileData();

        public void Awake()
        {
            myProfile = Data.LoadProfile();
            usernameField.text = myProfile.username;
            deathsText.text = "Deaths: " + myProfile.deaths;
            killsText.text = "Kills: " + myProfile.kills;

            Connect();
        }

        public void CreateRoom()
        {
            SetUsername();
            try
            {
                PhotonNetwork.CreateRoom(createInput.text);
            }
            catch
            {
                print("error creating room");
            }
        }

        public void JoinRoom()
        {
            SetUsername();
            try
            {
                PhotonNetwork.JoinRoom(joinInput.text);
            }
            catch
            {
                print("error joining room");
            }
        }

        public override void OnJoinedRoom()
        {
            SetUsername();

            PhotonNetwork.LoadLevel("Game");
        }

        public void Connect()
        {
            Debug.Log("Trying to Connect...");
            PhotonNetwork.GameVersion = "0.0.0";
            PhotonNetwork.ConnectUsingSettings();
        }

        private void SetUsername()
        {
            if (string.IsNullOrEmpty(usernameField.text))
            {
                myProfile.username = "User " + Random.Range(1, 100);
            }
            else
            {
                myProfile.username = usernameField.text;
            }
        }

    }
}