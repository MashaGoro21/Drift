using Photon.Pun;
using Photon.Realtime;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        else if (PhotonNetwork.InLobby)
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
