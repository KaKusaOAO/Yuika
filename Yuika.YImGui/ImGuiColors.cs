// - Yuika.YImGui
// Copyright (C) Yui (KaKusaOAO).
// All rights reserved.

using System.Numerics;

namespace Yuika.YImGui;

public class ImGuiColors
{
    private readonly ColorF[] _colors = new ColorF[(int) ImGuiCol.EntryCount];

    public static ImGuiColors Dark { get; } = CreateDarkPalette();
    public static ImGuiColors Classic { get; } = CreateClassicPalette();
    public static ImGuiColors Light { get; } = CreateLightPalette();

    public ColorF this[ImGuiCol color]
    {
        get => _colors[(int) color];
        set => _colors[(int) color] = value;
    }

    private static ImGuiColors CreateDarkPalette()
    {
        ImGuiColors colors = new ImGuiColors();

        // @formatter:off
        colors[ImGuiCol.Text]                   = ColorF.White; // ImVec4(1.00f, 1.00f, 1.00f, 1.00f);
        colors[ImGuiCol.TextDisabled]           = new ColorF(0.50f, 0.50f, 0.50f, 1.00f);
        colors[ImGuiCol.WindowBg]               = new ColorF(0.06f, 0.06f, 0.06f, 0.94f);
        colors[ImGuiCol.ChildBg]                = new ColorF(0.00f, 0.00f, 0.00f, 0.00f);
        colors[ImGuiCol.PopupBg]                = new ColorF(0.08f, 0.08f, 0.08f, 0.94f);
        colors[ImGuiCol.Border]                 = new ColorF(0.43f, 0.43f, 0.50f, 0.50f);
        colors[ImGuiCol.BorderShadow]           = new ColorF(0.00f, 0.00f, 0.00f, 0.00f);
        colors[ImGuiCol.FrameBg]                = new ColorF(0.16f, 0.29f, 0.48f, 0.54f);
        colors[ImGuiCol.FrameBgHovered]         = new ColorF(0.26f, 0.59f, 0.98f, 0.40f);
        colors[ImGuiCol.FrameBgActive]          = new ColorF(0.26f, 0.59f, 0.98f, 0.67f);
        colors[ImGuiCol.TitleBg]                = new ColorF(0.04f, 0.04f, 0.04f, 1.00f);
        colors[ImGuiCol.TitleBgActive]          = new ColorF(0.16f, 0.29f, 0.48f, 1.00f);
        colors[ImGuiCol.TitleBgCollapsed]       = new ColorF(0.00f, 0.00f, 0.00f, 0.51f);
        colors[ImGuiCol.MenuBarBg]              = new ColorF(0.14f, 0.14f, 0.14f, 1.00f);
        colors[ImGuiCol.ScrollbarBg]            = new ColorF(0.02f, 0.02f, 0.02f, 0.53f);
        colors[ImGuiCol.ScrollbarGrab]          = new ColorF(0.31f, 0.31f, 0.31f, 1.00f);
        colors[ImGuiCol.ScrollbarGrabHovered]   = new ColorF(0.41f, 0.41f, 0.41f, 1.00f);
        colors[ImGuiCol.ScrollbarGrabActive]    = new ColorF(0.51f, 0.51f, 0.51f, 1.00f);
        colors[ImGuiCol.CheckMark]              = new ColorF(0.26f, 0.59f, 0.98f, 1.00f);
        colors[ImGuiCol.SliderGrab]             = new ColorF(0.24f, 0.52f, 0.88f, 1.00f);
        colors[ImGuiCol.SliderGrabActive]       = new ColorF(0.26f, 0.59f, 0.98f, 1.00f);
        colors[ImGuiCol.Button]                 = new ColorF(0.26f, 0.59f, 0.98f, 0.40f);
        colors[ImGuiCol.ButtonHovered]          = new ColorF(0.26f, 0.59f, 0.98f, 1.00f);
        colors[ImGuiCol.ButtonActive]           = new ColorF(0.06f, 0.53f, 0.98f, 1.00f);
        colors[ImGuiCol.Header]                 = new ColorF(0.26f, 0.59f, 0.98f, 0.31f);
        colors[ImGuiCol.HeaderHovered]          = new ColorF(0.26f, 0.59f, 0.98f, 0.80f);
        colors[ImGuiCol.HeaderActive]           = new ColorF(0.26f, 0.59f, 0.98f, 1.00f);
        colors[ImGuiCol.Separator]              = colors[ImGuiCol.Border];
        colors[ImGuiCol.SeparatorHovered]       = new ColorF(0.10f, 0.40f, 0.75f, 0.78f);
        colors[ImGuiCol.SeparatorActive]        = new ColorF(0.10f, 0.40f, 0.75f, 1.00f);
        colors[ImGuiCol.ResizeGrip]             = new ColorF(0.26f, 0.59f, 0.98f, 0.20f);
        colors[ImGuiCol.ResizeGripHovered]      = new ColorF(0.26f, 0.59f, 0.98f, 0.67f);
        colors[ImGuiCol.ResizeGripActive]       = new ColorF(0.26f, 0.59f, 0.98f, 0.95f);
        colors[ImGuiCol.Tab]                    = Vector4.Lerp(colors[ImGuiCol.Header],       colors[ImGuiCol.TitleBgActive], 0.80f);
        colors[ImGuiCol.TabHovered]             = colors[ImGuiCol.HeaderHovered];
        colors[ImGuiCol.TabActive]              = Vector4.Lerp(colors[ImGuiCol.HeaderActive], colors[ImGuiCol.TitleBgActive], 0.60f);
        colors[ImGuiCol.TabUnfocused]           = Vector4.Lerp(colors[ImGuiCol.Tab],          colors[ImGuiCol.TitleBg], 0.80f);
        colors[ImGuiCol.TabUnfocusedActive]     = Vector4.Lerp(colors[ImGuiCol.TabActive],    colors[ImGuiCol.TitleBg], 0.40f);
#if USE_DOCKING
        colors[ImGuiCol.DockingPreview]         = colors[ImGuiCol.HeaderActive] * new ColorF(1.0f, 1.0f, 1.0f, 0.7f);
        colors[ImGuiCol.DockingEmptyBg]         = new ColorF(0.20f, 0.20f, 0.20f, 1.00f);
#endif
        colors[ImGuiCol.PlotLines]              = new ColorF(0.61f, 0.61f, 0.61f, 1.00f);
        colors[ImGuiCol.PlotLinesHovered]       = new ColorF(1.00f, 0.43f, 0.35f, 1.00f);
        colors[ImGuiCol.PlotHistogram]          = new ColorF(0.90f, 0.70f, 0.00f, 1.00f);
        colors[ImGuiCol.PlotHistogramHovered]   = new ColorF(1.00f, 0.60f, 0.00f, 1.00f);
        colors[ImGuiCol.TableHeaderBg]          = new ColorF(0.19f, 0.19f, 0.20f, 1.00f);
        colors[ImGuiCol.TableBorderStrong]      = new ColorF(0.31f, 0.31f, 0.35f, 1.00f);   // Prefer using Alpha=1.0 here
        colors[ImGuiCol.TableBorderLight]       = new ColorF(0.23f, 0.23f, 0.25f, 1.00f);   // Prefer using Alpha=1.0 here
        colors[ImGuiCol.TableRowBg]             = new ColorF(0.00f, 0.00f, 0.00f, 0.00f);
        colors[ImGuiCol.TableRowBgAlt]          = new ColorF(1.00f, 1.00f, 1.00f, 0.06f);
        colors[ImGuiCol.TextSelectedBg]         = new ColorF(0.26f, 0.59f, 0.98f, 0.35f);
        colors[ImGuiCol.DragDropTarget]         = new ColorF(1.00f, 1.00f, 0.00f, 0.90f);
        colors[ImGuiCol.NavHighlight]           = new ColorF(0.26f, 0.59f, 0.98f, 1.00f);
        colors[ImGuiCol.NavWindowingHighlight]  = new ColorF(1.00f, 1.00f, 1.00f, 0.70f);
        colors[ImGuiCol.NavWindowingDimBg]      = new ColorF(0.80f, 0.80f, 0.80f, 0.20f);
        colors[ImGuiCol.ModalWindowDimBg]       = new ColorF(0.80f, 0.80f, 0.80f, 0.35f);
        // @formatter:on
        
        return colors;
    }
    
