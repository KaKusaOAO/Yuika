// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Diagnostics;
using System.Numerics;

namespace Yuika.YImGui;

public class ImFontAtlas
{
    public ImFontAtlasFlags Flags { get; set; }
    public ImTextureID TexID { get; set; }
    public int TexDesiredWidth { get; set; }
    public int TexGlyphPadding { get; set; }
    public bool Locked { get; set; }
    public object? UserData { get; set; }
    
    public bool TexReady { get; set; }
    public bool TexPixelsUseColors { get; set; }
    public byte[]? TexPixelsAlpha8 { get; set; }

    public int[]? TexPixelsRGBA32 { get; set; }
    public int TexWidth { get; set; }
    public int TexHeight { get; set; }
    public Vector2 TexUvScale { get; set; }
    public Vector2 TexUvWhitePixel { get; set; }
    public List<ImFont> Fonts { get; set; } = new List<ImFont>();
    public List<ImFontConfig> ConfigData { get; set; } = new List<ImFontConfig>();

    private void ThrowIfLocked()
    {
        if (Locked)
        {
            throw new ImGuiException(
                $"Cannot modify a locked {nameof(ImFontAtlas)} between " +
                $"{nameof(ImGui.NewFrame)}() and {nameof(ImGui.EndFrame)}/{nameof(ImGui.Render)}()!");
        }
    }

    public ImFont AddFont(ImFontConfig fontConfig)
    {
        ThrowIfLocked();

        // Debug.Assert();

        throw new NotImplementedException();
    }

    public ImFont AddFontDefault(ImFontConfig? fontConfig = null)
    {
        throw new NotImplementedException();
    }

    public ImFont AddFontFromStreamTTF(Stream stream, float sizePixels, ImFontConfig fontConfig, char[] glyphRanges)
    {
        MemoryStream dst = new MemoryStream();
        stream.CopyTo(dst);
        return AddFontFromMemoryTTF(dst.GetBuffer(), sizePixels, fontConfig, glyphRanges);
    }

    public ImFont AddFontFromMemoryTTF(byte[] fontData, float sizePixels, ImFontConfig fontConfig, char[] glyphRanges)
    {
        throw new NotImplementedException();
    }

    public void ClearInputData()
    {
        ThrowIfLocked();
        
        foreach (ImFontConfig config in ConfigData)
        {
            if (config.FontData != null && config.FontDataOwnedByAtlas)
            {
                config.FontData = null;
            }    
        }

        foreach (ImFont font in Fonts)
        {
            // What are those pointer comparing?
            throw new NotImplementedException();
        }

        ConfigData.Clear();
        throw new NotImplementedException();
    }

    public void ClearTexData()
    {
        ThrowIfLocked();
        TexPixelsAlpha8 = null;
        TexPixelsRGBA32 = null;
        TexPixelsUseColors = false;
    }

    public void ClearFonts()
    {
        ThrowIfLocked();
        Fonts.Clear();
        TexReady = false;
    }

    public void Clear()
    {
        ClearInputData();
        ClearTexData();
        ClearFonts();
    }

    public bool Build()
    {
        ThrowIfLocked();
        throw new NotImplementedException();
    }

    public unsafe ImFontTexData GetTexDataAsAlpha8()
    {
        if (TexPixelsAlpha8 == null) Build();

        fixed (byte* buf = TexPixelsAlpha8)
        {
            return new ImFontTexData
            {
                PixelsBuffer = buf,
                Width = TexWidth,
                Height = TexHeight,
                BytesPerPixel = 1
            };
        }
    }

    public unsafe ImFontTexData GetTexDataAsRGBA32()
    {
        if (TexPixelsRGBA32 == null)
        {
            ImFontTexData data = GetTexDataAsAlpha8();
            if (data.PixelsBuffer != null)
            {
                TexPixelsRGBA32 = new int[TexWidth * TexHeight];

                byte* src = data.PixelsBuffer;
                fixed (int* ptr = TexPixelsRGBA32)
                {
                    int* dst = ptr;
                    
                    for (int n = TexWidth * TexHeight; n > 0; n--)
                    {
                        *dst++ = new Abgr32(255, 255, 255, *src++).Raw;
                    }
                }
            }
        }

        fixed (int* buf = TexPixelsRGBA32)
        {
            return new ImFontTexData
            {
                PixelsBuffer = (byte*) buf,
                Width = TexWidth,
                Height = TexHeight,
                BytesPerPixel = 4
            };
        }
    }
}