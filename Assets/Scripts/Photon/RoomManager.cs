using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField inputCreate;
    [SerializeField] private TMP_InputField inputJoin;
    [SerializeField] private GameObject messageObject;

    private TMP_Text messageText;

    private void Awake()
    {
        messageText = messageObject.GetComponent<TMP_Text>();
    }

    public void CreateRoom()
    {
        if(!IsRoomNameValid(inputCreate))
            return;

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

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if(returnCode == ErrorCode.GameIdAlreadyExists)
        {
            //
        }
    }

    public void JoinRoom()
    {
        if (!IsRoomNameValid(inputJoin))
            return;

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

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(returnCode == ErrorCode.GameDoesNotExist)
        {
            messageText.text = "Room not found";
            messageObject.SetActive(true);
            StartCoroutine(HideMessage());
        }

        if(returnCode == ErrorCode.GameFull)
        {

        }
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private bool IsRoomNameValid(TMP_InputField input)
    {
        if (string.IsNullOrWhiteSpace(input.text))
        {
            messageText.text = "Room name cannot be empty";
            messageObject.SetActive(true);
            StartCoroutine(HideMessage());
            return false;
        }

        return true;
    }

    private IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(2f);
        messageObject.SetActive(false);
    }
}