    private static ImGuiColors CreateClassicPalette()
    {
        ImGuiColors colors = new ImGuiColors();

        // @formatter:off
        colors[ImGuiCol.Text]                   = new ColorF(0.90f, 0.90f, 0.90f, 1.00f);
        colors[ImGuiCol.TextDisabled]           = new ColorF(0.60f, 0.60f, 0.60f, 1.00f);
        colors[ImGuiCol.WindowBg]               = new ColorF(0.00f, 0.00f, 0.00f, 0.85f);
        colors[ImGuiCol.ChildBg]                = new ColorF(0.00f, 0.00f, 0.00f, 0.00f);
        colors[ImGuiCol.PopupBg]                = new ColorF(0.11f, 0.11f, 0.14f, 0.92f);
        colors[ImGuiCol.Border]                 = new ColorF(0.50f, 0.50f, 0.50f, 0.50f);
        colors[ImGuiCol.BorderShadow]           = new ColorF(0.00f, 0.00f, 0.00f, 0.00f);
        colors[ImGuiCol.FrameBg]                = new ColorF(0.43f, 0.43f, 0.43f, 0.39f);
        colors[ImGuiCol.FrameBgHovered]         = new ColorF(0.47f, 0.47f, 0.69f, 0.40f);
        colors[ImGuiCol.FrameBgActive]          = new ColorF(0.42f, 0.41f, 0.64f, 0.69f);
        colors[ImGuiCol.TitleBg]                = new ColorF(0.27f, 0.27f, 0.54f, 0.83f);
        colors[ImGuiCol.TitleBgActive]          = new ColorF(0.32f, 0.32f, 0.63f, 0.87f);
        colors[ImGuiCol.TitleBgCollapsed]       = new ColorF(0.40f, 0.40f, 0.80f, 0.20f);
        colors[ImGuiCol.MenuBarBg]              = new ColorF(0.40f, 0.40f, 0.55f, 0.80f);
        colors[ImGuiCol.ScrollbarBg]            = new ColorF(0.20f, 0.25f, 0.30f, 0.60f);
        colors[ImGuiCol.ScrollbarGrab]          = new ColorF(0.40f, 0.40f, 0.80f, 0.30f);
        colors[ImGuiCol.ScrollbarGrabHovered]   = new ColorF(0.40f, 0.40f, 0.80f, 0.40f);
        colors[ImGuiCol.ScrollbarGrabActive]    = new ColorF(0.41f, 0.39f, 0.80f, 0.60f);
        colors[ImGuiCol.CheckMark]              = new ColorF(0.90f, 0.90f, 0.90f, 0.50f);
        colors[ImGuiCol.SliderGrab]             = new ColorF(1.00f, 1.00f, 1.00f, 0.30f);
        colors[ImGuiCol.SliderGrabActive]       = new ColorF(0.41f, 0.39f, 0.80f, 0.60f);
        colors[ImGuiCol.Button]                 = new ColorF(0.35f, 0.40f, 0.61f, 0.62f);
        colors[ImGuiCol.ButtonHovered]          = new ColorF(0.40f, 0.48f, 0.71f, 0.79f);
        colors[ImGuiCol.ButtonActive]           = new ColorF(0.46f, 0.54f, 0.80f, 1.00f);
        colors[ImGuiCol.Header]                 = new ColorF(0.40f, 0.40f, 0.90f, 0.45f);
        colors[ImGuiCol.HeaderHovered]          = new ColorF(0.45f, 0.45f, 0.90f, 0.80f);
        colors[ImGuiCol.HeaderActive]           = new ColorF(0.53f, 0.53f, 0.87f, 0.80f);
        colors[ImGuiCol.Separator]              = new ColorF(0.50f, 0.50f, 0.50f, 0.60f);
        colors[ImGuiCol.SeparatorHovered]       = new ColorF(0.60f, 0.60f, 0.70f, 1.00f);
        colors[ImGuiCol.SeparatorActive]        = new ColorF(0.70f, 0.70f, 0.90f, 1.00f);
        colors[ImGuiCol.ResizeGrip]             = new ColorF(1.00f, 1.00f, 1.00f, 0.10f);
        colors[ImGuiCol.ResizeGripHovered]      = new ColorF(0.78f, 0.82f, 1.00f, 0.60f);
        colors[ImGuiCol.ResizeGripActive]       = new ColorF(0.78f, 0.82f, 1.00f, 0.90f);
        colors[ImGuiCol.Tab]                    = Vector4.Lerp(colors[ImGuiCol.Header],       colors[ImGuiCol.TitleBgActive], 0.80f);
        colors[ImGuiCol.TabHovered]             = colors[ImGuiCol.HeaderHovered];
        colors[ImGuiCol.TabActive]              = Vector4.Lerp(colors[ImGuiCol.HeaderActive], colors[ImGuiCol.TitleBgActive], 0.60f);
        colors[ImGuiCol.TabUnfocused]           = Vector4.Lerp(colors[ImGuiCol.Tab],          colors[ImGuiCol.TitleBg], 0.80f);
        colors[ImGuiCol.TabUnfocusedActive]     = Vector4.Lerp(colors[ImGuiCol.TabActive],    colors[ImGuiCol.TitleBg], 0.40f);
#if USE_DOCKING
        colors[ImGuiCol.DockingPreview]         = colors[ImGuiCol.Header] * new ColorF(1.0f, 1.0f, 1.0f, 0.7f);
        colors[ImGuiCol.DockingEmptyBg]         = new ColorF(0.20f, 0.20f, 0.20f, 1.00f);
#endif
        colors[ImGuiCol.PlotLines]              = new ColorF(1.00f, 1.00f, 1.00f, 1.00f);
        colors[ImGuiCol.PlotLinesHovered]       = new ColorF(0.90f, 0.70f, 0.00f, 1.00f);
        colors[ImGuiCol.PlotHistogram]          = new ColorF(0.90f, 0.70f, 0.00f, 1.00f);
        colors[ImGuiCol.PlotHistogramHovered]   = new ColorF(1.00f, 0.60f, 0.00f, 1.00f);
        colors[ImGuiCol.TableHeaderBg]          = new ColorF(0.27f, 0.27f, 0.38f, 1.00f);
        colors[ImGuiCol.TableBorderStrong]      = new ColorF(0.31f, 0.31f, 0.45f, 1.00f);   // Prefer using Alpha=1.0 here
        colors[ImGuiCol.TableBorderLight]       = new ColorF(0.26f, 0.26f, 0.28f, 1.00f);   // Prefer using Alpha=1.0 here
        colors[ImGuiCol.TableRowBg]             = new ColorF(0.00f, 0.00f, 0.00f, 0.00f);
        colors[ImGuiCol.TableRowBgAlt]          = new ColorF(1.00f, 1.00f, 1.00f, 0.07f);
        colors[ImGuiCol.TextSelectedBg]         = new ColorF(0.00f, 0.00f, 1.00f, 0.35f);
        colors[ImGuiCol.DragDropTarget]         = new ColorF(1.00f, 1.00f, 0.00f, 0.90f);
        colors[ImGuiCol.NavHighlight]           = colors[ImGuiCol.HeaderHovered];
        colors[ImGuiCol.NavWindowingHighlight]  = new ColorF(1.00f, 1.00f, 1.00f, 0.70f);
        colors[ImGuiCol.NavWindowingDimBg]      = new ColorF(0.80f, 0.80f, 0.80f, 0.20f);
        colors[ImGuiCol.ModalWindowDimBg]       = new ColorF(0.20f, 0.20f, 0.20f, 0.35f);
        // @formatter:on
        
        return colors;
    }

