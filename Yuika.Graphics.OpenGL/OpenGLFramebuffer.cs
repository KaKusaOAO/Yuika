﻿using OpenTK.Graphics.OpenGL4;
using static Yuika.Graphics.OpenGL.OpenGLUtil;

using GLPixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using GLPixelType = OpenTK.Graphics.OpenGL4.PixelType;
using GLFramebufferAttachment = OpenTK.Graphics.OpenGL4.FramebufferAttachment;

namespace Yuika.Graphics.OpenGL;

internal unsafe class OpenGLFramebuffer : Framebuffer, OpenGLDeferredResource
{
    private readonly OpenGLGraphicsDevice _gd;
    private uint _framebuffer;

    private string _name;
    private bool _nameChanged;
    private bool _disposeRequested;
    private bool _disposed;

    public override string Name { get => _name; set { _name = value; _nameChanged = true; } }

    public uint Framebuffer => _framebuffer;

    public bool Created { get; private set; }

    public override bool IsDisposed => _disposeRequested;

    public OpenGLFramebuffer(OpenGLGraphicsDevice gd, ref FramebufferDescription description)
        : base(description.DepthTarget, description.ColorTargets)
    {
        _gd = gd;
    }

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
                SetObjectLabel(ObjectLabelIdentifier.Framebuffer, _framebuffer, _name);
            }
        }
    }

    public void CreateGLResources()
    {
        GL.GenFramebuffers(1, out _framebuffer);
        CheckLastError();

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);
        CheckLastError();

        uint colorCount = (uint)ColorTargets.Count;

        if (colorCount > 0)
        {
            for (int i = 0; i < colorCount; i++)
            {
                FramebufferAttachment colorAttachment = ColorTargets[i];
                OpenGLTexture glTex = Util.AssertSubtype<Texture, OpenGLTexture>(colorAttachment.Target);
                glTex.EnsureResourcesCreated();

                _gd.TextureSamplerManager.SetTextureTransient(glTex.TextureTarget, glTex.Texture);
                CheckLastError();

                TextureTarget textureTarget = GetTextureTarget (glTex, colorAttachment.ArrayLayer);

                if (glTex.ArrayLayers == 1)
                {
                    GL.FramebufferTexture2D(
                        FramebufferTarget.Framebuffer,
                        GLFramebufferAttachment.ColorAttachment0 + i,
                        textureTarget,
                        glTex.Texture,
                        (int)colorAttachment.MipLevel);
                    CheckLastError();
                }
                else
                {
                    GL.FramebufferTextureLayer(
                        FramebufferTarget.Framebuffer,
                        GLFramebufferAttachment.ColorAttachment0 + i,
                        (uint)glTex.Texture,
                        (int)colorAttachment.MipLevel,
                        (int)colorAttachment.ArrayLayer);
                    CheckLastError();
                }
            }

            DrawBuffersEnum* bufs = stackalloc DrawBuffersEnum[(int)colorCount];
            for (int i = 0; i < colorCount; i++)
            {
                bufs[i] = DrawBuffersEnum.ColorAttachment0 + i;
            }
            GL.DrawBuffers((int)colorCount, bufs);
            CheckLastError();
        }

        uint depthTextureID = 0;
        TextureTarget depthTarget = TextureTarget.Texture2D;
        if (DepthTarget != null)
        {
            OpenGLTexture glDepthTex = Util.AssertSubtype<Texture, OpenGLTexture>(DepthTarget.Value.Target);
            glDepthTex.EnsureResourcesCreated();
            depthTarget = glDepthTex.TextureTarget;

            depthTextureID = glDepthTex.Texture;

            _gd.TextureSamplerManager.SetTextureTransient(depthTarget, glDepthTex.Texture);
            CheckLastError();

            depthTarget = GetTextureTarget (glDepthTex, DepthTarget.Value.ArrayLayer);

            GLFramebufferAttachment framebufferAttachment = GLFramebufferAttachment.DepthAttachment;
            if (FormatHelpers.IsStencilFormat(glDepthTex.Format))
            {
                framebufferAttachment = GLFramebufferAttachment.DepthStencilAttachment;
            }

            if (glDepthTex.ArrayLayers == 1)
            {
                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    framebufferAttachment,
                    depthTarget,
                    depthTextureID,
                    (int)DepthTarget.Value.MipLevel);
                CheckLastError();
            }
            else
            {
                GL.FramebufferTextureLayer(
                    FramebufferTarget.Framebuffer,
                    framebufferAttachment,
                    glDepthTex.Texture,
                    (int)DepthTarget.Value.MipLevel,
                    (int)DepthTarget.Value.ArrayLayer);
                CheckLastError();
            }

        }

        FramebufferErrorCode errorCode = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        CheckLastError();
        if (errorCode != FramebufferErrorCode.FramebufferComplete)
        {
            throw new VeldridException("Framebuffer was not successfully created: " + errorCode);
        }

        Created = true;
    }

    public override void Dispose()
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
            uint framebuffer = _framebuffer;
            GL.DeleteFramebuffers(1, ref framebuffer);
            CheckLastError();
        }
    }
}