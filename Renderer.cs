using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using System.Text.Json;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace HellstromReign_Cheat
{
    internal class Renderer : Overlay
    {
        private List<NodeServer> servers = new List<NodeServer>();
        private int selectedServerIndex = -1;
        private bool showAddServerPopup = false;
        private string newServerName = "";
        private string newServerAddress = "";
        private string addServerError = "";
        private readonly string serverFile = "servers.json";

        public Renderer()
        {
            LoadServers();
        }

        private void LoadServers()
        {
            try
            {
                if (File.Exists(serverFile))
                {
                    string json = File.ReadAllText(serverFile);
                    var loaded = JsonSerializer.Deserialize<List<NodeServer>>(json);
                    if (loaded != null)
                        servers = loaded;
                }
                else
                {
                    SaveServers();
                }
            }
            catch { }
        }

        private void SaveServers()
        {
            try
            {
                string json = JsonSerializer.Serialize(servers, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(serverFile, json);
            }
            catch { }
        }

        protected override void Render()
        {
            var io = ImGui.GetIO();
            var screenSize = io.DisplaySize;
            var screenResolution = new Vector2(screenSize.X, screenSize.Y);

            // Tema renkleri
            var bgBlack = new Vector4(0.07f, 0.09f, 0.13f, 1.0f);
            var cardBlue = new Vector4(0.13f, 0.18f, 0.28f, 1.0f);
            var accentBlue = new Vector4(0.23f, 0.56f, 0.95f, 1.0f);
            var accentBlueLight = new Vector4(0.35f, 0.65f, 1.0f, 1.0f);
            var textGray = new Vector4(0.85f, 0.90f, 1.0f, 1.0f);
            var textDark = new Vector4(0.55f, 0.60f, 0.70f, 1.0f);

            ImGui.PushStyleColor(ImGuiCol.WindowBg, bgBlack);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 12f);
            if (ImGui.Begin("Node Server Manager", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBringToFrontOnFocus))
            {
                ImGui.PopStyleVar();
                ImGui.PopStyleColor();
                ImGui.Columns(2, "MainColumns", false);
                // Sidebar - Server List
                ImGui.BeginChild("Sidebar", new Vector2(240, 0));
                ImGui.PushStyleColor(ImGuiCol.ChildBg, cardBlue);
                ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 10f);
                ImGui.SetCursorPosY(18);
                ImGui.TextColored(accentBlue, "Node Servers");
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
                for (int i = 0; i < servers.Count; i++)
                {
                    var server = servers[i];
                    Vector4 color = server.Status == ServerStatus.Running ? accentBlueLight : new Vector4(1.0f, 0.3f, 0.3f, 1.0f);
                    ImGui.PushStyleColor(ImGuiCol.Text, color);
                    ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 8f);
                    if (ImGui.Selectable($"{server.Name}##{i}", selectedServerIndex == i, ImGuiSelectableFlags.None, new Vector2(200, 36)))
                        selectedServerIndex = i;
                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor();
                    ImGui.Spacing();
                }
                ImGui.Spacing();
                ImGui.Spacing();
                ImGui.PushStyleColor(ImGuiCol.Button, accentBlue);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, accentBlueLight);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, accentBlueLight);
                ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 8f);
                if (ImGui.Button("+ Add Server", new Vector2(200, 36)))
                {
                    showAddServerPopup = true;
                    newServerName = "";
                    newServerAddress = "";
                    addServerError = "";
                }
                ImGui.PopStyleVar();
                ImGui.PopStyleColor(3);
                ImGui.PopStyleVar();
                ImGui.PopStyleColor();
                ImGui.EndChild();
                ImGui.NextColumn();
                // Main Content - Server Details
                ImGui.BeginChild("MainContent", new Vector2(0, 0));
                ImGui.SetCursorPosY(30);
                ImGui.PushStyleColor(ImGuiCol.ChildBg, cardBlue);
                ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 12f);
                ImGui.BeginChild("ServerDetails", new Vector2(380, 420));
                if (selectedServerIndex >= 0 && selectedServerIndex < servers.Count)
                {
                    var server = servers[selectedServerIndex];
                    ImGui.TextColored(accentBlue, $"{server.Name}");
                    ImGui.Spacing(); ImGui.Spacing();
                    ImGui.TextColored(textGray, $"Address: {server.Address}");
                    ImGui.Spacing();
                    ImGui.TextColored(textGray, $"Status: {(server.Status == ServerStatus.Running ? "Running" : "Stopped")}");
                    ImGui.Spacing(); ImGui.Spacing();

                    if (server.Status == ServerStatus.Stopped)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Button, accentBlue);
                        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, accentBlueLight);
                        ImGui.PushStyleColor(ImGuiCol.ButtonActive, accentBlueLight);
                        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 8f);
                        if (ImGui.Button("Start Server", new Vector2(200, 48)))
                        {
                            server.Status = ServerStatus.Running;
                            SaveServers();
                        }
                        ImGui.PopStyleVar();
                        ImGui.PopStyleColor(3);
                    }
                    else
                    {
                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.3f, 0.5f, 1.0f));
                        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, accentBlueLight);
                        ImGui.PushStyleColor(ImGuiCol.ButtonActive, accentBlueLight);
                        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 8f);
                        if (ImGui.Button("Stop Server", new Vector2(200, 48)))
                        {
                            server.Status = ServerStatus.Stopped;
                            SaveServers();
                        }
                        ImGui.PopStyleVar();
                        ImGui.PopStyleColor(3);
                    }
                    ImGui.SameLine();
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1.0f, 0.2f, 0.2f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1.0f, 0.4f, 0.4f, 1.0f));
                    ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1.0f, 0.6f, 0.6f, 1.0f));
                    ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 8f);
                    if (ImGui.Button("Delete Server", new Vector2(200, 48)))
                    {
                        servers.RemoveAt(selectedServerIndex);
                        selectedServerIndex = -1;
                        SaveServers();
                    }
                    ImGui.PopStyleVar();
                    ImGui.PopStyleColor(3);
                }
                else
                {
                    ImGui.TextColored(textDark, "Select a server to view details.");
                }
                ImGui.EndChild();
                ImGui.PopStyleVar();
                ImGui.PopStyleColor();
                ImGui.EndChild();
                // Add Server Popup
                if (showAddServerPopup)
                {
                    ImGui.OpenPopup("Add Server");
                    if (ImGui.BeginPopupModal("Add Server", ref showAddServerPopup, ImGuiWindowFlags.AlwaysAutoResize))
                    {
                        ImGui.InputText("Server Name", ref newServerName, 100);
                        ImGui.InputText("Server Address", ref newServerAddress, 100);
                        if (!string.IsNullOrEmpty(addServerError))
                        {
                            ImGui.TextColored(new Vector4(1, 0.2f, 0.2f, 1), addServerError);
                        }
                        ImGui.Spacing();
                        ImGui.PushStyleColor(ImGuiCol.Button, accentBlue);
                        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, accentBlueLight);
                        ImGui.PushStyleColor(ImGuiCol.ButtonActive, accentBlueLight);
                        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 8f);
                        if (ImGui.Button("Add", new Vector2(120, 32)))
                        {
                            if (string.IsNullOrWhiteSpace(newServerName) || string.IsNullOrWhiteSpace(newServerAddress))
                            {
                                addServerError = "Name and address required!";
                            }
                            else
                            {
                                servers.Add(new NodeServer { Name = newServerName, Address = newServerAddress, Status = ServerStatus.Stopped });
                                SaveServers();
                                showAddServerPopup = false;
                            }
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("Cancel", new Vector2(120, 32)))
                        {
                            showAddServerPopup = false;
                        }
                        ImGui.PopStyleVar();
                        ImGui.PopStyleColor(3);
                        ImGui.EndPopup();
                    }
                }
            }
            ImGui.End();
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
        Running,
        Stopped
    }
}
