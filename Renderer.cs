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
        private List<NodeServer> servers = new List<NodeServer>();
        private int selectedServerIndex = -1;
        private string newServerName = "";
        private string newServerAddress = "";
        private bool showAddServerPopup = false;

        public Renderer()
        {
            servers.Add(new NodeServer { Name = "Pixeldev1", Address = "192.168.1.100", Status = ServerStatus.Online });
            servers.Add(new NodeServer { Name = "Pixeldev2", Address = "192.168.1.101", Status = ServerStatus.Offline });
            servers.Add(new NodeServer { Name = "Pixeldev3", Address = "192.168.1.102", Status = ServerStatus.Maintenance });
        }

        protected override void Render()
        {
            var io = ImGui.GetIO();
            var screenSize = io.DisplaySize;
            var screenResolution = new System.Numerics.Vector2(screenSize.X, screenSize.Y);

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 0));
            ImGui.SetNextWindowSize(screenResolution);

            if (ImGui.Begin("NodeDeck",
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoBringToFrontOnFocus))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.2f, 0.6f, 1.0f, 1.0f));
                ImGui.SetCursorPosX((ImGui.GetWindowWidth() - ImGui.CalcTextSize("Node Server Manager").X) * 0.5f);
                ImGui.Text("Node Server Manager");
                ImGui.PopStyleColor();
                ImGui.Separator();

                ImGui.Columns(2, "ServerColumns", true);

                ImGui.BeginChild("ServerList", new System.Numerics.Vector2(ImGui.GetColumnWidth(), 0));

                if (ImGui.Button("+ Add New Server", new System.Numerics.Vector2(ImGui.GetColumnWidth(), 30)))
                {
                    showAddServerPopup = true;
                }
                ImGui.Separator();

                for (int i = 0; i < servers.Count; i++)
                {
                    var server = servers[i];
                    bool isSelected = (selectedServerIndex == i);

                    ImGui.PushStyleColor(ImGuiCol.Text, GetStatusColor(server.Status));

                    if (ImGui.Selectable($"{server.Name}##{i}", isSelected))
                    {
                        selectedServerIndex = i;
                    }

                    ImGui.PopStyleColor();
                }
                ImGui.EndChild();

                ImGui.NextColumn();

                ImGui.BeginChild("ServerDetails", new System.Numerics.Vector2(0, 0));
                if (selectedServerIndex >= 0 && selectedServerIndex < servers.Count)
                {
                    var selectedServer = servers[selectedServerIndex];

                    ImGui.Text($"Server Name: {selectedServer.Name}");
                    ImGui.Text($"Address: {selectedServer.Address}");
                    ImGui.Text($"Status: {selectedServer.Status}");

                    ImGui.Spacing();

                    if (ImGui.Button("Start Server", new System.Numerics.Vector2(120, 30)))
                    {
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Stop Server", new System.Numerics.Vector2(120, 30)))
                    {
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Delete Server"))
                    {
                        servers.RemoveAt(selectedServerIndex);
                        selectedServerIndex = -1;
                    }
                }
                else
                {
                    ImGui.Text("Select a server to view details");
                }
                ImGui.EndChild();

                ImGui.Columns(1);

                if (showAddServerPopup)
                {
                    ImGui.OpenPopup("Add New Server");
                    if (ImGui.BeginPopupModal("Add New Server", ref showAddServerPopup))
                    {
                        ImGui.InputText("Server Name", ref newServerName, 100);
                        ImGui.InputText("Server Address", ref newServerAddress, 100);

                        if (ImGui.Button("Add"))
                        {
                            if (!string.IsNullOrEmpty(newServerName) && !string.IsNullOrEmpty(newServerAddress))
                            {
                                servers.Add(new NodeServer
                                {
                                    Name = newServerName,
                                    Address = newServerAddress,
                                    Status = ServerStatus.Offline
                                });
                                newServerName = "";
                                newServerAddress = "";
                                showAddServerPopup = false;
                            }
                        }

                        ImGui.SameLine();
                        if (ImGui.Button("Cancel"))
                        {
                            showAddServerPopup = false;
                        }

                        ImGui.EndPopup();
                    }
                }
            }
            ImGui.End();
        }

        private System.Numerics.Vector4 GetStatusColor(ServerStatus status)
        {
            return status switch
            {
                ServerStatus.Online => new System.Numerics.Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                ServerStatus.Offline => new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                ServerStatus.Maintenance => new System.Numerics.Vector4(1.0f, 0.5f, 0.0f, 1.0f),
                _ => new System.Numerics.Vector4(1.0f, 1.0f, 1.0f, 1.0f)
            };
        }
    }

    public class NodeServer
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public ServerStatus Status { get; set; }
    }

    public enum ServerStatus
    {
        Online,
        Offline,
        Maintenance
    }
}
