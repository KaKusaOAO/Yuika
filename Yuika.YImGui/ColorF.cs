// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Yuika.YImGui;

public readonly struct ColorF
{
    public readonly float R;
    public readonly float G;
    public readonly float B;
    public readonly float A;

    public static ColorF White { get; } = new ColorF(1, 1, 1);
    public static ColorF Black { get; } = new ColorF(0, 0, 0);

    public ColorF(float r, float g, float b, float a = 1)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public ColorF(double r, double g, double b, double a = 1)
    {
        R = (float) r;
        G = (float) g;
        B = (float) b;
        A = (float) a;
    }

    public static implicit operator Vector4(ColorF color) => Unsafe.As<ColorF, Vector4>(ref color);
    public static implicit operator ColorF(Vector4 v) => Unsafe.As<Vector4, ColorF>(ref v);
    
    public static implicit operator Color(ColorF color) => 
        Color.FromArgb((int) (color.A * 255), (int) (color.R * 255), (int) (color.G * 255), (int) (color.B * 255));

    public static implicit operator ColorF(Color color) =>
        new ColorF(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

    public static ColorF operator +(ColorF a, ColorF b) => new ColorF(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
    public static ColorF operator -(ColorF a, ColorF b) => new ColorF(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
    public static ColorF operator *(ColorF a, ColorF b) => new ColorF(a.R * b.R, a.G * b.G, a.B * b.B, a.A * b.A);
    public static ColorF operator /(ColorF a, ColorF b) => new ColorF(a.R / b.R, a.G / b.G, a.B / b.B, a.A / b.A);
}