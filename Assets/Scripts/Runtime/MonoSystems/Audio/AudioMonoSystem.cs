using System.Collections.Generic;
using UnityEngine;

namespace LoJam.MonoSystem
{
    public class AudioMonoSystem : MonoBehaviour, IAudioMonoSystem
    {
        [SerializeField] private AudioSource _musicSrc;
        [SerializeField] private AudioSource _sfxSrc;

        [SerializeField] private List<AudioClip> _sfx;

        public void PlaySfX(int id)
        {
            _sfxSrc.PlayOneShot(_sfx[id]);
        }
    }
}
