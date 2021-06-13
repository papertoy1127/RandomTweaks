using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityModManagerNet;
using UnityEngine;
using ADOLib.Settings;
using UnityEngine.SceneManagement;
using static RandomTweaksFontModule.RandomTweaksFontModule;
using Font = UnityEngine.Font;

namespace RandomTweaksFontModule {
	[Category(
		TabName = "RandomTweaks", 
		Name = "RandomTweaks Font Module", 
		MinVersion = 71, 
		PatchClass = typeof(Patch.Patch),
		ForceType = ForceType.ForceEnable)]
	public class Settings : Category {
		public override UnityModManager.ModEntry ModEntry => RandomTweaksFontModule.ModEntry;

		public static GUIStyle Selection = GUIExtended.Selection;
		public static GUIStyle SelectionActive = GUIExtended.SelectionActive;
		public static GUIStyle Text = GUIExtended.Text;
		public static GUIStyle TextInput = GUIExtended.TextInput;
		public static Font SelectedFont = RDConstants.data.latinFont;
		public static Font FontTmp;
		public static Vector2 scrollPos;
		public static int SelectedFallbakLoc;
		public static List<String> FallbackFontNamesTMP;
		public List<String> FallbackFontNames;
		public bool CustomFontEnabled;
		public float fontSize = 0.75f;
		public float lineSpace = 0.85f;
		public int FontIndex;

		public static FontData GetFontData() {
			int index = settings.FontIndex;
			return GetFontData(index);
		}
		public static FontData GetFontData(int index) {
			FontData result = default(FontData);
			if (index == 1) {
				result.fontScale = 1.25f;
				result.lineSpacing = 0.75f;
			} else if (index == 2) {
				result.fontScale = 0.7f;
				result.lineSpacing = 1.1f;
			} else if (index == 3) {
				result.fontScale = 0.82f;
				result.lineSpacing = 1.1f;
			} else if (index == 4) {
				result.fontScale = 1.25f;
				result.lineSpacing = 0.75f;
			} else {
				result.fontScale = settings.fontSize;
				result.lineSpacing = settings.lineSpace;
			}
			result.font = SelectedFont;
			return result;
		}

		public static FontData GetFontData(Font font) {
			FontData result = default(FontData);
			RDConstants data = RDConstants.data;
			if (font == data.koreanFont) {
				result.fontScale = 1.25f;
				result.lineSpacing = 0.75f;
			} else if (font == data.japaneseFont) {
				result.fontScale = 0.7f;
				result.lineSpacing = 1.1f;
			} else if (font == data.chineseFont) {
				result.fontScale = 0.82f;
				result.lineSpacing = 1.1f;
			} else if (font == data.legacyFont) {
				result.fontScale = 1.25f;
				result.lineSpacing = 0.75f;
			} else if (font.fontNames[0].Contains("Avenir")) {
				result.fontScale = 1f;
				result.lineSpacing = 1f;
			} else {
				result.fontScale = settings.fontSize;
				result.lineSpacing = settings.lineSpace;
			}
			result.font = SelectedFont;
			return result;
		}

