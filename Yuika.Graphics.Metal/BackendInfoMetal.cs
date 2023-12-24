using System.Collections.ObjectModel;
using Metal;
using ObjCRuntime;

#if __MACOS__ || (__IOS__ && !__MACCATALYST__)
using MetalFX;
#endif

namespace Yuika.Graphics.Metal
{
    /// <summary>
    /// Exposes Metal-specific functionality,
    /// useful for interoperating with native components which interface directly with Metal.
    /// Can only be used on <see cref="GraphicsBackend.Metal"/>.
    /// </summary>
    public class BackendInfoMetal : IGraphicsBackendInfo
    {
        private readonly MTLGraphicsDevice _gd;
        private ReadOnlyCollection<MTLFeatureSet> _featureSet;

        internal BackendInfoMetal(MTLGraphicsDevice gd)
        {
            _gd = gd;
            _featureSet = new ReadOnlyCollection<MTLFeatureSet>(_gd.MetalFeatures.ToArray());
        }

        public ReadOnlyCollection<MTLFeatureSet> FeatureSet => _featureSet;

        public MTLFeatureSet MaxFeatureSet => _gd.MetalFeatures.MaxFeatureSet;

#if __MACOS__ || (__IOS__ && !__MACCATALYST__)
        public bool SupportsMetalFX => MTLFXTemporalScalerDescriptor.SupportsDevice(_gd.Device);

        public void Apply(IMTLFXTemporalScaler scaler, CommandList list)
        {
            MTLCommandList mtlCL = Util.AssertSubtype<CommandList, MTLCommandList>(list);
            scaler.Encode(mtlCL.CommandBuffer);
        }
        
        public void Apply(IMTLFXSpatialScaler scaler, CommandList list)
        {
            MTLCommandList mtlCL = Util.AssertSubtype<CommandList, MTLCommandList>(list);
            scaler.Encode(mtlCL.CommandBuffer);
        }

        public MTLFXTemporalScalerDescriptor CreateTemporalScalerDescriptor()
        {
            Class clz = new Class(typeof(MTLFXTemporalScalerDescriptor));
            NSObject descObj = NSObject.Alloc(clz);
            return Runtime.GetNSObject<MTLFXTemporalScalerDescriptor>(descObj.Handle.Handle)!;
        }
        
        public MTLFXSpatialScalerDescriptor CreateSpatialScalerDescriptor()
        {
            Class clz = new Class(typeof(MTLFXSpatialScalerDescriptor));
            NSObject descObj = NSObject.Alloc(clz);
            return Runtime.GetNSObject<MTLFXSpatialScalerDescriptor>(descObj.Handle.Handle)!;
        }
#endif
    }
}
