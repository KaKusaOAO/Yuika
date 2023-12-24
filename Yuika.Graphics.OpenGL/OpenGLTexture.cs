using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using static Yuika.Graphics.OpenGL.OpenGLUtil;

using GLPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using GLPixelType = OpenTK.Graphics.OpenGL4.PixelType;
using GLFramebufferAttachment = OpenTK.Graphics.OpenGL4.FramebufferAttachment;

namespace Yuika.Graphics.OpenGL;

internal unsafe class OpenGLTexture : Texture, OpenGLDeferredResource
{
    private readonly OpenGLGraphicsDevice _gd;
    private uint _texture;
    private uint[] _framebuffers;
    private uint[] _pbos;
    private uint[] _pboSizes;
    private bool _disposeRequested;
    private bool _disposed;

    private string _name;
    private bool _nameChanged;
        
    public override string Name { get => _name; set { _name = value; _nameChanged = true; } }

    public uint Texture => _texture;

    public OpenGLTexture(OpenGLGraphicsDevice gd, ref TextureDescription description)
    {
        _gd = gd;

        Width = description.Width;
        Height = description.Height;
        Depth = description.Depth;
        Format = description.Format;
        MipLevels = description.MipLevels;
        ArrayLayers = description.ArrayLayers;
        Usage = description.Usage;
        Type = description.Type;
        SampleCount = description.SampleCount;

        _framebuffers = new uint[MipLevels * ArrayLayers];
        _pbos = new uint[MipLevels * ArrayLayers];
        _pboSizes = new uint[MipLevels * ArrayLayers];

        GLPixelFormat = OpenGLFormats.VdToGLPixelFormat(Format);
        GLPixelType = OpenGLFormats.VdToGLPixelType(Format);
        GLInternalFormat = OpenGLFormats.VdToGLPixelInternalFormat(Format);

        if ((Usage & TextureUsage.DepthStencil) == TextureUsage.DepthStencil)
        {
            GLPixelFormat = FormatHelpers.IsStencilFormat(Format)
                ? GLPixelFormat.DepthStencil
                : GLPixelFormat.DepthComponent;
            if (Format == PixelFormat.R16_UNorm)
            {
                GLInternalFormat = PixelInternalFormat.DepthComponent16;
            }
            else if (Format == PixelFormat.R32_Float)
            {
                GLInternalFormat = PixelInternalFormat.DepthComponent32f;
            }
        }

        if ((Usage & TextureUsage.Cubemap) == TextureUsage.Cubemap)
        {
            TextureTarget = ArrayLayers == 1 ? TextureTarget.TextureCubeMap : TextureTarget.TextureCubeMapArray;
        }
        else if (Type == TextureType.Texture1D)
        {
            TextureTarget = ArrayLayers == 1 ? TextureTarget.Texture1D : TextureTarget.Texture1DArray;
        }
        else if (Type == TextureType.Texture2D)
        {
            if (ArrayLayers == 1)
            {
                TextureTarget = SampleCount == TextureSampleCount.Count1 ? TextureTarget.Texture2D : TextureTarget.Texture2DMultisample;
            }
            else
            {
                TextureTarget = SampleCount == TextureSampleCount.Count1 ? TextureTarget.Texture2DArray : TextureTarget.Texture2DMultisampleArray;
            }
        }
        else
        {
            Debug.Assert(Type == TextureType.Texture3D);
            TextureTarget = TextureTarget.Texture3D;
        }
    }

