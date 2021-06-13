using ADOLib;
using ADOLib.Misc;
using ADOLib.SafeTools;
using HarmonyLib;
using UnityEngine;

namespace RandomTweaksPlayingModule.Patch {
	class PlayingUI {
		private static GameObject _gameObject;
		private static Behavior.PlayingUI _mainBehavior;
		private static bool IsPlaying = false;
		internal static bool UI = false;
		[SafePatch("RTP.ResetUI", "scnEditor", "Start")]
		private static class ResetUI {
			public static void Postfix() {
				UI = false;
			}
		}
		[SafePatch("RTP.CtrResetUI", "scrController", "Start")]
		private static class CtrResetUI {
			public static void Postfix() {
				UI = false;
			}
		}
		[SafePatch("RTP.ControllerUIPatch", "scrController", "Update")]
		private static class ControllerUIPatch {
			private static void Prefix() {
				if (!Startup.IsEnabled || !RandomTweaksPlayingModule.settings.EnableOverloadGauge || !scrController.isGameWorld) {
					DestroyUI();
					return;
				}

				if (!UI) {
					StartUI();
				}
			}
		}

		[SafePatch("RTP.EditorUIPatch", "scnEditor", "Update")]
		private static class EditorUIPatch {
			private static void Prefix() {
				if (GCS.standaloneLevelMode) IsPlaying = true;
				if (!Startup.IsEnabled || !RandomTweaksPlayingModule.settings.EnableOverloadGauge || !IsPlaying) {
					DestroyUI();
					return;
				}

				if (!UI) {
					StartUI();
				}
			}
		}

		[SafePatch("RTP.EditorPlayingUIPatch", "scnEditor", "Play")]
		private static class EditorPlayingUIPatch {
			private static void Prefix() {
				IsPlaying = true;
			}
		}

		[SafePatch("RTP.EditorEditorUIPatch", "scnEditor", "SwitchToEditMode")]
		private static class EditorEditorUIPatch {
			private static void Prefix() {
				IsPlaying = false;
			}
		}

		[SafePatch("RTP.SetOverloadGauge", "scrFailBar", "Update")]
		internal static class SetOverloadGauge {
			public static void Postfix(scrFailBar __instance)
			{
				float value;
				var valueField = typeof(scrFailBar).GetField("value");
				if (valueField != null)
				{
					value = (float) valueField.GetValue(__instance);
				}
				else
				{
					value = __instance.get<float>("overloadCounter");
				}

				Behavior.PlayingUI.OverloadGauge = value;
			}
		}
		//commands
		public static void DestroyUI() {
			if (_gameObject == null) {
				return;
			}
			Object.DestroyImmediate(_gameObject);
			Object.DestroyImmediate(_mainBehavior);
			_gameObject = null;
			_mainBehavior = null;
			UI = false;
		}
		public static void StartUI() {
			_gameObject = new GameObject();
			_mainBehavior = _gameObject.AddComponent<Behavior.PlayingUI>();
			UI = true;
		}
	}
}