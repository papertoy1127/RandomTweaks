using System.Collections;
using System.Threading;
using ADOLib.SafeTools;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RandomTweaksFontModule.Patch {
	public static class Patch {
		[SafePatch("RTF.ChangeFontForLanguagePatch", "RDString", "GetFontDataForLanguage")]
		private static class ChangeFontForLanguagePatch {
			public static bool Prefix(SystemLanguage language, ref FontData __result) {
				if (RandomTweaksFontModule.settings.FontIndex == 0) return true;
				__result = Settings.GetFontData();
				return false;
			}
		}

		public static IEnumerator UpdateFontCo() {
			if (RandomTweaksFontModule.settings.FontIndex == 0) yield break;
			var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (var i in gameObjects) {
				foreach (var j in i.GetComponentsInChildren<Text>()) {
					if (RandomTweaksFontModule.settings.FontIndex == 11)
						j.SetLocalizedFont();
					else {
						FontData fnt = Settings.GetFontData();

						if (j.name == "txtLevelName") {
							j.font = fnt.font;
							j.resizeTextMaxSize = Mathf.RoundToInt(fnt.fontScale * 68);
							j.GetComponent<Text>().lineSpacing = fnt.lineSpacing;
						}

						if (j.name == "txtDescription") {
							j.resizeTextMaxSize = Mathf.RoundToInt(40 * fnt.fontScale);
							j.GetComponent<Text>().lineSpacing = fnt.lineSpacing;
						}

						if (j.name.StartsWith("Help")) {
							j.resizeTextMaxSize = Mathf.RoundToInt(6.4f * fnt.fontScale);
							j.GetComponent<Text>().lineSpacing = fnt.lineSpacing;
						}
					}
				}
				yield return null;
			}
		}

		public static void UpdateFont() {
			if (RandomTweaksFontModule.settings.FontIndex == 0) return;
			var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (var i in gameObjects) {
				foreach (var j in i.GetComponentsInChildren<Text>()) {
					if (RandomTweaksFontModule.settings.FontIndex == 11)
						j.SetLocalizedFont();
					else {
						FontData fnt = Settings.GetFontData();

						if (j.name == "txtLevelName") {
							j.font = fnt.font;
							j.resizeTextMaxSize = Mathf.RoundToInt(fnt.fontScale * 68);
							j.GetComponent<Text>().lineSpacing = fnt.lineSpacing;
						}

						if (j.name == "txtDescription") {
							j.resizeTextMaxSize = Mathf.RoundToInt(40 * fnt.fontScale);
							j.GetComponent<Text>().lineSpacing = fnt.lineSpacing;
						}

						if (j.name.StartsWith("Help")) {
							j.resizeTextMaxSize = Mathf.RoundToInt(6.4f * fnt.fontScale);
							j.GetComponent<Text>().lineSpacing = fnt.lineSpacing;
						}
					}
				}
			}
		}

		[SafePatch("RTF.UpdateTitleDescPlay", "scnEditor", "Play")]
		private static class UpdateTitleDescPlay {
			public static void Postfix() {
				UpdateFont();
			}
		}

		public static int isUpdating = 0;

		[SafePatch("RTF.UpdateTitleDescStart", "scnEditor", "Start")]
		private static class UpdateTitleDescStart {
			public static void Postfix() {
				UpdateFont();
				isUpdating = 60;
			}
		}

		[SafePatch("RTF.UpdateTitleDescUD", "scnEditor", "Update")]
		private static class UpdateTitleDescUD {
			public static void Postfix(scnEditor __instance) {
				if (isUpdating > 0) {
					UpdateFont();
					isUpdating -= 1;
				}
				//if (scrController.instance.paused) {
				//UpdateFont();
				//}
			}
		}

		public static void changeAllTexts(string txt) {
			for (int i = 0; i < 10; i++) {
				Scene scene = SceneManager.GetSceneAt(i);
				var objects = scene.GetRootGameObjects();
				foreach (var O in objects) {
					foreach (var Text in O.GetComponentsInChildren<Text>()) {
						FontData fnt = RDString.GetFontDataForLanguage(RDString.language);
						if (Text.text != "") {
							Text.text = txt;
						}
					}

					foreach (var Text in O.GetComponentsInChildren<TextMesh>()) {
						FontData fnt = RDString.GetFontDataForLanguage(RDString.language);
						if (Text.text != "") {
							Text.text = txt;
						}
					}

					foreach (var Text in O.GetComponentsInChildren<TextEditor>()) {
						FontData fnt = RDString.GetFontDataForLanguage(RDString.language);
						if (Text.text != "") {
							Text.text = txt;
						}
					}

				}
			}
		}
	}
}