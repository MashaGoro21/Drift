using TMPro;
using UnityEngine;

public class DriftUI : MonoBehaviour
{
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text factorText;
    [SerializeField] private TMP_Text driftAngleText;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI(int totalScore, float driftFactor, float currentScore, float driftAngle)
    {
        totalScoreText.text = $"Total: {totalScore:###,###,000}";
        factorText.text = $"X{driftFactor:0.0}";
        currentScoreText.text = $"{currentScore:###,###,000}";
        driftAngleText.text = $"{driftAngle:0}°";
    }
}
