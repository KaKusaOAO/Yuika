// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

namespace Yuika.YImGui;

[Flags]
public enum ImGuiDragDropFlags
{
    None,

    // BeginDragDropSource() flags
    SourceNoPreviewTooltip   = 1 << 0,
    SourceNoDisableHover     = 1 << 1,
    SourceNoHoldToOpenOthers = 1 << 2,
    SourceAllowNullID        = 1 << 3,
    SourceExtern             = 1 << 4,
    SourceAutoExpirePayload  = 1 << 5,
    
    // AcceptDragDropPayload() flags
    AcceptBeforeDelivery     = 1 << 10,
    AcceptNoDrawDefaultRect  = 1 << 11,
    AcceptNoPreviewTooltip   = 1 << 12,
    AcceptPeekOnly           = AcceptBeforeDelivery | AcceptNoDrawDefaultRect
}
