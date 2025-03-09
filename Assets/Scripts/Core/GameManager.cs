using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

namespace LoJam.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private MonoSystemManager _ms;
        private EventManager _eventManager;


        [Header("MonoSystem Holder")]
        [SerializeField] private GameObject _msHolder;

        //[Header("MonoSystems")]

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
            OnLoad();
        }

        private void DeleteMe(Component senderm, object data) {
            Debug.Log(((GameEvents.TestEvent)data).test);
        }

        private void AddEvents() {
            _eventManager.AddEvent<GameEvents.TestEvent>(DeleteMe);
        }

        private void AttachMonoSystems() {
            // Add MonoSystems Here
        }

        private static void OnLoad() {
            // Attaches MonoSystem
            _instance.AttachMonoSystems();
            
            // Added Events
            _instance.AddEvents();

            // Ensure all MonoSystems call Awake at the same time
            _instance._msHolder.SetActive(true);

            //_instance._eventManager.InvokeEvent<GameEvents.TestEvent>(null, new GameEvents.TestEvent("Works!"));
        }

        public void AddMonoSystem<TMonoSystem, TBindTo>(TMonoSystem ms) where TMonoSystem : TBindTo, IMonoSystem {
            _ms.AddMonoSystem<TMonoSystem, TBindTo>(ms);
        }

        public TMonoSystem GetMonoSystem<TMonoSystem>() {
            return _ms.GetMonoSystem<TMonoSystem>();
        }
    }
}
