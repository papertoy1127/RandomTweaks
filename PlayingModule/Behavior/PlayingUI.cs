using System;
using UnityEngine;


namespace RandomTweaksPlayingModule.Behavior {
    internal class PlayingUI : MonoBehaviour {
        public static float WidthX = 0.33f;

        public static float OverloadGauge;
        internal static Texture2D GaugeTextureOutside = new Texture2D(1156, 144);
        internal static Texture2D GaugeTextureInside = new Texture2D(1156, 118);
        public static GUIStyle OverloadText = new GUIStyle();
        public void Start() {
            OverloadText.font = RDString.GetFontDataForLanguage(RDString.language).font;
            OverloadText.fontSize = Mathf.RoundToInt(35 * RDString.GetFontDataForLanguage(RDString.language).fontScale);
            OverloadText.normal.textColor = Color.white;
        }

        private void OnGUI()
        {	
            GUILayout.BeginArea(new Rect(Screen.width - (GaugeTextureOutside.width + 20) * WidthX, 20 * WidthX, GaugeTextureOutside.width * WidthX, GaugeTextureOutside.height * WidthX));
            GUILayout.Label(GaugeTextureOutside, GUILayout.Width(1156 * WidthX));
            GUILayout.EndArea();
            GUI.BeginClip(new Rect(Screen.width - (GaugeTextureOutside.width + 20) * WidthX, 30 * WidthX, GaugeTextureOutside.width * WidthX * OverloadGauge, (GaugeTextureOutside.height - 10) * WidthX));
            GUILayout.Label(GaugeTextureInside, GUILayout.Width(1156 * WidthX));
            GUI.EndClip();
            if (OverloadText.font == RDConstants.data.koreanFont) {
                GUI.Label(new Rect(Screen.width - (GaugeTextureOutside.width + 15) * WidthX, 0 * WidthX, 25, 25), $"Overload: {Math.Min(Mathf.Round(OverloadGauge * 100), 100)}%", OverloadText);
            } else {
                GUI.Label(new Rect(Screen.width - (GaugeTextureOutside.width + 15) * WidthX, 20 * WidthX, 25, 25), $"Overload: {Math.Min(Mathf.Round(OverloadGauge * 100), 100)}%", OverloadText);
            }
        }
    }
}