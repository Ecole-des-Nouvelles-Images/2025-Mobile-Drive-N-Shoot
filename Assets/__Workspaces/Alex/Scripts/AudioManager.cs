using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Utils.Singletons;

namespace __Workspaces.Alex.Scripts
{
    public class AudioManager : MonoBehaviourSingletonDontDestroyOnLoad<AudioManager>
    {
        private readonly List<EventInstance> _active = new();

        // Store looped unique events (music, ambiance…)
        private readonly Dictionary<EventReference, EventInstance> _uniqueInstances = new();
        
        //Player prefs keys for volume settings
        private const string PREF_GAMEPLAY_VOL = "Audio_GameplayVolume";
        private const string PREF_MUSIC_VOL = "Audio_MusicVolume";

        private const float DEFAULT_GAMEPLAY_VOL = 0.8f;
        private const float DEFAULT_MUSIC_VOL = 0.8f;
        
        // Cached volume levels
        private float _gameplayVolume;
        private float _musicVolume;
        
        // PUBLIC GETTERS
        public float GameplayVolume => _gameplayVolume;
        public float MusicVolume => _musicVolume;

        // ----------------------------------------------------------------------
        // PLAY (MAIN METHOD)
        // ----------------------------------------------------------------------
        
        protected override void Awake()
        {
            base.Awake();

            LoadVolumes();
        }
        
        public EventInstance Play(EventReference eventRef, bool loop = false, GameObject follow = null)
        {
            if (eventRef.IsNull)
            {
                Debug.LogWarning("[AudioManager] EventReference is null.");
                return default;
            }

            // ---------------------------------------------------------
            // UNIQUE LOOPED EVENTS (MUSIC, AMBIANCE…)
            // ---------------------------------------------------------
            if (loop)
            {
                if (_uniqueInstances.TryGetValue(eventRef, out var existing))
                {
                    existing.getPlaybackState(out var state);

                    // If already playing → return it
                    if (state != PLAYBACK_STATE.STOPPED)
                        return existing;

                    // If stopped → restart
                    existing.start();
                    return existing;
                }
            }

            // ---------------------------------------------------------
            // NORMAL BEHAVIOUR (create new instance)
            // ---------------------------------------------------------
            var instance = RuntimeManager.CreateInstance(eventRef);

            if (follow != null)
                RuntimeManager.AttachInstanceToGameObject(instance, follow);

            instance.start();

            if (!loop)
            {
                // Non-looping events are released after playback
                instance.release();
            }
            else
            {
                // Looped events are managed manually
                _active.Add(instance);
                _uniqueInstances[eventRef] = instance;
            }

            return instance;
        }

        // ----------------------------------------------------------------------
        // PLAY AT POSITION
        // ----------------------------------------------------------------------
        public EventInstance PlayAtPosition(EventReference eventRef, Vector3 position, bool loop = false)
        {
            if (eventRef.IsNull)
            {
                Debug.LogWarning("[AudioManager] EventReference is null.");
                return default;
            }

            var instance = RuntimeManager.CreateInstance(eventRef);
            instance.set3DAttributes(position.To3DAttributes());
            instance.start();

            if (!loop)
                instance.release();
            else
                _active.Add(instance);

            return instance;
        }

        // ----------------------------------------------------------------------
        // STOP SINGLE INSTANCE
        // ----------------------------------------------------------------------
        public void Stop(EventInstance instance, FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
        {
            if (EqualityComparer<EventInstance>.Default.Equals(instance, default))
                return;

            instance.stop(mode);
            instance.release();

            // Remove from active list
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (EqualityComparer<EventInstance>.Default.Equals(_active[i], instance))
                {
                    _active.RemoveAt(i);
                }
            }

            // Remove from unique dictionary
            foreach (var kvp in _uniqueInstances)
            {
                if (EqualityComparer<EventInstance>.Default.Equals(kvp.Value, instance))
                {
                    _uniqueInstances.Remove(kvp.Key);
                    break;
                }
            }
        }

        // ----------------------------------------------------------------------
        // STOP ALL
        // ----------------------------------------------------------------------
        public void StopAll(FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
        {
            foreach (var e in _active)
                e.stop(mode);

            _active.Clear();
            _uniqueInstances.Clear();
        }

        // ----------------------------------------------------------------------
        // CLEANUP LOOPED INSTANCES
        // ----------------------------------------------------------------------
        private void Update()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                var e = _active[i];
                e.getPlaybackState(out var state);

                if (state == PLAYBACK_STATE.STOPPED)
                {
                    e.release();
                    _active.RemoveAt(i);
                }
            }
        }

        // ----------------------------------------------------------------------
        // BUS CONTROL
        // ----------------------------------------------------------------------
        public void SetBusVolume(AudioBus bus, float volume)
        {
            string busPath = GetPath(bus);
            if (RuntimeManager.StudioSystem.getBus(busPath, out var busObj) == FMOD.RESULT.OK)
                busObj.setVolume(Mathf.Clamp01(volume));
            else
                Debug.LogWarning($"[AudioManager] Bus not found: {busPath}");
        }

        public void MuteBus(AudioBus bus, bool mute)
        {
            string busPath = GetPath(bus);
            if (RuntimeManager.StudioSystem.getBus(busPath, out var busObj) == FMOD.RESULT.OK)
                busObj.setMute(mute);
            else
                Debug.LogWarning($"[AudioManager] Bus not found: {busPath}");
        }

        private string GetPath(AudioBus bus)
        {
            return bus switch
            {
                AudioBus.Master => "bus:/",
                AudioBus.Music => "bus:/Music",
                AudioBus.SFX => "bus:/SFX",
                AudioBus.UI => "bus:/UI",
                AudioBus.Ambient => "bus:/Ambient",
                _ => "bus:/"
            };
        }

        public enum AudioBus
        {
            Master,
            Music,
            SFX,
            UI,
            Ambient
        }
        // ----------------------------------------------------------------------
        // VOLUME GROUPS (UI SLIDERS)
        // ----------------------------------------------------------------------
        private float ApplyCurve(float value)
        {
            // simple quadratic curve for volume perception
            return Mathf.Pow(Mathf.Clamp01(value), 2f);
        }

        public void SetGameplayVolume(float volume)
        {
            _gameplayVolume = Mathf.Clamp01(volume);

            float v = ApplyCurve(_gameplayVolume);

            SetBusVolume(AudioBus.SFX, v);
            SetBusVolume(AudioBus.Ambient, v);

            PlayerPrefs.SetFloat(PREF_GAMEPLAY_VOL, _gameplayVolume);
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);

            float v = ApplyCurve(_musicVolume);

            SetBusVolume(AudioBus.Music, v);

            PlayerPrefs.SetFloat(PREF_MUSIC_VOL, _musicVolume);
        }
        
        private void LoadVolumes()
        {
            _gameplayVolume = PlayerPrefs.GetFloat(PREF_GAMEPLAY_VOL, DEFAULT_GAMEPLAY_VOL);
            _musicVolume = PlayerPrefs.GetFloat(PREF_MUSIC_VOL, DEFAULT_MUSIC_VOL);

            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            SetGameplayVolume(_gameplayVolume);
            SetMusicVolume(_musicVolume);
        }
    }
}
