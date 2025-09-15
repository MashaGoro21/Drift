using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarShopManager : MonoBehaviour
{
    [Header("UI Texts")]
    [SerializeField] private TMP_Text carNameText;
    [SerializeField] private TMP_Text priceText;

    [Header("UI Sliders")]
    [SerializeField] private Slider handlingSlider;
    [SerializeField] private Slider accelerationSlider;
    [SerializeField] private Slider brakingSlider;

    [Header("UI Buttons")]
    [SerializeField] private GameObject buyButton;
    [SerializeField] private GameObject chooseButton;

    [Header("Cars")]
    [SerializeField] private Transform carSpawnPoint;
    [SerializeField] private List<CarData> cars = new List<CarData>();

    private int currentCarIndex = 0;
    private GameObject currentCarInstance;

    private void Awake() =>SaveSystem.EnsureCarDefaults(cars);
    private void OnEnable() => ShowCar(currentCarIndex);

    private void OnDisable()
    {
        if (currentCarInstance != null) Destroy(currentCarInstance);
    }

    private void ShowCar(int index)
    {
        if (cars == null || cars.Count <= 0) return; 

        if (currentCarInstance != null) Destroy(currentCarInstance);

        CarData car = cars[index];
        currentCarInstance = Instantiate(car.carPrefab, carSpawnPoint.position, carSpawnPoint.rotation);
        
        currentCarInstance.GetComponent<Drift>().enabled = false;
        currentCarInstance.GetComponent<CarController>().enabled = false;

        int colorIndex = SaveSystem.GetInt(PrefsKeys.CarColor(car.carName));
        currentCarInstance.GetComponent<CarVisual>().ApplyColor(car.colors[colorIndex]);

        UpdateCarParameters(car);
        UpdateCarButtons(car.carName);
    }

    private void UpdateCarParameters(CarData car)
    {
        carNameText.text = car.carName;
        priceText.text = car.price.ToString() + "$";

        handlingSlider.value = SaveSystem.GetFloat(PrefsKeys.CarHandling(car.carName));
        accelerationSlider.value = SaveSystem.GetFloat(PrefsKeys.CarAcceleration(car.carName));
        brakingSlider.value = SaveSystem.GetFloat(PrefsKeys.CarBraking(car.carName));
    }

    private void UpdateCarButtons(string carName)
    {
        bool isOwned = SaveSystem.GetInt(PrefsKeys.CarOwned(carName)) == 1;
        chooseButton.SetActive(isOwned);
        buyButton.SetActive(!isOwned);
    }

    public void NextCar() => ChangeCar(1);
    public void PreviousCar() => ChangeCar(-1);

    private void ChangeCar(int direction)
    {
        currentCarIndex = (currentCarIndex + direction + cars.Count) % cars.Count;
        ShowCar(currentCarIndex);
    }

    public void BuyCar()
    {
        CarData car = cars[currentCarIndex];

        if(Bank.Instance.SpendCash(car.price))
        {
            SaveSystem.SetInt(PrefsKeys.CarOwned(car.carName), 1);
            UpdateCarButtons(car.carName);
        }
    }

    public void ChooseCar()
    {
        SaveSystem.SetString(PrefsKeys.SELECTED_CAR, cars[currentCarIndex].carName);
    }
}
