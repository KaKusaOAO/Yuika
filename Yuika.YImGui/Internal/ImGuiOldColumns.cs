// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;

namespace Yuika.YImGui.Internal;

internal class ImGuiOldColumns
{
    public uint Id { get; set; }
    public ImGuiOldColumnFlags Flags { get; set; }
    public bool IsFirstFrame { get; set; }
    public bool IsBeingResized { get; set; }
    public int Current { get; set; }
    public int Count { get; set; }
    public float OffMinX { get; set; }
    public float OffMaxX { get; set; }
    public float LineMinY { get; set; }
    public float LineMaxY { get; set; }
    public float HostCursorPosY { get; set; }
    public float HostCursorMaxPosX { get; set; }
    public Rectangle HostInitialClipRect { get; set; }
    public Rectangle HostBackupClipRect { get; set; }
    public Rectangle HostBackupParentWorkRect { get; set; }
    public List<ImGuiOldColumnData> Columns { get; set; } = new List<ImGuiOldColumnData>();
    public ImDrawListSplitter Splitter { get; set; }
}