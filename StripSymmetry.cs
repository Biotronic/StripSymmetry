using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace StripSymmetry
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class StripSymmetry : MonoBehaviour
    {
        private OSD osd = new OSD();
        private HotKey hotkey = new HotKey("triggerWith", "LeftAlt+LeftShift+Mouse0");

        public void Awake()
        {
            hotkey.load();
        }

        public void OnGUI()
        {
            var editor = EditorLogic.fetch;
            if (editor == null) return;
            if (editor.editorScreen != EditorLogic.EditorScreen.Parts) return;

            osd.Update();
        }

        public void Update()
        {
            var editor = EditorLogic.fetch;
            if (editor == null) return;
            if (editor.editorScreen != EditorLogic.EditorScreen.Parts) return;

            if (hotkey.isTriggered())
            {
                var p = GetPartUnderCursor(editor.ship);
                if (p == null)
                {
                    return;
                }
                print(String.Format("({0}).symmetryMode = {1}", p.partInfo.title, p.symmetryMode));
                print(String.Format("({0}).symmetryCounterparts.Count = {1}", p.partInfo.title, p.symmetryCounterparts.Count));
                if (p.symmetryCounterparts.Count == 0)
                {
                    osd.Error("Part has no symmetry: " + p.partInfo.title);
                    return;
                }
                osd.Info("Removing symmetry...");
                RemoveSymmetry(p);
            }
        }

        private static Part GetPartUnderCursor(ShipConstruct ship)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return ship.Parts.Find(p => p.gameObject == hit.transform.gameObject);
            }
            return null;
        }

        private static void RemoveSymmetry(Part symmPart)
        {
            // remove the symmetry of parts that have counterparts outside this branch but leave symmetry for groups wholly within this branch.
            foreach (Part child in symmPart.children)
            {
                foreach (Part otherSymm in child.symmetryCounterparts)
                {
                    if (!symmPart.children.Contains(otherSymm))
                    {
                        RemoveSymmetry(child);
                        break;
                    }
                }
            }
            RemovePartSymmetry(symmPart);
        }

        private static void RemovePartSymmetry(Part p)
        {
            foreach (Part c in p.symmetryCounterparts)
            {
                c.symmetryCounterparts.Clear();
                c.symmetryMode = 0;
            }
            p.symmetryCounterparts.Clear();
            p.symmetryMode = 0;
        }
    }

    public class HotKey
    {
        private List<KeyCode> modifiers;
        private KeyCode trigger;
        private string name;
        private string defaultKey;

        public HotKey(string name, string defaultKey)
        {
            this.name = name;
            this.defaultKey = defaultKey;
        }

        public void load()
        {
            var config = KSP.IO.PluginConfiguration.CreateForType<StripSymmetry>();
            config.load();
            var rawNames = config.GetValue(name, defaultKey);
            var names = rawNames.Split('+');
            config.SetValue(name, rawNames);
            config.save(); // Recreate config in case it's deleted.

            var keys = names.Select(Enums.Parse<KeyCode>).ToList();
            trigger = keys.Last();
            modifiers = keys.SkipLast().ToList();

            foreach (var k in keys)
            {
                Debug.Log("Key: " + k.ToString());
            }
        }

        public bool isTriggered()
        {
            return modifiers.All(Input.GetKey) && Input.GetKeyDown(trigger);
        }
    }

    // Utils
    public class OSD
    {
        private class Message
        {
            public String text;
            public Color color;
            public float hideAt;
        }

        private List<OSD.Message> msgs = new List<OSD.Message>();

        private static GUIStyle CreateStyle(Color color)
        {
            var style = new GUIStyle();
            style.stretchWidth = true;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 16;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = color;
            return style;
        }

        private float calcHeight()
        {
            var style = CreateStyle(Color.white);
            return msgs.Aggregate(.0f, (a, m) => a + style.CalcSize(new GUIContent(m.text)).y);
        }

        public void Update()
        {
            if (msgs.Count == 0) return;
            msgs.RemoveAll(m => Time.time >= m.hideAt);
            var h = calcHeight();
            GUILayout.BeginArea(new Rect(0, Screen.height * 0.1f, Screen.width, h), CreateStyle(Color.white));
            msgs.ForEach(m => GUILayout.Label(m.text, CreateStyle(m.color)));
            GUILayout.EndArea();
        }

        public void Error(String text)
        {
            AddMessage(text, XKCDColors.LightRed);
        }

        public void Warn(String text)
        {
            AddMessage(text, XKCDColors.Yellow);
        }

        public void Success(String text)
        {
            AddMessage(text, XKCDColors.Cerulean);
        }

        public void Info(String text)
        {
            AddMessage(text, XKCDColors.OffWhite);
        }

        public void AddMessage(String text, Color color, float shownFor = 3)
        {
            var msg = new OSD.Message();
            msg.text = text;
            msg.color = color;
            msg.hideAt = Time.time + shownFor;
            msgs.Add(msg);
        }
    }

    public class Log
    {
        public static void Debug(String msg)
        {
            //MonoBehaviour.print("StripSymmetry: " + msg);
        }
    }
}