    public OpenGLTexture(OpenGLGraphicsDevice gd, uint nativeTexture, ref TextureDescription description)
    {
        _gd = gd;
        _texture = nativeTexture;
        Width = description.Width;
        Height = description.Height;
        Depth = description.Depth;
        Format = description.Format;
        MipLevels = description.MipLevels;
        ArrayLayers = description.ArrayLayers;
        Usage = description.Usage;
        Type = description.Type;
        SampleCount = description.SampleCount;

        _framebuffers = new uint[MipLevels * ArrayLayers];
        _pbos = new uint[MipLevels * ArrayLayers];
        _pboSizes = new uint[MipLevels * ArrayLayers];

        GLPixelFormat = OpenGLFormats.VdToGLPixelFormat(Format);
        GLPixelType = OpenGLFormats.VdToGLPixelType(Format);
        GLInternalFormat = OpenGLFormats.VdToGLPixelInternalFormat(Format);

        if ((Usage & TextureUsage.DepthStencil) == TextureUsage.DepthStencil)
        {
            GLPixelFormat = FormatHelpers.IsStencilFormat(Format)
                ? GLPixelFormat.DepthStencil
                : GLPixelFormat.DepthComponent;
            if (Format == PixelFormat.R16_UNorm)
            {
                GLInternalFormat = PixelInternalFormat.DepthComponent16;
            }
            else if (Format == PixelFormat.R32_Float)
            {
                GLInternalFormat = PixelInternalFormat.DepthComponent32f;
            }
        }

        if ((Usage & TextureUsage.Cubemap) == TextureUsage.Cubemap)
        {
            TextureTarget = ArrayLayers == 1 ? TextureTarget.TextureCubeMap : TextureTarget.TextureCubeMapArray;
        }
        else if (Type == TextureType.Texture1D)
        {
            TextureTarget = ArrayLayers == 1 ? TextureTarget.Texture1D : TextureTarget.Texture1DArray;
        }
        else if (Type == TextureType.Texture2D)
        {
            if (ArrayLayers == 1)
            {
                TextureTarget = SampleCount == TextureSampleCount.Count1 ? TextureTarget.Texture2D : TextureTarget.Texture2DMultisample;
            }
            else
            {
                TextureTarget = SampleCount == TextureSampleCount.Count1 ? TextureTarget.Texture2DArray : TextureTarget.Texture2DMultisampleArray;
            }
        }
        else
        {
            Debug.Assert(Type == TextureType.Texture3D);
            TextureTarget = TextureTarget.Texture3D;
        }

        Created = true;
    }

    public override uint Width { get; }

    public override uint Height { get; }

    public override uint Depth { get; }

    public override PixelFormat Format { get; }

    public override uint MipLevels { get; }

    public override uint ArrayLayers { get; }

    public override TextureUsage Usage { get; }

    public override TextureType Type { get; }

    public override TextureSampleCount SampleCount { get; }

    public override bool IsDisposed => _disposeRequested;

    public GLPixelFormat GLPixelFormat { get; }
    public GLPixelType GLPixelType { get; }
    public PixelInternalFormat GLInternalFormat { get; }
    public TextureTarget TextureTarget { get; internal set; }

    public bool Created { get; private set; }

    public void EnsureResourcesCreated()
    {
        if (!Created)
        {
            CreateGLResources();
        }
        if (_nameChanged)
        {
            _nameChanged = false;
            if (_gd.Extensions.KHR_Debug)
            {
                SetObjectLabel(ObjectLabelIdentifier.Texture, _texture, _name);
            }
        }
    }

