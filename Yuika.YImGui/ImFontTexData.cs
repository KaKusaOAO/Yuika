// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public unsafe class ImFontTexData
{
    public byte* PixelsBuffer { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int BytesPerPixel { get; init; }
    public int BufferSize => Width * Height * BytesPerPixel;
}