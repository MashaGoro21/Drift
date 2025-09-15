using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    [Header("Level UI")]
    [SerializeField] private GameObject levelUIObject;
    [SerializeField] private TMP_Text countDownText;
    [SerializeField] private TMP_Text timerText;

    [Header("End Level UI")]
    [SerializeField] private GameObject endLevelUIObject;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text rewardX2Text;

    private void Start()
    {
        levelUIObject.SetActive(false);
        endLevelUIObject.SetActive(false);
        countDownText.gameObject.SetActive(true);
    }

    public void ShowCountDownText(string text) =>
        countDownText.text = text;

    public void HideCountDownText() =>
        countDownText.gameObject.SetActive(false);

    public void SetLevelUIVisibility(bool isVisible) =>
        levelUIObject.SetActive(isVisible);

    public void ShowEndLevelUI(int earnedCash)
    {
        endLevelUIObject.SetActive(true);
        rewardText.text = $"+{earnedCash}";
        rewardX2Text.text = $"+{earnedCash * 2}";
        timerText.text = "00:00";
    }

    public void HideEndLevelUI() =>
        endLevelUIObject.SetActive(false);

    public void UpdateTimer(float remainingSeconds)
    {
        int min = Mathf.FloorToInt(remainingSeconds / 60);
        int sec = Mathf.FloorToInt(remainingSeconds % 60);
        timerText.text = $"{min:00}:{sec:00}";
    }
}
