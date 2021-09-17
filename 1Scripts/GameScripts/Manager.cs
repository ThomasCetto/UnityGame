using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace NoNameGame
{
    public class PlayerInfo
    {
        public ProfileData profile;
        public int actor;
        public short kills;
        public short deaths;

        public PlayerInfo(ProfileData p, int a, short k, short d)
        {
            this.profile = p;
            this.actor = a;
            this.kills = k;
            this.deaths = d;
        }
    }

    public enum GameState
    {
        Waiting = 0,
        Starting = 1,
        Playing = 2,
        Ending = 3
    }

    public class Manager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        #region Fields

        public string player_prefab_string;
        public GameObject player_prefab;
        public Transform[] spawn_points;

        public List<PlayerInfo> playerInfo = new List<PlayerInfo>();
        public int myind;

        private Text myKills;
        private Text myDeaths;
        private Transform leaderboard;

        #endregion

        #region Codes

        public enum EventCodes : byte
        {
            NewPlayer,
            UpdatePlayers,
            ChangeStat
        }

        #endregion

        #region MB Callbacks

        private void Awake()
        {
            initializeUI();

            RefreshMyStats();
        }

        private void Start()
        {
            ValidateConnection();
            NewPlayer_S(CreateAndJoinRooms.myProfile);
            Spawn();
           
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!leaderboard.gameObject.activeSelf) Leaderboard(leaderboard);
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                if (leaderboard.gameObject.activeSelf) leaderboard.gameObject.SetActive(false);
            }
        }

        public override void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }
        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        #endregion

        #region Photon

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code >= 200) return;

            EventCodes e = (EventCodes)photonEvent.Code;
            object[] o = (object[])photonEvent.CustomData;

            switch (e)
            {
                case EventCodes.NewPlayer:
                    NewPlayer_R(o);
                    break;

                case EventCodes.UpdatePlayers:
                    UpdatePlayers_R(o);
                    break;

                case EventCodes.ChangeStat:
                    ChangeStat_R(o);
                    break;
            }
        }


        #endregion

        #region Methods

        public void Spawn()
        {
            Transform t_spawn = spawn_points[Random.Range(0, spawn_points.Length)];

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Instantiate(player_prefab_string, t_spawn.position, t_spawn.rotation);
            }
            else
            {
                GameObject newPlayer = Instantiate(player_prefab, t_spawn.position, t_spawn.rotation) as GameObject;
            }
        }

        private void initializeUI()
        {
            myKills = GameObject.Find("/UI/Canvas/HUD/Texts/KillCounter").GetComponent<Text>();
            myDeaths = GameObject.Find("/UI/Canvas/HUD/Texts/DeathCounter").GetComponent<Text>();
            leaderboard = GameObject.Find("/UI/Canvas/Leaderboard").transform;
        }


        private void ValidateConnection()
        {
            if (PhotonNetwork.IsConnected) return;
            SceneManager.LoadScene(0);
        }

        private void RefreshMyStats()
        {
            if (playerInfo.Count > myind)
            {
                myKills.text = $"{playerInfo[myind].kills} kills";
                myDeaths.text = $"{playerInfo[myind].deaths} deaths";
            }
            else
            {
                myKills.text = "0 kills";
                myDeaths.text = "0 deaths";
            }
        }

        private void Leaderboard(Transform p_lb)
        {

            // clean up
            for (int i = 2; i < p_lb.childCount; i++)
            {
                Destroy(p_lb.GetChild(i).gameObject);
            }

            // cache prefab
            GameObject playercard = p_lb.GetChild(1).gameObject;
            playercard.SetActive(false);

            // sort
            List<PlayerInfo> sorted = SortPlayers(playerInfo);

            // display
            bool t_alternateColors = false;
            foreach (PlayerInfo a in sorted)
            {
                GameObject newcard = Instantiate(playercard, p_lb) as GameObject;

                if (t_alternateColors) newcard.GetComponent<Image>().color = new Color32(0, 0, 0, 180);
                t_alternateColors = !t_alternateColors;

                newcard.transform.Find("UsernameValue").GetComponent<Text>().text = a.profile.username;
                newcard.transform.Find("LevelValue").GetComponent<Text>().text = a.profile.level.ToString();
                newcard.transform.Find("ScoreValue").GetComponent<Text>().text = ((a.kills * 75) - (a.deaths * 25)).ToString();
                newcard.transform.Find("KillsValue").GetComponent<Text>().text = a.kills.ToString();
                newcard.transform.Find("DeathsValue").GetComponent<Text>().text = a.deaths.ToString();

                newcard.SetActive(true);
            }

            // activate
            p_lb.gameObject.SetActive(true);
            p_lb.parent.gameObject.SetActive(true);
        }

        private List<PlayerInfo> SortPlayers(List<PlayerInfo> p_info)
        {
            List<PlayerInfo> sorted = new List<PlayerInfo>();

                while (sorted.Count < p_info.Count)
                {
                    // set defaults
                    short highest = -1;
                    PlayerInfo selection = p_info[0];

                    // grab next highest player
                    foreach (PlayerInfo a in p_info)
                    {
                        if (sorted.Contains(a)) continue;
                        if (a.kills > highest)
                        {
                            selection = a;
                            highest = a.kills;
                        }
                    }

                    // add player
                    sorted.Add(selection);
                }

            return sorted;
        }

        #endregion

        #region Events

        public void NewPlayer_S(ProfileData p)
        {
            object[] package = new object[8];

            package[0] = p.username;
            package[1] = p.level;
            package[2] = p.xp;
            package[3] = p.kills;
            package[4] = p.deaths;
            package[5] = PhotonNetwork.LocalPlayer.ActorNumber;
            package[6] = (short)0;
            package[7] = (short)0;

            PhotonNetwork.RaiseEvent(
                (byte)EventCodes.NewPlayer,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                new SendOptions { Reliability = true }
            );
        }
        public void NewPlayer_R(object[] data)
        {
            PlayerInfo p = new PlayerInfo(
                new ProfileData(
                    (string)data[0],
                    (int)data[1],
                    (int)data[2],
                    (short)data[3],
                    (short)data[4]
                ),
                (int)data[5],
                (short)data[6],
                (short)data[7]
            );

            playerInfo.Add(p);

            UpdatePlayers_S(playerInfo);
        }

        public void UpdatePlayers_S(List<PlayerInfo> info)
        {
            object[] package = new object[info.Count];

            for (int i = 0; i < info.Count; i++)
            {
                object[] piece = new object[8];

                piece[0] = info[i].profile.username;
                piece[1] = info[i].profile.level;
                piece[2] = info[i].profile.xp;
                piece[3] = info[i].profile.kills;
                piece[4] = info[i].profile.deaths;
                piece[5] = info[i].actor;
                piece[6] = info[i].kills;
                piece[7] = info[i].deaths;

                package[i] = piece;
            }

            PhotonNetwork.RaiseEvent(
                (byte)EventCodes.UpdatePlayers,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
            );
        }


        public void UpdatePlayers_R(object[] data)
        {

            playerInfo = new List<PlayerInfo>();

            for (int i = 0; i < data.Length; i++)
            {
                object[] extract = (object[])data[i];

                PlayerInfo p = new PlayerInfo(
                    new ProfileData(
                        (string)extract[0], //username
                        (int)extract[1], //level
                        (int)extract[2], //xp
                        (short)extract[3], //kills
                        (short)extract[4] //dealths

                    ),
                    (int)extract[5], //actor
                    (short)extract[6], //kills
                    (short)extract[7] //deaths
                );

                playerInfo.Add(p);

                if (PhotonNetwork.LocalPlayer.ActorNumber == p.actor)
                {
                    myind = i;
                }
            }
        }

        public void ChangeStat_S(int actor, byte stat, byte amt)
        {
            object[] package = new object[] { actor, stat, amt };

            PhotonNetwork.RaiseEvent(
                (byte)EventCodes.ChangeStat,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
            );
        }
        public void ChangeStat_R(object[] data)
        {
            int actor = (int)data[0];
            byte stat = (byte)data[1];
            byte amt = (byte)data[2];

            for (int i = 0; i < playerInfo.Count; i++)
            {
                if (playerInfo[i].actor == actor)
                {
                    switch (stat)
                    {
                        case 0: //kills
                            playerInfo[i].kills += amt;

                            Debug.Log($"Player {playerInfo[i].profile.username} : kills = {playerInfo[i].kills}");
                            break;

                        case 1: //deaths
                            playerInfo[i].deaths += amt;

                            Debug.Log($"Player {playerInfo[i].profile.username} : deaths = {playerInfo[i].deaths}");
                            break;
                    }

                    if (i == myind) RefreshMyStats();
                    if (leaderboard.gameObject.activeSelf) Leaderboard(leaderboard); //aggiorna la leaderboard se è attiva

                    return;
                }
            }
        }


        #endregion
    }
}


/*using UnityEngine;
using Photon.Pun;

namespace NoNameGame
{
    public class Manager : MonoBehaviourPunCallbacks
    {
        public string playerPrefab;
        public Transform[] spawnPoint;

        public void Start()
        {
            Spawn();
        }

        public void Spawn()
        {

            Transform point = spawnPoint[Random.Range(0, spawnPoint.Length)];
            point.Rotate(0, point.localRotation.y, 0, Space.World);
            PhotonNetwork.Instantiate(playerPrefab, point.position, point.localRotation);
        }

    }
}*/
