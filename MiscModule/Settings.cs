using System.IO;
using System.Windows.Forms;
using ADOLib.Settings;
using ADOLib.Translation;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityModManagerNet;
using Ookii.Dialogs;

namespace RandomTweaksMiscModule {
	public enum ParticleType {
		None,
		Fire,
		Shield
	}
	[Category(
		TabName = "RandomTweaks",
		Name = "RandomTweaks Misc Module",
		MinVersion = 71,
		MaxVersion = 74,
		PatchClass = typeof(Patch.Patch),
		ForceType = ForceType.ForceEnable
	)]
	public class Settings : Category
	{
		public override UnityModManager.ModEntry ModEntry => RandomTweaksMiscModule.ModEntry;

		public override void OnGUI()
		{
			GUILayout.Label(RandomTweaksMiscModule.Translator.Translate("UI.HelpSilverPlanet"), GUIExtended.Text);
			DisableGoldPlanetParticle =
				GUIExtended.Toggle(DisableGoldPlanetParticle,
					RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.DisableGoldPlanetParticle"));
			EnableCustomTexture = GUIExtended.Toggle(EnableCustomTexture,
				RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.EnableCustomTexture") + 
				(!EnableCustomTexture && CTEnabled ? RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.NeedsRestart") : ""));
			CTEnabled = EnableCustomTexture || CTEnabled;
			if (EnableCustomTexture) {
				GUIExtended.BeginIndent(40);
				GUILayout.Space(5);
				GUILayout.Label(RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.FindImage"), GUIExtended.Text);
				GUILayout.Space(5);
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(
					RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.FindRedImage"),
					GUIExtended.Selection, GUILayout.Width(80), GUILayout.Height(80))) {
					var filePath = FileOpen();
					File.Copy(filePath, System.IO.Path.Combine(ModEntry.Path + "RedPlanet.png"), true);
				}
				GUILayout.Space(10);
				if (GUILayout.Button(
					RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.FindBlueImage"),
					GUIExtended.Selection, GUILayout.Width(80), GUILayout.Height(80))) {
					var filePath = FileOpen();
					File.Copy(filePath, System.IO.Path.Combine(ModEntry.Path + "BluePlanet.png"), true);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(15);
				GUILayout.Label(RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.Scale"), GUIExtended.Text);
				GUIExtended.BeginIndent(10);
				GUILayout.BeginHorizontal();
				GUILayout.Label(RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.FindRedImage") + ": ", GUIExtended.Text, GUILayout.Width(80));
				string tmpW = GUILayout.TextField(ScaleStrRed, GUIExtended.TextInput, GUILayout.Width(200));
				string tmp2W = tmpW;
				if (tmp2W == "-") tmp2W = "0";
				if (tmp2W == "") tmp2W = "0";
				if (tmp2W.EndsWith(".")) tmp2W = tmp2W.Substring(0, tmp2W.Length - 1);
				if (float.TryParse(tmp2W, out var tmp3W)) {
					if (tmp3W >= 20) {
						tmp3W = 20;
						tmpW = "20";
					}
					if (tmp3W <= 0) {
						tmp3W = -1;
						tmpW = tmp2W == "0" ? tmpW : tmp2W;
					}
					ScaleRed = tmp3W;
					ScaleStrRed = tmpW;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10);
				
				GUILayout.BeginHorizontal();
				GUILayout.Label(RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.FindBlueImage") + ": ", GUIExtended.Text, GUILayout.Width(80));
				string btmpW = GUILayout.TextField(ScaleStrBlue, GUIExtended.TextInput, GUILayout.Width(200));
				string btmp2W = btmpW;
				if (btmp2W == "-") btmp2W = "0";
				if (btmp2W == "") btmp2W = "0";
				if (btmp2W.EndsWith(".")) btmp2W = btmp2W.Substring(0, btmp2W.Length - 1);
				if (float.TryParse(btmp2W, out var btmp3W)) {
					if (btmp3W >= 20) {
						btmp3W = 20;
						btmpW = "20";
					}
					if (btmp3W <= 0) {
						btmp3W = -1;
						btmpW = btmp2W == "0" ? btmpW : btmp2W;
					}
					ScaleBlue = btmp3W;
					ScaleStrBlue = btmpW;
				}
				GUILayout.EndHorizontal();
				GUIExtended.EndIndent();
				GUILayout.Space(20);
				
				var sizered = Mathf.RoundToInt(84 * ScaleRed);
				var sizeblue = Mathf.RoundToInt(84 * ScaleBlue);
				if (GUILayout.Button(RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.ChangeSprite"), GUIExtended.Selection, GUILayout.Width(300))) {
					var redTexture = new Texture2D(2, 2);
					redTexture.LoadRawTextureData(Texture2D.whiteTexture.GetRawTextureData());
					if (File.Exists($"{ModEntry.Path}RedPlanet.png")) 
						redTexture.LoadImage(File.ReadAllBytes($"{ModEntry.Path}RedPlanet.png"));
					new TextureScale().Bilinear(redTexture, sizered, sizered);
					Patch.Patch.RedPlanet = Sprite.Create(redTexture, new Rect(0, 0, redTexture.width, redTexture.height), new Vector2(0.5f, 0.5f));
					
					var blueTexture = new Texture2D(2, 2);
					blueTexture.LoadRawTextureData(Texture2D.whiteTexture.GetRawTextureData());
					if (File.Exists($"{ModEntry.Path}BluePlanet.png")) 
						blueTexture.LoadImage(File.ReadAllBytes($"{ModEntry.Path}BluePlanet.png"));
					new TextureScale().Bilinear(blueTexture, sizeblue, sizeblue);
					Patch.Patch.BluePlanet = Sprite.Create(blueTexture, new Rect(0, 0, blueTexture.width, blueTexture.height), new Vector2(0.5f, 0.5f));

					GCS.sceneToLoad = SceneManager.GetActiveScene().name;
					scrController.instance.StartLoadingScene(WipeDirection.StartsFromLeft);
				}
				GUILayout.Space(5);
				GUIExtended.EndIndent();
			}
			GUILayout.Space(10);
			GUILayout.Label(RandomTweaksMiscModule.Translator.Translate("RandomTweaksMiscModule.Settings.Particle"), GUIExtended.Text);
			GUILayout.Space(5);
			var particleType = (ParticleType) GUIExtended.SelectionGrid((int) ParticleType, new[] {"None", "Fire", "Ring"}, 3, null, null, GUILayout.Width(100));
			if (particleType != ParticleType) {
				ParticleType = particleType;
				Patch.Patch.CustomParticle(particleType, scrController.instance.redPlanet);
				Patch.Patch.CustomParticle(particleType, scrController.instance.bluePlanet);
				scrController.instance.redPlanet.LoadPlanetColor();
				scrController.instance.bluePlanet.LoadPlanetColor();
			}
		}


		private VistaOpenFileDialog OpenDialog { get; set; }
		private Stream openStream { get; set; }
		
		public override void Init()
		{
			OpenDialog = new VistaOpenFileDialog();
			CTEnabled = EnableCustomTexture;
		}
    
		public string FileOpen()
		{
			OpenDialog.Filter = $"{RandomTweaksMiscModule.Translator.Translate("Dialog.ImageFile")}| *.png; *.jpg; *.bmp; *.gif";
			OpenDialog.FilterIndex = 1 ;
			OpenDialog.Title = RandomTweaksMiscModule.Translator.Translate("Dialog.ImageDialog");
			if (OpenDialog.ShowDialog() == DialogResult.OK)
			{
				if ((openStream = OpenDialog.OpenFile()) != null)
				{
					return OpenDialog.FileName;
				}
			}
			return null;
		}

		public ParticleType ParticleType = ParticleType.None;
		public bool DisableGoldPlanetParticle;
		public bool EnableCustomTexture;
		public bool CTEnabled { get; set; }
		public float ScaleRed = 1;
		public string ScaleStrRed = "1";
		public float ScaleBlue = 1;
		public string ScaleStrBlue = "1";
		public int PlanetColor;
	}
}
