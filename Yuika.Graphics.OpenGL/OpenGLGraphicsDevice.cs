﻿using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if __IOS__ || __MACOS__ || __TVOS__
using CoreAnimation;
using ObjCRuntime;
#if !__MACCATALYST__ && !__MACOS__
using OpenGLES;
#endif
#endif
using OpenTK;
#if __ANDROID__
using OpenTK.Graphics.Egl;
#endif
using static Yuika.Graphics.OpenGL.OpenGLUtil;
// using NativeLibrary = NativeLibraryLoader.NativeLibrary;
using OpenTK.Graphics.OpenGL4;
using Buffer = System.Buffer;
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;
using GLPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using GLPixelType = OpenTK.Graphics.OpenGL4.PixelType;
using GLFramebufferAttachment = OpenTK.Graphics.OpenGL4.FramebufferAttachment;

namespace Yuika.Graphics.OpenGL;

internal unsafe class OpenGLGraphicsDevice : GraphicsDevice
{
    private ResourceFactory _resourceFactory;
    private string _deviceName;
    private string _vendorName;
    private string _version;
    private string _shadingLanguageVersion;
    private GraphicsApiVersion _apiVersion;
    private GraphicsBackend _backendType;
    private GraphicsDeviceFeatures _features;
    private uint _vao;
    private readonly ConcurrentQueue<OpenGLDeferredResource> _resourcesToDispose
        = new ConcurrentQueue<OpenGLDeferredResource>();
    private IntPtr _glContext;
    private Action<IntPtr> _makeCurrent;
    private Func<IntPtr> _getCurrentContext;
    private Action<IntPtr> _deleteContext;
    private Action _swapBuffers;
    private Action<bool> _setSyncToVBlank;
    private OpenGLSwapchainFramebuffer _swapchainFramebuffer;
    private OpenGLTextureSamplerManager _textureSamplerManager;
    private OpenGLCommandExecutor _commandExecutor;
    private DebugProc _debugMessageCallback;
    private OpenGLExtensions _extensions;
    private bool _isDepthRangeZeroToOne;
    private BackendInfoOpenGL _openglInfo;

    private TextureSampleCount _maxColorTextureSamples;
    private uint _maxTextureSize;
    private uint _maxTexDepth;
    private uint _maxTexArrayLayers;
    private uint _minUboOffsetAlignment;
    private uint _minSsboOffsetAlignment;

    private readonly StagingMemoryPool _stagingMemoryPool = new StagingMemoryPool();
    private BlockingCollection<ExecutionThreadWorkItem> _workItems;
    private ExecutionThread _executionThread;
    private readonly object _commandListDisposalLock = new object();
    private readonly Dictionary<OpenGLCommandList, int> _submittedCommandListCounts
        = new Dictionary<OpenGLCommandList, int>();
    private readonly HashSet<OpenGLCommandList> _commandListsToDispose = new HashSet<OpenGLCommandList>();

    private readonly object _mappedResourceLock = new object();
    private readonly Dictionary<MappedResourceCacheKey, MappedResourceInfoWithStaging> _mappedResources
        = new Dictionary<MappedResourceCacheKey, MappedResourceInfoWithStaging>();

    private readonly object _resetEventsLock = new object();
    private readonly List<ManualResetEvent[]> _resetEvents = new List<ManualResetEvent[]>();
    private Swapchain _mainSwapchain;

    private bool _syncToVBlank;

    public override IGraphicsBackendInfo BackendInfo => _openglInfo;

    public override string DeviceName => _deviceName;

    public override string VendorName => _vendorName;

    public override GraphicsApiVersion ApiVersion => _apiVersion;

    public override GraphicsBackend BackendType => _backendType;

    public override bool IsUvOriginTopLeft => false;

    public override bool IsDepthRangeZeroToOne => _isDepthRangeZeroToOne;

    public override bool IsClipSpaceYInverted => false;

    public override ResourceFactory ResourceFactory => _resourceFactory;

    public OpenGLExtensions Extensions => _extensions;

    public override Swapchain MainSwapchain => _mainSwapchain;

    public override bool SyncToVerticalBlank
    {
        get => _syncToVBlank;
        set
        {
            if (_syncToVBlank != value)
            {
                _syncToVBlank = value;
                _executionThread.SetSyncToVerticalBlank(value);
            }
        }
    }

    public string Version => _version;

    public string ShadingLanguageVersion => _shadingLanguageVersion;

    public OpenGLTextureSamplerManager TextureSamplerManager => _textureSamplerManager;

    public override GraphicsDeviceFeatures Features => _features;

    public StagingMemoryPool StagingMemoryPool => _stagingMemoryPool;

    internal static Func<string, IntPtr>? _getProcAddress;
    
    public OpenGLGraphicsDevice(
        GraphicsDeviceOptions options,
        OpenGLPlatformInfo platformInfo,
        uint width,
        uint height)
    {
        Init(options, platformInfo, width, height, true);
    }

    private void Init(
        GraphicsDeviceOptions options,
        OpenGLPlatformInfo platformInfo,
        uint width,
        uint height,
        bool loadFunctions)
    {
        _syncToVBlank = options.SyncToVerticalBlank;
        _glContext = platformInfo.OpenGLContextHandle;
        _makeCurrent = platformInfo.MakeCurrent;
        _getCurrentContext = platformInfo.GetCurrentContext;
        _deleteContext = platformInfo.DeleteContext;
        _swapBuffers = platformInfo.SwapBuffers;
        _setSyncToVBlank = platformInfo.SetSyncToVerticalBlank;
        OpenTK.Graphics.ES30.GL.LoadBindings(platformInfo);
        GL.LoadBindings(platformInfo);
        
        // LoadGetString(_glContext, platformInfo.GetProcAddress);
        // _version = Util.GetString(glGetString(StringName.Version));
        // _shadingLanguageVersion = Util.GetString(glGetString(StringName.ShadingLanguageVersion));
        // _vendorName = Util.GetString(glGetString(StringName.Vendor));
        // _deviceName = Util.GetString(glGetString(StringName.Renderer));
        _version = GL.GetString(StringName.Version); 
        _shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion); 
        _vendorName = GL.GetString(StringName.Vendor); 
        _deviceName = GL.GetString(StringName.Renderer); 
        
        _backendType = _version.StartsWith("OpenGL ES") ? GraphicsBackend.OpenGLES : GraphicsBackend.OpenGL;

        _getProcAddress = platformInfo.GetProcAddress;
        // LoadAllFunctions(_glContext, platformInfo.GetProcAddress, _backendType == GraphicsBackend.OpenGLES);

        int majorVersion, minorVersion;
        GL.GetInteger(GetPName.MajorVersion, &majorVersion);
        CheckLastError();
        GL.GetInteger(GetPName.MinorVersion, &minorVersion);
        CheckLastError();

        GraphicsApiVersion.TryParseGLVersion(_version, out _apiVersion);
        if (_apiVersion.Major != majorVersion ||
            _apiVersion.Minor != minorVersion)
        {
            // This mismatch should never be hit in valid OpenGL implementations.
            _apiVersion = new GraphicsApiVersion(majorVersion, minorVersion, 0, 0);
        }

        int extensionCount;
        GL.GetInteger(GetPName.NumExtensions, &extensionCount);
        CheckLastError();

        HashSet<string> extensions = new HashSet<string>();
        for (uint i = 0; i < extensionCount; i++)
        {
            string extensionName = GL.GetString(StringNameIndexed.Extensions, i);
            CheckLastError();
            extensions.Add(extensionName);
        }

        _extensions = new OpenGLExtensions(extensions, _backendType, majorVersion, minorVersion);

