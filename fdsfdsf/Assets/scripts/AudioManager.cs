using System;
using System.Collections.Generic;
using Dino.Utility.Audio;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering;
using AudioType = Dino.Utility.Audio.AudioType;

namespace Dino.Utility.Audio
{
    /// <summary>
    /// Last update 27/04/2025 Dino
    /// A class that allows you to manage audio.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        public static AudioManager Instance { get; private set; }
        #endregion
        
        #region serialized fields

        
        
        [Header("Audio Manager Data")] 
        [SerializeField]
        private AudioManagerData _audioManagerData;

        
        [Header("Audio Test")]
        public string _soundNameTest;

        #endregion

        #region private fields

        private GameObject _soundsContainer;
        private List<AudioSource> _audioSources = new List<AudioSource>();
        private Dictionary<string, AudioSource> _sourcesByName = new Dictionary<string, AudioSource>();

        private List<AudioData> _sfxAudioData = new List<AudioData>();
        private List<AudioData> _musicAudioData = new List<AudioData>();

        #endregion

        public AudioManagerData AudioManagerData
        {
            get => _audioManagerData;
            set => _audioManagerData = value;
        }


        #region unity methods

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
           
            
        }

        
        #endregion

        #region private methods
        
       

        private AudioSource FindAudioSource(string soundName)
        {
            return _sourcesByName.TryGetValue(soundName, out AudioSource source) ? source : null;
        }

        private void Initialize()
        {
            _soundsContainer = new GameObject("SoundsContainer");
            _audioSources = new List<AudioSource>();
            _sourcesByName = new Dictionary<string, AudioSource>();
            _soundsContainer.transform.SetParent(transform);
            if (_audioManagerData == null) { Debug.LogError("AudioManagerData is not assigned.", this); return; }
            PrepareLists();
            AudioListener.volume = Mathf.Clamp01(PlayerPrefs.GetFloat("MasterVolume", 1f));
            UpdateAudioMixerGroupVolume(PlayerPrefs.GetFloat("MusicVolume", 1f), AudioType.Music);
            UpdateAudioMixerGroupVolume(PlayerPrefs.GetFloat("SfxVolume", 1f), AudioType.SFX);
            PlaySound("music");
        }

        private static float SavedChannelVolume(AudioType audioType)
        {
            string key = audioType == AudioType.Music ? "MusicVolume" : "SfxVolume";
            return Mathf.Clamp01(PlayerPrefs.GetFloat(key, 1f));
        }

        

        private void PrepareLists()
        {
            _sfxAudioData = _audioManagerData.audioData.FindAll(x => x.audioType == AudioType.SFX);
            _musicAudioData = _audioManagerData.audioData.FindAll(x => x.audioType == AudioType.Music);
        }

        private void SetAudioInMixerGroup(AudioSource audioSource, AudioType audioType)
        {
            switch (audioType)
            {
                case AudioType.Music:
                    audioSource.outputAudioMixerGroup = _audioManagerData.MusicAudioMixerGroup;
                    break;
                case AudioType.SFX:
                    audioSource.outputAudioMixerGroup = _audioManagerData.SfxAudioMixerGroup;
                    break;
                case AudioType.Ambience:
                    audioSource.outputAudioMixerGroup = _audioManagerData.AmbienceAudioMixerGroup;
                    break;
            }
        }
        
        #endregion


        #region public methdos

        public void PlaySound(string soundName)
        {
            if (_audioManagerData == null || _soundsContainer == null) return;
            AudioData audioData = _audioManagerData.audioData.Find(x => x.name == soundName);
            if (audioData == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }
            if (audioData.clip == null) return;

            AudioSource audioSource = FindAudioSource(audioData.name);

            if (audioSource == null)
            {
                GameObject go = new GameObject("AS : " + audioData.name);
                audioSource = go.AddComponent<AudioSource>();
                audioSource.transform.SetParent(_soundsContainer.transform);
                audioSource.clip = audioData.clip;
                audioSource.loop = audioData.loop;
                audioSource.volume = audioData.volume * SavedChannelVolume(audioData.audioType);
                audioSource.pitch = audioData.pitch;
                audioSource.spatialBlend = 0f;
                SetAudioInMixerGroup(audioSource, audioData.audioType);
                _audioSources.Add(audioSource);
                _sourcesByName.Add(audioData.name, audioSource);
                audioSource.Play();
            }
            else
            {
                audioSource.Play();
            }
        }

        public void StopSound(string soundName)
        {
            if (_audioManagerData == null) return;
            AudioData audioData = _audioManagerData.audioData.Find(x => x.name == soundName);
            if (audioData == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }

            AudioSource audioSource = FindAudioSource(audioData.name);
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }
        
        public void UpdateAudioMixerGroupVolume(float value, AudioType audioType)
        {
            value = Mathf.Clamp01(value);
            float db = Mathf.Clamp(20f * Mathf.Log10(Mathf.Max(0.0001f, value)), -80f, 0f);
            if (_audioManagerData == null) return;

            AudioMixerGroup group = null;
            string parameter = string.Empty;
            switch (audioType)
            {
                case AudioType.Music:
                    group = _audioManagerData.MusicAudioMixerGroup;
                    parameter = _audioManagerData.MusicVolumeParameterName;
                    break;
                case AudioType.SFX:
                    group = _audioManagerData.SfxAudioMixerGroup;
                    parameter = _audioManagerData.SfxVolumeParameterName;
                    break;
                case AudioType.Ambience:
                    group = _audioManagerData.AmbienceAudioMixerGroup;
                    parameter = _audioManagerData.AmbienceVolumeParameterName;
                    break;
            }

            if (group != null && group.audioMixer != null && group.audioMixer.SetFloat(parameter, db)) return;

            foreach (AudioData data in _audioManagerData.audioData)
            {
                bool isMusic = data.audioType == AudioType.Music;
                bool controlledChannel = audioType == AudioType.Music ? isMusic : !isMusic;
                if (controlledChannel && _sourcesByName.TryGetValue(data.name, out AudioSource source) && source != null)
                    source.volume = data.volume * value;
            }
        }

        #endregion
        
        [Button]
        void TestSound()
        {
            PlaySound(_soundNameTest);
            Debug.Log("Playing sound: " + _soundNameTest);
            
        }
    }
}

namespace Dino.Utility.Audio
{
    public enum AudioType
    {
        Music,
        SFX,
        Ambience
    }
}
