using UnityEngine;

[CreateAssetMenu(fileName = "CarData", menuName = "Garage/Car Data")]
public class CarData : ScriptableObject
{
    public string carName;
    public int price;
    public GameObject carPrefab;
    public Material[] colors;

    public float acceleration;
    public float handling;
    public float braking;
}
