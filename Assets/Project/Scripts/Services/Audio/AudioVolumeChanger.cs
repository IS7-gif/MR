using Project.Scripts.Services.Audio.AudioSystem;
using UnityEngine;
using VContainer;

namespace Project.Scripts.Services.Audio
{
    public class AudioVolumeChanger : MonoBehaviour
    {
        private AudioManager _audioManager;


        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }


        public void SetMusicVolume(float volume)
        {
            _audioManager.SetMusicVolume(volume);
        }

        public void SetSFXVolume(float volume)
        {
            _audioManager.SetSFXVolume(volume);
        }
    }
}