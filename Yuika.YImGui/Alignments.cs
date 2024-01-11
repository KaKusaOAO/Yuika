// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Numerics;

namespace Yuika.YImGui;

public static class Alignments
{
    public static Vector2 TopLeft { get; } = Vector2.Zero;
    public static Vector2 TopCenter { get; } = new Vector2(0.5f, 0);
    public static Vector2 TopRight { get; } = Vector2.UnitX;

    public static Vector2 MiddleLeft { get; } = new Vector2(0, 0.5f);
    public static Vector2 MiddleCenter { get; } = new Vector2(0.5f, 0.5f);
    public static Vector2 MiddleRight { get; } = new Vector2(1, 0.5f);
    
    public static Vector2 BottomLeft { get; } = Vector2.UnitY;
    public static Vector2 BottomCenter { get; } = new Vector2(0.5f, 1);
    public static Vector2 BottomRight { get; } = Vector2.One;
}