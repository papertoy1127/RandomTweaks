using System.Reflection;
using ADOLib;
using ADOLib.Misc;
using UnityEngine;
using HarmonyLib;

namespace RandomTweaksEditorModule.Command
{
    public class Commands
    {
        public static string MOVETO(float x, float y = float.MinValue)
        {
            Camera.current.transform.LocalMoveXY(x * 1.5f, y * 1.5f);
            return $"Moved to position x: {x} y: {y}";
        }

        public static string MOVETOCENTER()
        {
            Camera.current.transform.LocalMoveXY(0, 0);
            return $"Moved to center";
        }

        public static string CREATEFLOOR(float angle)
        {
            scnEditor.instance.invoke<object>("CreateFloorWithCharOrAngle", new object[] {angle, '?', true, false});
            return "Created Floor";
        }

        public static string GET(string memberName)
        {
            var Fields = memberName.Split('.');
            var toGet = Assembly.GetAssembly(typeof(scrController)).GetType(Fields[0]);
            if (toGet == null)
            {
                return $"Type {Fields[0]} not found";
            }

            object value = null;
            var forCount = 0;
            foreach (var f in Fields)
            {
                if (forCount == 0)
                {
                    forCount++;
                    continue;
                }

                MemberInfo member = toGet.GetField(f, AccessTools.all);
                if (member == null) member = toGet.GetProperty(f, AccessTools.all);
                if (member == null)
                {
                    return $"<color=#ff0000>Field {member} not found</color>";
                }

                if (member is FieldInfo field)
                {
                    value = field.GetValue(value);
                    if (value == null)
                    {
                        if (forCount + 1 == Fields.Length) return "null";
                        return "<color=#ff0000>Null Reference</color>";
                    }

                    toGet = value.GetType();
                    continue;
                }

                if (member is PropertyInfo property)
                {
                    value = property.GetValue(value);
                    if (value == null)
                    {
                        if (forCount + 1 == Fields.Length) return "null";
                        return "<color=#ff0000>Null Reference</color>";
                    }

                    toGet = value.GetType();
                }

                forCount++;
            }

            if (forCount == 1) return $"Type: {memberName}";
            value ??= "null";
            return $"{value}";
        }
    }
}