		public override void OnGUI() {
			GUILayout.BeginVertical(GUILayout.Width(1000));
			settings.FontIndex = GUIExtended.SelectionGrid(settings.FontIndex, FontNames.ToArray(), 6);
			GUILayout.EndVertical();
			if (settings.FontIndex == 11) {
				GUILayout.Label(Translator.Translate("UI.Fontname"), Text);
				
				GUILayout.Label(Translator.Translate("UI.PrimaryFont"), Text);
				if (GUILayout.Button(FallbackFontNamesTMP[0],
					SelectedFallbakLoc == 0 ? TextInputCFontActive : TextInputCFont, GUILayout.Width(350),
					GUILayout.Height(30))) {
					
					SelectedFallbakLoc = 0;
					FontLoc = -1;
				}

				GUILayout.Label(Translator.Translate("UI.Fallback") + " #1", Text);
				if (GUILayout.Button(FallbackFontNamesTMP[1],
					SelectedFallbakLoc == 1 ? TextInputFont1Active : TextInputFFont1 , GUILayout.Width(350),
					GUILayout.Height(30))) {
					
					SelectedFallbakLoc = 1;
					FontLoc = -1;
				}

				GUILayout.Label(Translator.Translate("UI.Fallback") + " #2", Text);
				if (GUILayout.Button(FallbackFontNamesTMP[2],
					SelectedFallbakLoc == 2 ? TextInputFont2Active : TextInputFFont2, GUILayout.Width(350),
					GUILayout.Height(30))) {
					SelectedFallbakLoc = 2;
					FontLoc = -1;
				}
				
				if (!OSFonts.ContainsKey(FallbackFontNamesTMP[0])) OSFonts[FallbackFontNamesTMP[0]] = CreateOsFallbackFont(FallbackFontNamesTMP[0]);
				if (!OSFonts.ContainsKey(FallbackFontNamesTMP[1])) OSFonts[FallbackFontNamesTMP[1]] = CreateOsFallbackFont(FallbackFontNamesTMP[1]);
				if (!OSFonts.ContainsKey(FallbackFontNamesTMP[2])) OSFonts[FallbackFontNamesTMP[2]] = CreateOsFallbackFont(FallbackFontNamesTMP[2]);
				TextInputCFont.font = OSFonts[FallbackFontNamesTMP[0]];
				TextInputCFontActive.font = OSFonts[FallbackFontNamesTMP[0]];
				
				TextInputFFont1.font = OSFonts[FallbackFontNamesTMP[1]];
				TextInputFont1Active.font = OSFonts[FallbackFontNamesTMP[1]];
				
				TextInputFFont2.font = OSFonts[FallbackFontNamesTMP[2]];
				TextInputFont2Active.font = OSFonts[FallbackFontNamesTMP[2]];
				
				GUILayout.Space(10);
				GUILayout.BeginVertical(GUILayout.Width(270));
				scrollPos = GUILayout.BeginScrollView(scrollPos, Text, GUILayout.Height(500));
				var fontLocPrev = FontLoc;
				FontLoc = GUIExtended.SelectionGrid(FontLoc, Font.GetOSInstalledFontNames(), 1);
				if (FontLoc != -1) FallbackFontNamesTMP[SelectedFallbakLoc] = Font.GetOSInstalledFontNames()[FontLoc];
				GUILayout.EndScrollView();
				GUILayout.EndVertical();
				GUILayout.Label(Translator.Translate("UI.Fontsize"), Text);
				string fTmp = GUILayout.TextField(settings.fontSize.ToString(CultureInfo.CurrentCulture), TextInput, GUILayout.Width(150), GUILayout.Height(30));
				settings.fontSize = float.TryParse(fTmp, out _) ? float.Parse(fTmp) : settings.fontSize;
				GUILayout.Label(Translator.Translate("UI.Linespacing"), Text);
				string fTmp2 = GUILayout.TextField(settings.lineSpace.ToString(CultureInfo.CurrentCulture), TextInput, GUILayout.Width(150), GUILayout.Height(30));
				settings.lineSpace = float.TryParse(fTmp2, out _) ? float.Parse(fTmp2) : settings.lineSpace;
				if (FontLoc != fontLocPrev) {
					OSFonts[FallbackFontNamesTMP[0]] = CreateOsFallbackFont(FallbackFontNamesTMP.ToArray());
					FontTmp = OSFonts[FallbackFontNamesTMP[0]];
				}
			} else {
				FontTmp = FontList[settings.FontIndex];
			}
			GUILayout.Space(10);
			if (GUILayout.Button(Translator.Translate("UI.ChangeFont"), Selection, GUILayout.Width(200), GUILayout.Height(50))) {
				FallbackFontNames = FallbackFontNamesTMP;
				if (FontTmp == null) {
					SelectedFont = CreateOsFallbackFont(FallbackFontNamesTMP.ToArray()); //OSFonts[FontLoc];
				} else {
					SelectedFont = FontTmp;
				}

				if (settings.FontIndex == 11) {
					SelectedFont.fontNames = FallbackFontNames.ToArray();
					settings.FallbackFontNames[0] = SelectedFont.name;
				}

				typeof(RDString).GetProperty("fontData")?.SetValue(null, GetFontData());
				GCS.sceneToLoad = SceneManager.GetActiveScene().name;
				scrController.instance.StartLoadingScene(WipeDirection.StartsFromRight);
				Thread.Sleep(150);
				scrController.instance.StartLoadingScene(WipeDirection.StartsFromRight);
			}
		}
	}
}
