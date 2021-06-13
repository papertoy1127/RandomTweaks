using System;
using ADOLib;
using ADOLib.Misc;
using ADOLib.SafeTools;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityModManagerNet;
using Object = UnityEngine.Object;

namespace RandomTweaksMiscModule.Patch {
    public partial class Patch {
        public enum PlanetColorEx {
            DefaultRed,
            DefaultBlue,
            Orange,
            LightBlue,
            Green,
            Pink,
            Purple,
            Grass,
            TransPink,
            TransBlue,
            Violet,
            Aqua,
            Black,
            White,
            Gold,
            Rainbow,
            Crimson,
            Maroon,
            Jungle,
            Vine,
            Cyan,
            Teal,
            Jester,
            Stone,
            Rust,
            Metal,
            Silver = 100
        }

        public static readonly Color silverColor = new Color(0, 0, 1f, 0);

        public static PlanetColorEx PlanetGoldColor {
            get => (PlanetColorEx) RandomTweaksMiscModule.settings.PlanetColor;
            set => RandomTweaksMiscModule.settings.PlanetColor = (int) value;
        }

        public static Material createParticleMaterial() {
            return RandomTweaksMiscModule.Default_Particle;
        }

        public static Color GoldColor = scrPlanet.goldColor;
        internal static Texture2D NewPlanetTexture = new Texture2D(2, 2);

        internal static Sprite PlanetSprite = Sprite.Create(NewPlanetTexture,
            new Rect(0, 0, NewPlanetTexture.width, NewPlanetTexture.height), Vector2.zero);


        [SafePatch("RTM.ColorFloorAwake", "scrColorPlanet", "Awake")]
        internal static class ColorFloorAwake {
            public static bool Prefix(scrColorPlanet __instance) {
                __instance.cond = scrConductor.instance;
                __instance.cam = scrCamera.instance;
                __instance.ctrl = scrController.instance;
                __instance.floor = __instance.gameObject.GetComponent<scrFloor>();
                scrFloor component = __instance.gameObject.GetComponent<scrFloor>();
                component.dontChangeMySprite = true;
                component.topglow.color = Color.clear;
                component.bottomglow.color = Color.clear;
                return false;
            }
        }

        [SafePatch("RTM.ReloadFloor", "scrController", "QuitToMainMenu")]
        internal static class ReloadFloor {
            public static void Postfix(scrController __instance) {
                //__instance.StopAllCoroutines();
                DestroySilverObject();
                MakeColorFloor();

            }
        }

        [SafePatch("RTM.LoadPlanet", "scrPlanet", "LoadPlanetColor")]
        internal static class LoadPlanet {
            public static bool Prefix(scrPlanet __instance) {
                __instance.SetRainbow(false);
                Color playerColor = Persistence.GetPlayerColor(__instance.isRed);
                Color white = Color.white;
                if (playerColor == scrPlanet.goldColor || GCS.d_forceGoldPlanets) {
                    __instance.SetColor(PlanetColor.Gold);
                    __instance.sparks.gameObject.GetComponent<Renderer>().material =
                        __instance.coreParticles.gameObject.GetComponent<Renderer>().material;
                }
                else if (playerColor == silverColor) {
                    __instance.SetColor((PlanetColor) PlanetColorEx.Silver);
                }
                else if (playerColor == scrPlanet.rainbowColor) {
                    __instance.EnableCustomColor();
                    __instance.SetRainbow(true);
                }
                else if (playerColor == Color.red || playerColor == Color.blue) {
                    int defaultColor = (playerColor == Color.red) ? 0 : 1;
                    __instance.DisableCustomColor(defaultColor);
                }
                else {
                    __instance.EnableCustomColor();
                    Color planetColor = playerColor;
                    Color tailColor = playerColor;
                    if (playerColor == scrPlanet.transBlueColor) {
                        planetColor = new Color(0.3607843f, 0.7882353f, 0.9294118f);
                        tailColor = Color.white;
                    }
                    else if (playerColor == scrPlanet.transPinkColor) {
                        planetColor = new Color(0.9568627f, 0.6431373f, 0.7098039f);
                        tailColor = Color.white;
                    }
                    else if (playerColor == scrPlanet.nbYellowColor) {
                        planetColor = new Color(0.996f, 0.953f, 0.18f);
                        tailColor = Color.white;
                    }
                    else if (playerColor == scrPlanet.nbPurpleColor) {
                        planetColor = new Color(0.612f, 0.345f, 0.82f);
                        tailColor = Color.black;
                    }

                    __instance.SetPlanetColor(planetColor);
                    __instance.SetTailColor(tailColor);
                }

                if (scrLogoText.instance != null) {
                    scrLogoText.instance.UpdateColors();
                }

                return false;
            }
        }

