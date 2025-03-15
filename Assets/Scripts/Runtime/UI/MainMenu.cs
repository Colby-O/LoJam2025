using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using LoJam.Core;
using LoJam.MonoSystem;

namespace LoJam
{
    public class MainMenu : View
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        [SerializeField] private Slider _volume;

        [SerializeField] private View _settingsOverlay;

        private void ChangeVolume(float volume) 
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetVolume(volume);
        }

        void Start()
        {
            _startButton.onClick.AddListener(OnStartClicked);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
            _quitButton.onClick.AddListener(OnQuitClicked);

            _volume.onValueChanged.AddListener(ChangeVolume);

            _volume.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetVolume();
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }

        private void OnStartClicked()
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayMusic(1);
            LoJamGameManager.isPaused = false;

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
