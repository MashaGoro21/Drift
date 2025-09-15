using System;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public static Bank Instance { get; private set; }

    public event Action<int> OnCashChanged;
    
    private int cash;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        cash = SaveSystem.GetInt(PrefsKeys.MONEY);
    }

    private void OnDestroy() => SaveSystem.SetInt(PrefsKeys.MONEY, cash);

    public void AddCash(int amount)
    {
        cash += amount;
        OnCashChanged?.Invoke(cash);
    }

    public bool SpendCash(int amount)
    {
        if (cash < amount) return false;

        cash -= amount;
        OnCashChanged?.Invoke(cash);
        return true;
    }

    public int GetCash() => cash;
}