        [SafePatch("RTM.LoadColorExtended", "Persistence", "GetPlayerColor")]
        internal static class LoadColorExtended {
            public static bool Prefix(bool red, ref Color __result) {
                string @string =
                    PlayerPrefsJson.GetString(red ? "colorRed" : "colorBlue", red ? "FF0000" : "0000FF");
                Color result = Color.white;
                if (@string == "gold") {
                    result = scrPlanet.goldColor;
                }
                else if (@string == "rainbow") {
                    result = scrPlanet.rainbowColor;
                }
                else if (@string == "transPink") {
                    result = scrPlanet.transPinkColor;
                }
                else if (@string == "transBlue") {
                    result = scrPlanet.transBlueColor;
                }
                else if (@string == "nbYellow") {
                    result = scrPlanet.nbYellowColor;
                }
                else if (@string == "nbPurple") {
                    result = scrPlanet.nbPurpleColor;
                }
                else if (@string == "dfdfdfdf") {
                    result = silverColor;
                }
                else {
                    result = @string.HexToColor();
                }

                __result = result;
                return false;
            }
        }

        [SafePatch("RTM.SaveColorExtended", "Persistence", "SetPlayerColor")]
        internal static class SaveColorExtended {
            public static bool Prefix(Color planetColor, bool red) {
                string value;
                if (planetColor == scrPlanet.goldColor) {
                    value = "gold";
                }
                else if (planetColor == scrPlanet.rainbowColor) {
                    value = "rainbow";
                }
                else if (planetColor == scrPlanet.transPinkColor) {
                    value = "transPink";
                }
                else if (planetColor == scrPlanet.transBlueColor) {
                    value = "transBlue";
                }
                else if (planetColor == scrPlanet.nbYellowColor) {
                    value = "nbYellow";
                }
                else if (planetColor == scrPlanet.nbPurpleColor) {
                    value = "nbPurple";
                }
                else if (planetColor == silverColor) {
                    value = "dfdfdfdf";
                }
                else {
                    value = ColorUtility.ToHtmlStringRGB(planetColor);
                }

                PlayerPrefsJson.SetString(red ? "colorRed" : "colorBlue", value);
                return false;
            }
        }

        [SafePatch("RTM.ColorStartup", "scrController", "LateUpdate")]
        internal static class ColorStartup {
            public static void Postfix() {
                if (!Input.anyKeyDown) return;
                if (SceneManager.GetActiveScene().name.Contains("Intro")) {
                    MakeColorFloor();
                    
                    Sprite sprite = null;
                    foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
                        foreach (var com in obj.GetComponentsInChildren<SpriteRenderer>()) {
                            if (com.sprite?.name == null) continue;
                            if (com.sprite.name == "tile_white")
                            {
                                sprite = com.sprite;
                                break;
                            }
                        }
                    }
                
                    var renderer = Silver.GetComponent<SpriteRenderer>();
                    renderer.sprite = sprite;
                    renderer.color = new Color(0.9f, 0.9f, 0.9f);
                    renderer.sortingOrder = 100;
                    renderer.sortingLayerName = "Floor";
                
                    var renderer2 = Silver2.GetComponent<SpriteRenderer>();
                    renderer2.sprite = sprite;
                    renderer2.color = new Color(0.9f, 0.9f, 0.9f);
                    renderer2.sortingOrder = 100;
                    renderer2.sortingLayerName = "Floor";
                }
                
            }
        }

