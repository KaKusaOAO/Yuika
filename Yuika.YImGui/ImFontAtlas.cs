using System.Diagnostics;

namespace Yuika.YImGui;

public class ImFontAtlas
{
    public bool Locked { get; set; }

    public ImFont AddFont(ImFontConfig fontConfig)
    {
        if (Locked)
        {
            throw new ImGuiException(
                $"Cannot modify a locked {nameof(ImFontAtlas)} between " +
                $"{nameof(ImGui.NewFrame)}() and {nameof(ImGui.EndFrame)}/{nameof(ImGui.Render)}()!");
        }
        
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
}

public class ImGuiException : Exception
{
    public ImGuiException() {}
    public ImGuiException(string message) : base(message) {}
}