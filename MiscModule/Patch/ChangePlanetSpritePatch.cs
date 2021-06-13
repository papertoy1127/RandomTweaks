using System.Collections.Generic;
using ADOLib.Misc;
using ADOLib.SafeTools;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace RandomTweaksMiscModule.Patch {
    public partial class Patch {
        public static Sprite RedPlanet;
        public static Sprite BluePlanet;
        public static AnimatorOverrideController Controller;

        [SafePatch("RTM.Controller", "scrPlanet", "Start")]
        internal static class ControllerPatch {
            public static void Postfix(scrPlanet __instance) {
                if (!RandomTweaksMiscModule.settings.EnableCustomTexture) return;
                __instance.GetComponent<Animator>().enabled = false;

                var NewSprite = BluePlanet;
                if (__instance.isRed) NewSprite = RedPlanet;
                __instance.whiteSprite = NewSprite;
                __instance.gameObject.GetComponent<SpriteRenderer>().sprite = NewSprite;
            }
        }

        [SafePatch("RTM.RedBlue", "scrPlanet", "DisableCustomColor")]
        internal static class RedBluePatch {
            public static void Postfix(scrPlanet __instance, int defaultColor) {
                if (!RandomTweaksMiscModule.settings.EnableCustomTexture) return;
                Color color;
                switch (defaultColor) {
                    case -1: {
                        color = __instance.isRed ? Color.red : Color.blue;
                        break;
                    }
                    case 0:
                        color = Color.red;
                        break;
                    case 1:
                        color = Color.blue;
                        break;
                    default:
                        return;
                }

                __instance.sprite.sprite = __instance.whiteSprite;
                __instance.sprite.color = color;
                __instance.GetComponent<Animator>().enabled = false;

            }
        }

        [SafePatch("RTM.CustomColor", "scrPlanet", "EnableCustomColor")]
        internal static class CustomColorPatch {
            public static void Postfix(scrPlanet __instance) {
                if (!RandomTweaksMiscModule.settings.EnableCustomTexture) return;
                __instance.sprite.sprite = __instance.whiteSprite;
                __instance.GetComponent<Animator>().enabled = false;
            }
        }
        
        [SafePatch("RTM.GetPlanetColor", "RDConstants", "GetPlanetColor")]
        internal static class GetPlanetColor {
            public static bool Prefix(PlanetColor color, ref Color __result)
            {
                if (color == (PlanetColor) PlanetColorEx.Silver) {
                    __result = new Color(0.95f, 0.95f, 0.95f);
                    return false;
                }

                return true;
            }
        }
        [SafePatch("RTM.NoSprite", "scrColorPlanet", "Start")]
        internal static class NoSprite {
            public static void Prefix(scrColorPlanet __instance) {
                var floor = __instance.GetComponent<scrFloor>();
                floor.topglow.sprite = null;
                floor.bottomglow.sprite = null;
            }
        }
    }
}