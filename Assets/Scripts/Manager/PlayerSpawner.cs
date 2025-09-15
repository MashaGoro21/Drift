using Cinemachine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Car Data")]
    [SerializeField] private List<CarData> cars;
    [SerializeField] private Transform carSpawnPoint;
    [SerializeField] private DriftUI driftUI;
    [SerializeField] private float spawnSpacing = 4f;

    [Header("Car Buttons")]
    [SerializeField] private MyButton gasPedal;
    [SerializeField] private MyButton brakePedal;
    [SerializeField] private MyButton leftButton;
    [SerializeField] private MyButton rightButton;
    
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
    }

    public GameObject SpawnCar()
    {
        string selectedCarName = SaveSystem.GetString(PrefsKeys.SELECTED_CAR);

        CarData carData = cars.Find(c => c.carName == selectedCarName);
        GameObject carInstance;

        var players = PhotonNetwork.CurrentRoom.Players.Values.OrderBy(p => p.ActorNumber).ToList();
        int myIndex = players.IndexOf(PhotonNetwork.LocalPlayer);
        Vector3 offset = carSpawnPoint.right * (myIndex * spawnSpacing);

        carInstance = PhotonNetwork.Instantiate(carData.carName, carSpawnPoint.position + offset, carSpawnPoint.rotation);

        SetupLocalPlayer(carInstance, carData);

        return carInstance;
    }

    private void SetupLocalPlayer(GameObject carInstance, CarData carData)
    {
        PhotonView photonView = carInstance.GetComponent<PhotonView>();
        if (!photonView.IsMine) return;
        
        CarController carController = carInstance.GetComponent<CarController>();

        SetButtons(carController);
        SetDrift(carInstance);
        SetVirtualCamera(carInstance);
        ApplySavedColor(carData, carInstance);
        ApplySavedParameters(carData.carName, carController);
    }

    private void SetButtons(CarController carController)
    {
        carController.SetGasPedal(gasPedal);
        carController.SetBrakePedal(brakePedal);
        carController.SetLeftButton(leftButton);
        carController.SetRightButton(rightButton);
    }

    private void SetDrift(GameObject carInstance)
    {
        Drift drift = carInstance.GetComponent<Drift>();
        drift.SetDriftUI(driftUI);
    }

    private void SetVirtualCamera(GameObject carInstance)
    {
        virtualCamera.Follow = carInstance.transform;
        virtualCamera.LookAt = carInstance.transform;
    }

    private void ApplySavedColor(CarData carData, GameObject carInstance)
    {
        CarVisual carVisual = carInstance.GetComponent<CarVisual>();
        int colorIndex = SaveSystem.GetInt(PrefsKeys.CarColor(carData.carName));
        carVisual.ApplyColor(carData.colors[colorIndex]);
    }

    private void ApplySavedParameters(string selectedCarName, CarController carController)
    {
        carController.enabled = false;

        float acceleration = SaveSystem.GetFloat(PrefsKeys.CarAcceleration(selectedCarName));
        float handling = SaveSystem.GetFloat(PrefsKeys.CarHandling(selectedCarName));
        float braking = SaveSystem.GetFloat(PrefsKeys.CarBraking(selectedCarName));

        carController.SetAcceleration(acceleration);
        carController.SetHandling(handling);
        carController.SetBraking(braking);
    }
}