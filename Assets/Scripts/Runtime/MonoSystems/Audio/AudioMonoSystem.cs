using System.Collections.Generic;
using UnityEngine;

namespace LoJam.MonoSystem
{
    public class AudioMonoSystem : MonoBehaviour, IAudioMonoSystem
    {
        [SerializeField] private AudioSource _musicSrc;
        [SerializeField] private AudioSource _sfxSrc;

        [SerializeField, Range(0f, 1f)] private float _volume;

        [SerializeField] private List<AudioClip> _sfx;
        [SerializeField] private List<AudioClip> _music;

        private float _sfxBaseline;
        private float _musicBaseline;

        public float GetVolume() => _volume;

        public void SetVolume(float volume) 
        {
            _volume = volume;
            _musicSrc.volume = _musicBaseline * _volume;
            _sfxSrc.volume = _sfxBaseline * _volume;
        }

        public void PlayMusic(int id)
        {
            //if (_musicSrc.clip == _music[id]) return;
            _musicSrc.Stop();
            _musicSrc.clip = _music[id];
            _musicSrc.Play();
        }

        public void PlaySfX(int id)
        {
            _sfxSrc.PlayOneShot(_sfx[id]);
        }

        private void Awake()
        {
            _sfxBaseline = _sfxSrc.volume;
            _musicBaseline = _musicSrc.volume;
            SetVolume(_volume);
        }
    }
}