        [SafePatch("RTM.MoreLogoColor", "scrLogoText", "LoadLogoColor")]
        internal static class MoreLogoColor {
            public static bool Prefix(bool isRed, ref Color __result) {
                Color playerColor = Persistence.GetPlayerColor(isRed);
                if (playerColor == scrPlanet.goldColor) {
                    playerColor = new Color(1f, 0.8078431f, 0.1607843f);
                }
                else if (playerColor == scrPlanet.transBlueColor) {
                    playerColor = new Color(0.3607843f, 0.7882353f, 0.9294118f);
                }
                else if (playerColor == scrPlanet.transPinkColor) {
                    playerColor = new Color(0.9568627f, 0.6431373f, 0.7098039f);
                }
                else if (playerColor == scrPlanet.nbYellowColor) {
                    playerColor = new Color(0.996f, 0.953f, 0.18f);
                }
                else if (playerColor == scrPlanet.nbPurpleColor) {
                    playerColor = new Color(0.612f, 0.345f, 0.82f);
                }
                else if (playerColor == silverColor) {
                    playerColor = new Color(0.9f, 0.9f, 0.9f);
                }

                __result = playerColor;
                return false;
            }
        }

        public static void DestroySilverObject() {
            foreach (var obj in GameObject.Find("Floor Container").GetComponentsInChildren<scrColorPlanet>()) {
                if (obj.name.StartsWith("Silver")) {
                    foreach (Transform child in obj.transform) {
                        Object.Destroy(child.gameObject);
                    }

                    GameObject.Destroy(obj);
                }
            }
        }

