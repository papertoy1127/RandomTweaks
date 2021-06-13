using System.Windows.Forms;
using ADOLib.SafeTools;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace RandomTweaksEditorModule.Patch {
	public partial class Patch {
		private static GameObject _gameObject;
		private static MonoBehaviour _mainBehavior;
		internal static bool UI = false;
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
			_mainBehavior = _gameObject.AddComponent<Behavior.CoordinateUI>();
			UI = true;
		}

		private static int state = 0;
		
		[SafePatch("RTE.EditorRefresh", "scnEditor", "Awake")]
		private static class EditorRefresh {
			private static void Prefix()
			{
				UI = false;
			}
		}
		[SafePatch("RTE.EditorUIPatch", "scnEditor", "Update")]
		private static class EditorUIPatch {
			private static void Prefix() {
				if (!RandomTweaksEditorModule.settings.IsEnabled) {
					DestroyUI();
					return;
				}

				if (Input.GetKeyUp(KeyCode.F3))
				{
					UnityModManager.Logger.Log("asdf");
					if (state % 2 == 0)
					{
						if (!UI) StartUI();
						else DestroyUI();
					}

					state++;
				}
			}
		}
	}
}