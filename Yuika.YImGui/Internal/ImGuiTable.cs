// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiTable
{
    public uint Id { get; set; }
    public ImGuiTableFlags Flags { get; set; }
    public object? RawData { get; set; }
    public ImGuiTableTempData TempData { get; set; }
    public float RowPosY1 { get; set; }
    public float RowPosY2 { get; set; }
    public float BorderX1 { get; set; }
    public float BorderX2 { get; set; }
    public bool IsLayoutLocked { get; set; }
    public bool IsInsideRow { get; set; }
    public bool IsInitializing { get; set; }
    public bool IsSortSpecsDirty { get; set; }
    public bool IsUsingHeaders { get; set; }
    public bool IsContextPopupOpen { get; set; }
}