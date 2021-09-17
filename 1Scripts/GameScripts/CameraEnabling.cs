using UnityEngine;
using Photon.Pun;

namespace NoNameGame {
    public class CameraEnabling : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject cam; //main camera che viene attivata



        // Start is called before the first frame update
        void Start()
        {
            cam.SetActive(photonView.IsMine);
        }
    }
}
