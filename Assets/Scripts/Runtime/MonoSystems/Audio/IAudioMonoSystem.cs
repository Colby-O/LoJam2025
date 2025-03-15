using LoJam.Core;
using UnityEngine;

namespace LoJam.MonoSystem
{
    public interface IAudioMonoSystem : IMonoSystem
    {
        public void PlaySfX(int id);
        public void SetVolume(float volume);
        public float GetVolume();
    }
}
