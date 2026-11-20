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

    public override void Initialize()
    {
        base.Initialize();
        if (masterSlider == null || musicSlider == null || sfxSlider == null) BuildRuntimeControls();
        if (masterSlider != null) { Configure(masterSlider, "MasterVolume", 1f); masterSlider.onValueChanged.AddListener(SetMaster); }
        if (musicSlider != null) { Configure(musicSlider, "MusicVolume", 1f); musicSlider.onValueChanged.AddListener(SetMusic); }
        if (sfxSlider != null) { Configure(sfxSlider, "SfxVolume", 1f); sfxSlider.onValueChanged.AddListener(SetSfx); }
        if (backButton != null) backButton.onClick.AddListener(() => UiManager.Instance?.HideWindow("SettingsUi"));
        if (masterSlider != null) SetMaster(masterSlider.value);
        if (musicSlider != null) SetMusic(musicSlider.value);
        if (sfxSlider != null) SetSfx(sfxSlider.value);
    }

    private void BuildRuntimeControls()
    {
        RectTransform root = CanvasRectTransform;
        if (root == null) return;
        var panel = RuntimeUiFactory.Panel(root, "Audio Settings Panel", new Vector2(650, 510), Vector2.zero);
        RuntimeUiFactory.Text(panel, "SETTINGS", new Vector2(0, 195), new Vector2(500, 55), 34);
        masterSlider = RuntimeUiFactory.Slider(panel, "Master volume", 95);
        musicSlider = RuntimeUiFactory.Slider(panel, "Music volume", 0);
        sfxSlider = RuntimeUiFactory.Slider(panel, "Effects volume", -95);
        backButton = RuntimeUiFactory.Button(panel, "RETURN TO GAME", new Vector2(0, -190), new Vector2(280, 64), null);
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
