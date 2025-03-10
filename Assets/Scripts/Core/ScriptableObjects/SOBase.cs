using UnityEngine;

namespace LoJam.Core
{
    public abstract class BaseSO : ScriptableObject
    {
        [Header("Global SO Settings")]
        public int id = -1;
        public bool includeInDatabase = true;
    }
}
