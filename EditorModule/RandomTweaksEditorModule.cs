using System.IO;
using ADOLib.Settings;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using ADOLib.Translation;

namespace RandomTweaksEditorModule {
	class RandomTweaksEditorModule
	{
		public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
		public static Settings settings;
		public static Translator Translator;
		public static string Path;
		public static UnityModManager.ModEntry ModEntry;
		internal static void Setup(UnityModManager.ModEntry modEntry)
		{
			ModEntry = modEntry;
			Logger = modEntry.Logger;
			settings = Category.GetCategory<Settings>();
			Path = modEntry.Path;
			Translator = new Translator(Path);
			Behavior.CoordinateUI.ConsoleInput.LoadImage(File.ReadAllBytes($"{Path}ConsolePanelTyping.png"));
			Behavior.CoordinateUI.ConsoleOutput.LoadImage(File.ReadAllBytes($"{Path}ConsolePanel.png"));
		}
	}
}