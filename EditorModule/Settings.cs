using ADOLib.Settings;
using ADOLib.Translation;
using UnityEngine;
using UnityModManagerNet;

namespace RandomTweaksEditorModule {
	[Category(TabName = "RandomTweaks",
		Name = "RandomTweaksMiscModule Editor Module",
		MinVersion = 71,
		MaxVersion = 71,
		PatchClass = typeof(Patch.Patch))]
	public class Settings : Category {
		public override UnityModManager.ModEntry ModEntry => RandomTweaksEditorModule.ModEntry;
		public static GUIStyle Text = GUIExtended.Text;

		public bool EnableDecorationClickToEvent;
		public bool EnableDecorationClickMove;

		public override void OnGUI() {
			GUILayout.Label(RandomTweaksEditorModule.Translator.Translate("UI.HelpCoordinateUI"), Text);
			var DCM = EnableDecorationClickMove ? "☑" : "☐";
			var DCTE = EnableDecorationClickToEvent ? "☑" : "☐";
			EnableDecorationClickMove =
				GUILayout.Toggle(EnableDecorationClickMove,
					$"{DCM} " + RandomTweaksEditorModule.Translator.Translate("RandomTweaksMiscModule.Settings.EnableDecorationClickMove"), Text);
			EnableDecorationClickToEvent =
				GUILayout.Toggle(EnableDecorationClickToEvent,
					$"{DCTE} " + RandomTweaksEditorModule.Translator.Translate("RandomTweaksMiscModule.Settings.EnableDecorationClickToEvent"), Text);

		}
	}
}