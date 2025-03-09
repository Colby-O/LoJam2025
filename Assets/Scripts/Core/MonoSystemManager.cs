using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoJam.Core
{
    public class MonoSystemManager : MonoBehaviour
    {
        private Dictionary<Type, IMonoSystem> _monoSystems;

        public void AddMonoSystem<TMonoSystem>(IMonoSystem ms) where TMonoSystem : IMonoSystem {
            if (_monoSystems.ContainsKey(typeof(TMonoSystem))) return;
            else _monoSystems.Add(typeof(TMonoSystem), ms);
        }

        public void RemoveMonoSystem<TMonoSystem>() where TMonoSystem : IMonoSystem {
            if (_monoSystems.ContainsKey(typeof(TMonoSystem))) _monoSystems.Remove(typeof(TMonoSystem));
        }

        public TMonoSystem GetMonoSystem<TMonoSystem>() where TMonoSystem : IMonoSystem {
            return (TMonoSystem)_monoSystems[typeof(TMonoSystem)];   
        }
    }
}
