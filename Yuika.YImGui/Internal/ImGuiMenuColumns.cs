// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiMenuColumns
{
    public uint TotalWidth { get; set; }
    public uint NextTotalWidth { get; set; }
    public ushort Spacing { get; set; }
    public ushort OffsetIcon { get; set; }
    public ushort OffsetLabel { get; set; }
    public ushort OffsetShortcut { get; set; }
    public ushort OffsetMark { get; set; }
    public ushort[] Widths { get; set; } = new ushort[4];

    public void Update(float spacing, bool windowReappearing) => throw new NotImplementedException();
    public float DeclColumns(float wIcon, float wLabel, float wShortcut, float wMark) 
        => throw new NotImplementedException();
    public float CalcNextTotalWidth(bool updateOffsets) => throw new NotImplementedException();
}