    private void CreateGLResources()
    {
        bool dsa = _gd.Extensions.ARB_DirectStateAccess;
        if (dsa)
        {
            uint texture;
            GL.CreateTextures(TextureTarget, 1, &texture);
            CheckLastError();
            _texture = texture;
        }
        else
        {
            GL.GenTextures(1, out _texture);
            CheckLastError();

            _gd.TextureSamplerManager.SetTextureTransient(TextureTarget, _texture);
            CheckLastError();
        }

        bool isDepthTex = (Usage & TextureUsage.DepthStencil) == TextureUsage.DepthStencil;

        if (TextureTarget == TextureTarget.Texture1D)
        {
            if (dsa)
            {
                GL.TextureStorage1D(
                    _texture,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width);
                CheckLastError();
            }
            else if (_gd.Extensions.TextureStorage)
            {
                GL.TexStorage1D(
                    TextureTarget1d.Texture1D,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width);
                CheckLastError();
            }
            else
            {
                uint levelWidth = Width;
                for (int currentLevel = 0; currentLevel < MipLevels; currentLevel++)
                {
                    // Set size, load empty data into texture
                    GL.TexImage1D(
                        TextureTarget.Texture1D,
                        currentLevel,
                        GLInternalFormat,
                        (int) levelWidth,
                        0, // border
                        GLPixelFormat,
                        GLPixelType,
                        0);
                    CheckLastError();

                    levelWidth = Math.Max(1, levelWidth / 2);
                }
            }
        }
        else if (TextureTarget == TextureTarget.Texture2D || TextureTarget == TextureTarget.Texture1DArray)
        {
            uint heightOrArrayLayers = TextureTarget == TextureTarget.Texture2D ? Height : ArrayLayers;
            if (dsa)
            {
                GL.TextureStorage2D(
                    _texture,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) heightOrArrayLayers);
                CheckLastError();
            }
            else if (_gd.Extensions.TextureStorage)
            {
                GL.TexStorage2D(
                    (TextureTarget2d) TextureTarget,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) heightOrArrayLayers);
                CheckLastError();
            }
            else
            {
                uint levelWidth = Width;
                uint levelHeight = heightOrArrayLayers;
                for (int currentLevel = 0; currentLevel < MipLevels; currentLevel++)
                {
                    // Set size, load empty data into texture
                    GL.TexImage2D(
                        TextureTarget,
                        currentLevel,
                        GLInternalFormat,
                        (int) levelWidth,
                        (int) levelHeight,
                        0, // border
                        GLPixelFormat,
                        GLPixelType,
                        0);
                    CheckLastError();

                    levelWidth = Math.Max(1, levelWidth / 2);
                    if (TextureTarget == TextureTarget.Texture2D)
                    {
                        levelHeight = Math.Max(1, levelHeight / 2);
                    }
                }
            }
        }
        else if (TextureTarget == TextureTarget.Texture2DArray)
        {
            if (dsa)
            {
                GL.TextureStorage3D(
                    _texture,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height,
                    (int) ArrayLayers);
                CheckLastError();
            }
            else if (_gd.Extensions.TextureStorage)
            {
                GL.TexStorage3D(
                    TextureTarget3d.Texture2DArray,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height,
                    (int) ArrayLayers);
                CheckLastError();
            }
            else
            {
                uint levelWidth = Width;
                uint levelHeight = Height;
                for (int currentLevel = 0; currentLevel < MipLevels; currentLevel++)
                {
                    GL.TexImage3D(
                        TextureTarget.Texture2DArray,
                        currentLevel,
                        GLInternalFormat,
                        (int) levelWidth,
                        (int) levelHeight,
                        (int) ArrayLayers,
                        0, // border
                        GLPixelFormat,
                        GLPixelType,
                        0);
                    CheckLastError();

                    levelWidth = Math.Max(1, levelWidth / 2);
                    levelHeight = Math.Max(1, levelHeight / 2);
                }
            }
        }
        else if (TextureTarget == TextureTarget.Texture2DMultisample)
        {
            if (dsa)
            {
                GL.TextureStorage2DMultisample(
                    (int) _texture,
                    (int) FormatHelpers.GetSampleCountUInt32(SampleCount),
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height,
                    false);
                CheckLastError();
            }
            else
            {
                if (_gd.Extensions.TextureStorageMultisample)
                {
                    GL.TexStorage2DMultisample(
                        TextureTargetMultisample2d.Texture2DMultisample,
                        (int) FormatHelpers.GetSampleCountUInt32(SampleCount),
                        OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                        (int) Width,
                        (int) Height,
                        false);
                    CheckLastError();
                }
                else
                {
                    GL.TexImage2DMultisample(
                        TextureTargetMultisample.Texture2DMultisample,
                        (int) FormatHelpers.GetSampleCountUInt32(SampleCount),
                        GLInternalFormat,
                        (int) Width,
                        (int) Height,
                        false);
                }
                CheckLastError();
            }
        }
        else if (TextureTarget == TextureTarget.Texture2DMultisampleArray)
        {
            if (dsa)
            {
                GL.TextureStorage3DMultisample(
                    _texture,
                    (int) FormatHelpers.GetSampleCountUInt32(SampleCount),
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height,
                    (int) ArrayLayers,
                    false);
                CheckLastError();
            }
            else
            {
                if (_gd.Extensions.TextureStorageMultisample)
                {
                    GL.TexStorage3DMultisample(
                        TextureTargetMultisample3d.Texture2DMultisampleArray,
                        (int) FormatHelpers.GetSampleCountUInt32(SampleCount),
                        OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                        (int) Width,
                        (int) Height,
                        (int) ArrayLayers,
                        false);
                }
                else
                {
                    GL.TexImage3DMultisample(
                        TextureTargetMultisample.Texture2DMultisampleArray,
                        (int) FormatHelpers.GetSampleCountUInt32(SampleCount),
                        GLInternalFormat,
                        (int) Width,
                        (int) Height,
                        (int) ArrayLayers,
                        false);
                    CheckLastError();
                }
            }
        }
        else if (TextureTarget == TextureTarget.TextureCubeMap)
        {
            if (dsa)
            {
                GL.TextureStorage2D(
                    _texture,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height);
                CheckLastError();
            }
            else if (_gd.Extensions.TextureStorage)
            {
                GL.TexStorage2D(
                    TextureTarget2d.TextureCubeMap,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height);
                CheckLastError();
            }
            else
            {
                uint levelWidth = Width;
                uint levelHeight = Height;
                for (int currentLevel = 0; currentLevel < MipLevels; currentLevel++)
                {
                    for (int face = 0; face < 6; face++)
                    {
                        // Set size, load empty data into texture
                        GL.TexImage2D(
                            TextureTarget.TextureCubeMapPositiveX + face,
                            currentLevel,
                            GLInternalFormat,
                            (int) levelWidth,
                            (int) levelHeight,
                            0, // border
                            GLPixelFormat,
                            GLPixelType,
                            0);
                        CheckLastError();
                    }

                    levelWidth = Math.Max(1, levelWidth / 2);
                    levelHeight = Math.Max(1, levelHeight / 2);
                }
            }
        }
        else if (TextureTarget == TextureTarget.TextureCubeMapArray)
        {
            if (dsa)
            {
                GL.TextureStorage3D(
                    _texture,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height,
                    (int) ArrayLayers * 6);
                CheckLastError();
            }
            else if (_gd.Extensions.TextureStorage)
            {
                GL.TexStorage3D(
                    TextureTarget3d.TextureCubeMapArray,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height,
                    (int) ArrayLayers * 6);
                CheckLastError();
            }
            else
            {
                uint levelWidth = Width;
                uint levelHeight = Height;
                for (int currentLevel = 0; currentLevel < MipLevels; currentLevel++)
                {
                    for (int face = 0; face < 6; face++)
                    {
                        // Set size, load empty data into texture
                        GL.TexImage3D(
                            TextureTarget.Texture2DArray,
                            currentLevel,
                            GLInternalFormat,
                            (int) levelWidth,
                            (int) levelHeight,
                            (int) ArrayLayers * 6,
                            0, // border
                            GLPixelFormat,
                            GLPixelType,
                            0);
                        CheckLastError();
                    }

                    levelWidth = Math.Max(1, levelWidth / 2);
                    levelHeight = Math.Max(1, levelHeight / 2);
                }
            }
        }
        else if (TextureTarget == TextureTarget.Texture3D)
        {
            if (dsa)
            {
                GL.TextureStorage3D(
                    _texture,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height,
                    (int) Depth);
                CheckLastError();
            }
            else if (_gd.Extensions.TextureStorage)
            {
                GL.TexStorage3D(
                    TextureTarget3d.Texture3D,
                    (int) MipLevels,
                    OpenGLFormats.VdToGLSizedInternalFormat(Format, isDepthTex),
                    (int) Width,
                    (int) Height,
                    (int) Depth);
                CheckLastError();
            }
            else
            {
                uint levelWidth = Width;
                uint levelHeight = Height;
                uint levelDepth = Depth;
                for (int currentLevel = 0; currentLevel < MipLevels; currentLevel++)
                {
                    for (int face = 0; face < 6; face++)
                    {
                        // Set size, load empty data into texture
                        GL.TexImage3D(
                            TextureTarget.Texture3D,
                            currentLevel,
                            GLInternalFormat,
                            (int) levelWidth,
                            (int) levelHeight,
                            (int) levelDepth,
                            0, // border
                            GLPixelFormat,
                            GLPixelType,
                            0);
                        CheckLastError();
                    }

                    levelWidth = Math.Max(1, levelWidth / 2);
                    levelHeight = Math.Max(1, levelHeight / 2);
                    levelDepth = Math.Max(1, levelDepth / 2);
                }
            }
        }
        else
        {
            throw new VeldridException("Invalid texture target: " + TextureTarget);
        }

        Created = true;
    }

    public uint GetFramebuffer(uint mipLevel, uint arrayLayer)
    {
        Debug.Assert(!FormatHelpers.IsCompressedFormat(Format));
        Debug.Assert(Created);

        uint subresource = CalculateSubresource(mipLevel, arrayLayer);
        if (_framebuffers[subresource] == 0)
        {
            FramebufferTarget framebufferTarget = SampleCount == TextureSampleCount.Count1
                ? FramebufferTarget.DrawFramebuffer
                : FramebufferTarget.ReadFramebuffer;

            GL.GenFramebuffers(1, out _framebuffers[subresource]);
            CheckLastError();

            GL.BindFramebuffer(framebufferTarget, _framebuffers[subresource]);
            CheckLastError();

            _gd.TextureSamplerManager.SetTextureTransient(TextureTarget, Texture);

            if (TextureTarget == TextureTarget.Texture2D || TextureTarget == TextureTarget.Texture2DMultisample)
            {
                GL.FramebufferTexture2D(
                    framebufferTarget,
                    GLFramebufferAttachment.ColorAttachment0,
                    TextureTarget,
                    Texture,
                    (int)mipLevel);
                CheckLastError();
            }
            else if (TextureTarget == TextureTarget.Texture2DArray
                     || TextureTarget == TextureTarget.Texture2DMultisampleArray
                     || TextureTarget == TextureTarget.Texture3D)
            {
                GL.FramebufferTextureLayer(
                    framebufferTarget,
                    GLFramebufferAttachment.ColorAttachment0,
                    Texture,
                    (int)mipLevel,
                    (int)arrayLayer);
                CheckLastError();
            }

            FramebufferErrorCode errorCode = GL.CheckFramebufferStatus(framebufferTarget);
            if (errorCode != FramebufferErrorCode.FramebufferComplete)
            {
                throw new VeldridException("Failed to create texture copy FBO: " + errorCode);
            }
        }

        return _framebuffers[subresource];
    }

    public uint GetPixelBuffer(uint subresource)
    {
        Debug.Assert(Created);
        if (_pbos[subresource] == 0)
        {
            GL.GenBuffers(1, out _pbos[subresource]);
            CheckLastError();

            GL.BindBuffer(BufferTarget.CopyWriteBuffer, _pbos[subresource]);
            CheckLastError();

            uint dataSize = Width * Height * FormatSizeHelpers.GetSizeInBytes(Format);
            GL.BufferData(
                BufferTarget.CopyWriteBuffer,
                (IntPtr)dataSize,
                0,
                BufferUsageHint.StaticCopy);
            CheckLastError();
            _pboSizes[subresource] = dataSize;
        }

        return _pbos[subresource];
    }

    public uint GetPixelBufferSize(uint subresource)
    {
        Debug.Assert(Created);
        Debug.Assert(_pbos[subresource] != 0);
        return _pboSizes[subresource];
    }

    protected override void DisposeCore()
    {
        if (!_disposeRequested)
        {
            _disposeRequested = true;
            _gd.EnqueueDisposal(this);
        }
    }

    public void DestroyGLResources()
    {
        if (!_disposed)
        {
            _disposed = true;

            GL.DeleteTextures(1, ref _texture);
            CheckLastError();

            for (int i = 0; i < _framebuffers.Length; i++)
            {
                if (_framebuffers[i] != 0)
                {
                    GL.DeleteFramebuffers(1, ref _framebuffers[i]);
                }
            }

            for (int i = 0; i < _pbos.Length; i++)
            {
                if (_pbos[i] != 0)
                {
                    GL.DeleteBuffers(1, ref _pbos[i]);
                }
            }
        }
    }
}