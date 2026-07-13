using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class Purchaser : MonoBehaviour
{
    public void OnOrderConfirmed(Order order)
    {
        switch(order.Info.PurchasedProductInfo[0].productId)
        {
            case "com.drift.1000":
                Bank.Instance.AddCash(1000);
                break;
            case "com.drift.5000":
                Bank.Instance.AddCash(5000);
                break;
        }
    }

}
