// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui.Internal;

internal class ImGuiStackSizes
{
    public short SizeOfIdStack { get; set; }
    public short SizeOfColorStack { get; set; }
    public short SizeOfStyleVarStack { get; set; }
    public short SizeOfFrontStack { get; set; }
    public short SizeOfFocusScopeStack { get; set; }
    public short SizeOfGroupStack { get; set; }
    public short SizeOfItemFlagsStack { get; set; }
    public short SizeOfBeginPopupStack { get; set; }
    public short SizeOfDisabledStack { get; set; }

    public void SetToContextState(ImGuiContext ctx) => throw new NotImplementedException();
    public void CompareWithContextState(ImGuiContext ctx) => throw new NotImplementedException();
}