using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

namespace LoJam.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private MonoSystemManager _ms;


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
            DontDestroyOnLoad(_instance);

            // Runs on load logic.
            OnLoad();
        }

        private void AttachMonoSystems() {

        }

        private static void OnLoad() {
            _instance.AttachMonoSystems();
            // Ensure all MonoSystems call Awake at the same time
            _instance._msHolder.SetActive(true);
        }

        public void AddMonoSystem<TMonoSystem, TBindTo>(TMonoSystem ms) where TMonoSystem : TBindTo, IMonoSystem {
            _ms.AddMonoSystem<TMonoSystem, TBindTo>(ms);
        }

        public TMonoSystem GetMonoSystem<TMonoSystem>() {
            return _ms.GetMonoSystem<TMonoSystem>();
        }
    }
}
