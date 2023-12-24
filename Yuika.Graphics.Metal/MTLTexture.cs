using Metal;
using ObjCRuntime;

namespace Yuika.Graphics.Metal
{
    internal class MTLTexture : Texture
    {
        private bool _disposed;

        /// <summary>
        /// The native MTLTexture object. This property is only valid for non-staging Textures.
        /// </summary>
        public IMTLTexture? DeviceTexture { get; }
        /// <summary>
        /// The staging MTLBuffer object. This property is only valid for staging Textures.
        /// </summary>
        public IMTLBuffer? StagingBuffer { get; }

        public override PixelFormat Format { get; }

        public override uint Width { get; }

        public override uint Height { get; }

        public override uint Depth { get; }

        public override uint MipLevels { get; }

        public override uint ArrayLayers { get; }

        public override TextureUsage Usage { get; }

        public override TextureType Type { get; }

        public override TextureSampleCount SampleCount { get; }
        public override string Name { get; set; }
        public override bool IsDisposed => _disposed;
        public MTLPixelFormat MTLPixelFormat { get; }
        public MTLTextureType MTLTextureType { get; }

        public MTLTexture(ref TextureDescription description, MTLGraphicsDevice _gd)
        {
            Width = description.Width;
            Height = description.Height;
            Depth = description.Depth;
            ArrayLayers = description.ArrayLayers;
            MipLevels = description.MipLevels;
            Format = description.Format;
            Usage = description.Usage;
            Type = description.Type;
            SampleCount = description.SampleCount;
            bool isDepth = (Usage & TextureUsage.DepthStencil) == TextureUsage.DepthStencil;

            MTLPixelFormat = MTLFormats.VdToMTLPixelFormat(Format, isDepth);
            MTLTextureType = MTLFormats.VdToMTLTextureType(
                    Type,
                    ArrayLayers,
                    SampleCount != TextureSampleCount.Count1,
                    (Usage & TextureUsage.Cubemap) != 0);
            if (Usage != TextureUsage.Staging)
            {
                MTLTextureDescriptor texDescriptor = new MTLTextureDescriptor();
                texDescriptor.Width = (UIntPtr)Width;
                texDescriptor.Height = (UIntPtr)Height;
                texDescriptor.Depth = (UIntPtr)Depth;
                texDescriptor.MipmapLevelCount = (UIntPtr)MipLevels;
                texDescriptor.ArrayLength = (UIntPtr)ArrayLayers;
                texDescriptor.SampleCount = (UIntPtr)FormatHelpers.GetSampleCountUInt32(SampleCount);
                texDescriptor.TextureType = MTLTextureType;
                texDescriptor.PixelFormat = MTLPixelFormat;
                texDescriptor.Usage = MTLFormats.VdToMTLTextureUsage(Usage);
                texDescriptor.StorageMode = MTLStorageMode.Private;

                DeviceTexture = _gd.Device.CreateTexture(texDescriptor);
                texDescriptor.Dispose();
                // ObjectiveCRuntime.release(texDescriptor.NativePtr);
            }
            else
            {
                uint blockSize = FormatHelpers.IsCompressedFormat(Format) ? 4u : 1u;
                uint totalStorageSize = 0;
                for (uint level = 0; level < MipLevels; level++)
                {
                    Util.GetMipDimensions(this, level, out uint levelWidth, out uint levelHeight, out uint levelDepth);
                    uint storageWidth = Math.Max(levelWidth, blockSize);
                    uint storageHeight = Math.Max(levelHeight, blockSize);
                    totalStorageSize += levelDepth * FormatHelpers.GetDepthPitch(
                        FormatHelpers.GetRowPitch(levelWidth, Format),
                        levelHeight,
                        Format);
                }
                totalStorageSize *= ArrayLayers;

                StagingBuffer = _gd.Device.CreateBuffer(
                    (UIntPtr)totalStorageSize,
                    MTLResourceOptions.StorageModeShared)!;
            }
        }

        public MTLTexture(ulong nativeTexture, ref TextureDescription description)
        {
            DeviceTexture = Runtime.GetINativeObject<IMTLTexture>((IntPtr) nativeTexture, true)!; // new IMTLTexture((IntPtr)nativeTexture);
            Width = description.Width;
            Height = description.Height;
            Depth = description.Depth;
            ArrayLayers = description.ArrayLayers;
            MipLevels = description.MipLevels;
            Format = description.Format;
            Usage = description.Usage;
            Type = description.Type;
            SampleCount = description.SampleCount;
            bool isDepth = (Usage & TextureUsage.DepthStencil) == TextureUsage.DepthStencil;

            MTLPixelFormat = MTLFormats.VdToMTLPixelFormat(Format, isDepth);
            MTLTextureType = MTLFormats.VdToMTLTextureType(
                    Type,
                    ArrayLayers,
                    SampleCount != TextureSampleCount.Count1,
                    (Usage & TextureUsage.Cubemap) != 0);
        }

        internal uint GetSubresourceSize(uint mipLevel, uint arrayLayer)
        {
            uint blockSize = FormatHelpers.IsCompressedFormat(Format) ? 4u : 1u;
            Util.GetMipDimensions(this, mipLevel, out uint width, out uint height, out uint depth);
            uint storageWidth = Math.Max(blockSize, width);
            uint storageHeight = Math.Max(blockSize, height);
            return depth * FormatHelpers.GetDepthPitch(
                FormatHelpers.GetRowPitch(storageWidth, Format),
                storageHeight,
                Format);
        }

        internal void GetSubresourceLayout(uint mipLevel, uint arrayLayer, out uint rowPitch, out uint depthPitch)
        {
            uint blockSize = FormatHelpers.IsCompressedFormat(Format) ? 4u : 1u;
            Util.GetMipDimensions(this, mipLevel, out uint mipWidth, out uint mipHeight, out uint mipDepth);
            uint storageWidth = Math.Max(blockSize, mipWidth);
            uint storageHeight = Math.Max(blockSize, mipHeight);
            rowPitch = FormatHelpers.GetRowPitch(storageWidth, Format);
            depthPitch = FormatHelpers.GetDepthPitch(rowPitch, storageHeight, Format);
        }

        protected override void DisposeCore()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (StagingBuffer != null)
                {
                    StagingBuffer.Dispose();
                    // ObjectiveCRuntime.release(StagingBuffer.NativePtr);
                }
                else
                {
                    DeviceTexture?.Dispose();
                    // ObjectiveCRuntime.release(DeviceTexture.NativePtr);
                }
            }
        }
    }
}
