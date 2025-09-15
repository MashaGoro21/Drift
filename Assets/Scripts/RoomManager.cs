using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField inputCreate;
    [SerializeField] private TMP_InputField inputJoin;

    public void CreateRoom()
    {
        int selectedLevel = PlayerPrefs.GetInt("Level");

        var roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "Level", selectedLevel }
            },
            CustomRoomPropertiesForLobby = new string[] { "Level" }
        };

        PhotonNetwork.CreateRoom(inputCreate.text, roomOptions);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(inputJoin.text);
    }

    public override void OnJoinedRoom()
    {
        int roomLevel = (int)PhotonNetwork.CurrentRoom.CustomProperties["Level"];
        int selectedLevel = PlayerPrefs.GetInt("Level");

        if (roomLevel != selectedLevel)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
            return;
        }

        switch (roomLevel)
        {
            case 1: PhotonNetwork.LoadLevel("Level1"); break;
            case 2: PhotonNetwork.LoadLevel("Level2"); break;
            case 3: PhotonNetwork.LoadLevel("Level3"); break;
        }

    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
