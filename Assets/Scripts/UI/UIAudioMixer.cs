using __Workspaces.Alex.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioMixer : MonoBehaviour
{
    [Header("Sliders")] 
    [SerializeField] private Slider gameplaySlider;
    [SerializeField] private Slider musicSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameplaySlider.SetValueWithoutNotify(AudioManager.Instance.GameplayVolume);
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);

        gameplaySlider.onValueChanged.AddListener(OnGameplayVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
    }
    
    private void OnDestroy()
    {
        gameplaySlider.onValueChanged.RemoveListener(OnGameplayVolumeChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
    }

    private void OnGameplayVolumeChanged(float value)
    {
        AudioManager.Instance.SetGameplayVolume(value);
    }
    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

}
