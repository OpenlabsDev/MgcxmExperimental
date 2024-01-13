using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terminal.Gui;

namespace Openlabs.Mgcxm.GUI
{
    internal abstract class GuiPanel : Window
    {
        public GuiPanel() { InitialDraw(); }
        protected abstract void InitialDraw();
    }
}
