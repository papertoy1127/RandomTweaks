using System.IO;
using ADOLib.Settings;
using ADOLib.Translation;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace RandomTweaksMiscModule
{
	internal static class RandomTweaksMiscModule
	{
		public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

		public static Settings settings => Category.GetCategory<Settings>();

		internal static void Setup(UnityModManager.ModEntry modEntry)
		{
			ModEntry = modEntry;
			Harmony harmony = new Harmony(modEntry.Info.Id);
			Logger = modEntry.Logger;
			Path = modEntry.Path;
			Translator = new Translator(Path);
			Default_Particle = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
			Texture2D particleTexture = new Texture2D(2, 2);
			particleTexture.LoadImage(File.ReadAllBytes(System.IO.Path.Combine(ModEntry.Path, "Default_Particle.png")));
			Default_Particle.mainTexture = particleTexture;
			int sizered = Mathf.RoundToInt(84f * settings.ScaleRed);
			int sizeblue = Mathf.RoundToInt(84f * settings.ScaleBlue);
			Texture2D redTexture = new Texture2D(2, 2);
			redTexture.LoadRawTextureData(Texture2D.whiteTexture.GetRawTextureData());
			bool flag = File.Exists(ModEntry.Path + "RedPlanet.png");
			if (flag)
			{
				redTexture.LoadImage(File.ReadAllBytes(ModEntry.Path + "RedPlanet.png"));
			}
			new TextureScale().Bilinear(redTexture, sizered, sizered);
			Patch.Patch.RedPlanet = Sprite.Create(redTexture, new Rect(0f, 0f, (float)redTexture.width, (float)redTexture.height), new Vector2(0.5f, 0.5f));
			Texture2D blueTexture = new Texture2D(2, 2);
			blueTexture.LoadRawTextureData(Texture2D.whiteTexture.GetRawTextureData());
			bool flag2 = File.Exists(ModEntry.Path + "BluePlanet.png");
			if (flag2)
			{
				blueTexture.LoadImage(File.ReadAllBytes(ModEntry.Path + "BluePlanet.png"));
			}
			new TextureScale().Bilinear(blueTexture, sizeblue, sizeblue);
			Patch.Patch.BluePlanet = Sprite.Create(blueTexture, new Rect(0f, 0f, (float)blueTexture.width, (float)blueTexture.height), new Vector2(0.5f, 0.5f));
		}

		public static Translator Translator;

		public static string Path;

		public static Material Default_Particle;

		public static UnityModManager.ModEntry ModEntry;
	}
}
