using ADOLib.SafeTools;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RandomTweaksPlayingModule.Patch {
	class EditorTweaks {
		[SafePatch("RTP.CheckpointStartPatch", "CustomLevel", "Play")]
		private static class CheckpointStartPatch {
			public static bool Prefix(CustomLevel __instance, int seqID)
			{
				if (seqID == 0 || !RandomTweaksPlayingModule.settings.DisableRestartAtCheckpoint) {
					return true;
				}
				__instance.Play(0);
				return false;
			}
		}

		[SafePatch("RTP.OfficialCheckpointStartPatch", "scrUIController", "WipeToBlack")]
		private static class OfficialCheckpointStartPatch {
			public static void Prefix(scrController __instance) {
				//L.og(_currentSeqID);
				if ( RandomTweaksPlayingModule.settings.DisableRestartAtCheckpoint) {
					GCS._checkpointNum = 0;
				}
			}
		}

		[SafePatch("RTP.ClsStartPatch", "scnCLS", "Start")]
		private static class ClsStartPatch {
			public static void Postfix() {
				PlayEditorLevelPatch.IsNotPlayingAlone = true;
			}
		}

		[SafePatch("RTP.ClsStartPatch", "scnEditor", "Update")]
		private static class PlayEditorLevelPatch {
			public static bool IsNotPlayingAlone;
			public static bool Prefix(scnEditor __instance, ref bool ___showingPopup) {
				bool flag = (!(__instance.eventSystem.currentSelectedGameObject != null) || !(__instance.eventSystem.currentSelectedGameObject.GetComponent<InputField>() != null)) && !___showingPopup && RandomTweaksPlayingModule.settings.PlayCLSInEditor;
				if (flag) {
					if (Input.GetKeyDown(KeyCode.P) && ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) && !GCS.standaloneLevelMode) {
						if (__instance.levelPath == null) {
							__instance.ShowPopup(true, scnEditor.PopupType.SaveBeforeLevelExport);
							return true;
						}
						GCS.standaloneLevelMode = true;
						IsNotPlayingAlone = false;
						GCS.customLevelPaths = CustomLevel.GetWorldPaths(__instance.levelPath, false, true);
						SceneManager.LoadScene("scnEditor");
						return false;
					}
					if (Input.GetKeyDown(KeyCode.Escape))
					{
						if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
						{
							GCS.standaloneLevelMode = false;
							GCS.customLevelPaths = null;
							__instance.SwitchToEditMode();
							return false;

						}
					}
				}
				return true;
			}
		}
	}
}
