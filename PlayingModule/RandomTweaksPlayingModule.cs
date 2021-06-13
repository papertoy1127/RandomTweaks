using System.IO;
using ADOLib.Settings;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using ADOLib.Translation;
using static ADOLib.Settings.SettingsUI;

namespace RandomTweaksPlayingModule {
	class RandomTweaksPlayingModule
	{
		public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
		public static Settings settings => Category.GetCategory<Settings>();
		public static Translator Translator;
		public static string Path;
		internal static Harmony harmony;
		public static UnityModManager.ModEntry ModEntry;
		internal static void Setup(UnityModManager.ModEntry modEntry)
		{
			ModEntry = modEntry;
			harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll();
			Logger = modEntry.Logger;
			Path = modEntry.Path;
			Translator = new Translator(Path);
			Behavior.PlayingUI.GaugeTextureInside.LoadImage(File.ReadAllBytes($"{Path}GaugeInside.png"));
			Behavior.PlayingUI.GaugeTextureOutside.LoadImage(File.ReadAllBytes($"{Path}GaugeOutside.png"));
		}
	}
}