        bool drawIndirect = _extensions.DrawIndirect || _extensions.MultiDrawIndirect;
        _features = new GraphicsDeviceFeatures(
            computeShader: _extensions.ComputeShaders,
            geometryShader: _extensions.GeometryShader,
            tessellationShaders: _extensions.TessellationShader,
            multipleViewports: _extensions.ARB_ViewportArray,
            samplerLodBias: _backendType == GraphicsBackend.OpenGL,
            drawBaseVertex: _extensions.DrawElementsBaseVertex,
            drawBaseInstance: _extensions.GLVersion(4, 2),
            drawIndirect: drawIndirect,
            drawIndirectBaseInstance: drawIndirect,
            fillModeWireframe: _backendType == GraphicsBackend.OpenGL,
            samplerAnisotropy: _extensions.AnisotropicFilter,
            depthClipDisable: _backendType == GraphicsBackend.OpenGL,
            texture1D: _backendType == GraphicsBackend.OpenGL,
            independentBlend: _extensions.IndependentBlend,
            structuredBuffer: _extensions.StorageBuffers,
            subsetTextureView: _extensions.ARB_TextureView,
            commandListDebugMarkers: _extensions.KHR_Debug || _extensions.EXT_DebugMarker,
            bufferRangeBinding: _extensions.ARB_uniform_buffer_object,
            shaderFloat64: _extensions.ARB_GpuShaderFp64);

        int uboAlignment;
        GL.GetInteger(GetPName.UniformBufferOffsetAlignment, &uboAlignment);
        CheckLastError();
        _minUboOffsetAlignment = (uint)uboAlignment;

        if (_features.StructuredBuffer)
        {
            int ssboAlignment;
            GL.GetInteger((GetPName) 37087, &ssboAlignment);
            CheckLastError();
            _minSsboOffsetAlignment = (uint)ssboAlignment;
        }

        _resourceFactory = new OpenGLResourceFactory(this);

        GL.GenVertexArrays(1, out _vao);
        CheckLastError();

        GL.BindVertexArray(_vao);
        CheckLastError();

        if (options.Debug && (_extensions.KHR_Debug || _extensions.ARB_DebugOutput))
        {
            EnableDebugCallback();
        }

        bool backbufferIsSrgb = ManualSrgbBackbufferQuery();

