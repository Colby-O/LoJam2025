using LoJam.Core;
using LoJam.Interactable;
using LoJam.Logic;
using LoJam.MonoSystem;
using LoJam.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoJam
{
	public sealed class LoJamGameManager : GameManager
	{

		[Header("MonoSystem Holder")]
		[SerializeField] private GameObject _msHolder;

		[Header("MonoSystems")]
		[SerializeField] private GridMonoSystem _gridSystem;
		[SerializeField] private UIMonoSystem _uiSystem;
		[SerializeField] private CraftingMonoSystem _craftingSystem;
		[SerializeField] private AudioMonoSystem _audioSystem;

		public static float time = 5f * 60f;
		public static  bool isPaused;

		public static List<Interactor> players;
		public static List<CraftingStation> craftingStations;

		public static string GetFormattedTime() 
		{
        	float minutes = Mathf.FloorToInt(time / 60);
        	float seconds = Mathf.FloorToInt(time % 60);
			float ms = (time - Mathf.Floor(time)) * 60;

        	return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, ms);
		}

		private void AddEvents() {
			//AddEvent<GameEvents.TestEvent>(DeleteMe);
		}

		private void AttachMonoSystems() {

			AddMonoSystem<GridMonoSystem, IGridMonoSystem>(_gridSystem);
			AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiSystem);
			AddMonoSystem<CraftingMonoSystem, ICraftingMonoSystem>(_craftingSystem);
			AddMonoSystem<AudioMonoSystem, IAudioMonoSystem>(_audioSystem);
		}

		protected override void OnLoad() {
			// Attaches MonoSystem
			AttachMonoSystems();

			// Added Events
			AddEvents();

			// Ensure all MonoSystems call Awake at the same time
			_msHolder.SetActive(true);
		}

		public static void EndGame(Side side)
		{
			IUIMonoSystem ui = GetMonoSystem<IUIMonoSystem>();

            bool foundFade = ui.GetViews().TryGetValue("FadedOverlay", out View vFadedOverlay);
            bool foundPlayer = ui.GetViews().TryGetValue("PlayerWin", out View vPlayerWin);
            if (foundFade && foundPlayer && vFadedOverlay is FadedOverlay fadedOverlay && vPlayerWin is UIPlayerWin playerWin)
            {
                ui.PushView(fadedOverlay);
				ui.PushView(playerWin);
				playerWin.SetWinner(side);
            }
			Debug.Log($"Wow game is over! Side {side} won.");
        }

		public void ReassignControllers()
		{
			Interactor.ResetRegisteredControllerList();
			foreach (Interactor player in players) player.myId = -1;
		}

		private void Awake()
		{
			craftingStations = new List<CraftingStation>();
		}

		private void Start()
		{
			players = FindObjectsByType<Interactor>(FindObjectsSortMode.None).ToList();

            _uiSystem.PushView(FindObjectsByType<MainMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault());
		}

		private void Update()
		{
			time = time - Time.deltaTime;

            if (time < 0)
            {
				EndGame(GameManager.GetMonoSystem<IGridMonoSystem>().GetSide(GameManager.GetMonoSystem<IGridMonoSystem>().GetFirewallController().transform.position).Opposide());
            }
			else if (time < 30f)
			{
				// Set new music
			}
        }
	}
}
