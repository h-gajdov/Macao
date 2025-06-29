using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour {
    public Slider musicSlider;
    public Slider sfxSlider;
    public Image musicImage;
    public Image sfxImage;
    public TMP_Dropdown resolutionsDropdown;
    public Toggle fullscreenToggle;
    public Button applyButton;
    private TextMeshProUGUI buttonText;

    public Sprite[] fullscreenToggleImages;
    public Sprite[] musicEnabledImages;
    public Sprite[] sfxEnabledImages;

    private List<Resolution> availableResolutions = new List<Resolution>();
    private bool isFullscreen;

    private void Start() {
        SetResolutionsOnDropdown();

        if(!PlayerPrefs.HasKey("SettingsSet")) {
            SetDefaultSettings();
        } else {
            LoadSettings();
        }

        applyButton.interactable = false;
        buttonText = applyButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update() {
        Color target = buttonText.color;
        target.a = (applyButton.interactable) ? 1f : 0.5f;
        buttonText.color = target;
    }

    public static Resolution[] GetAllResolutions() {
        return Screen.resolutions
            .GroupBy(r => (r.width, r.height))
            .Select(g => g.OrderByDescending(r => r.refreshRate).First())
            .ToArray();
    }

    private void SetResolutionsOnDropdown() {
        resolutionsDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
        foreach(Resolution res in GetAllResolutions()) {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData(res.width + "x" + res.height);
            availableResolutions.Add(res);
            list.Add(data);
        }
        list.Reverse();
        availableResolutions.Reverse();
        resolutionsDropdown.AddOptions(list);
    }

    public void SetDefaultSettings() {
        isFullscreen = true;
        fullscreenToggle.isOn = isFullscreen;
        ToggleFullscreen();

        resolutionsDropdown.value = 0;
        ChangeResolution();

        InitializeAudio(0.5f, 0.5f);

        PlayerPrefs.SetInt("SettingsSet", 1);
    }

    public void ChangeResolution() {
        Resolution curr = availableResolutions[resolutionsDropdown.value];
        Screen.SetResolution(curr.width, curr.height, isFullscreen);
        applyButton.interactable = true;
    }

    public void ChangeMusicVolume() {
        AudioManager.musicVolume = musicSlider.value;
        musicImage.sprite = musicEnabledImages[Mathf.CeilToInt(AudioManager.musicVolume)];
        AudioManager.UpdateVolumes();
        SaveAudioSettings();
        applyButton.interactable = true;
    }

    public void ChangeSFXVolume() {
        AudioManager.sfxVolume = sfxSlider.value;
        sfxImage.sprite = sfxEnabledImages[Mathf.CeilToInt(AudioManager.sfxVolume)];
        AudioManager.UpdateVolumes();
        SaveAudioSettings();
        applyButton.interactable = true;
    }

    public void ToggleFullscreen() {
        isFullscreen = fullscreenToggle.isOn;
        fullscreenToggle.image.sprite = (isFullscreen) ? fullscreenToggleImages[1] : fullscreenToggleImages[0];
        Screen.fullScreen = isFullscreen;
        applyButton.interactable = true;
    }

    private void SaveAudioSettings() {
        PlayerPrefs.SetFloat("MusicVolume", AudioManager.musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", AudioManager.sfxVolume);
    }

    public void SaveSettings() {
        PlayerPrefs.SetFloat("MusicVolume", AudioManager.musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", AudioManager.sfxVolume);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionsDropdown.value);
        PlayerPrefs.SetInt("IsFullscreen", (isFullscreen) ? 1 : 0);
        applyButton.interactable = false;
    }

    private void LoadSettings() {
        fullscreenToggle.isOn = PlayerPrefs.GetInt("IsFullscreen") == 1;
        ToggleFullscreen();

        int resIndex = PlayerPrefs.GetInt("ResolutionIndex");
        Resolution curr = availableResolutions[resIndex];
        Screen.SetResolution(curr.width, curr.height, isFullscreen);
        resolutionsDropdown.value = resIndex;

        InitializeAudio(PlayerPrefs.GetFloat("MusicVolume"), PlayerPrefs.GetFloat("SFXVolume"));
    }

    private void InitializeAudio(float music, float sfx) {
        AudioManager.musicVolume = music;
        AudioManager.sfxVolume = sfx;
        musicSlider.value = AudioManager.musicVolume;
        sfxSlider.value = AudioManager.sfxVolume;
        musicImage.sprite = musicEnabledImages[Mathf.CeilToInt(AudioManager.musicVolume)];
        sfxImage.sprite = sfxEnabledImages[Mathf.CeilToInt(AudioManager.sfxVolume)];
        AudioManager.UpdateVolumes();
    }
}