        PixelFormat swapchainFormat;
        if (options.SwapchainSrgbFormat && (backbufferIsSrgb || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)))
        {
            swapchainFormat = PixelFormat.B8_G8_R8_A8_UNorm_SRgb;
        }
        else
        {
            swapchainFormat = PixelFormat.B8_G8_R8_A8_UNorm;
        }

        _swapchainFramebuffer = new OpenGLSwapchainFramebuffer(
            width,
            height,
            swapchainFormat,
            options.SwapchainDepthFormat,
            swapchainFormat != PixelFormat.B8_G8_R8_A8_UNorm_SRgb);

        // Set miscellaneous initial states.
        if (_backendType == GraphicsBackend.OpenGL)
        {
            GL.Enable(EnableCap.TextureCubeMapSeamless);
            CheckLastError();
        }

        _textureSamplerManager = new OpenGLTextureSamplerManager(_extensions);
        _commandExecutor = new OpenGLCommandExecutor(this, platformInfo);

        int maxColorTextureSamples;
        if (_backendType == GraphicsBackend.OpenGL)
        {
            GL.GetInteger(GetPName.MaxColorTextureSamples, &maxColorTextureSamples);
            CheckLastError();
        }
        else
        {
            GL.GetInteger(GetPName.MaxSamples, &maxColorTextureSamples);
            CheckLastError();
        }
        if (maxColorTextureSamples >= 32)
        {
            _maxColorTextureSamples = TextureSampleCount.Count32;
        }
        else if (maxColorTextureSamples >= 16)
        {
            _maxColorTextureSamples = TextureSampleCount.Count16;
        }
        else if (maxColorTextureSamples >= 8)
        {
            _maxColorTextureSamples = TextureSampleCount.Count8;
        }
        else if (maxColorTextureSamples >= 4)
        {
            _maxColorTextureSamples = TextureSampleCount.Count4;
        }
        else if (maxColorTextureSamples >= 2)
        {
            _maxColorTextureSamples = TextureSampleCount.Count2;
        }
        else
        {
            _maxColorTextureSamples = TextureSampleCount.Count1;
        }

        int maxTexSize;

        GL.GetInteger(GetPName.MaxTextureSize, &maxTexSize);
        CheckLastError();

        int maxTexDepth;
        GL.GetInteger(GetPName.Max3DTextureSize, &maxTexDepth);
        CheckLastError();

        int maxTexArrayLayers;
        GL.GetInteger(GetPName.MaxArrayTextureLayers, &maxTexArrayLayers);
        CheckLastError();

        if (options.PreferDepthRangeZeroToOne && _extensions.ARB_ClipControl)
        {
            GL.ClipControl(ClipOrigin.LowerLeft, ClipDepthMode.ZeroToOne);
            CheckLastError();
            _isDepthRangeZeroToOne = true;
        }

        _maxTextureSize = (uint)maxTexSize;
        _maxTexDepth = (uint)maxTexDepth;
        _maxTexArrayLayers = (uint)maxTexArrayLayers;

        _mainSwapchain = new OpenGLSwapchain(
            this,
            _swapchainFramebuffer,
            platformInfo.ResizeSwapchain);

        _workItems = new BlockingCollection<ExecutionThreadWorkItem>(new ConcurrentQueue<ExecutionThreadWorkItem>());
        platformInfo.ClearCurrentContext();
        _executionThread = new ExecutionThread(this, _workItems, _makeCurrent, _glContext);
        _openglInfo = new BackendInfoOpenGL(this);

        PostDeviceCreated();
    }

    private bool ManualSrgbBackbufferQuery()
    {
        if (_backendType == GraphicsBackend.OpenGLES && !_extensions.EXT_sRGBWriteControl)
        {
            return false;
        }

        GL.GenTextures(1, out uint copySrc);
        CheckLastError();

        float* data = stackalloc float[4];
        data[0] = 0.5f;
        data[1] = 0.5f;
        data[2] = 0.5f;
        data[3] = 1f;

        GL.ActiveTexture(TextureUnit.Texture0);
        CheckLastError();
        GL.BindTexture(TextureTarget.Texture2D, copySrc);
        CheckLastError();
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, 1, 1, 0, GLPixelFormat.Rgba, GLPixelType.Float, (IntPtr)data);
        CheckLastError();
        GL.GenFramebuffers(1, out uint copySrcFb);
        CheckLastError();

        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, copySrcFb);
        CheckLastError();
        GL.FramebufferTexture2D(FramebufferTarget.ReadFramebuffer, GLFramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, copySrc, 0);
        CheckLastError();

        GL.Enable(EnableCap.FramebufferSrgb);
        CheckLastError();
        GL.BlitFramebuffer(
            0, 0, 1, 1,
            0, 0, 1, 1,
            ClearBufferMask.ColorBufferBit,
            BlitFramebufferFilter.Nearest);
        CheckLastError();

        GL.Disable(EnableCap.FramebufferSrgb);
        CheckLastError();

        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
        CheckLastError();
        GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, copySrcFb);
        CheckLastError();
        GL.BlitFramebuffer(
            0, 0, 1, 1,
            0, 0, 1, 1,
            ClearBufferMask.ColorBufferBit,
            BlitFramebufferFilter.Nearest);
        CheckLastError();
        if (_backendType == GraphicsBackend.OpenGLES)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, copySrc);
            CheckLastError();
            GL.ReadPixels(
                0, 0, 1, 1,
                GLPixelFormat.Rgba,
                GLPixelType.Float,
                (IntPtr)data);
            CheckLastError();
        }
        else
        {
            GL.GetTexImage(TextureTarget.Texture2D, 0, GLPixelFormat.Rgba, GLPixelType.Float, (IntPtr)data);
            CheckLastError();
        }

        GL.DeleteFramebuffers(1, ref copySrcFb);
        GL.DeleteTextures(1, ref copySrc);

        return data[0] > 0.6f;
    }

    public OpenGLGraphicsDevice(GraphicsDeviceOptions options, SwapchainDescription swapchainDescription)
    {
        options.SwapchainDepthFormat = swapchainDescription.DepthFormat;
        options.SwapchainSrgbFormat = swapchainDescription.ColorSrgb;
        options.SyncToVerticalBlank = swapchainDescription.SyncToVerticalBlank;

        SwapchainSource source = swapchainDescription.Source;
        
#if __IOS__ && !__MACCATALYST__
        if (source is UIViewSwapchainSource uiViewSource)
        {
            InitializeUIView(options, uiViewSource.UIView);
        }
#endif
        
#if __ANDROID__
        if (source is AndroidSurfaceSwapchainSource androidSource)
        {
            IntPtr aNativeWindow = Android.AndroidRuntime.ANativeWindow_fromSurface(
                androidSource.JniEnv,
                androidSource.Surface);
            InitializeANativeWindow(options, aNativeWindow, swapchainDescription);
        }
#endif
        {
            throw new VeldridException(
                "This function does not support creating an OpenGLES GraphicsDevice with the given SwapchainSource.");
        }
    }

    private class SimpleBindingsContext : IBindingsContext
    {
        private readonly Func<string, IntPtr> _getProcAddress;

        public SimpleBindingsContext(Func<string, IntPtr> getProcAddress) => 
            _getProcAddress = getProcAddress;

        public IntPtr GetProcAddress(string procName) => _getProcAddress(procName);
    }
    
    #if __IOS__ && !__MACCATALYST__
    private void InitializeUIView(GraphicsDeviceOptions options, IntPtr uIViewPtr)
    {
        EAGLContext eaglContext = new EAGLContext(EAGLRenderingAPI.OpenGLES3);
        if (!EAGLContext.SetCurrentContext(eaglContext))
        {
            throw new VeldridException("Unable to make newly-created EAGLContext current.");
        }

        UIView uiView = Runtime.GetNSObject<UIView>(uIViewPtr)!;
        CAEAGLLayer eaglLayer = new CAEAGLLayer();
        eaglLayer.Opaque = true;
        eaglLayer.Frame = uiView.Frame;
        uiView.Layer.AddSublayer(eaglLayer);
        
        IntPtr esLibrary = NativeLibrary.Load("/System/Library/Frameworks/OpenGLES.framework/OpenGLES");
        Func<string, IntPtr> getProcAddress = name => NativeLibrary.GetExport(esLibrary, name);

        _getProcAddress = getProcAddress;
        // LoadAllFunctions(eaglContext.NativePtr, getProcAddress, true);
        SimpleBindingsContext bindingsContext = new SimpleBindingsContext(getProcAddress);
        GL.LoadBindings(bindingsContext);
        
        GL.GenFramebuffers(1, out uint fb);
        CheckLastError();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fb);
        CheckLastError();

        GL.GenRenderbuffers(1, out uint colorRB);
        CheckLastError();

        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colorRB);
        CheckLastError();

        bool result = eaglContext.RenderBufferStorage((UIntPtr)RenderbufferTarget.Renderbuffer, eaglLayer);
        if (!result)
        {
            throw new VeldridException($"Failed to associate OpenGLES Renderbuffer with CAEAGLLayer.");
        }

        GL.GetRenderbufferParameter(
            RenderbufferTarget.Renderbuffer,
            RenderbufferParameterName.RenderbufferWidth,
            out int fbWidth);
        CheckLastError();

        GL.GetRenderbufferParameter(
            RenderbufferTarget.Renderbuffer,
            RenderbufferParameterName.RenderbufferHeight,
            out int fbHeight);
        CheckLastError();

        GL.FramebufferRenderbuffer(
            FramebufferTarget.Framebuffer,
            GLFramebufferAttachment.ColorAttachment0,
            RenderbufferTarget.Renderbuffer,
            colorRB);
        CheckLastError();

        uint depthRB = 0;
        bool hasDepth = options.SwapchainDepthFormat != null;
        if (hasDepth)
        {
            GL.GenRenderbuffers(1, out depthRB);
            CheckLastError();

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRB);
            CheckLastError();

            GL.RenderbufferStorage(
                RenderbufferTarget.Renderbuffer,
                (RenderbufferStorage)OpenGLFormats.VdToGLSizedInternalFormat(options.SwapchainDepthFormat.Value, true),
                fbWidth,
                fbHeight);
            CheckLastError();

            GL.FramebufferRenderbuffer(
                FramebufferTarget.Framebuffer,
                GLFramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer,
                depthRB);
            CheckLastError();
        }

        FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        CheckLastError();
        if (status != FramebufferErrorCode.FramebufferComplete)
        {
            throw new VeldridException($"The OpenGLES main Swapchain Framebuffer was incomplete after initialization.");
        }

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fb);
        CheckLastError();

        Action<IntPtr> setCurrentContext = ctx =>
        {
            EAGLContext context = Runtime.GetNSObject<EAGLContext>(ctx)!;
            if (!EAGLContext.SetCurrentContext(context))
            {
                throw new VeldridException($"Unable to set the thread's current GL context.");
            }
        };

        Action swapBuffers = () =>
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colorRB);
            CheckLastError();

            bool presentResult = eaglContext.PresentRenderBuffer((UIntPtr)RenderbufferTarget.Renderbuffer);
            CheckLastError();
            if (!presentResult)
            {
                throw new VeldridException($"Failed to present the EAGL RenderBuffer.");
            }
        };

        Action setSwapchainFramebuffer = () =>
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fb);
            CheckLastError();
        };

        Action<uint, uint> resizeSwapchain = (w, h) =>
        {
            eaglLayer.Frame = uiView.Frame;

            _executionThread.Run(() =>
            {
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colorRB);
                CheckLastError();

                bool rbStorageResult = eaglContext.RenderBufferStorage(
                    (UIntPtr)RenderbufferTarget.Renderbuffer,
                    eaglLayer);
                if (!rbStorageResult)
                {
                    throw new VeldridException($"Failed to associate OpenGLES Renderbuffer with CAEAGLLayer.");
                }

                GL.GetRenderbufferParameter(
                    RenderbufferTarget.Renderbuffer,
                    RenderbufferParameterName.RenderbufferWidth,
                    out int newWidth);
                CheckLastError();

                GL.GetRenderbufferParameter(
                    RenderbufferTarget.Renderbuffer,
                    RenderbufferParameterName.RenderbufferHeight,
                    out int newHeight);
                CheckLastError();

                if (hasDepth)
                {
                    Debug.Assert(depthRB != 0);
                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRB);
                    CheckLastError();

                    GL.RenderbufferStorage(
                        RenderbufferTarget.Renderbuffer,
                        (RenderbufferStorage)OpenGLFormats.VdToGLSizedInternalFormat(options.SwapchainDepthFormat.Value, true),
                        newWidth,
                        newHeight);
                    CheckLastError();
                }
            });
        };

        Action<IntPtr> destroyContext = ctx =>
        {
            eaglLayer.RemoveFromSuperLayer();
            eaglLayer.Dispose(); // Release();
            eaglContext.Dispose(); // Release();
            NativeLibrary.Free(esLibrary);
        };

        OpenGLPlatformInfo platformInfo = new OpenGLPlatformInfo(
            eaglContext.Handle.Handle,
            getProcAddress,
            setCurrentContext,
            () => EAGLContext.CurrentContext?.Handle.Handle ?? IntPtr.Zero,
            () => setCurrentContext(IntPtr.Zero),
            destroyContext,
            swapBuffers,
            syncInterval => { },
            setSwapchainFramebuffer,
            resizeSwapchain);

        Init(options, platformInfo, (uint)fbWidth, (uint)fbHeight, false);
    }
