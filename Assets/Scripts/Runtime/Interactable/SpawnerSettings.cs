using LoJam.Core;
using LoJam.Interactable;
using System.Collections.Generic;
using System.Linq;
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

        [Header("Crafting Materials")]
        [Range(0, 1)] public float randomness;
        public List<CraftingMaterial> matieralList;

        public CraftingMaterial FetchMaterial(List<CraftingMaterial> materials)
        {
            List<int> counts = new List<int>();

            foreach (CraftingMaterial mat in matieralList)
            {
                counts.Add(materials.Where(m => m.GetMaterialType() == mat.GetMaterialType()).Count());
            }

            List<int> minIndice = counts
            .Select((value, index) => new { value, index })
            .Where(pair => pair.value == counts.Min())
            .Select(pair => pair.index)
            .ToList();

            List<int> secondMinIndice = counts
            .Select((value, index) => new { value, index })
            .Where(pair => pair.value == counts.OrderBy(x => x).Skip(1).First())
            .Select(pair => pair.index)
            .ToList();
            Debug.Log(secondMinIndice.Count);
            return (Random.value > randomness || secondMinIndice.Count == 0) ? 
                matieralList[minIndice[Random.Range(0, minIndice.Count)]] : 
                matieralList[secondMinIndice[Random.Range(0, secondMinIndice.Count)]];
        }
    }
}
