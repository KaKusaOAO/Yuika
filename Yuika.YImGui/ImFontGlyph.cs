// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public class ImFontGlyph
{
    public bool Colored { get; set; }
    public bool Visible { get; set; }
    public int Codepoint { get; set; }
    public float AdvanceX { get; set; }
    public float X0 { get; set; }
    public float Y0 { get; set; }
    public float X1 { get; set; }
    public float Y1 { get; set; }
    public float U0 { get; set; }
    public float V0 { get; set; }
    public float U1 { get; set; }
    public float V1 { get; set; }
}