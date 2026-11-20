using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Audio settings kept in PlayerPrefs and applied immediately.</summary>
public class SettingsUi : UIWindow
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button backButton;
    [SerializeField] private string returnWindow = "GameplayUi";

    public override void Initialize()
    {
        base.Initialize();
        if (masterSlider != null) { Configure(masterSlider, "MasterVolume", 1f); masterSlider.onValueChanged.AddListener(SetMaster); }
        if (musicSlider != null) { Configure(musicSlider, "MusicVolume", 1f); musicSlider.onValueChanged.AddListener(SetMusic); }
        if (sfxSlider != null) { Configure(sfxSlider, "SfxVolume", 1f); sfxSlider.onValueChanged.AddListener(SetSfx); }
        if (backButton != null) backButton.onClick.AddListener(() => UiManager.Instance?.ShowWindow(returnWindow));
    }

    private static void Configure(Slider slider, string key, float fallback)
    {
        slider.minValue = 0f; slider.maxValue = 1f;
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(key, fallback));
    }

    private void SetMaster(float value) { PlayerPrefs.SetFloat("MasterVolume", value); AudioListener.volume = value; PlayerPrefs.Save(); }
    private void SetMusic(float value) { SaveAndApply("MusicVolume", value, Dino.Utility.Audio.AudioType.Music); }
    private void SetSfx(float value) { SaveAndApply("SfxVolume", value, Dino.Utility.Audio.AudioType.SFX); }

    private static void SaveAndApply(string key, float value, Dino.Utility.Audio.AudioType type)
    {
        PlayerPrefs.SetFloat(key, value); PlayerPrefs.Save();
        if (Dino.Utility.Audio.AudioManager.Instance != null) Dino.Utility.Audio.AudioManager.Instance.UpdateAudioMixerGroupVolume(value, type);
    }

    private void Awake() { AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f); }
}
