using System.Diagnostics;
using Metal;

namespace Yuika.Graphics.Metal
{
    internal class MTLSwapchainFramebuffer : MTLFramebufferBase
    {
        private readonly MTLGraphicsDevice _gd;
        private readonly MTLPlaceholderTexture _placeholderTexture;
        private MTLTexture _depthTexture;
        private readonly MTLSwapchain _parentSwapchain;
        private bool _disposed;

        public override uint Width => _placeholderTexture.Width;
        public override uint Height => _placeholderTexture.Height;

        public override OutputDescription OutputDescription { get; }

        private readonly FramebufferAttachment[] _colorTargets;
        private readonly FramebufferAttachment? _depthTarget;
        private readonly PixelFormat? _depthFormat;

        public override IReadOnlyList<FramebufferAttachment> ColorTargets => _colorTargets;
        public override FramebufferAttachment? DepthTarget => _depthTarget;

        public override bool IsDisposed => _disposed;

        public MTLSwapchainFramebuffer(
            MTLGraphicsDevice gd,
            MTLSwapchain parent,
            uint width,
            uint height,
            PixelFormat? depthFormat,
            PixelFormat colorFormat)
            : base()
        {
            _gd = gd;
            _parentSwapchain = parent;

            OutputAttachmentDescription? depthAttachment = null;
            if (depthFormat != null)
            {
                _depthFormat = depthFormat;
                depthAttachment = new OutputAttachmentDescription(depthFormat.Value);
                RecreateDepthTexture(width, height);
                _depthTarget = new FramebufferAttachment(_depthTexture, 0);
            }
            OutputAttachmentDescription colorAttachment = new OutputAttachmentDescription(colorFormat);

            OutputDescription = new OutputDescription(depthAttachment, colorAttachment);
            _placeholderTexture = new MTLPlaceholderTexture(colorFormat);
            _placeholderTexture.Resize(width, height);
            _colorTargets = new[] { new FramebufferAttachment(_placeholderTexture, 0) };
        }

        private void RecreateDepthTexture(uint width, uint height)
        {
            Debug.Assert(_depthFormat.HasValue);
            if (_depthTexture != null)
            {
                _depthTexture.Dispose();
            }

            _depthTexture = Util.AssertSubtype<Texture, MTLTexture>(
                _gd.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
                    width, height, 1, 1, _depthFormat.Value, TextureUsage.DepthStencil)));
        }

        public void Resize(uint width, uint height)
        {
            _placeholderTexture.Resize(width, height);

            if (_depthFormat.HasValue)
            {
                RecreateDepthTexture(width, height);
            }
        }

        public override bool IsRenderable => _parentSwapchain.CurrentDrawable != null;

        public override MTLRenderPassDescriptor CreateRenderPassDescriptor()
        {
            MTLRenderPassDescriptor ret = new MTLRenderPassDescriptor();
            var color0 = ret.ColorAttachments[0];
            color0.Texture = _parentSwapchain.CurrentDrawable.Texture;
            color0.LoadAction = MTLLoadAction.Load;

            if (_depthTarget != null)
            {
                var depthAttachment = ret.DepthAttachment;
                depthAttachment.Texture = _depthTexture.DeviceTexture;
                depthAttachment.LoadAction = MTLLoadAction.Load;
            }

            return ret;
        }

        public override void Dispose()
        {
            _depthTexture?.Dispose();
            _disposed = true;
        }
    }
}