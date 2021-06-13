using System.Collections;
using ADOLib.SafeTools;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace RandomTweaksMiscModule.Patch {
    public partial class Patch {
        public static ParticleSystem p1;
        public static ParticleSystem p2;
        public static ParticleSystemRenderer r1;
        public static ParticleSystemRenderer r2;
        
        [SafePatch("RTM.ResizePatch", "scrPlanet", "Update")]
        public static class ResizePatch {
            public static void Postfix(scrPlanet __instance) {
                if (RandomTweaksMiscModule.settings.ParticleType == ParticleType.None) return;
                var current = Camera.current;
                var Camsize = current.orthographicSize / 5f;
                var CamRot = current.transform.eulerAngles.z;
                ParticleSystem.MainModule main;
                ParticleSystemRenderer renderer;
                ParticleSystem obj;
                if (__instance.isRed) {
                    p1.transform.localScale = Vector3.one / Mathf.Pow(Camsize, 1 / 8f);
                    main = p1.main;
                    renderer = r1;
                    obj = p1;
                }
                else {
                    p2.transform.localScale = Vector3.one / Mathf.Pow(Camsize, 1 / 8f);
                    main = p2.main;
                    renderer = r2;
                    obj = p2;
                }

                switch (RandomTweaksMiscModule.settings.ParticleType) {
                    case ParticleType.Fire:
                        main.startSize = new ParticleSystem.MinMaxCurve(0.15f / Camsize, 0.25f / Camsize);
                        renderer.maxParticleSize = 0.08f / Camsize;
                        renderer.minParticleSize = 0.04f / Camsize;
                        obj.transform.eulerAngles = new Vector3(0, 0, CamRot);
                        break;
                    case ParticleType.Shield:
                        main.startSize = 0.1f / Camsize;
                        renderer.maxParticleSize = 0.01f / Camsize;
                        renderer.minParticleSize = 0.01f / Camsize;
                        break;
                }
            }
        }
        [SafePatch("RTM.HideParticlePatch1", "scnEditor", "Start")]
        public static class HideParticlePatch1 {
            public static void Postfix() {
                if (GCS.standaloneLevelMode) return;
                scrController.instance.redPlanet.transform.Find("Custom Particle").gameObject.SetActive(false);
                scrController.instance.bluePlanet.transform.Find("Custom Particle").gameObject.SetActive(false);
            }
        }
        [SafePatch("RTM.HideParticlePatch2", "scnEditor", "SwitchToEditMode")]
        public static class HideParticlePatch2 {
            public static void Postfix() {
                scrController.instance.redPlanet.transform.Find("Custom Particle").gameObject.SetActive(false);
                scrController.instance.bluePlanet.transform.Find("Custom Particle").gameObject.SetActive(false);
            }
        }
        [SafePatch("RTM.ShowParticlePatch1", "scnEditor", "Play")]
        public static class ShowParticlePatch1 {
            public static void Postfix() {
                if (RandomTweaksMiscModule.settings.ParticleType == ParticleType.None) return;
                scrController.instance.redPlanet.transform.Find("Custom Particle").gameObject.SetActive(true);
                scrController.instance.bluePlanet.transform.Find("Custom Particle").gameObject.SetActive(true);
            }
        }
        [SafePatch("RTM.ChangeFireColorPatch", "scrPlanet", "SetCoreColor")]
        public static class ChangeFireColorPatch {
            public static void Postfix(scrPlanet __instance, Color color) {
                var particleSytem = __instance.transform.Find("Custom Particle").GetComponent<ParticleSystem>();
                var colorOverLifetime = particleSytem.colorOverLifetime;
                switch (RandomTweaksMiscModule.settings.ParticleType) {
                    case ParticleType.Fire:
                        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(new Gradient() {
                            colorKeys = new[] {
                                new GradientColorKey(color + Color.white + Color.white, 0f),
                                new GradientColorKey(color, 1f)
                            },
                            alphaKeys = new[] {
                                new GradientAlphaKey(0f, 0),
                                new GradientAlphaKey(1f, 1)
                            }
                        });
                        break;
                    case ParticleType.Shield:
                        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(new Gradient {
                            colorKeys = new [] {
                                new GradientColorKey(color + Color.white, 0f),
                                new GradientColorKey(color, 0.5f)
                            },
                            alphaKeys = new [] {
                                new GradientAlphaKey(0.05f, 0)
                            }
                        });
                        break;
                }
            }
        }
        
        [SafePatch("RTM.ChangeFireGoldPatch", "scrPlanet", "SwitchToGold")]
        public static class ChangeFireGoldPatch {
            public static void Postfix(scrPlanet __instance) {
                ChangeFireColorPatch.Postfix(__instance, GoldColor);
            }
        }

        [SafePatch("RTM.AddParticlePatch", "scrPlanet", "Awake")]
        public static class AddParticlePatch {
            public static void Postfix(scrPlanet __instance) {
                var ParticleObject = new GameObject("Custom Particle");
                ParticleObject.transform.parent = __instance.transform;
                var particleSystem = ParticleObject.AddComponent<ParticleSystem>();

                var color = Persistence.GetPlayerColor(__instance.isRed);
                if (color == scrPlanet.goldColor) color = Color.yellow;
                else if (color == scrPlanet.transBlueColor) color = new Color(0.3607843f, 0.7882353f, 0.9294118f);
                else if (color == scrPlanet.transPinkColor) color = new Color(0.9568627f, 0.6431373f, 0.7098039f);
                else if (color == scrPlanet.nbYellowColor) color = new Color(0.996f, 0.953f, 0.18f);
                else if (color == scrPlanet.nbPurpleColor) color = new Color(0.612f, 0.345f, 0.82f);
                else if (color == scrPlanet.rainbowColor) color = new Color(1, 0, 0);

                CustomParticle(RandomTweaksMiscModule.settings.ParticleType, __instance);
            }
        }

        public static void CustomParticle(ParticleType particleType, scrPlanet planet) {
            var particleSystem = planet.transform.Find("Custom Particle").GetComponent<ParticleSystem>();
            var particle = particleSystem.main;
            var emission = particleSystem.emission;
            var shape = particleSystem.shape;
            var colorOverLifetime = particleSystem.colorOverLifetime;
            var velocityOverLifetime = particleSystem.velocityOverLifetime;
            var sizeOverLifetime = particleSystem.sizeOverLifetime;
            var particleRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            if (planet.isRed) {
                p1 = particleSystem;
                r1 = particleRenderer;
            }
            else {
                p2 = particleSystem;
                r2 = particleRenderer;
            }
            
            switch (particleType) {
                case ParticleType.None:
                    particleSystem.Stop();
                    break;
                case ParticleType.Fire:
                    particleSystem.Play();
                    emission.enabled = true;
                    emission.rateOverTime = 400;
                    emission.rateOverDistance = 0;

                    shape.enabled = true;
                    shape.shapeType = ParticleSystemShapeType.Circle;
                    shape.texture = null;
                    shape.position = Vector3.zero;
                    shape.rotation = Vector3.zero;
                    shape.scale = Vector3.one * 0.2f;
                    shape.alignToDirection = false;
                    shape.randomDirectionAmount = 1;
                    shape.sphericalDirectionAmount = 0;
                    shape.randomPositionAmount = 0;
                    shape.randomDirectionAmount = 0.1f;

                    colorOverLifetime.enabled = true;
                    /*
                    colorOverLifetime.color = new ParticleSystem.MinMaxGradient(new Gradient() {
                        colorKeys = new[] {
                            new GradientColorKey(color + Color.white + Color.white, 0f),
                            new GradientColorKey(color, 1f)
                        },
                        alphaKeys = new[] {
                            new GradientAlphaKey(0f, 0),
                            new GradientAlphaKey(1f, 1)
                        }
                    });
                    */

                    sizeOverLifetime.enabled = false;
                    velocityOverLifetime.enabled = true;
                    var x = 0;
                    var z = 0;
                    velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(x, x);
                    velocityOverLifetime.y =  new ParticleSystem.MinMaxCurve(0, 1.8f);
                    velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(z, z);

                    particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
                    particleRenderer.normalDirection = 1;
                    particleRenderer.sortMode = ParticleSystemSortMode.YoungestInFront;
                    particleRenderer.alignment = ParticleSystemRenderSpace.View;
                    particleRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
                    particleRenderer.sortingOrder = 0;
                    particleRenderer.sortingLayerName = "Planet";
                    particleRenderer.material = createParticleMaterial();
                    particleRenderer.pivot = Vector3.zero;
                    particleRenderer.minParticleSize = 0.04f;
                    particleRenderer.maxParticleSize = 0.08f;

                    var transform = particleSystem.transform;
                    transform.localPosition = Vector3.zero;
                    particle.duration = 0.7f;
                    particle.loop = true;
                    particle.prewarm = false;
                    particle.startLifetime = new ParticleSystem.MinMaxCurve(0.01f, 1f);
                    particle.startSpeed = 0;
                    particle.startSize3D = false;
                    particle.startSize = new ParticleSystem.MinMaxCurve(0.015f, 0.25f);
                    particle.startRotation3D = false;
                    particle.startRotation = 0;
                    particle.flipRotation = 0;
                    particle.startColor = new ParticleSystem.MinMaxGradient(Color.white);
                    particle.gravityModifier = 0f; //-0.18f;
                    particle.simulationSpace = ParticleSystemSimulationSpace.Local;
                    particle.simulationSpeed = 1;
                    particle.scalingMode = ParticleSystemScalingMode.Hierarchy;
                    particle.playOnAwake = true;
                    particle.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody;
                    particle.maxParticles = 3000;
                    particle.stopAction = ParticleSystemStopAction.None;
                    particle.cullingMode = ParticleSystemCullingMode.Automatic;
                    particle.ringBufferMode = ParticleSystemRingBufferMode.Disabled;

                    transform.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case ParticleType.Shield:
                    particleSystem.Play();
                    emission.enabled = true;
                    emission.rateOverTime = 2000;
                    emission.rateOverDistance = 0;

                    shape.enabled = true;
                    shape.shapeType = ParticleSystemShapeType.Donut;
                    shape.texture = null;
                    shape.position = Vector3.zero;
                    shape.rotation = Vector3.zero;
                    shape.scale = Vector3.one * 0.65f;
                    shape.alignToDirection = false;
                    shape.sphericalDirectionAmount = 360;
                    shape.randomPositionAmount = 0;
                    shape.randomDirectionAmount = 360f;

                    velocityOverLifetime.enabled = false;
                    
                    colorOverLifetime.enabled = true;
                    /*
                    colorOverLifetime.color = new ParticleSystem.MinMaxGradient(new Gradient() {
                        colorKeys = new[] {
                            new GradientColorKey(color + Color.white + Color.white, 0f),
                            new GradientColorKey(color, 1f)
                        },
                        alphaKeys = new[] {
                            new GradientAlphaKey(0f, 0),
                            new GradientAlphaKey(1f, 1)
                        }
                    });
                    */

                    sizeOverLifetime.enabled = false;

                    particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
                    particleRenderer.normalDirection = 1;
                    particleRenderer.sortMode = ParticleSystemSortMode.YoungestInFront;
                    particleRenderer.alignment = ParticleSystemRenderSpace.View;
                    particleRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
                    particleRenderer.sortingOrder = 10;
                    particleRenderer.sortingLayerName = "Planet";
                    particleRenderer.material = createParticleMaterial();
                    particleRenderer.pivot = Vector3.zero;
                    particleRenderer.minParticleSize = 0.01f;
                    particleRenderer.maxParticleSize = 0.01f;

                    particleSystem.transform.localPosition = Vector3.zero;
                    particle.duration = 10f;
                    particle.loop = true;
                    particle.prewarm = false;
                    particle.startLifetime = 1f;
                    particle.startSpeed = 0;
                    particle.startSize3D = false;
                    particle.startSize = 0.1f;
                    particle.startRotation3D = false;
                    particle.flipRotation = 0;
                    particle.startColor = new ParticleSystem.MinMaxGradient(Color.white);
                    particle.gravityModifier = 0;
                    particle.simulationSpace = ParticleSystemSimulationSpace.Local;
                    particle.simulationSpeed = 1;
                    particle.scalingMode = ParticleSystemScalingMode.Hierarchy;
                    particle.playOnAwake = true;
                    particle.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody;
                    particle.maxParticles = 3000;
                    particle.stopAction = ParticleSystemStopAction.None;
                    particle.cullingMode = ParticleSystemCullingMode.Automatic;
                    particle.ringBufferMode = ParticleSystemRingBufferMode.Disabled;

                    break;
            }
        }
    }
}