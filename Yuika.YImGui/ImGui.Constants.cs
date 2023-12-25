namespace Yuika.YImGui;

public static partial class ImGui
{
    public const int NamedKeyBegin = 512;
    public const int NamedKeyEnd = (int) ImGuiKey.KeyCount;
    public const int NamedKeyCount = NamedKeyEnd - NamedKeyBegin;

    public const bool DisableObsoleteKeyIO = true;
    public const int KeysDataSize = DisableObsoleteKeyIO ? NamedKeyCount : (int) ImGuiKey.KeyCount;
    public const int KeysDataOffset = DisableObsoleteKeyIO ? NamedKeyBegin : 0;

    internal const int ArcFastTableSize = 48;

#if USE_DOCKING
    internal const uint ViewportDefaultId = /*0x*/11111111;
#endif
}