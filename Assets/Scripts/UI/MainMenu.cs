using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text cashText;

    private void Start()
    {
        Bank.Instance.OnCashChanged += UpdateCashText;
        UpdateCashText(Bank.Instance.GetCash());
    }

    private void OnDestroy() => Bank.Instance.OnCashChanged -= UpdateCashText;

    public void StartLevel1()
    {
        PlayerPrefs.SetInt("Level", 1);
        SceneManager.LoadScene("Loading");
    }

    public void StartLevel2()
    {
        PlayerPrefs.SetInt("Level", 2);
        SceneManager.LoadScene("Loading");
    }

    public void StartLevel3()
    {
        PlayerPrefs.SetInt("Level", 3);
        SceneManager.LoadScene("Loading");
    }

    public void ExitGame() => Application.Quit();

    private void UpdateCashText(int cash) => cashText.text = cash + "$";
}
