using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ADOLib.SafeTools;
using ADOLib.Settings;
using UnityEngine.SceneManagement;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using ADOLib.Translation;
using Object = System.Object;

namespace RandomTweaksFontModule {
	class RandomTweaksFontModule
	{
		public static UnityModManager.ModEntry ModEntry;
		public static Translator Translator;
		public static int FontLoc = -1;
		public static List<Font> FontList = new List<Font> {
			RDConstants.data.latinFont,
			RDConstants.data.koreanFont,
			RDConstants.data.japaneseFont,
			RDConstants.data.chineseFont,
			RDConstants.data.legacyFont,
			RDConstants.data.arialFont,
			RDConstants.data.comicSansMSFont,
			RDConstants.data.courierNewFont,
			RDConstants.data.georgiaFont,
			RDConstants.data.impactFont,
			RDConstants.data.timesNewRomanFont
			};
		public static Dictionary<String, Font> OSFonts = new Dictionary<String, Font>();
		public static List<string> FontNames = new List<string> {
			"Default",
			"Korean Font",
			"Japanese Font",
			"Chinese Font",
			"Legacy Font",
			"Arial",
			"Comic Sans",
			"Courier",
			"Georgia",
			"Impact",
			"Times New Roman",
			"Other Fonts"
		};
		public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
		public static Settings settings;
		
		public static GUIStyle TextInputCFont = new GUIStyle();
		public static GUIStyle TextInputCFontActive = new GUIStyle();
		
		public static GUIStyle TextInputFFont1 = new GUIStyle();
		public static GUIStyle TextInputFont1Active = new GUIStyle();
		public static GUIStyle TextInputFFont2 = new GUIStyle();
		public static GUIStyle TextInputFont2Active = new GUIStyle();

		public static Font CreateOsFallbackFont(params string[] fontNames) {
			var font = Font.CreateDynamicFontFromOSFont(fontNames[0], 16);
			font.fontNames = fontNames;
			return font;
		}

		internal static void Setup(UnityModManager.ModEntry modEntry)
		{
			ModEntry = modEntry;
			Logger = modEntry.Logger;
			
			settings = Category.GetCategory<Settings>();
			
			if (settings.FallbackFontNames == null || settings.FallbackFontNames.Count < 3) {
				settings.FallbackFontNames = new List<string> {
					"Arial", "Arial", "Arial"
				};
			}
			Settings.FallbackFontNamesTMP = settings.FallbackFontNames;
			
			if (settings.FontIndex == 11) {
				if (new List<string>(Font.GetOSInstalledFontNames()).Contains(settings.FallbackFontNames[0])) {
					Settings.SelectedFont = CreateOsFallbackFont(settings.FallbackFontNames.ToArray());
				}
			} else {
				Settings.SelectedFont = FontList[settings.FontIndex];
            }

			
			TextInputCFont.fontSize = 20;
			TextInputCFont.alignment = TextAnchor.MiddleLeft;
			TextInputCFont.normal.background = SettingsUI.BG;
			TextInputCFont.normal.textColor = Color.gray;
			
			TextInputCFontActive.fontSize = 25;
			TextInputCFontActive.alignment = TextAnchor.MiddleLeft;
			TextInputCFontActive.normal.background = SettingsUI.BG;
			TextInputCFontActive.normal.textColor = Color.white;
			
			TextInputFFont1.fontSize = 20;
			TextInputFFont1.alignment = TextAnchor.MiddleLeft;
			TextInputFFont1.normal.background = SettingsUI.BG;
			TextInputFFont1.normal.textColor = Color.gray;
			
			TextInputFont1Active.fontSize = 25;
			TextInputFont1Active.alignment = TextAnchor.MiddleLeft;
			TextInputFont1Active.normal.background = SettingsUI.BG;
			TextInputFont1Active.normal.textColor = Color.white;
			
			TextInputFFont2.fontSize = 20;
			TextInputFFont2.alignment = TextAnchor.MiddleLeft;
			TextInputFFont2.normal.background = SettingsUI.BG;
			TextInputFFont2.normal.textColor = Color.gray;
			
			TextInputFont2Active.fontSize = 25;
			TextInputFont2Active.alignment = TextAnchor.MiddleLeft;
			TextInputFont2Active.normal.background = SettingsUI.BG;
			TextInputFont2Active.normal.textColor = Color.white;
			
			Translator = new Translator(modEntry.Path);
			
			settings.harmony.PatchCategory<Settings>();

			var asdf = new GameObject();
			UnityEngine.Object.DontDestroyOnLoad(asdf);
			asdf.AddComponent<scrAlways>();
		}
	}
}
internal static class L {
	public static void og(object log) {
		UnityModManager.Logger.Log(log.ToString());
    }
}