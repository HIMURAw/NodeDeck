using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace HellstromReign_Cheat
{
    internal class Renderer : Overlay
    {
        public bool WebPanel = false;

        protected override void Render()
        {
            ImGui.Begin("NodeDeck");
            ImGui.Checkbox("Test", ref WebPanel);
            ImGui.End();
        }
    }
}
