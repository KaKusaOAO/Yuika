namespace Yuika.YImGui;

using IdxType = IntPtr;

public struct ImDrawIdx
{
    public readonly IdxType Value;

    public ImDrawIdx(IdxType value) => Value = value;

    public static implicit operator IdxType(ImDrawIdx id) => id.Value;
    public static implicit operator ImDrawIdx(IdxType id) => new(id);
}