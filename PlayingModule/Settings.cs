using ADOLib.SafeTools;
using ADOLib.Settings;
using ADOLib.Translation;
using UnityEngine;
using UnityModManagerNet;
using static ADOLib.Settings.GUIExtended;

namespace RandomTweaksPlayingModule {
	[Category(
		TabName = "RandomTweaks", 
		Name = "RandomTweaks Playing Module", 
		MinVersion = 71)]
	public class Settings : Category {

		public override UnityModManager.ModEntry ModEntry => RandomTweaksPlayingModule.ModEntry;

		public bool PlayCLSInEditor;
		public bool DisableRestartAtCheckpoint;
		public bool EnableOverloadGauge;

		public override void OnGUI() {
			var DRP = DisableRestartAtCheckpoint ? "☑️" : "☐";
			var PCIE = PlayCLSInEditor ? "☑️" : "☐";
			var EOG = EnableOverloadGauge ? "☑️" : "☐";
			DisableRestartAtCheckpoint =
				GUILayout.Toggle(DisableRestartAtCheckpoint,
					$"{DRP} " + RandomTweaksPlayingModule.Translator.Translate(
						"RandomTweaksPlayingModule.Settings.DisableRestartAtCheckpoint"), Text);
			PlayCLSInEditor =
				GUILayout.Toggle(PlayCLSInEditor,
					$"{PCIE} " +
					RandomTweaksPlayingModule.Translator.Translate("RandomTweaksPlayingModule.Settings.PlayCLSInEditor"), Text);
			EnableOverloadGauge =
				GUILayout.Toggle(EnableOverloadGauge,
					$"{EOG} " + RandomTweaksPlayingModule.Translator.Translate(
						"RandomTweaksPlayingModule.Settings.EnableOverloadGauge"), Text);
		}

		public override void OnEnable() {
			RandomTweaksPlayingModule.harmony.UnpatchCategory<Settings>();
			RandomTweaksPlayingModule.harmony.PatchCategory<Settings>();
		}
		
		public override void OnDisable() {
			RandomTweaksPlayingModule.harmony.UnpatchCategory<Settings>();
		}
	}
}