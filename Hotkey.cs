using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StripSymmetry
{
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
            var names = Config.Value.GetValue(name, defaultKey).Split('+');

            var keys = names.Select(Enums.Parse<KeyCode>).ToList();
            trigger = keys.Last();
            modifiers = keys.SkipLast().ToList();
        }

        public bool Triggered
        {
            get
            {
                return modifiers.All(Input.GetKey) && Input.GetKeyDown(trigger);
            }
        }
    }
}