#endif

    #if __ANDROID__
    private void InitializeANativeWindow(
        GraphicsDeviceOptions options,
        IntPtr aNativeWindow,
        SwapchainDescription swapchainDescription)
    {
        IntPtr display = Egl.GetDisplay(0);
        if (display == IntPtr.Zero)
        {
            throw new VeldridException($"Failed to get the default Android EGLDisplay: {Egl.GetError()}");
        }

        if (!Egl.Initialize(display, out int major, out int minor))
        {
            throw new VeldridException($"Failed to initialize EGL: {Egl.GetError()}");
        }

        int[] attribs =
        {
            Egl.RED_SIZE, 8,
            Egl.GREEN_SIZE, 8,
            Egl.BLUE_SIZE, 8,
            Egl.ALPHA_SIZE, 8,
            Egl.DEPTH_SIZE,
            swapchainDescription.DepthFormat != null
                ? GetDepthBits(swapchainDescription.DepthFormat.Value)
                : 0,
            Egl.SURFACE_TYPE, Egl.WINDOW_BIT,
            Egl.RENDERABLE_TYPE, Egl.OPENGL_ES3_BIT,
            Egl.NONE,
        };

        IntPtr[] configs = new IntPtr[50];
        
        if (!Egl.ChooseConfig(display, attribs, configs, 50, out int num_config))
        {
            throw new VeldridException($"Failed to select a valid EGLConfig: {Egl.GetError()}");
        }

        IntPtr bestConfig = configs[0];

        int format;
        // if (eglGetConfigAttrib(display, bestConfig, EGL_NATIVE_VISUAL_ID, &format) == 0)
        if (!Egl.GetConfigAttrib(display, bestConfig, Egl.NATIVE_VISUAL_ID, out format))
        {
            throw new VeldridException($"Failed to get the EGLConfig's format: {Egl.GetError()}");
        }

        Android.AndroidRuntime.ANativeWindow_setBuffersGeometry(aNativeWindow, 0, 0, format);

        IntPtr eglWindowSurface = Egl.CreateWindowSurface(display, bestConfig, aNativeWindow, 0);
        if (eglWindowSurface == IntPtr.Zero)
        {
            throw new VeldridException(
                $"Failed to create an EGL surface from the Android native window: {Egl.GetError()}");
        }

        int[] contextAttribs = new int[3];
        contextAttribs[0] = Egl.CONTEXT_CLIENT_VERSION;
        contextAttribs[1] = 2;
        contextAttribs[2] = Egl.NONE;
        IntPtr context = Egl.CreateContext(display, bestConfig, IntPtr.Zero, contextAttribs);
        if (context == IntPtr.Zero)
        {
            throw new VeldridException($"Failed to create an EGLContext: " + Egl.GetError());
        }

        Action<IntPtr> makeCurrent = ctx =>
        {
            if (!Egl.MakeCurrent(display, eglWindowSurface, eglWindowSurface, ctx))
            {
                throw new VeldridException($"Failed to make the EGLContext {ctx} current: {Egl.GetError()}");
            }
        };

        makeCurrent(context);

        Action clearContext = () =>
        {
            if (!Egl.MakeCurrent(display, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
            {
                throw new VeldridException("Failed to clear the current EGLContext: " + Egl.GetError());
            }
        };

        Action swapBuffers = () =>
        {
            if (!Egl.SwapBuffers(display, eglWindowSurface))
            {
                throw new VeldridException("Failed to swap buffers: " + Egl.GetError());
            }
        };

        Action<bool> setSync = vsync =>
        {
            if (!Egl.SwapInterval(display, vsync ? 1 : 0))
            {
                throw new VeldridException($"Failed to set the swap interval: " + Egl.GetError());
            }
        };

        // Set the desired initial state.
        setSync(swapchainDescription.SyncToVerticalBlank);

        Action<IntPtr> destroyContext = ctx =>
        {
            if (!Egl.DestroyContext(display, ctx))
            {
                throw new VeldridException($"Failed to destroy EGLContext {ctx}: {Egl.GetError()}");
            }
        };

        OpenGLPlatformInfo platformInfo = new OpenGLPlatformInfo(
            context,
            Egl.GetProcAddress,
            makeCurrent,
            Egl.GetCurrentContext,
            clearContext,
            destroyContext,
            swapBuffers,
            setSync);

        Init(options, platformInfo, swapchainDescription.Width, swapchainDescription.Height, true);
    }
#endif

    private static int GetDepthBits(PixelFormat value)
    {
        switch (value)
        {
            case PixelFormat.R16_UNorm:
                return 16;
            case PixelFormat.R32_Float:
                return 32;
            default:
                throw new VeldridException($"Unsupported depth format: {value}");
        }
    }

    protected override void SubmitCommandsCore(
        CommandList cl,
        Fence fence)
    {
        lock (_commandListDisposalLock)
        {
            OpenGLCommandList glCommandList = Util.AssertSubtype<CommandList, OpenGLCommandList>(cl);
            OpenGLCommandEntryList entryList = glCommandList.CurrentCommands;
            IncrementCount(glCommandList);
            _executionThread.ExecuteCommands(entryList);
            if (fence is OpenGLFence glFence)
            {
                glFence.Set();
            }
        }
    }

    private int IncrementCount(OpenGLCommandList glCommandList)
    {
        if (_submittedCommandListCounts.TryGetValue(glCommandList, out int count))
        {
            count += 1;
        }
        else
        {
            count = 1;
        }

        _submittedCommandListCounts[glCommandList] = count;
        return count;
    }

    private int DecrementCount(OpenGLCommandList glCommandList)
    {
        if (_submittedCommandListCounts.TryGetValue(glCommandList, out int count))
        {
            count -= 1;
        }
        else
        {
            count = -1;
        }

        if (count == 0)
        {
            _submittedCommandListCounts.Remove(glCommandList);
        }
        else
        {
            _submittedCommandListCounts[glCommandList] = count;
        }
        return count;
    }

    private int GetCount(OpenGLCommandList glCommandList)
    {
        return _submittedCommandListCounts.TryGetValue(glCommandList, out int count) ? count : 0;
    }

    protected override void SwapBuffersCore(Swapchain swapchain)
    {
        WaitForIdle();

        _executionThread.SwapBuffers();
    }

    protected override void WaitForIdleCore()
    {
        _executionThread.WaitForIdle();
    }

    public override TextureSampleCount GetSampleCountLimit(PixelFormat format, bool depthFormat)
    {
        return _maxColorTextureSamples;
    }

    protected override bool GetPixelFormatSupportCore(
        PixelFormat format,
        TextureType type,
        TextureUsage usage,
        out PixelFormatProperties properties)
    {
        if (type == TextureType.Texture1D && !_features.Texture1D
            || !OpenGLFormats.IsFormatSupported(_extensions, format, _backendType))
        {
            properties = default(PixelFormatProperties);
            return false;
        }

        uint sampleCounts = 0;
        int max = (int)_maxColorTextureSamples + 1;
        for (int i = 0; i < max; i++)
        {
            sampleCounts |= (uint)(1 << i);
        }

        properties = new PixelFormatProperties(
            _maxTextureSize,
            type == TextureType.Texture1D ? 1 : _maxTextureSize,
            type != TextureType.Texture3D ? 1 : _maxTexDepth,
            uint.MaxValue,
            type == TextureType.Texture3D ? 1 : _maxTexArrayLayers,
            sampleCounts);
        return true;
    }

    protected override MappedResource MapCore(MappableResource resource, MapMode mode, uint subresource)
    {
        MappedResourceCacheKey key = new MappedResourceCacheKey(resource, subresource);
        lock (_mappedResourceLock)
        {
            if (_mappedResources.TryGetValue(key, out MappedResourceInfoWithStaging info))
            {
                if (info.Mode != mode)
                {
                    throw new VeldridException("The given resource was already mapped with a different MapMode.");
                }

                info.RefCount += 1;
                _mappedResources[key] = info;
                return info.MappedResource;
            }
        }

        return _executionThread.Map(resource, mode, subresource);
    }

    protected override void UnmapCore(MappableResource resource, uint subresource)
    {
        _executionThread.Unmap(resource, subresource);
    }

    protected override void UpdateBufferCore(DeviceBuffer buffer, uint bufferOffsetInBytes, IntPtr source, uint sizeInBytes)
    {
        lock (_mappedResourceLock)
        {
            if (_mappedResources.ContainsKey(new MappedResourceCacheKey(buffer, 0)))
            {
                throw new VeldridException("Cannot call UpdateBuffer on a currently-mapped Buffer.");
            }
        }
        StagingBlock sb = _stagingMemoryPool.Stage(source, sizeInBytes);
        _executionThread.UpdateBuffer(buffer, bufferOffsetInBytes, sb);
    }

    protected override void UpdateTextureCore(
        Texture texture,
        IntPtr source,
        uint sizeInBytes,
        uint x,
        uint y,
        uint z,
        uint width,
        uint height,
        uint depth,
        uint mipLevel,
        uint arrayLayer)
    {
        StagingBlock textureData = _stagingMemoryPool.Stage(source, sizeInBytes);
        StagingBlock argBlock = _stagingMemoryPool.GetStagingBlock(UpdateTextureArgsSize);
        ref UpdateTextureArgs args = ref Unsafe.AsRef<UpdateTextureArgs>(argBlock.Data);
        args.Data = (IntPtr)textureData.Data;
        args.X = x;
        args.Y = y;
        args.Z = z;
        args.Width = width;
        args.Height = height;
        args.Depth = depth;
        args.MipLevel = mipLevel;
        args.ArrayLayer = arrayLayer;

        _executionThread.UpdateTexture(texture, argBlock.Id, textureData.Id);
    }

    private static readonly uint UpdateTextureArgsSize = (uint)Unsafe.SizeOf<UpdateTextureArgs>();

    private struct UpdateTextureArgs
    {
        public IntPtr Data;
        public uint X;
        public uint Y;
        public uint Z;
        public uint Width;
        public uint Height;
        public uint Depth;
        public uint MipLevel;
        public uint ArrayLayer;
    }

    public override bool WaitForFence(Fence fence, ulong nanosecondTimeout)
    {
        return Util.AssertSubtype<Fence, OpenGLFence>(fence).Wait(nanosecondTimeout);
    }

    public override bool WaitForFences(Fence[] fences, bool waitAll, ulong nanosecondTimeout)
    {
        int msTimeout;
        if (nanosecondTimeout == ulong.MaxValue)
        {
            msTimeout = -1;
        }
        else
        {
            msTimeout = (int)Math.Min(nanosecondTimeout / 1_000_000, int.MaxValue);
        }

        ManualResetEvent[] events = GetResetEventArray(fences.Length);
        for (int i = 0; i < fences.Length; i++)
        {
            events[i] = Util.AssertSubtype<Fence, OpenGLFence>(fences[i]).ResetEvent;
        }
        bool result;
        if (waitAll)
        {
            result = WaitHandle.WaitAll(events, msTimeout);
        }
        else
        {
            int index = WaitHandle.WaitAny(events, msTimeout);
            result = index != WaitHandle.WaitTimeout;
        }

        ReturnResetEventArray(events);

        return result;
    }

    private ManualResetEvent[] GetResetEventArray(int length)
    {
        lock (_resetEventsLock)
        {
            for (int i = _resetEvents.Count - 1; i > 0; i--)
            {
                ManualResetEvent[] array = _resetEvents[i];
                if (array.Length == length)
                {
                    _resetEvents.RemoveAt(i);
                    return array;
                }
            }
        }

        ManualResetEvent[] newArray = new ManualResetEvent[length];
        return newArray;
    }

    private void ReturnResetEventArray(ManualResetEvent[] array)
    {
        lock (_resetEventsLock)
        {
            _resetEvents.Add(array);
        }
    }

    public override void ResetFence(Fence fence)
    {
        Util.AssertSubtype<Fence, OpenGLFence>(fence).Reset();
    }

    internal void EnqueueDisposal(OpenGLDeferredResource resource)
    {
        _resourcesToDispose.Enqueue(resource);
    }

    internal void EnqueueDisposal(OpenGLCommandList commandList)
    {
        lock (_commandListDisposalLock)
        {
            if (GetCount(commandList) > 0)
            {
                _commandListsToDispose.Add(commandList);
            }
            else
            {
                commandList.DestroyResources();
            }
        }
    }

    internal bool CheckCommandListDisposal(OpenGLCommandList commandList)
    {

        lock (_commandListDisposalLock)
        {
            int count = DecrementCount(commandList);
            if (count == 0)
            {
                if (_commandListsToDispose.Remove(commandList))
                {
                    commandList.DestroyResources();
                    return true;
                }
            }

            return false;
        }
    }

    private void FlushDisposables()
    {
        while (_resourcesToDispose.TryDequeue(out OpenGLDeferredResource resource))
        {
            resource.DestroyGLResources();
        }
    }

    public void EnableDebugCallback() => EnableDebugCallback(DebugSeverity.DebugSeverityNotification);
    public void EnableDebugCallback(DebugSeverity minimumSeverity) => EnableDebugCallback(DefaultDebugCallback(minimumSeverity));
    public void EnableDebugCallback(DebugProc callback)
    {
        GL.Enable(EnableCap.DebugOutput);
        CheckLastError();
        // The debug callback delegate must be persisted, otherwise errors will occur
        // when the OpenGL drivers attempt to call it after it has been collected.
        _debugMessageCallback = callback;
        GL.DebugMessageCallback(_debugMessageCallback, 0);
        CheckLastError();
    }

    private DebugProc DefaultDebugCallback(DebugSeverity minimumSeverity)
    {
        return (source, type, id, severity, length, message, userParam) =>
        {
            if (severity >= minimumSeverity
                && type != DebugType.DebugTypeMarker
                && type != DebugType.DebugTypePushGroup
                && type != DebugType.DebugTypePopGroup)
            {
                string messageString = Marshal.PtrToStringAnsi((IntPtr)message, (int)length);
                Debug.WriteLine($"GL DEBUG MESSAGE: {source}, {type}, {id}. {severity}: {messageString}");
            }
        };
    }

    protected override void PlatformDispose()
    {
        FlushAndFinish();
        _executionThread.Terminate();
    }

    internal void ExecuteOnGLThread(Action action)
    {
        _executionThread.Run(action);
        _executionThread.WaitForIdle();
    }

    internal void FlushAndFinish()
    {
        _executionThread.FlushAndFinish();
    }

    internal void EnsureResourceInitialized(OpenGLDeferredResource deferredResource)
    {
        _executionThread.InitializeResource(deferredResource);
    }

    protected override uint GetUniformBufferMinOffsetAlignmentCore() => _minUboOffsetAlignment;

    protected override uint GetStructuredBufferMinOffsetAlignmentCore() => _minSsboOffsetAlignment;

    private class ExecutionThread
    {
        private readonly OpenGLGraphicsDevice _gd;
        private readonly BlockingCollection<ExecutionThreadWorkItem> _workItems;
        private readonly Action<IntPtr> _makeCurrent;
        private readonly IntPtr _context;
        private bool _terminated;
        private readonly List<Exception> _exceptions = new List<Exception>();
        private readonly object _exceptionsLock = new object();

        public ExecutionThread(
            OpenGLGraphicsDevice gd,
            BlockingCollection<ExecutionThreadWorkItem> workItems,
            Action<IntPtr> makeCurrent,
            IntPtr context)
        {
            _gd = gd;
            _workItems = workItems;
            _makeCurrent = makeCurrent;
            _context = context;
            Thread thread = new Thread(Run);
            thread.IsBackground = true;
            thread.Start();
        }

        private void Run()
        {
            _makeCurrent(_context);
            while (!_terminated)
            {
                ExecutionThreadWorkItem workItem = _workItems.Take();
                ExecuteWorkItem(workItem);
            }
        }

        private void ExecuteWorkItem(ExecutionThreadWorkItem workItem)
        {
            try
            {
                switch (workItem.Type)
                {
                    case WorkItemType.ExecuteList:
                    {
                        OpenGLCommandEntryList list = (OpenGLCommandEntryList)workItem.Object0;
                        try
                        {
                            list.ExecuteAll(_gd._commandExecutor);
                        }
                        finally
                        {
                            if (!_gd.CheckCommandListDisposal(list.Parent))
                            {
                                list.Parent.OnCompleted(list);
                            }
                        }
                    }
                        break;
                    case WorkItemType.Map:
                    {
                        MappableResource resourceToMap = (MappableResource)workItem.Object0;
                        ManualResetEventSlim mre = (ManualResetEventSlim)workItem.Object1;

                        MapParams* resultPtr = (MapParams*)Util.UnpackIntPtr(workItem.UInt0, workItem.UInt1);

                        if (resultPtr->Map)
                        {
                            ExecuteMapResource(
                                resourceToMap,
                                mre,
                                resultPtr);
                        }
                        else
                        {
                            ExecuteUnmapResource(resourceToMap, resultPtr->Subresource, mre);
                        }
                    }
                        break;
                    case WorkItemType.UpdateBuffer:
                    {
                        DeviceBuffer updateBuffer = (DeviceBuffer)workItem.Object0;
                        uint offsetInBytes = workItem.UInt0;
                        StagingBlock stagingBlock = _gd.StagingMemoryPool.RetrieveById(workItem.UInt1);

                        _gd._commandExecutor.UpdateBuffer(
                            updateBuffer,
                            offsetInBytes,
                            (IntPtr)stagingBlock.Data,
                            stagingBlock.SizeInBytes);

                        _gd.StagingMemoryPool.Free(stagingBlock);
                    }
                        break;
                    case WorkItemType.UpdateTexture:
                        Texture texture = (Texture)workItem.Object0;
                        StagingMemoryPool pool = _gd.StagingMemoryPool;
                        StagingBlock argBlock = pool.RetrieveById(workItem.UInt0);
                        StagingBlock textureData = pool.RetrieveById(workItem.UInt1);
                        ref UpdateTextureArgs args = ref Unsafe.AsRef<UpdateTextureArgs>(argBlock.Data);

                        _gd._commandExecutor.UpdateTexture(
                            texture, args.Data, args.X, args.Y, args.Z,
                            args.Width, args.Height, args.Depth, args.MipLevel, args.ArrayLayer);

                        pool.Free(argBlock);
                        pool.Free(textureData);
                        break;
                    case WorkItemType.GenericAction:
                    {
                        ((Action)workItem.Object0)();
                    }
                        break;
                    case WorkItemType.TerminateAction:
                    {
                        // Check if the OpenGL context has already been destroyed by the OS. If so, just exit out.
                        ErrorCode error = GL.GetError();
                        if (error == ErrorCode.InvalidOperation)
                        {
                            return;
                        }
                        _makeCurrent(_gd._glContext);

                        _gd.FlushDisposables();
                        _gd._deleteContext(_gd._glContext);
                        _gd.StagingMemoryPool.Dispose();
                        _terminated = true;
                    }
                        break;
                    case WorkItemType.SetSyncToVerticalBlank:
                    {
                        bool value = workItem.UInt0 == 1 ? true : false;
                        _gd._setSyncToVBlank(value);
                    }
                        break;
                    case WorkItemType.SwapBuffers:
                    {
                        _gd._swapBuffers();
                        _gd.FlushDisposables();
                    }
                        break;
                    case WorkItemType.WaitForIdle:
                    {
                        _gd.FlushDisposables();
                        bool isFullFlush = workItem.UInt0 != 0;
                        if (isFullFlush)
                        {
                            GL.Flush();
                            GL.Finish();
                        }
                        ((ManualResetEventSlim)workItem.Object0).Set();
                    }
                        break;
                    case WorkItemType.InitializeResource:
                    {
                        InitializeResourceInfo info = (InitializeResourceInfo)workItem.Object0;
                        try
                        {
                            info.DeferredResource.EnsureResourcesCreated();
                        }
                        catch (Exception e)
                        {
                            info.Exception = e;
                        }
                        finally
                        {
                            info.ResetEvent.Set();
                        }
                    }
                        break;
                    default:
                        throw new InvalidOperationException("Invalid command type: " + workItem.Type);
                }
            }
            catch (Exception e) when (!Debugger.IsAttached)
            {
                lock (_exceptionsLock)
                {
                    _exceptions.Add(e);
                }
            }
        }

        private void ExecuteMapResource(
            MappableResource resource,
            ManualResetEventSlim mre,
            MapParams* result)
        {
            uint subresource = result->Subresource;
            MapMode mode = result->MapMode;

            MappedResourceCacheKey key = new MappedResourceCacheKey(resource, subresource);
            try
            {
                lock (_gd._mappedResourceLock)
                {
                    Debug.Assert(!_gd._mappedResources.ContainsKey(key));
                    if (resource is OpenGLBuffer buffer)
                    {
                        buffer.EnsureResourcesCreated();
                        void* mappedPtr;
                        BufferAccessMask accessMask = OpenGLFormats.VdToGLMapMode(mode);
                        if (_gd.Extensions.ARB_DirectStateAccess)
                        {
                            mappedPtr = (void*)GL.MapNamedBufferRange(buffer.Buffer, IntPtr.Zero, (int)buffer.SizeInBytes, accessMask);
                            CheckLastError();
                        }
                        else
                        {
                            GL.BindBuffer(BufferTarget.CopyWriteBuffer, buffer.Buffer);
                            CheckLastError();

                            mappedPtr = (void*)GL.MapBufferRange(BufferTarget.CopyWriteBuffer, IntPtr.Zero, (IntPtr)buffer.SizeInBytes, accessMask);
                            CheckLastError();
                        }

                        MappedResourceInfoWithStaging info = new MappedResourceInfoWithStaging();
                        info.MappedResource = new MappedResource(
                            resource,
                            mode,
                            (IntPtr)mappedPtr,
                            buffer.SizeInBytes);
                        info.RefCount = 1;
                        info.Mode = mode;
                        _gd._mappedResources.Add(key, info);
                        result->Data = (IntPtr)mappedPtr;
                        result->DataSize = buffer.SizeInBytes;
                        result->RowPitch = 0;
                        result->DepthPitch = 0;
                        result->Succeeded = true;
                    }
                    else
                    {
                        OpenGLTexture texture = Util.AssertSubtype<MappableResource, OpenGLTexture>(resource);
                        texture.EnsureResourcesCreated();

                        Util.GetMipLevelAndArrayLayer(texture, subresource, out uint mipLevel, out uint arrayLayer);
                        Util.GetMipDimensions(texture, mipLevel, out uint mipWidth, out uint mipHeight, out uint mipDepth);

                        uint depthSliceSize = FormatHelpers.GetDepthPitch(
                            FormatHelpers.GetRowPitch(mipWidth, texture.Format),
                            mipHeight,
                            texture.Format);
                        uint subresourceSize = depthSliceSize * mipDepth;
                        int compressedSize = 0;

                        bool isCompressed = FormatHelpers.IsCompressedFormat(texture.Format);
                        if (isCompressed)
                        {
                            GL.GetTexLevelParameter(
                                texture.TextureTarget,
                                (int)mipLevel,
                                GetTextureParameter.TextureCompressedImageSize,
                                &compressedSize);
                            CheckLastError();
                        }

                        StagingBlock block = _gd._stagingMemoryPool.GetStagingBlock(subresourceSize);

                        uint packAlignment = 4;
                        if (!isCompressed)
                        {
                            packAlignment = FormatSizeHelpers.GetSizeInBytes(texture.Format);
                        }

                        if (packAlignment < 4)
                        {
                            GL.PixelStore(PixelStoreParameter.PackAlignment, (int)packAlignment);
                            CheckLastError();
                        }

                        if (mode == MapMode.Read || mode == MapMode.ReadWrite)
                        {
                            if (!isCompressed)
                            {
                                // Read data into buffer.
                                if (_gd.Extensions.ARB_DirectStateAccess && texture.ArrayLayers == 1)
                                {
                                    int zoffset = texture.ArrayLayers > 1 ? (int)arrayLayer : 0;
                                    GL.GetTextureSubImage(
                                        texture.Texture,
                                        (int)mipLevel,
                                        0, 0, zoffset,
                                        (int)mipWidth, (int)mipHeight, (int)mipDepth,
                                        texture.GLPixelFormat,
                                        texture.GLPixelType,
                                        (int)subresourceSize,
                                        (IntPtr)block.Data);
                                    CheckLastError();
                                }
                                else
                                {
                                    for (uint layer = 0; layer < mipDepth; layer++)
                                    {
                                        uint curLayer = arrayLayer + layer;
                                        uint curOffset = depthSliceSize * layer;
                                        GL.GenFramebuffers(1, out uint readFB);
                                        CheckLastError();
                                        GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, readFB);
                                        CheckLastError();

                                        if (texture.ArrayLayers > 1 || texture.Type == TextureType.Texture3D)
                                        {
                                            GL.FramebufferTextureLayer(
                                                FramebufferTarget.ReadFramebuffer,
                                                GLFramebufferAttachment.ColorAttachment0,
                                                texture.Texture,
                                                (int)mipLevel,
                                                (int)curLayer);
                                            CheckLastError();
                                        }
                                        else if (texture.Type == TextureType.Texture1D)
                                        {
                                            GL.FramebufferTexture1D(
                                                FramebufferTarget.ReadFramebuffer,
                                                GLFramebufferAttachment.ColorAttachment0,
                                                TextureTarget.Texture1D,
                                                texture.Texture,
                                                (int)mipLevel);
                                            CheckLastError();
                                        }
                                        else
                                        {
                                            GL.FramebufferTexture2D(
                                                FramebufferTarget.ReadFramebuffer,
                                                GLFramebufferAttachment.ColorAttachment0,
                                                TextureTarget.Texture2D,
                                                texture.Texture,
                                                (int)mipLevel);
                                            CheckLastError();
                                        }

                                        GL.ReadPixels(
                                            0, 0,
                                            (int)mipWidth, (int)mipHeight,
                                            texture.GLPixelFormat,
                                            texture.GLPixelType,
                                            (IntPtr)((byte*)block.Data + curOffset));
                                        CheckLastError();
                                        GL.DeleteFramebuffers(1, ref readFB);
                                        CheckLastError();
                                    }
                                }
                            }
                            else // isCompressed
                            {
                                if (texture.TextureTarget == TextureTarget.Texture2DArray
                                    || texture.TextureTarget == TextureTarget.Texture2DMultisampleArray
                                    || texture.TextureTarget == TextureTarget.TextureCubeMapArray)
                                {
                                    // We only want a single subresource (array slice), so we need to copy
                                    // a subsection of the downloaded data into our staging block.

                                    uint fullDataSize = (uint)compressedSize;
                                    StagingBlock fullBlock = _gd._stagingMemoryPool.GetStagingBlock(fullDataSize);

                                    if (_gd.Extensions.ARB_DirectStateAccess)
                                    {
                                        GL.GetCompressedTextureImage(
                                            texture.Texture,
                                            (int)mipLevel,
                                            (int)fullBlock.SizeInBytes,
                                            (IntPtr)fullBlock.Data);
                                        CheckLastError();
                                    }
                                    else
                                    {
                                        _gd.TextureSamplerManager.SetTextureTransient(texture.TextureTarget, texture.Texture);
                                        CheckLastError();

                                        GL.GetCompressedTexImage(texture.TextureTarget, (int)mipLevel, (IntPtr)fullBlock.Data);
                                        CheckLastError();
                                    }
                                    byte* sliceStart = (byte*)fullBlock.Data + (arrayLayer * subresourceSize);
                                    Buffer.MemoryCopy(sliceStart, block.Data, subresourceSize, subresourceSize);
                                    _gd._stagingMemoryPool.Free(fullBlock);
                                }
                                else
                                {
                                    if (_gd.Extensions.ARB_DirectStateAccess)
                                    {
                                        GL.GetCompressedTextureImage(
                                            texture.Texture,
                                            (int)mipLevel,
                                            (int)block.SizeInBytes,
                                            (IntPtr)block.Data);
                                        CheckLastError();
                                    }
                                    else
                                    {
                                        _gd.TextureSamplerManager.SetTextureTransient(texture.TextureTarget, texture.Texture);
                                        CheckLastError();

                                        GL.GetCompressedTexImage(texture.TextureTarget, (int)mipLevel, (IntPtr)block.Data);
                                        CheckLastError();
                                    }
                                }
                            }
                        }

                        if (packAlignment < 4)
                        {
                            GL.PixelStore(PixelStoreParameter.PackAlignment, 4);
                            CheckLastError();
                        }

                        uint rowPitch = FormatHelpers.GetRowPitch(mipWidth, texture.Format);
                        uint depthPitch = FormatHelpers.GetDepthPitch(rowPitch, mipHeight, texture.Format);
                        MappedResourceInfoWithStaging info = new MappedResourceInfoWithStaging();
                        info.MappedResource = new MappedResource(
                            resource,
                            mode,
                            (IntPtr)block.Data,
                            subresourceSize,
                            subresource,
                            rowPitch,
                            depthPitch);
                        info.RefCount = 1;
                        info.Mode = mode;
                        info.StagingBlock = block;
                        _gd._mappedResources.Add(key, info);
                        result->Data = (IntPtr)block.Data;
                        result->DataSize = subresourceSize;
                        result->RowPitch = rowPitch;
                        result->DepthPitch = depthPitch;
                        result->Succeeded = true;
                    }
                }
            }
            catch
            {
                result->Succeeded = false;
                throw;
            }
            finally
            {
                mre.Set();
            }
        }

        private void ExecuteUnmapResource(MappableResource resource, uint subresource, ManualResetEventSlim mre)
        {
            MappedResourceCacheKey key = new MappedResourceCacheKey(resource, subresource);
            lock (_gd._mappedResourceLock)
            {
                MappedResourceInfoWithStaging info = _gd._mappedResources[key];
                if (info.RefCount == 1)
                {
                    if (resource is OpenGLBuffer buffer)
                    {
                        if (_gd.Extensions.ARB_DirectStateAccess)
                        {
                            GL.UnmapNamedBuffer(buffer.Buffer);
                            CheckLastError();
                        }
                        else
                        {
                            GL.BindBuffer(BufferTarget.CopyWriteBuffer, buffer.Buffer);
                            CheckLastError();

                            GL.UnmapBuffer(BufferTarget.CopyWriteBuffer);
                            CheckLastError();
                        }
                    }
                    else
                    {
                        OpenGLTexture texture = Util.AssertSubtype<MappableResource, OpenGLTexture>(resource);

                        if (info.Mode == MapMode.Write || info.Mode == MapMode.ReadWrite)
                        {
                            Util.GetMipLevelAndArrayLayer(texture, subresource, out uint mipLevel, out uint arrayLayer);
                            Util.GetMipDimensions(texture, mipLevel, out uint width, out uint height, out uint depth);

                            IntPtr data = (IntPtr)info.StagingBlock.Data;

                            _gd._commandExecutor.UpdateTexture(
                                texture,
                                data,
                                0, 0, 0,
                                width, height, depth,
                                mipLevel,
                                arrayLayer);
                        }

                        _gd.StagingMemoryPool.Free(info.StagingBlock);
                    }

                    _gd._mappedResources.Remove(key);
                }
            }

            mre.Set();
        }

        private void CheckExceptions()
        {
            lock (_exceptionsLock)
            {
                if (_exceptions.Count > 0)
                {
                    Exception innerException = _exceptions.Count == 1
                        ? _exceptions[0]
                        : new AggregateException(_exceptions.ToArray());
                    _exceptions.Clear();
                    throw new VeldridException(
                        "Error(s) were encountered during the execution of OpenGL commands. See InnerException for more information.",
                        innerException);

                }
            }
        }

        public MappedResource Map(MappableResource resource, MapMode mode, uint subresource)
        {
            CheckExceptions();

            MapParams mrp = new MapParams();
            mrp.Map = true;
            mrp.Subresource = subresource;
            mrp.MapMode = mode;

            ManualResetEventSlim mre = new ManualResetEventSlim(false);
            _workItems.Add(new ExecutionThreadWorkItem(resource, &mrp, mre));
            mre.Wait();
            if (!mrp.Succeeded)
            {
                throw new VeldridException("Failed to map OpenGL resource.");
            }

            mre.Dispose();

            return new MappedResource(resource, mode, mrp.Data, mrp.DataSize, mrp.Subresource, mrp.RowPitch, mrp.DepthPitch);
        }

        internal void Unmap(MappableResource resource, uint subresource)
        {
            CheckExceptions();

            MapParams mrp = new MapParams();
            mrp.Map = false;
            mrp.Subresource = subresource;

            ManualResetEventSlim mre = new ManualResetEventSlim(false);
            _workItems.Add(new ExecutionThreadWorkItem(resource, &mrp, mre));
            mre.Wait();
            mre.Dispose();
        }

        public void ExecuteCommands(OpenGLCommandEntryList entryList)
        {
            CheckExceptions();
            entryList.Parent.OnSubmitted(entryList);
            _workItems.Add(new ExecutionThreadWorkItem(entryList));
        }

        internal void UpdateBuffer(DeviceBuffer buffer, uint offsetInBytes, StagingBlock stagingBlock)
        {
            CheckExceptions();

            _workItems.Add(new ExecutionThreadWorkItem(buffer, offsetInBytes, stagingBlock));
        }

        internal void UpdateTexture(Texture texture, uint argBlockId, uint dataBlockId)
        {
            CheckExceptions();

            _workItems.Add(new ExecutionThreadWorkItem(texture, argBlockId, dataBlockId));
        }

        internal void Run(Action a)
        {
            CheckExceptions();

            _workItems.Add(new ExecutionThreadWorkItem(a));
        }

        internal void Terminate()
        {
            CheckExceptions();

            _workItems.Add(new ExecutionThreadWorkItem(WorkItemType.TerminateAction));
        }

        internal void WaitForIdle()
        {
            ManualResetEventSlim mre = new ManualResetEventSlim();
            _workItems.Add(new ExecutionThreadWorkItem(mre, isFullFlush: false));
            mre.Wait();
            mre.Dispose();

            CheckExceptions();
        }

        internal void SetSyncToVerticalBlank(bool value)
        {
            _workItems.Add(new ExecutionThreadWorkItem(value));
        }

        internal void SwapBuffers()
        {
            _workItems.Add(new ExecutionThreadWorkItem(WorkItemType.SwapBuffers));
        }

        internal void FlushAndFinish()
        {
            ManualResetEventSlim mre = new ManualResetEventSlim();
            _workItems.Add(new ExecutionThreadWorkItem(mre, isFullFlush: true));
            mre.Wait();
            mre.Dispose();

            CheckExceptions();
        }

        internal void InitializeResource(OpenGLDeferredResource deferredResource)
        {
            InitializeResourceInfo info = new InitializeResourceInfo(deferredResource, new ManualResetEventSlim());
            _workItems.Add(new ExecutionThreadWorkItem(info));
            info.ResetEvent.Wait();
            info.ResetEvent.Dispose();

            if (info.Exception != null)
            {
                throw info.Exception;
            }
        }
    }

    public enum WorkItemType : byte
    {
        Map,
        Unmap,
        ExecuteList,
        UpdateBuffer,
        UpdateTexture,
        GenericAction,
        TerminateAction,
        SetSyncToVerticalBlank,
        SwapBuffers,
        WaitForIdle,
        InitializeResource,
    }

    private unsafe struct ExecutionThreadWorkItem
    {
        public readonly WorkItemType Type;
        public readonly object Object0;
        public readonly object Object1;
        public readonly uint UInt0;
        public readonly uint UInt1;
        public readonly uint UInt2;

        public ExecutionThreadWorkItem(
            MappableResource resource,
            MapParams* mapResult,
            ManualResetEventSlim resetEvent)
        {
            Type = WorkItemType.Map;
            Object0 = resource;
            Object1 = resetEvent;

            Util.PackIntPtr((IntPtr)mapResult, out UInt0, out UInt1);
            UInt2 = 0;
        }

        public ExecutionThreadWorkItem(OpenGLCommandEntryList commandList)
        {
            Type = WorkItemType.ExecuteList;
            Object0 = commandList;
            Object1 = null;

            UInt0 = 0;
            UInt1 = 0;
            UInt2 = 0;
        }

        public ExecutionThreadWorkItem(DeviceBuffer updateBuffer, uint offsetInBytes, StagingBlock stagedSource)
        {
            Type = WorkItemType.UpdateBuffer;
            Object0 = updateBuffer;
            Object1 = null;

            UInt0 = offsetInBytes;
            UInt1 = stagedSource.Id;
            UInt2 = 0;
        }

        public ExecutionThreadWorkItem(Action a, bool isTermination = false)
        {
            Type = isTermination ? WorkItemType.TerminateAction : WorkItemType.GenericAction;
            Object0 = a;
            Object1 = null;

            UInt0 = 0;
            UInt1 = 0;
            UInt2 = 0;
        }

        public ExecutionThreadWorkItem(Texture texture, uint argBlockId, uint dataBlockId)
        {
            Type = WorkItemType.UpdateTexture;
            Object0 = texture;
            Object1 = null;

            UInt0 = argBlockId;
            UInt1 = dataBlockId;
            UInt2 = 0;
        }

        public ExecutionThreadWorkItem(ManualResetEventSlim mre, bool isFullFlush)
        {
            Type = WorkItemType.WaitForIdle;
            Object0 = mre;
            Object1 = null;

            UInt0 = isFullFlush ? 1u : 0u;
            UInt1 = 0;
            UInt2 = 0;
        }

        public ExecutionThreadWorkItem(bool value)
        {
            Type = WorkItemType.SetSyncToVerticalBlank;
            Object0 = null;
            Object1 = null;

            UInt0 = value ? 1u : 0u;
            UInt1 = 0;
            UInt2 = 0;
        }

        public ExecutionThreadWorkItem(WorkItemType type)
        {
            Type = type;
            Object0 = null;
            Object1 = null;

            UInt0 = 0;
            UInt1 = 0;
            UInt2 = 0;
        }

        public ExecutionThreadWorkItem(InitializeResourceInfo info)
        {
            Type = WorkItemType.InitializeResource;
            Object0 = info;
            Object1 = null;

            UInt0 = 0;
            UInt1 = 0;
            UInt2 = 0;
        }
    }

    private struct MapParams
    {
        public MapMode MapMode;
        public uint Subresource;
        public bool Map;
        public bool Succeeded;
        public IntPtr Data;
        public uint DataSize;
        public uint RowPitch;
        public uint DepthPitch;
    }

    internal struct MappedResourceInfoWithStaging
    {
        public int RefCount;
        public MapMode Mode;
        public MappedResource MappedResource;
        public StagingBlock StagingBlock;
    }

    private class InitializeResourceInfo
    {
        public OpenGLDeferredResource DeferredResource;
        public ManualResetEventSlim ResetEvent;
        public Exception Exception;

        public InitializeResourceInfo(OpenGLDeferredResource deferredResource, ManualResetEventSlim mre)
        {
            DeferredResource = deferredResource;
            ResetEvent = mre;
        }
    }
}