    private static ImGuiColors CreateLightPalette()
    {
        ImGuiColors colors = new ImGuiColors();

        // @formatter:off
        colors[ImGuiCol.Text]                   = ColorF.Black; // new ColorF(0.00f, 0.00f, 0.00f, 1.00f);
        colors[ImGuiCol.TextDisabled]           = new ColorF(0.60f, 0.60f, 0.60f, 1.00f);
        colors[ImGuiCol.WindowBg]               = new ColorF(0.94f, 0.94f, 0.94f, 1.00f);
        colors[ImGuiCol.ChildBg]                = new ColorF(0.00f, 0.00f, 0.00f, 0.00f);
        colors[ImGuiCol.PopupBg]                = new ColorF(1.00f, 1.00f, 1.00f, 0.98f);
        colors[ImGuiCol.Border]                 = new ColorF(0.00f, 0.00f, 0.00f, 0.30f);
        colors[ImGuiCol.BorderShadow]           = new ColorF(0.00f, 0.00f, 0.00f, 0.00f);
        colors[ImGuiCol.FrameBg]                = new ColorF(1.00f, 1.00f, 1.00f, 1.00f);
        colors[ImGuiCol.FrameBgHovered]         = new ColorF(0.26f, 0.59f, 0.98f, 0.40f);
        colors[ImGuiCol.FrameBgActive]          = new ColorF(0.26f, 0.59f, 0.98f, 0.67f);
        colors[ImGuiCol.TitleBg]                = new ColorF(0.96f, 0.96f, 0.96f, 1.00f);
        colors[ImGuiCol.TitleBgActive]          = new ColorF(0.82f, 0.82f, 0.82f, 1.00f);
        colors[ImGuiCol.TitleBgCollapsed]       = new ColorF(1.00f, 1.00f, 1.00f, 0.51f);
        colors[ImGuiCol.MenuBarBg]              = new ColorF(0.86f, 0.86f, 0.86f, 1.00f);
        colors[ImGuiCol.ScrollbarBg]            = new ColorF(0.98f, 0.98f, 0.98f, 0.53f);
        colors[ImGuiCol.ScrollbarGrab]          = new ColorF(0.69f, 0.69f, 0.69f, 0.80f);
        colors[ImGuiCol.ScrollbarGrabHovered]   = new ColorF(0.49f, 0.49f, 0.49f, 0.80f);
        colors[ImGuiCol.ScrollbarGrabActive]    = new ColorF(0.49f, 0.49f, 0.49f, 1.00f);
        colors[ImGuiCol.CheckMark]              = new ColorF(0.26f, 0.59f, 0.98f, 1.00f);
        colors[ImGuiCol.SliderGrab]             = new ColorF(0.26f, 0.59f, 0.98f, 0.78f);
        colors[ImGuiCol.SliderGrabActive]       = new ColorF(0.46f, 0.54f, 0.80f, 0.60f);
        colors[ImGuiCol.Button]                 = new ColorF(0.26f, 0.59f, 0.98f, 0.40f);
        colors[ImGuiCol.ButtonHovered]          = new ColorF(0.26f, 0.59f, 0.98f, 1.00f);
        colors[ImGuiCol.ButtonActive]           = new ColorF(0.06f, 0.53f, 0.98f, 1.00f);
        colors[ImGuiCol.Header]                 = new ColorF(0.26f, 0.59f, 0.98f, 0.31f);
        colors[ImGuiCol.HeaderHovered]          = new ColorF(0.26f, 0.59f, 0.98f, 0.80f);
        colors[ImGuiCol.HeaderActive]           = new ColorF(0.26f, 0.59f, 0.98f, 1.00f);
        colors[ImGuiCol.Separator]              = new ColorF(0.39f, 0.39f, 0.39f, 0.62f);
        colors[ImGuiCol.SeparatorHovered]       = new ColorF(0.14f, 0.44f, 0.80f, 0.78f);
        colors[ImGuiCol.SeparatorActive]        = new ColorF(0.14f, 0.44f, 0.80f, 1.00f);
        colors[ImGuiCol.ResizeGrip]             = new ColorF(0.35f, 0.35f, 0.35f, 0.17f);
        colors[ImGuiCol.ResizeGripHovered]      = new ColorF(0.26f, 0.59f, 0.98f, 0.67f);
        colors[ImGuiCol.ResizeGripActive]       = new ColorF(0.26f, 0.59f, 0.98f, 0.95f);
        colors[ImGuiCol.Tab]                    = Vector4.Lerp(colors[ImGuiCol.Header],       colors[ImGuiCol.TitleBgActive], 0.90f);
        colors[ImGuiCol.TabHovered]             = colors[ImGuiCol.HeaderHovered];
        colors[ImGuiCol.TabActive]              = Vector4.Lerp(colors[ImGuiCol.HeaderActive], colors[ImGuiCol.TitleBgActive], 0.60f);
        colors[ImGuiCol.TabUnfocused]           = Vector4.Lerp(colors[ImGuiCol.Tab],          colors[ImGuiCol.TitleBg], 0.80f);
        colors[ImGuiCol.TabUnfocusedActive]     = Vector4.Lerp(colors[ImGuiCol.TabActive],    colors[ImGuiCol.TitleBg], 0.40f);
#if USE_DOCKING
        colors[ImGuiCol.DockingPreview]         = colors[ImGuiCol.Header] * new ColorF(1.0f, 1.0f, 1.0f, 0.7f);
        colors[ImGuiCol.DockingEmptyBg]         = new ColorF(0.20f, 0.20f, 0.20f, 1.00f);
#endif
        colors[ImGuiCol.PlotLines]              = new ColorF(0.39f, 0.39f, 0.39f, 1.00f);
        colors[ImGuiCol.PlotLinesHovered]       = new ColorF(1.00f, 0.43f, 0.35f, 1.00f);
        colors[ImGuiCol.PlotHistogram]          = new ColorF(0.90f, 0.70f, 0.00f, 1.00f);
        colors[ImGuiCol.PlotHistogramHovered]   = new ColorF(1.00f, 0.45f, 0.00f, 1.00f);
        colors[ImGuiCol.TableHeaderBg]          = new ColorF(0.78f, 0.87f, 0.98f, 1.00f);
        colors[ImGuiCol.TableBorderStrong]      = new ColorF(0.57f, 0.57f, 0.64f, 1.00f);   // Prefer using Alpha=1.0 here
        colors[ImGuiCol.TableBorderLight]       = new ColorF(0.68f, 0.68f, 0.74f, 1.00f);   // Prefer using Alpha=1.0 here
        colors[ImGuiCol.TableRowBg]             = new ColorF(0.00f, 0.00f, 0.00f, 0.00f);
        colors[ImGuiCol.TableRowBgAlt]          = new ColorF(0.30f, 0.30f, 0.30f, 0.09f);
        colors[ImGuiCol.TextSelectedBg]         = new ColorF(0.26f, 0.59f, 0.98f, 0.35f);
        colors[ImGuiCol.DragDropTarget]         = new ColorF(0.26f, 0.59f, 0.98f, 0.95f);
        colors[ImGuiCol.NavHighlight]           = colors[ImGuiCol.HeaderHovered];
        colors[ImGuiCol.NavWindowingHighlight]  = new ColorF(0.70f, 0.70f, 0.70f, 0.70f);
        colors[ImGuiCol.NavWindowingDimBg]      = new ColorF(0.20f, 0.20f, 0.20f, 0.20f);
        colors[ImGuiCol.ModalWindowDimBg]       = new ColorF(0.20f, 0.20f, 0.20f, 0.35f);
        // @formatter:on
        
        return colors;
    }
}