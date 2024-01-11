// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

public class ImFont
{
    public List<float> IndexAdvanceX { get; set; } = new List<float>();
    public float FallbackAdvanceX { get; set; }
    public float FontSize { get; set; }

    public List<char> IndexLookup { get; set; } = new List<char>();
    public List<ImFontGlyph> Glyphs { get; set; } = new List<ImFontGlyph>();
    public ImFontGlyph? FallbackGlyph { get; set; }

    public ImFontAtlas ContainerAtlas { get; set; }
    public List<ImFontConfig> ConfigData { get; set; } = new List<ImFontConfig>();
    public char FallbackChar { get; set; }
    public char EllipsisChar { get; set; }
    public short EllipsisCharCount { get; set; }
    
}