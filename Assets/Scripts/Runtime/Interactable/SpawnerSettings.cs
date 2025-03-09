using LoJam.Core;
using LoJam.Interactable;
using System.Collections.Generic;
using UnityEngine;

namespace LoJam
{
    [CreateAssetMenu(fileName = "DefaultSpawnerSettings", menuName = "Settings/Spawner")]
    public class SpawnerSettings : ScriptableObject
    {
        [Header("Tick")]
        public float tickRate;

        [Header("Sampling")]
        public int seed = -1;
        public float radius;
        public int k = 30;

        [Header("Powerups")]
        [Range(0, 1)] public float powerUpSpawnRate;
        public List<BasePowerup> powerupList;
    }
}
