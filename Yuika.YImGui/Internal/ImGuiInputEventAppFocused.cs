﻿// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiInputEventAppFocused : ImGuiInputEvent
{
    public bool Focused { get; set; }
}