// - Yuika
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Numerics;

namespace Yuika.YImGui.Internal;

internal class ImGuiPopupData
{
    public uint PopupId { get; set; }
    public ImGuiWindow Window { get; set; }
    public ImGuiWindow BackupNavWindow { get; set; }
    public int ParentNavLayer { get; set; }
    public int OpenFrameCount { get; set; }
    public uint OpenParentId { get; set; }
    public Vector2 OpenPopupPos { get; set; }
    public Vector2 OpenMousePos { get; set; }
}