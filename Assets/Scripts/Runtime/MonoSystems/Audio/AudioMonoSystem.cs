using UnityEngine;

namespace LoJam.MonoSystem
{
    public class AudioMonoSystem : MonoBehaviour, IAudioMonoSystem
    {
        [SerializeField] private AudioSource _musicSrc;
    }
}