        private static GameObject Silver;
        private static GameObject Silver2;
        public static void MakeColorFloor() {
            bool ColorFloorGenerated = GameObject.Find("Silver") != null;
            if (ColorFloorGenerated == false) {
                string a = SceneManager.GetActiveScene().name;
                if (!a.Contains("Intro")) return;

                DestroySilverObject();

                GameObject Floors = GameObject.Find("1-6 Colors");
                GameObject ColorFloors_1_6 = Floors.transform.Find("ColorFloors").gameObject;
                GameObject GoldContainer = ColorFloors_1_6.transform.Find("Gold").gameObject;
                GameObject Gold = null;
                foreach (var obj in GameObject.FindGameObjectsWithTag("Beat")) {
                    if (obj.name == "Gold") Gold = obj;
                }

                ColorFloorGenerated = true;
                Silver = new GameObject("Silver");
                GameObject SpecIcon = new GameObject("SpecIcon");
                GameObject TopGlow = new GameObject("TopGlow");
                GameObject BottomGlow = new GameObject("BottomGlow");
                GameObject SparksSilver = new GameObject("SparksSilver");

                SpecIcon.transform.parent = Silver.transform;
                TopGlow.transform.parent = Silver.transform;
                BottomGlow.transform.parent = Silver.transform;
                SparksSilver.transform.parent = Silver.transform;
                Silver.transform.parent = GoldContainer.transform;

                Silver.tag = "Beat";
                Silver.layer = 9;

                var r1 = SpecIcon.AddComponent<SpriteRenderer>();
                SpecIcon.transform.position = Vector3.zero;
                r1.sortingLayerName = "Floor";
                r1.sortingOrder = 100;
                var r2 = TopGlow.AddComponent<SpriteRenderer>();
                //var f2 = TopGlow.AddComponent<scrSpriteFade>();
                //TopGlow.transform.position = Vector3.zero;
                //r2.sortingLayerName = "Floor";
                //r2.sortingOrder = 101;
                //r2.color = Color.clear;
                //r2.sprite = Gold.transform.Find("TopGlow").gameObject.GetComponent<SpriteRenderer>().sprite;
                //f2.fadeTime = 0.3f;
                //f2.oriFade = 0;
                var r3 = BottomGlow.AddComponent<SpriteRenderer>();
                var f3 = BottomGlow.AddComponent<scrSpriteFade>();
                //BottomGlow.transform.position = Vector3.zero;
                //r3.sortingLayerName = "Floor";
                //r3.sortingOrder = 88;
                //r3.color = Color.clear;
                //r3.sprite = Gold.transform.Find("BottomGlow").gameObject.GetComponent<SpriteRenderer>().sprite;
                //f3.fadeTime = 0.3f;
                //f3.oriFade = 0;
                var p4 = SparksSilver.AddComponent<ParticleSystem>();
                var particle = p4.main;
                var emission = p4.emission;
                var shape = p4.shape;
                var colorOverLifetime = p4.colorOverLifetime;
                var sizeOverLifetime = p4.sizeOverLifetime;
                var particleRenderer = SparksSilver.GetComponent<ParticleSystemRenderer>();


                emission.enabled = true;
                emission.rateOverTime = 600;
                emission.rateOverDistance = 0;

                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Rectangle;
                shape.texture = null;
                shape.position = Vector3.zero;
                shape.rotation = Vector3.zero;
                shape.scale = Vector3.one;
                shape.alignToDirection = false;
                shape.randomDirectionAmount = 1;
                shape.sphericalDirectionAmount = 0;
                shape.randomPositionAmount = 0;

                colorOverLifetime.enabled = true;
                colorOverLifetime.color = new ParticleSystem.MinMaxGradient(Color.white, Color.clear);

                sizeOverLifetime.enabled = true;
                sizeOverLifetime.separateAxes = false;
                sizeOverLifetime.size = Gold.transform.Find("TailGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.size;
                sizeOverLifetime.x = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.x;
                sizeOverLifetime.y = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.y;
                sizeOverLifetime.z = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.z;
                sizeOverLifetime.xMultiplier = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.xMultiplier;
                sizeOverLifetime.yMultiplier = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.yMultiplier;
                sizeOverLifetime.zMultiplier = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.zMultiplier;

                particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
                particleRenderer.normalDirection = 1;
                particleRenderer.sortMode = ParticleSystemSortMode.None;
                particleRenderer.alignment = ParticleSystemRenderSpace.View;
                particleRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
                particleRenderer.sortingOrder = -1;
                particleRenderer.sortingLayerName = "Default";

                SparksSilver.transform.position = Vector3.zero;
                particle.duration = 2.34f;
                particle.loop = true;
                particle.prewarm = true;
                particle.startLifetime = 1.5f;
                particle.startSpeed = 0;
                particle.startSize3D = false;
                particle.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.25f);
                particle.startRotation3D = false;
                particle.startRotation = 0;
                particle.flipRotation = 0;
                particle.startColor = new ParticleSystem.MinMaxGradient(Color.white);
                particle.gravityModifier = 0;
                particle.simulationSpace = ParticleSystemSimulationSpace.World;
                particle.simulationSpeed = 1;
                particle.scalingMode = ParticleSystemScalingMode.Shape;
                particle.playOnAwake = true;
                particle.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody;
                particle.maxParticles = 3000;
                particle.stopAction = ParticleSystemStopAction.None;
                particle.cullingMode = ParticleSystemCullingMode.Automatic;
                particle.ringBufferMode = ParticleSystemRingBufferMode.Disabled;

                SparksSilver.SetActive(true);


                var renderer = Silver.AddComponent<SpriteRenderer>();
                var collider = Silver.AddComponent<BoxCollider2D>();
                var floor = Silver.AddComponent<scrFloor>();
                var fade = Silver.AddComponent<scrSpriteFade>();


                Silver.transform.position = new Vector3(5, -19, 0);

                collider.isTrigger = true;
                collider.size = Vector2.one;

                floor.iconsprite = r1;
                floor.outlineSprite = null;
                floor.topglow = r2;
                floor.bottomglow = r3;
                floor.changeDir = false;
                floor.rotatecamera = 0f;
                floor.arrcolors = Gold.GetComponent<scrFloor>().arrcolors;
                floor.arrColorsBottomGlow = Gold.GetComponent<scrFloor>().arrColorsBottomGlow;
                var directionField = typeof(scrFloor).GetField("direction");
                if (directionField != null) {
                    directionField.SetValue(floor, 'R');
                }
                else {
                    floor.set("stringDirection", 'R');
                }

                floor.exitangle = 1.5707964;
                floor.entryangle = 4.712389;
                floor.angleLength = 0;
                fade.fadeTime = 0.3f;
                fade.oriFade = 0;
                fade.dontWatchForChangesToOriAlpha = true;

                var ColorPlanet = Silver.AddComponent<scrColorPlanet>();
                ColorPlanet.color = new Color(128, 128, 128);
                ColorPlanet.planetColor = (PlanetColor) PlanetColorEx.Silver;
                Silver.SetActive(true);

                Silver2 = new GameObject("Silver2");
                GameObject SpecIcon2 = new GameObject("SpecIcon");
                GameObject TopGlow2 = new GameObject("TopGlow");
                GameObject BottomGlow2 = new GameObject("BottomGlow");
                GameObject SparksSilver2 = new GameObject("SparksSilver");

                SpecIcon2.transform.parent = Silver2.transform;
                TopGlow2.transform.parent = Silver2.transform;
                BottomGlow2.transform.parent = Silver2.transform;
                SparksSilver2.transform.parent = Silver2.transform;
                Silver2.transform.parent = GoldContainer.transform;

                Silver2.tag = "Beat";
                Silver2.layer = 9;

                var r1_2 = SpecIcon2.AddComponent<SpriteRenderer>();
                SpecIcon2.transform.position = Vector3.zero;
                r1_2.sortingLayerName = "Floor";
                r1_2.sortingOrder = 100;
                var r2_2 = TopGlow2.AddComponent<SpriteRenderer>();
                var f2_2 = TopGlow2.AddComponent<scrSpriteFade>();
                //TopGlow2.transform.position = Vector3.zero;
                //r2_2.sortingLayerName = "Floor";
                //r2_2.sortingOrder = 101;
                //r2_2.color = Color.clear;
                //r2_2.sprite = Gold.transform.Find("TopGlow").gameObject.GetComponent<SpriteRenderer>().sprite;
                //f2_2.fadeTime = 0.3f;
                //f2_2.oriFade = 0;
                var r3_2 = BottomGlow2.AddComponent<SpriteRenderer>();
                var f3_2 = BottomGlow2.AddComponent<scrSpriteFade>();
                //BottomGlow2.transform.position = Vector3.zero;
                //r3_2.sortingLayerName = "Floor";
                //r3_2.sortingOrder = 88;
                //r3_2.color = Color.clear;
                //r3_2.sprite = Gold.transform.Find("BottomGlow").gameObject.GetComponent<SpriteRenderer>().sprite;
                //f3_2.fadeTime = 0.3f;
                //f3_2.oriFade = 0;
                var p4_2 = SparksSilver2.AddComponent<ParticleSystem>();
                var particle2 = p4_2.main;
                var emission2 = p4_2.emission;
                var shape2 = p4_2.shape;
                var colorOverLifetime2 = p4_2.colorOverLifetime;
                var sizeOverLifetime2 = p4_2.sizeOverLifetime;
                var particleRenderer2 = SparksSilver2.GetComponent<ParticleSystemRenderer>();


                emission2.enabled = true;
                emission2.rateOverTime = 600;
                emission2.rateOverDistance = 0;

                shape2.enabled = true;
                shape2.shapeType = ParticleSystemShapeType.Rectangle;
                shape2.texture = null;
                shape2.position = Vector3.zero;
                shape2.rotation = Vector3.zero;
                shape2.scale = Vector3.one;
                shape2.alignToDirection = false;
                shape2.randomDirectionAmount = 1;
                shape2.sphericalDirectionAmount = 0;
                shape2.randomPositionAmount = 0;

                colorOverLifetime2.enabled = true;
                colorOverLifetime2.color = new ParticleSystem.MinMaxGradient(Color.white, Color.clear);

                sizeOverLifetime2.enabled = true;
                sizeOverLifetime2.separateAxes = false;
                sizeOverLifetime2.size = Gold.transform.Find("TailGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.size;
                sizeOverLifetime2.x = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.x;
                sizeOverLifetime2.y = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.y;
                sizeOverLifetime2.z = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.z;
                sizeOverLifetime2.xMultiplier = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.xMultiplier;
                sizeOverLifetime2.yMultiplier = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.yMultiplier;
                sizeOverLifetime2.zMultiplier = Gold.transform.Find("SparksGold").GetComponent<ParticleSystem>()
                    .sizeOverLifetime.zMultiplier;

                particleRenderer2.renderMode = ParticleSystemRenderMode.Billboard;
                particleRenderer2.normalDirection = 1;
                particleRenderer2.sortMode = ParticleSystemSortMode.None;
                particleRenderer2.alignment = ParticleSystemRenderSpace.View;
                particleRenderer2.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
                particleRenderer2.sortingOrder = -1;
                particleRenderer2.sortingLayerName = "Default";

                SparksSilver2.transform.position = Vector3.zero;
                particle2.duration = 2.34f;
                particle2.loop = true;
                particle2.prewarm = true;
                particle2.startLifetime = 1.5f;
                particle2.startSpeed = 0;
                particle2.startSize3D = false;
                particle2.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.25f);
                particle2.startRotation3D = false;
                particle2.startRotation = 0;
                particle2.flipRotation = 0;
                particle2.startColor = new ParticleSystem.MinMaxGradient(Color.white);
                particle2.gravityModifier = 0;
                particle2.simulationSpace = ParticleSystemSimulationSpace.World;
                particle2.simulationSpeed = 1;
                particle2.scalingMode = ParticleSystemScalingMode.Shape;
                particle2.playOnAwake = true;
                particle2.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody;
                particle2.maxParticles = 3000;
                particle2.stopAction = ParticleSystemStopAction.None;
                particle2.cullingMode = ParticleSystemCullingMode.Automatic;
                particle2.ringBufferMode = ParticleSystemRingBufferMode.Disabled;

                SparksSilver2.SetActive(true);


                var renderer2 = Silver2.AddComponent<SpriteRenderer>();
                var collider2 = Silver2.AddComponent<BoxCollider2D>();
                var floor2 = Silver2.AddComponent<scrFloor>();
                var fade2 = Silver2.AddComponent<scrSpriteFade>();


                Silver2.transform.position = new Vector3(5, -20, 0);

                collider2.isTrigger = true;
                collider2.size = Vector2.one;

                floor2.iconsprite = r1_2;
                floor2.outlineSprite = null;
                floor2.topglow = r2_2;
                floor2.bottomglow = r3_2;
                floor2.changeDir = false;
                floor2.rotatecamera = 0f;
                floor2.arrcolors = Gold.GetComponent<scrFloor>().arrcolors;
                floor2.arrColorsBottomGlow = Gold.GetComponent<scrFloor>().arrColorsBottomGlow;
                if (directionField != null) {
                    directionField.SetValue(floor2, 'R');
                }
                else {
                    floor2.set("stringDirection", 'R');
                }

                floor2.exitangle = 1.5707964;
                floor2.entryangle = 4.712389;
                floor2.angleLength = 0;
                fade2.fadeTime = 0.3f;
                fade2.oriFade = 0;
                fade2.dontWatchForChangesToOriAlpha = true;

                var ColorPlanet2 = Silver2.AddComponent<scrColorPlanet>();
                ColorPlanet2.color = new Color(128, 128, 128);
                ColorPlanet2.planetColor = (PlanetColor) PlanetColorEx.Silver;
                Silver2.SetActive(true);

                p4.GetComponent<Renderer>().material =
                    createParticleMaterial();
                p4_2.GetComponent<Renderer>().material =
                    createParticleMaterial();
            }
        }
       

