// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Numerics;

namespace Yuika.YImGui;

public class ImGuiStyle
{
    public float Alpha { get; set; } = 1;
    public float DisabledAlpha { get; set; } = 0.6f;
    public Vector2 WindowPadding { get; set; } = new Vector2(8, 8);
    public float WindowRounding { get; set; } = 0;
    public float WindowBorderSize { get; set; } = 1;
    public Vector2 WindowMinSize { get; set; } = new Vector2(32, 32);
    public Vector2 WindowTitleAlign { get; set; } = Alignments.MiddleLeft;
    public ImGuiDir WindowMenuButtonPosition { get; set; } = ImGuiDir.Left;
    public float ChildRounding { get; set; } = 0;
    public float ChildBorderSize { get; set; } = 1;
    public float PopupRounding { get; set; } = 0;
    public float PopupBorderSize { get; set; } = 1;
    public Vector2 FramePadding { get; set; } = new Vector2(4, 3);
    public float FrameRounding { get; set; } = 0;
    public float FrameBorderSize { get; set; } = 0;
    public Vector2 ItemSpacing { get; set; } = new Vector2(8, 4);
    public Vector2 ItemInnerSpacing { get; set; } = new Vector2(4, 4);
    public Vector2 CellPadding { get; set; } = new Vector2(4, 2);
    public Vector2 TouchExtraPadding { get; set; } = Vector2.Zero;
    public float IndentSpacing { get; set; } = 21;
    public float ColumnsMinSpacing { get; set; } = 6;
    public float ScrollbarSize { get; set; } = 14;
    public float ScrollbarRounding { get; set; } = 9;
    public float GrabMinSize { get; set; } = 12;
    public float GrabRounding { get; set; } = 0;
    public float LogSliderDeadzone { get; set; } = 4;
    public float TabRounding { get; set; } = 4;
    public float TabBorderSize { get; set; } = 0;
    public float TabMinWidthForCloseButton { get; set; } = 0;
    public float TabBarBorderSize { get; set; } = 1;
    public float TableAngledHeadersAngle { get; set; } = 35 * MathF.PI / 180;
    public ImGuiDir ColorButtonPosition { get; set; } = ImGuiDir.Right;
    public Vector2 ButtonTextAlign { get; set; } = Alignments.MiddleLeft;
    public Vector2 SelectableTextAlign { get; set; } = Alignments.TopLeft;
    public float SeparatorTextBorderSize { get; set; } = 3;
    public Vector2 SeparatorTextAlign { get; set; } = Alignments.MiddleLeft;
    public Vector2 SeparatorTextPadding { get; set; } = new Vector2(20, 3);
    public Vector2 DisplayWindowPadding { get; set; } = new Vector2(19, 19);
    public Vector2 DisplaySafeAreaPadding { get; set; } = new Vector2(3, 3);
    public float DockingSeparatorSize { get; set; } = 2;
    public float MouseCursorScale { get; set; } = 1;
    public bool AntiAliasedLines { get; set; } = true;
    public bool AntiAliasedLinesUseTex { get; set; } = true;
    public bool AntiAliasedFill { get; set; } = true;
    public float CurveTessellationTol { get; set; } = 1.25f;
    public float CircleTessellationMaxError { get; set; } = 0.3f;
    public ImGuiColors Colors { get; set; } = ImGuiColors.Dark;
    public float HoverStationaryDelay { get; set; } = 0.15f;
    public float HoverDelayShort { get; set; } = 0.15f;
    public float HoverDelayNormal { get; set; } = 0.4f;

    public ImGuiHoveredFlags HoverFlagsForTooltipMouse { get; set; } = ImGuiHoveredFlags.Stationary |
                                                                       ImGuiHoveredFlags.DelayShort |
                                                                       ImGuiHoveredFlags.AllowWhenDisabled;

    public ImGuiHoveredFlags HoverFlagsForTooltipNav { get; set; } = ImGuiHoveredFlags.NoSharedDelay |
                                                                     ImGuiHoveredFlags.DelayNormal |
                                                                     ImGuiHoveredFlags.AllowWhenDisabled;
}