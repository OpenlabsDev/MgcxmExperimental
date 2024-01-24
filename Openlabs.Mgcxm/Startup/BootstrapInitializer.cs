using Openlabs.Mgcxm.GUI;
using Openlabs.Mgcxm.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terminal.Gui;

namespace Openlabs.Mgcxm.Startup
{
    internal static class BootstrapInitializer
    {
        public static void Initialize()
        {
            if (MgcxmConfiguration.CurrentBootstrapConfiguration.useGui)
            {
                Application.Init();
                Colors.Base.Normal = Application.Driver.MakeAttribute(Terminal.Gui.Color.Gray, Terminal.Gui.Color.Black);

                Application.Run<MgcxmOverviewGui>();
                return;
            }
        }
    }
}
