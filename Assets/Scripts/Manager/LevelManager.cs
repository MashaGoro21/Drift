using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerSpawner))]
[RequireComponent(typeof(LevelUI))]
[RequireComponent(typeof(PhotonView))]
public class LevelManager : MonoBehaviourPun
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private float levelDuration = 120f;
    
    private PlayerSpawner playerSpawner;
    private LevelUI levelUI;
    private GameObject carInstance;

    private int earnedCash;
    private bool levelStarted = false;
    private double startTime; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playerSpawner = GetComponent<PlayerSpawner>();
        levelUI = GetComponent<LevelUI>();

        carInstance = playerSpawner.SpawnCar();
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_SetStartTime), RpcTarget.All, PhotonNetwork.Time + 3.0);
        }
    }

    [PunRPC]
    private void RPC_SetStartTime(double networkStartTime)
    {
        startTime = networkStartTime;
        StartCoroutine(StartCountdown());
    }

    private void Update()
    {
        if (!levelStarted) return;
        
        float timeLeft = (float)((startTime + levelDuration) - PhotonNetwork.Time);
        levelUI.UpdateTimer(timeLeft);

        if (timeLeft <= 0f) EndLevel();
    }

    IEnumerator StartCountdown()
    {
        int countdown = Mathf.CeilToInt((float)(startTime - PhotonNetwork.Time));
        while (countdown > 0)
        {
            levelUI.ShowCountDownText(countdown.ToString());
            countdown--;
            yield return new WaitForSeconds(1f);
        }

        levelUI.ShowCountDownText("GO!");
        yield return new WaitForSeconds(1f);
        levelUI.HideCountDownText();

        StartLevel();
    }

    private void StartLevel()
    {
        levelStarted = true;
        carInstance.GetComponent<CarController>().enabled = true;
        levelUI.SetLevelUIVisibility(true);
    }

    private void EndLevel()
    {
        levelStarted = false;
        
        Time.timeScale = 0f;
        carInstance.GetComponent<AudioSource>().enabled = false;
        levelUI.SetLevelUIVisibility(false);
        earnedCash = carInstance.GetComponent<Drift>().GetTotalScore();

        levelUI.ShowEndLevelUI(earnedCash);
    }

    private void RequestReward(int cash)
    {
        Bank.Instance.AddCash(cash);
        levelUI.HideEndLevelUI();

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }

    public void ReceiveReward() => RequestReward(earnedCash);
    public void ReceiveDoubleReward() => RequestReward(earnedCash*2);
}