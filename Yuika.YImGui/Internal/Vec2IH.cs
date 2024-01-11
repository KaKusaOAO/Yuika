// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal struct Vec2IH 
{
    public short X;
    public short Y;

    public Vec2IH(short x, short y) 
    {
        X = x;
        Y = y;
    }

    public static readonly Vec2IH Zero = new Vec2IH();
    public static readonly Vec2IH One = new Vec2IH(1, 1);
}