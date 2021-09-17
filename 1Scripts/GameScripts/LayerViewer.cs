using UnityEngine;
using Photon.Pun;

public class LayerViewer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject youssef;
    [SerializeField] private GameObject pistol;

    void Start()
    {
        if (!photonView.IsMine) //se non è il proprio pg gli cambia il layer in quello dei nemici
        {
            foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = 10;
            }

            foreach (Transform trans in pistol.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = 11;
            }

        }


    }
}
