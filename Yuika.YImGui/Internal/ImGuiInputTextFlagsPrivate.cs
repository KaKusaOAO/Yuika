// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

[Flags]
internal enum ImGuiInputTextFlagsPrivate 
{
    Multiline    = 1 << 26,
    NoMarkEdited = 1 << 27,
    MergedItem   = 1 << 28,
}
