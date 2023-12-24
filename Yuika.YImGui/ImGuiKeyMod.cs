namespace Yuika.YImGui;

[Flags]
public enum ImGuiKeyMod
{
    None = 0,
    Ctrl = 1 << 12,
    Shift = 1 << 13,
    Alt = 1 << 14,
    Super = 1 << 15,
    Shortcut = 1 << 11,
    Mask = 0xf800
}