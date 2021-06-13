using UnityModManagerNet;
using System.Reflection;
using HarmonyLib;

namespace RandomTweaksEditorModule {
    public class Startup {
        private static UnityModManager.ModEntry _mod;
        public static bool IsEnabled { get; private set; }

        public static void Load(UnityModManager.ModEntry modEntry) {
            _mod = modEntry;
            RandomTweaksEditorModule.Setup(modEntry);
        }
    }
}
