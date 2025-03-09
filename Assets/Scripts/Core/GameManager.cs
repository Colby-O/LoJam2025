using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace LoJam.Core
{
    public abstract class GameManager : MonoBehaviour
    {
        protected static GameManager _instance;
        private MonoSystemManager _ms;
        private EventManager _eventManager;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnInitalized() {

            // Checks if GameManager already exist
            if (_instance) return; 

            // Instaniate GameManager
            GameManager gm = Instantiate(Resources.Load<GameManager>("GameManager"));

            // Checks if a GameManager was found
            if (!gm) Debug.LogError("GameManager Cannot be found in Resources.");

            // Sets Instance state
            _instance = gm;
            _instance._ms = new MonoSystemManager();
            _instance._eventManager = new EventManager();
            DontDestroyOnLoad(_instance);

            // Runs on load logic.
            _instance.OnLoad();
        }

        protected abstract void OnLoad();

        public void AddEvent<TEvent>(UnityAction<Component, object> callback) {
            _eventManager.AddEvent<TEvent>(callback);
        }

        public void EmitEvent<TEvent>(Component sender, object data) {
            _eventManager.InvokeEvent<TEvent>(sender, data);
        }

        public void AddMonoSystem<TMonoSystem, TBindTo>(TMonoSystem ms) where TMonoSystem : TBindTo, IMonoSystem {
            _ms.AddMonoSystem<TMonoSystem, TBindTo>(ms);
        }

        public TMonoSystem GetMonoSystem<TMonoSystem>() {
            return _ms.GetMonoSystem<TMonoSystem>();
        }
    }
}
