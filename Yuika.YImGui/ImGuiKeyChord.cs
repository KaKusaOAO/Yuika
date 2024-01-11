// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public struct ImGuiKeyChord
{
    public readonly int Value;

    public ImGuiKeyChord(int val) => Value = val;

    public static implicit operator int(ImGuiKeyChord val) => val.Value;
    public static implicit operator ImGuiKeyChord(int val) => new(val);
}