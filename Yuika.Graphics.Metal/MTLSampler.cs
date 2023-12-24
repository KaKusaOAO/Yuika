using Metal;

namespace Yuika.Graphics.Metal
{
    internal class MTLSampler : Sampler
    {
        private bool _disposed;

        public IMTLSamplerState DeviceSampler { get; }

        public MTLSampler(ref SamplerDescription description, MTLGraphicsDevice gd)
        {
            MTLFormats.GetMinMagMipFilter(
                description.Filter,
                out MTLSamplerMinMagFilter min,
                out MTLSamplerMinMagFilter mag,
                out MTLSamplerMipFilter mip);

            MTLSamplerDescriptor mtlDesc = new MTLSamplerDescriptor();
            mtlDesc.SAddressMode = MTLFormats.VdToMTLAddressMode(description.AddressModeU);
            mtlDesc.TAddressMode = MTLFormats.VdToMTLAddressMode(description.AddressModeV);
            mtlDesc.RAddressMode = MTLFormats.VdToMTLAddressMode(description.AddressModeW);
            mtlDesc.MinFilter = min;
            mtlDesc.MagFilter = mag;
            mtlDesc.MipFilter = mip;
            if (gd.MetalFeatures.IsMacOS)
            {
#if !__TVOS__
                mtlDesc.BorderColor = MTLFormats.VdToMTLBorderColor(description.BorderColor);
#endif
            }
            if (description.ComparisonKind != null)
            {
                mtlDesc.CompareFunction = MTLFormats.VdToMTLCompareFunction(description.ComparisonKind.Value);
            }
            mtlDesc.LodMinClamp = description.MinimumLod;
            mtlDesc.LodMaxClamp = description.MaximumLod;
            mtlDesc.MaxAnisotropy = (UIntPtr)(Math.Max(1, description.MaximumAnisotropy));
            DeviceSampler = gd.Device.CreateSamplerState(mtlDesc)!;
            mtlDesc.Dispose();
            // ObjectiveCRuntime.release(mtlDesc.NativePtr);
        }

        public override string Name { get; set; }

        public override bool IsDisposed => _disposed;

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                DeviceSampler.Dispose();
                // ObjectiveCRuntime.release(DeviceSampler.NativePtr);
            }
        }
    }
}
