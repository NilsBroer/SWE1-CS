﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWebServer.Plugins
{
    /// <summary>
    /// Attribute which we put in every own Plugin our PluginManager knows which plugins to add!
    /// Attention: No need to apply to Plugins you drag into the Plugins Folder
    /// </summary>
    public class marked : Attribute
    {

    }
}