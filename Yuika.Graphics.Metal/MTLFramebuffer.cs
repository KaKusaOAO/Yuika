using Metal;

namespace Yuika.Graphics.Metal
{
    internal class MTLFramebuffer : MTLFramebufferBase
    {
        public override bool IsRenderable => true;
        private bool _disposed;

        public MTLFramebuffer(MTLGraphicsDevice gd, ref FramebufferDescription description)
            : base(gd, ref description)
        {
        }

        public override MTLRenderPassDescriptor CreateRenderPassDescriptor()
        {
            MTLRenderPassDescriptor ret = new MTLRenderPassDescriptor();
            for (int i = 0; i < ColorTargets.Count; i++)
            {
                FramebufferAttachment colorTarget = ColorTargets[i];
                MTLTexture mtlTarget = Util.AssertSubtype<Texture, MTLTexture>(colorTarget.Target);
                MTLRenderPassColorAttachmentDescriptor colorDescriptor = ret.ColorAttachments[i];
                colorDescriptor.Texture = mtlTarget.DeviceTexture;
                colorDescriptor.LoadAction = MTLLoadAction.Load;
                colorDescriptor.Slice = (UIntPtr)colorTarget.ArrayLayer;
                colorDescriptor.Level = (UIntPtr)colorTarget.MipLevel;
            }

            if (DepthTarget != null)
            {
                MTLTexture mtlDepthTarget = Util.AssertSubtype<Texture, MTLTexture>(DepthTarget.Value.Target);
                MTLRenderPassDepthAttachmentDescriptor depthDescriptor = ret.DepthAttachment;
                depthDescriptor.LoadAction = MTLLoadAction.Load;
                depthDescriptor.StoreAction = MTLStoreAction.Store;
                depthDescriptor.Texture = mtlDepthTarget.DeviceTexture;
                depthDescriptor.Slice = (UIntPtr)DepthTarget.Value.ArrayLayer;
                depthDescriptor.Level = (UIntPtr)DepthTarget.Value.MipLevel;

                if (FormatHelpers.IsStencilFormat(mtlDepthTarget.Format))
                {
                    MTLRenderPassStencilAttachmentDescriptor stencilDescriptor = ret.StencilAttachment;
                    stencilDescriptor.LoadAction = MTLLoadAction.Load;
                    stencilDescriptor.StoreAction = MTLStoreAction.Store;
                    stencilDescriptor.Texture = mtlDepthTarget.DeviceTexture;
                    stencilDescriptor.Slice = (UIntPtr)DepthTarget.Value.ArrayLayer;
                }
            }

            return ret;
        }

        public override bool IsDisposed => _disposed;

        public override void Dispose()
        {
            _disposed = true;
        }
    }
}
