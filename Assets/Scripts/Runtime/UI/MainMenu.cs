using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LoJam
{
    public class MainMenu : View
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;

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

        }

        private void OnSettingsClicked()
        {
            // bad
            SettingsMenu settingsMenu = FindObjectsByType<SettingsMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault();

            LoJamGameManager.GetMonoSystem<IUIMonoSystem>().PushView(settingsMenu);
        }
    }
}
