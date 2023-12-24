using System.Numerics;

namespace Yuika.YImGui;

public class ImFontConfig 
{
    // public IntPtr FontData { get; set; }
    // public int FontDataSize { get; set; }        

    public byte[] FontData { get; set; } = Array.Empty<byte>();
    public bool FontDataOwnedByAtlas { get; set; } = true;
    public int FontNo { get; set; }
    public float SizePixels { get; set; }
    public int OversampleH { get; set; } = 2;
    public int OversampleV { get; set; } = 1;
    public bool PixelSnapH { get; set; }
    public Vector2 GlyphExtraSpacing { get; set; }
    public Vector2 GlyphOffset { get; set; }
    public ImGlyphRange[] GlyphRanges { get; set; } = Array.Empty<ImGlyphRange>();
    public float GlyphMinAdvanceX { get; set; }   
    public float GlyphMaxAdvanceX { get; set; } = float.MaxValue;
    public bool MergeMode { get; set; }           
    public uint FontBuilderFlags { get; set; }    
    public float RasterizerMultiply { get; set; } = 1; 
    public float RasterizerDensity { get; set; } = 1;  

    /// <summary>
    /// Explicitly specify unicode codepoint of ellipsis character. 
    /// When fonts are being merged first specified ellipsis will be used.
    /// </summary>
    public char? EllipsisChar { get; set; }

    internal string Name { get; set; }
    internal ImFont DstFont { get; set; }
}