using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private static bool HasKey(string key) => PlayerPrefs.HasKey(key);

    public static int GetInt(string key, int defaultValue = 0) =>
        PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : defaultValue;

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static float GetFloat(string key, float defaultValue = 0f) =>
        PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public static string GetString(string key, string defaultValue = "") =>
        PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : defaultValue;

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public static void EnsureCarDefaults(List<CarData> cars)
    {
        if (cars == null) return;

        if (!HasKey(PrefsKeys.CarOwned(cars[0].carName)))
            SetInt(PrefsKeys.CarOwned(cars[0].carName), 1);
        
        if (!HasKey(PrefsKeys.SELECTED_CAR))
            SetString(PrefsKeys.SELECTED_CAR, cars[0].carName);

        foreach (var car in cars)
        {
            if (!HasKey(PrefsKeys.CarHandling(car.carName)))
                SetFloat(PrefsKeys.CarHandling(car.carName), car.handling);

            if (!HasKey(PrefsKeys.CarAcceleration(car.carName)))
                SetFloat(PrefsKeys.CarAcceleration(car.carName), car.acceleration);

            if (!HasKey(PrefsKeys.CarBraking(car.carName)))
                SetFloat(PrefsKeys.CarBraking(car.carName), car.braking);

            if (!HasKey(PrefsKeys.CarColor(car.carName)))
                SetInt(PrefsKeys.CarColor(car.carName), 0);
        }
    }
}
