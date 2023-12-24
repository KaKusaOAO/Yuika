namespace Yuika.YImGui.Internal;

internal class ImGuiKeyOwnerData
{
    public uint OwnerCurr { get; set; } = unchecked((uint)-1);
    public uint OwnerNext { get; set; } = unchecked((uint)-1);
    public bool LockThisFrame { get; set; }
    public bool LockUntilRelease { get; set; }
}