        [SafePatch("RTM.MoreColorPatch", "scrPlanet", "SetColor")]
        internal static class MoreColorPatch {
            public static bool Prefix(scrPlanet __instance, PlanetColor planetColor) {
                if (planetColor == PlanetColor.Gold)
                {
                    __instance.previousPaintedColor = planetColor;
                    __instance.RemoveGold();
                    GoldColor = new Color(0.96875f, 0.796875f, 0.015625f);
                    PlanetGoldColor = PlanetColorEx.Gold;
                    __instance.DisableCustomColor();
                    __instance.SwitchToGold();
                    Persistence.SetPlayerColor(scrPlanet.goldColor, __instance.isRed);
                    return false;
                }

                if (planetColor == (PlanetColor) PlanetColorEx.Silver) {
                    __instance.previousPaintedColor = planetColor;
                    __instance.RemoveGold();
                    GoldColor = new Color(0.95f, 0.95f, 0.95f);
                    PlanetGoldColor = PlanetColorEx.Silver;
                    __instance.DisableCustomColor();
                    __instance.SwitchToGold();
                    Persistence.SetPlayerColor(silverColor, __instance.isRed);
                    return false;
                }

                return true;
            }
        }

        [SafePatch("RTM.GoldPlanetColor", "scrPlanet", "SwitchToGold")]
        internal static class GoldPlanetColor {
            public static bool Prefix(scrPlanet __instance) {
                ParticleSystem.MainModule SparkColorMain;
                ParticleSystem.ColorOverLifetimeModule SparkColor;
                Transform transform = __instance.goldPlanet.transform;

                switch (PlanetGoldColor) {
                    case (PlanetColorEx) 999:
                        __instance.transform.Find("Tail").gameObject.SetActive(false);
                        __instance.tailParticles = transform.Find("TailGold").GetComponent<ParticleSystem>();
                        __instance.sparks.gameObject.SetActive(false);
                        __instance.sparks = transform.Find("SparksGold").GetComponent<ParticleSystem>();
                        __instance.coreParticles.gameObject.SetActive(false);
                        __instance.goldPlanet.SetActive(true);
                        __instance.ring.gameObject.SetActive(false);
                        __instance.ring = transform.Find("RingGold").GetComponent<SpriteRenderer>();
                        __instance.glow.gameObject.SetActive(false);
                        __instance.glow = transform.Find("GlowGold").GetComponent<SpriteRenderer>();


                        __instance.glow.color =
                            new Color(GoldColor.r, GoldColor.g, GoldColor.b, GoldColor.a * 0.5f);
                        __instance.ring.color =
                            new Color(GoldColor.r, GoldColor.g, GoldColor.b, GoldColor.a * 0.61f);

                        __instance.animator.runtimeAnimatorController = __instance.altController;
                        __instance.goldPlanet.transform.Find("PlanetGold").gameObject.SetActive(false);
                        __instance.sprite.sprite = __instance.whiteSprite;
                        __instance.sprite.color = GoldColor;

                        SparkColorMain = __instance.sparks.main;
                        SparkColor = __instance.sparks.colorOverLifetime;
                        SparkColorMain.startColor = Color.blue;
                        __instance.gameObject.GetComponent<SpriteRenderer>().color = GoldColor;
                        SparkColor.color = new ParticleSystem.MinMaxGradient(Color.blue, Color.clear);
                        break;
                    default:

                        __instance.animator.runtimeAnimatorController = __instance.altController;
                        __instance.transform.Find("Tail").gameObject.SetActive(false);
                        __instance.tailParticles = transform.Find("TailGold").GetComponent<ParticleSystem>();
                        __instance.sparks.gameObject.SetActive(false);
                        __instance.sparks = transform.Find("SparksGold").GetComponent<ParticleSystem>();
                        __instance.coreParticles.gameObject.SetActive(false);
                        __instance.goldPlanet.SetActive(true);
                        __instance.ring.gameObject.SetActive(false);
                        __instance.ring = transform.Find("RingGold").GetComponent<SpriteRenderer>();
                        __instance.glow.gameObject.SetActive(false);
                        __instance.glow = transform.Find("GlowGold").GetComponent<SpriteRenderer>();

                        __instance.glow.color = GoldColor;
                        __instance.ring.color =
                            new Color(GoldColor.r, GoldColor.g, GoldColor.b, GoldColor.a * 0.5f);


                        if (PlanetGoldColor == PlanetColorEx.Gold &&
                            !RandomTweaksMiscModule.settings.EnableCustomTexture) {
                            __instance.sprite.enabled = false;
                            __instance.sprite = transform.Find("PlanetGold").GetComponent<SpriteRenderer>();
                            __instance.goldPlanet.transform.Find("PlanetGold").gameObject.SetActive(true);

                            SparkColor = __instance.sparks.colorOverLifetime;
                            SparkColorMain = __instance.sparks.main;
                            var color = new ParticleSystem.MinMaxGradient();
                            color.mode = ParticleSystemGradientMode.TwoColors;
                            Color color1 = new Color(1, 0.4808381f, 0);
                            Color color2 = new Color(1, 0.05098045f, 0.05098045f);

                            var grad = new ParticleSystem.MinMaxGradient();
                            grad.colorMin = color2;
                            grad.colorMax = color1;
                            grad.mode = ParticleSystemGradientMode.TwoColors;
                            SparkColor.color = grad;
                            SparkColorMain.startColor = Color.white;
                        }
                        else {
                            __instance.sprite.enabled = true;
                            __instance.sprite.sprite =
                                __instance
                                    .whiteSprite; //transform.Find("PlanetGold").GetComponent<SpriteRenderer>();
                            __instance.sprite.color = GoldColor;
                            __instance.goldPlanet.transform.Find("PlanetGold").gameObject.SetActive(false);
                            if (PlanetGoldColor == PlanetColorEx.Gold) {
                                SparkColor = __instance.sparks.colorOverLifetime;
                                SparkColorMain = __instance.sparks.main;
                                var color = new ParticleSystem.MinMaxGradient();
                                color.mode = ParticleSystemGradientMode.TwoColors;
                                Color color1 = new Color(1, 0.4808381f, 0);
                                Color color2 = new Color(1, 0.05098045f, 0.05098045f);

                                var grad = new ParticleSystem.MinMaxGradient();
                                grad.colorMin = color2;
                                grad.colorMax = color1;
                                grad.mode = ParticleSystemGradientMode.TwoColors;
                                SparkColor.color = grad;
                                SparkColorMain.startColor = Color.white;
                            }
                            else {
                                SparkColorMain = __instance.sparks.main;
                                SparkColor = __instance.sparks.colorOverLifetime;
                                SparkColorMain.startColor = Color.cyan;
                                SparkColor.color = new ParticleSystem.MinMaxGradient(Color.cyan, Color.clear);
                            }
                        }

                        var main = __instance.deathExplosion.main;
                        main.startColor = GoldColor;
                        break;
                }

                var col2 = __instance.tailParticles.main;
                col2.startColor = GoldColor;
                var col = __instance.tailParticles.colorOverLifetime;
                col.color = new ParticleSystem.MinMaxGradient(new Color(GoldColor.r, GoldColor.g, GoldColor.b,
                    GoldColor.a * 0.53125f));
                __instance.sparks.gameObject.GetComponent<Renderer>().material =
                    __instance.coreParticles.gameObject.GetComponent<Renderer>().material;
                if (RandomTweaksMiscModule.settings.DisableGoldPlanetParticle) {
                    SparkColorMain.startColor = Color.clear;
                    SparkColor.color = Color.clear;
                }

                return false;
            }
        }
    }
}