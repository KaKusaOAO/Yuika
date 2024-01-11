// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;
using System.Numerics;

namespace Yuika.YImGui.Internal;

#if USE_DOCKING

internal class ImGuiDockNode 
{
    public uint Id { get; set; }
    public ImGuiDockNodeFlags SharedFlags { get; set; }
    public ImGuiDockNodeFlags LocalFlags { get; set; }
    public ImGuiDockNodeFlags LocalFlagsInWindows { get; set; }
    public ImGuiDockNodeFlags MergedFlags { get; set; }
    public Vector2 Position { get; set; }
    public SizeF Size { get; set; }
    public ImGuiWindowClass WindowClass { get; set; }
    
    public ImGuiWindow? HostWindow { get; set; }
    public ImGuiWindow? VisibleWindow { get; set; }
    public ImGuiDockNode? CentralNode { get; set; }
    public ImGuiDockNode? OnlyNodeWithWindows { get; set; }
    public int CountNodeWithWindows { get; set; }
    public int LastFrameAlive { get; set; }
    public int LastFrameActive { get; set; }
    public int LastFrameFocused { get; set; }
}

#endif