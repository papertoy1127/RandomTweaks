using UnityModManagerNet;
using System.Reflection;
using HarmonyLib;

namespace RandomTweaksPlayingModule {
    public class Startup {
        private static Harmony _harmony;
        private static UnityModManager.ModEntry _mod;
        public static bool IsEnabled { get; private set; }

        public static void Load(UnityModManager.ModEntry modEntry) {
            _mod = modEntry;
            _mod.OnToggle = OnToggle;
            RandomTweaksPlayingModule.Setup(modEntry);
        }

		internal static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
			_mod = modEntry;
			IsEnabled = value;

			if (IsEnabled) {
				StartTweaks();
			} else {
				StopTweaks();
			}

			return true;
		}

		internal static void StartTweaks() {
			_harmony = new Harmony(_mod.Info.Id);
			_harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		internal static void StopTweaks() {
			_harmony.UnpatchAll(_harmony.Id);
		}
	}
}
