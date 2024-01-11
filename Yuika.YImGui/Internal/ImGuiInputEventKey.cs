// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiInputEventKey : ImGuiInputEvent
{
    public ImGuiKey Key { get; set; }
    public bool Down { get; set; }
    public float AnalogValue { get; set; }
}