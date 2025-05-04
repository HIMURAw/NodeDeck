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
        protected override void Render()
        {
            if (ImGui.Begin("NodeDeck"))
            {

                ImGui.Text("Active Node Server");





            }
            ImGui.End();
        }
    }
}
