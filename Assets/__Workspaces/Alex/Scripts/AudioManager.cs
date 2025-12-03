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

        public EventInstance Play(EventReference eventRef, bool loop = false, GameObject follow = null)
        {
            if (eventRef.IsNull)
            {
                Debug.LogWarning("[AudioManager] EventReference is null.");
                return default;
            }

            var instance = RuntimeManager.CreateInstance(eventRef);

            if (follow != null)
                RuntimeManager.AttachInstanceToGameObject(instance, follow);

            instance.start();

            if (!loop)
                instance.release();
            else
                _active.Add(instance);

            return instance;
        }

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

        public void StopAll(FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
        {
            foreach (var e in _active)
                e.stop(mode);

            _active.Clear();
        }

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

        /// <summary>
        /// Define the bus volume (0-1)
        /// </summary>
        /// <param name="bus">Bus to modify</param>
        /// <param name="volume">Normalized float value for volume</param>
        public void SetBusVolume(AudioBus bus, float volume)
        {
            string busPath = GetPath(bus);
            if (RuntimeManager.StudioSystem.getBus(busPath, out var busObj) == FMOD.RESULT.OK)
                busObj.setVolume(Mathf.Clamp01(volume));
            else
                Debug.LogWarning($"[AudioManager] Bus not found: {busPath}");
        }
    
        /// <summary>
        /// Mute the bus
        /// </summary>
        /// <param name="bus">Bus to mute</param>
        /// <param name="mute">Boolean value for mute or demute</param>
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
                AudioBus.Master  => "bus:/",
                AudioBus.Music   => "bus:/Music",
                AudioBus.SFX     => "bus:/SFX",
                AudioBus.UI      => "bus:/UI",
                AudioBus.Ambient => "bus:/Ambient",
                _                => "bus:/"
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
    }
}