using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoJam.Core
{
    public class MonoSystemManager 
    {
        private Dictionary<Type, IMonoSystem> _monoSystems = new Dictionary<Type, IMonoSystem>();

        public void AddMonoSystem<TMonoSystem, TBindTo>(TMonoSystem ms) where TMonoSystem : TBindTo, IMonoSystem {
            if (_monoSystems.ContainsKey(typeof(TBindTo))) return;
            else _monoSystems.Add(typeof(TBindTo), ms);
        }

        public void RemoveMonoSystem<TMonoSystem>() where TMonoSystem : IMonoSystem {
            if (_monoSystems.ContainsKey(typeof(TMonoSystem))) _monoSystems.Remove(typeof(TMonoSystem));
        }

        public TMonoSystem GetMonoSystem<TMonoSystem>() {
            if (_monoSystems.TryGetValue(typeof(TMonoSystem), out IMonoSystem ms)) return (TMonoSystem)ms;
            else {
                throw new Exception($"MonoSystem of type {typeof(TMonoSystem)} cannot be found.");
            }
        }
    }
}
