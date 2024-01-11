// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

using IdType = IntPtr;

public struct ImTextureID
{
    public readonly IdType Value;

    public ImTextureID(IdType value) => Value = value;

    public static implicit operator IdType(ImTextureID id) => id.Value;
    public static implicit operator ImTextureID(IdType id) => new(id);
}