using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private void Start()
    {
        musicVolumeSlider.value = MusicPlayer.Instance.GetVolume();
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        sfxVolumeSlider.value = 0.4f;
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        OnSFXVolumeChanged(sfxVolumeSlider.value);

        qualityDropdown.value = 2;
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        OnQualityChanged(2);
    }

    public void OnMusicVolumeChanged(float value)
    {
        MusicPlayer.Instance.SetVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        SaveSystem.SetFloat(PrefsKeys.SFX_VOLUME, value);
    }

    public void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}
