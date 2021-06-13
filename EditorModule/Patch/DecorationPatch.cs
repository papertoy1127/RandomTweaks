using System;
using System.Collections.Generic;
using System.IO;
using ADOFAI;
using ADOLib;
using ADOLib.Misc;
using ADOLib.SafeTools;
using HarmonyLib;
using RandomTweaksEditorModule.Behavior;
using UnityEngine;

namespace RandomTweaksEditorModule.Patch
{
    public partial class Patch {
	    public static scrDecorationClick TMP;
	    public static LevelEvent tempEvent;
	    [SafePatch("RTE.AddDecorationPatch", "CustomLevel", "UpdateDecorationSprites")]
	    private static class AddDecorationPatch
	    {
		    public static bool Prefix(CustomLevel __instance)
		    {
			    if (!(RandomTweaksEditorModule.settings.EnableDecorationClickMove || RandomTweaksEditorModule.settings.EnableDecorationClickToEvent)) return true;
			    if (!GCS.speedTrialMode)
			    {
				    __instance.spritesLoaded = false;
			    }

			    scrDecorationManager component = __instance.decHolder.GetComponent<scrDecorationManager>();
			    component.ClearDecorations();
			    foreach (LevelEvent levelEvent in __instance.events.FindAll(x =>
				    x.eventType == LevelEventType.AddDecoration || x.eventType == LevelEventType.AddText))
			    {
				    DecorationType decType = DecorationType.Image;
				    string text = levelEvent.data["decText"].ToString();
				    LevelEventType eventType = levelEvent.eventType;
				    if (eventType != LevelEventType.AddDecoration)
				    {
					    if (eventType == LevelEventType.AddText)
					    {
						    decType = DecorationType.Text;
					    }
				    }
				    else if (!string.IsNullOrEmpty(text) && !__instance.spritesLoaded)
				    {
					    string filePath = Path.Combine(Path.GetDirectoryName(__instance.levelPath), text);
					    __instance.imgHolder.AddSprite(text, filePath);
				    }

				    string keyTag = levelEvent.data["tag"].ToString();
				    Vector2 vector = RDUtils.StringToVector2(levelEvent.data["position"].ToString()) * 1.5f;
				    Vector2 pivotOffset = RDUtils.StringToVector2(levelEvent.data["pivotOffset"].ToString()) * 1.5f;
				    float angle = Convert.ToSingle(levelEvent.data["rotation"]);
				    float d = Convert.ToSingle(levelEvent.data["scale"]) / 100f;
				    int depth = Convert.ToInt32(levelEvent.data["depth"]);
				    Color color = levelEvent.data["color"].ToString().HexToColor();
				    scrPlanet followPlanet = null;
				    switch ((DecPlacementType) Enum.Parse(typeof(DecPlacementType),
					    levelEvent.data["relativeTo"].ToString()))
				    {
					    case DecPlacementType.Tile:
					    {
						    Vector3 position = __instance.get<List<scrFloor>>("floors")[levelEvent.floor].transform.position;
						    vector += new Vector2(position.x, position.y);
						    break;
					    }
					    case DecPlacementType.RedPlanet:
						    followPlanet = scrController.instance.redPlanet;
						    break;
					    case DecPlacementType.BluePlanet:
						    followPlanet = scrController.instance.bluePlanet;
						    break;
				    }

				    var fontName = levelEvent.data.ContainsKey("font")
					    ? RDUtils.ParseEnum(levelEvent.data["font"].ToString(), FontName.Default)
					    : FontName.Default;
				    tempEvent = levelEvent;
				    component.AddDecoration(decType, keyTag, text, vector, pivotOffset, Vector2.one * d, angle, depth,
					    color, followPlanet, fontName);

			    }

			    if (GCS.standaloneLevelMode)
			    {
				    __instance.spritesLoaded = true;
			    }

			    return false;
		    }
	    }
	    
	    [SafePatch("RTE.DecorationMgrPatch", "scrDecorationManager", "AddDecoration")]
	    private static class DecorationMgrPatch
	    {
		    public static void Prefix(scrDecorationManager __instance, DecorationType decType, string keyTag, string textOrFilename, Vector2 pos, Vector2 pivotOffset, Vector2 scale, float angle, int depth, Color color, scrPlanet followPlanet, FontName fontName = FontName.Default)
		    {
			    if (!(RandomTweaksEditorModule.settings.EnableDecorationClickMove || RandomTweaksEditorModule.settings.EnableDecorationClickToEvent)) return;
			    string[] array = keyTag.Split(new char[]
			    {
				    ' '
			    }, StringSplitOptions.RemoveEmptyEntries);
			    if (array.Length == 0)
			    {
				    array = new string[]
				    {
					    "NO TAG"
				    };
			    }
			    GameObject original;
			    if (decType != DecorationType.Image && decType == DecorationType.Text)
			    {
				    original = __instance.txtDecPrefab;
			    }
			    else
			    {
				    original = __instance.visDecPrefab;
			    }
			    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, __instance.transform);

			    scrDecoration component;
			    if (decType != DecorationType.Image && decType == DecorationType.Text)
			    {
				    component = gameObject.GetComponent<scrTextDecoration>();
			    }
			    else
			    {
				    component = gameObject.GetComponent<scrVisualDecoration>();
			    }
			    TMP = gameObject.GetOrAddComponent<scrDecorationClick>();
			    TMP.Event = tempEvent;
			    TMP.Floor = CustomLevel.instance.levelMaker.listFloors[tempEvent.floor];
			    foreach (string key in array)
			    {
				    if (!__instance.decorations.ContainsKey(key))
				    {
					    __instance.decorations[key] = new List<scrDecoration>();
				    }
				    __instance.decorations[key].Add(component);
			    }
			    __instance.allDecorations.Add(component);
			    if (decType != DecorationType.Image && decType == DecorationType.Text)
			    {
				    scrTextDecoration scrTextDecoration = (scrTextDecoration)component;
				    scrTextDecoration.SetText(textOrFilename);
				    scrTextDecoration.SetFont(fontName);
			    }
			    else
			    {
				    Sprite sprite = (!textOrFilename.IsNullOrEmpty()) ? __instance.get<scrExtImgHolder>("imageHolder").customSprites[textOrFilename].sprite : __instance.defaultSprite;
				    ((scrVisualDecoration)component).SetSprite(sprite);
			    }
			    component.SetPosition(pos, pivotOffset);
			    component.SetScale(scale);
			    component.startRot = angle;
			    component.SetDepth(depth);
			    component.SetColor(color);
			    component.startPos = pos;
			    component.SetRotation(0f);
			    component.followPlanet = followPlanet;
		    }
	    }
    }
}