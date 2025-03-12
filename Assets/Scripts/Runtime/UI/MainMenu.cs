using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LoJam
{
    public class MainMenu : View
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;

        [SerializeField] private View _settingsOverlay;

        private void Awake()
        {
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _startButton.onClick.AddListener(OnStartClicked);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnStartClicked()
        {
            LoJamGameManager.GetMonoSystem<IUIMonoSystem>().PopView();
            Hud hud = FindFirstObjectByType<Hud>(FindObjectsInactive.Include);
            LoJamGameManager.GetMonoSystem<IUIMonoSystem>().PushView(hud);
        }

        private void OnSettingsClicked()
        {
            LoJamGameManager.GetMonoSystem<IUIMonoSystem>().PushView(_settingsOverlay);
        }
    }
}
