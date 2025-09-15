using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CarParameters
{
    Handling,
    Acceleration,
    Braking
}

public class CustomizationManager : MonoBehaviour
{
    [Header("UI Texts")]
    [SerializeField] private TMP_Text handlingBuyText;
    [SerializeField] private TMP_Text accelerationBuyText;
    [SerializeField] private TMP_Text brakingBuyText;
    [SerializeField] private TMP_Text paintText;
    [SerializeField] private TMP_Text carNameText;

    [Header("UI Panels")]
    [SerializeField] private GameObject parametersPanel;

    [Header("UI Sliders")]
    [SerializeField] private Slider handlingSlider;
    [SerializeField] private Slider accelerationSlider;
    [SerializeField] private Slider brakingSlider;

    [Header("Cars")]
    [SerializeField] private Transform carSpawnPoint;
    [SerializeField] private List<CarData> cars;

    [Header("Prices")]
    [SerializeField] private int paintPrice = 200;

    private int currentColorIndex = 0;
    private CarData selectedCar;
    private GameObject currentCarInstance;

    private const int MAX_PARAMETER_LEVEL = 10;

    private void Awake()
    {
        SaveSystem.EnsureCarDefaults(cars);
        parametersPanel.SetActive(false);
        paintText.text = "Painted";
    }

    private void OnEnable()
    {
        string selectedCarName = SaveSystem.GetString(PrefsKeys.SELECTED_CAR);
        selectedCar = cars.Find(c => c.carName == selectedCarName);

        ShowCar();
    }

    private void OnDisable()
    {
        if (currentCarInstance != null) Destroy(currentCarInstance);
        parametersPanel.SetActive(false);
    }

    private void ShowCar()
    {
        if (selectedCar == null) return;

        parametersPanel.SetActive(true);

        currentCarInstance = Instantiate(selectedCar.carPrefab, carSpawnPoint.position, carSpawnPoint.rotation);
        currentCarInstance.GetComponent<Drift>().enabled = false;
        currentCarInstance.GetComponent<CarController>().enabled = false;

        int colorIndex = SaveSystem.GetInt(PrefsKeys.CarColor(selectedCar.carName));
        currentCarInstance.GetComponent<CarVisual>().ApplyColor(selectedCar.colors[colorIndex]);

        UpdateCarParameters();
    }

    private void UpdateCarParameters()
    {
        carNameText.text = selectedCar.carName;
        UpdateCarParameter(CarParameters.Handling, handlingSlider, handlingBuyText);
        UpdateCarParameter(CarParameters.Acceleration, accelerationSlider, accelerationBuyText);
        UpdateCarParameter(CarParameters.Braking, brakingSlider, brakingBuyText);
    }

    private void UpdateCarParameter(CarParameters parameter, Slider slider, TMP_Text text)
    {
        slider.value = SaveSystem.GetFloat(PrefsKeys.CarParameter(selectedCar.carName, parameter));
        text.text = slider.value >= MAX_PARAMETER_LEVEL ? "Bought" : $"Buy {GetParameterPrice(parameter)}$";
    }

    private int GetParameterPrice(CarParameters parameter)
    {
        float currentValue = SaveSystem.GetFloat(PrefsKeys.CarParameter(selectedCar.carName, parameter));
        return Mathf.RoundToInt(20 * Mathf.Pow(2f, currentValue));
    }

    private void BuyUpgrade(CarParameters parameter)
    {
        int price = GetParameterPrice(parameter);
        float value = SaveSystem.GetFloat(PrefsKeys.CarParameter(selectedCar.carName, parameter)) + 1;

        if(Bank.Instance.SpendCash(price) && value <= 10)
        {
            SaveSystem.SetFloat(PrefsKeys.CarParameter(selectedCar.carName, parameter), value);
            UpdateCarParameters();
        }
    }

    public void BuyHandling() => BuyUpgrade(CarParameters.Handling);
    public void BuyAcceleration() => BuyUpgrade(CarParameters.Acceleration);
    public void BuyBraking() => BuyUpgrade(CarParameters.Braking);
    public void NextColor() => ChangeColor(1);
    public void PreviousColor() => ChangeColor(-1);

    private void ChangeColor(int direction)
    {
        currentColorIndex = (currentColorIndex + direction + selectedCar.colors.Length) % selectedCar.colors.Length;
        currentCarInstance.GetComponent<CarVisual>().ApplyColor(selectedCar.colors[currentColorIndex]);
        IsColorApplied();
    }

    public void PaintCar()
    {
        if(Bank.Instance.SpendCash(paintPrice) && !IsColorApplied())
        {
            SaveSystem.SetInt(PrefsKeys.CarColor(selectedCar.carName), currentColorIndex);
            IsColorApplied();
        }
    }

    private bool IsColorApplied()
    {
        if (currentColorIndex == SaveSystem.GetInt(PrefsKeys.CarColor(selectedCar.carName)))
        {
            paintText.text = "Painted";
            return true;
        }
        paintText.text = "Paint " + paintPrice + "$";
        return false;
    }
}