using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StripSymmetry
{
    class Config
    {
        static KSP.IO.PluginConfiguration config;

        public static KSP.IO.PluginConfiguration Value
        {
            get
            {
                if (config == null)
                {
                    config = KSP.IO.PluginConfiguration.CreateForType<StripSymmetry>();
                    config.load();
                    config.SetValue("triggerWith", "LeftAlt+LeftShift+Mouse0");
                    config.save();
                }
                return config;
            }
        }
    }
}
