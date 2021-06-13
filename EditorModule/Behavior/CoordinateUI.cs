using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RandomTweaksEditorModule.Command;
using UnityModManagerNet;

namespace RandomTweaksEditorModule.Behavior
{
    public class CoordinateUI : MonoBehaviour
    {
        public static GUIStyle Coord = new GUIStyle();
        public static GUIStyle Coordx2 = new GUIStyle();
        public static GUIStyle InputStyle = new GUIStyle();
        public static GUIStyle OutputStyle = new GUIStyle();

        public static Texture2D ConsoleInput = new Texture2D(2 ,2);
        public static Texture2D ConsoleOutput = new Texture2D(2 ,2);

        public static string Command = "";
        public static string CommandOutput;
        private int Outputs = 0;
        
        bool Console = false;

        public void Start()
        {
            Coord.font = RDString.GetFontDataForLanguage(RDString.language).font;
            Coord.fontSize = Mathf.RoundToInt(35 * RDString.GetFontDataForLanguage(RDString.language).fontScale);
            Coord.normal.textColor = UnityEngine.Color.white;

            Coordx2.font = RDString.GetFontDataForLanguage(RDString.language).font;
            Coordx2.fontSize = Mathf.RoundToInt(45 * RDString.GetFontDataForLanguage(RDString.language).fontScale);
            Coordx2.normal.textColor = UnityEngine.Color.green;

            InputStyle.fixedWidth = ConsoleInput.width;
            InputStyle.fixedHeight = ConsoleInput.height;
            //.font = RDConstants.data.chineseFont;
            InputStyle.fontSize = Mathf.RoundToInt(30);
            InputStyle.alignment = TextAnchor.MiddleLeft;
            InputStyle.normal.textColor = UnityEngine.Color.white;
            InputStyle.richText = true;

            //OutputStyle.font = RDConstants.data.chineseFont;
            OutputStyle.fontSize = Mathf.RoundToInt(30);
            OutputStyle.alignment = TextAnchor.UpperLeft;
            OutputStyle.normal.textColor = new Color(1, 1, 1, 0.75f);
            OutputStyle.richText = true;
            OutputStyle.border = new RectOffset(1, 1, 1, 1);
        }

        public static object ParseArg(string arg)
        {
            float resF;
            int resI;
            var argL = arg.ToLower();
            if (argL == "true") return true;
            if (argL == "false") return false;
            if (float.TryParse(argL, out resF)) return resF;
            if (int.TryParse(argL, out resI)) return resI;
            return arg;
        }
        
        public static string RunCommand(string Command)
        {
            var commandStrings = new List<string>(Command.Split(' '));
            string commandName = commandStrings[0];
            commandStrings.RemoveAt(0);
            var args = new List<object>();
            foreach (var arg in commandStrings)
            {
                args.Add(ParseArg(arg));
            }
            
            MethodInfo command = null;
            foreach (var method in typeof(Commands).GetMethods())
            {
                if (method.Name == commandName.ToUpper())
                {
                    command = method;
                    break;
                }
            }

            if (command == null) return $"<color=#ff0000>Command {commandName} not found</color>";
            try
            {
                return $"<color=#00ff00>{command.Invoke(null, args.ToArray())}</color>";
            }
            catch (Exception E)
            {
                return $"<color=#ff0000>Syntax Error ({E.GetType().Name})</color>";
                throw E;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote) && Input.GetKeyDown(KeyCode.LeftShift))
            {
                Console = true;
            }
        }

        private void OnGUI()
        {
            double CameraPositionX = Math.Round(Camera.current.transform.position.x / 1.5f, 2);
            double CameraPositionY = Math.Round(Camera.current.transform.position.y / 1.5f, 2);
            float CameraSize = Camera.current.orthographicSize;
            double MousePositionX = Math.Round((Input.mousePosition.x - Screen.width / 2.0f) / 162 * 1920 / Screen.width * CameraSize / 5, 2);
            double MousePositionY = Math.Round((Input.mousePosition.y - Screen.height / 2.0f) / 162 * 1080 / Screen.height * CameraSize / 5, 2);
            Vector2 SelectedPos = Vector2.zero;
            Vector2 SelectedPosDisplay = Vector2.zero;
            bool isSelected = true;
            if (scnEditor.instance.selectedFloor != null)
            {
                isSelected = false;
                SelectedPos = scnEditor.instance.selectedFloor.gameObject.transform.position / 1.5f;
                SelectedPosDisplay = SelectedPos;
                GUILayout.BeginArea(new Rect(20, 10, 400, 160));
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    SelectedPos = Vector2.zero;
                    GUILayout.Label("[Absolute Position Mode]", Coordx2);
                }
                else
                {
                    GUILayout.Label("[Relative Position Mode]", Coordx2);
                }
                
                GUILayout.EndArea();
            }
            GUILayout.BeginArea(new Rect(Screen.width - 320, 0, 320, 360));
            GUILayout.BeginArea(new Rect(0, 0, 160, 200));
            GUILayout.Label($"Camera\nX: {Math.Round(CameraPositionX - SelectedPos.x, 2)} tile\nY: {Math.Round(CameraPositionY - SelectedPos.y, 2)} tile\nScale: {Math.Round(CameraSize * 20)}%", Coord);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(160, 0, 160, 150));
            GUILayout.Label($"Mouse\nX: {Math.Round(CameraPositionX + MousePositionX - SelectedPos.x, 2)} tile\nY: {Math.Round(CameraPositionY + MousePositionY - SelectedPos.y, 2)} tile", Coord);
            GUILayout.EndArea();
            if (!isSelected)
            {
                GUILayout.BeginArea(new Rect(0, 220, 300, 160));
                GUILayout.Label($"Selected Tile\nX: {Math.Round(SelectedPosDisplay.x, 2)}    Y: {Math.Round(SelectedPosDisplay.y, 2)} tile", Coord);
                GUILayout.EndArea();
            }

            GUILayout.EndArea();

            if (Input.GetKeyDown(KeyCode.Escape)) Console = false;
            if (Console)
            {
                GUILayout.BeginArea(new Rect(0, Screen.height - 77, ConsoleOutput.width, 500));
                GUILayout.Label(CommandOutput, OutputStyle);
                GUILayout.EndArea();
                GUILayout.BeginArea(new Rect(0, Screen.height - 77, ConsoleOutput.width, 500));
                GUILayout.Label(ConsoleOutput);
                GUILayout.EndArea();
               
                GUILayout.BeginArea(new Rect(0, Screen.height - 38, 1920, 77));
                GUILayout.Label(ConsoleInput);
                GUILayout.EndArea();
                GUILayout.BeginArea(new Rect(45, Screen.height - 40, 1920, 45));
                Command = GUILayout.TextArea(Command.PadRight(0, ' '), InputStyle).Replace("  ", "");
                if (Command.Contains("\n"))
                {
                    Command = Command.Replace("\n", "");
                    CommandOutput += "\n";
                    CommandOutput = RunCommand(Command);
                    Outputs += 1;
                    Command = "";
                }
                GUILayout.EndArea();
            }
        }
    }
}