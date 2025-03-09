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

        public static void AddEvent<TEvent>(UnityAction<Component, object> callback) {
            _instance._eventManager.AddEvent<TEvent>(callback);
        }

        public static void EmitEvent<TEvent>(Component sender, object data) {
            _instance._eventManager.InvokeEvent<TEvent>(sender, data);
        }

        public static void AddMonoSystem<TMonoSystem, TBindTo>(TMonoSystem ms) where TMonoSystem : TBindTo, IMonoSystem {
            _instance._ms.AddMonoSystem<TMonoSystem, TBindTo>(ms);
        }

        public static  TMonoSystem GetMonoSystem<TMonoSystem>() {
            return _instance._ms.GetMonoSystem<TMonoSystem>();
        }
    }
}
