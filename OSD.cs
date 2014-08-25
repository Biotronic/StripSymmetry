using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StripSymmetry
{
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
}
