// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Yuika.YImGui;

[StructLayout(LayoutKind.Sequential)]
public struct Abgr32
{
    public readonly byte A;
    public readonly byte B;
    public readonly byte G;
    public readonly byte R;

    public int Raw => Unsafe.As<Abgr32, int>(ref this);

    public Abgr32(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public static implicit operator Color(Abgr32 color) => 
        Color.FromArgb(color.A, color.R, color.G, color.B);

    public static implicit operator Abgr32(Color color) =>
        new Abgr32(color.R, color.G, color.B, color.A);
}