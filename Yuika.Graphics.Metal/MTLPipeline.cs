using System.Diagnostics;
using Metal;

namespace Yuika.Graphics.Metal
{
    internal class MTLPipeline : Pipeline
    {
        private bool _disposed;
        private List<IMTLFunction> _specializedFunctions;

        public IMTLRenderPipelineState? RenderPipelineState { get; }
        public IMTLComputePipelineState? ComputePipelineState { get; }
        public MTLPrimitiveType PrimitiveType { get; }
        public new MTLResourceLayout[] ResourceLayouts { get; }
        public ResourceBindingModel ResourceBindingModel { get; }
        public uint VertexBufferCount { get; }
        public uint NonVertexBufferCount { get; }
        public MTLCullMode CullMode { get; }
        public MTLWinding FrontFace { get; }
        public MTLTriangleFillMode FillMode { get; }
        public IMTLDepthStencilState DepthStencilState { get; }
        public MTLDepthClipMode DepthClipMode { get; }
        public override bool IsComputePipeline { get; }
        public bool ScissorTestEnabled { get; }
        public MTLSize ThreadsPerThreadgroup { get; } = new MTLSize(1, 1, 1);
        public bool HasStencil { get; }
        public override string Name { get; set; }
        public uint StencilReference { get; }
        public RgbaFloat BlendColor { get; }
        public override bool IsDisposed => _disposed;

        public MTLPipeline(ref GraphicsPipelineDescription description, MTLGraphicsDevice gd)
            : base(ref description)
        {
            PrimitiveType = MTLFormats.VdToMTLPrimitiveTopology(description.PrimitiveTopology);
            ResourceLayouts = new MTLResourceLayout[description.ResourceLayouts.Length];
            NonVertexBufferCount = 0;
            for (int i = 0; i < ResourceLayouts.Length; i++)
            {
                ResourceLayouts[i] = Util.AssertSubtype<ResourceLayout, MTLResourceLayout>(description.ResourceLayouts[i]);
                NonVertexBufferCount += ResourceLayouts[i].BufferCount;
            }
            ResourceBindingModel = description.ResourceBindingModel ?? gd.ResourceBindingModel;

            CullMode = MTLFormats.VdToMTLCullMode(description.RasterizerState.CullMode);
            FrontFace = MTLFormats.VdVoMTLFrontFace(description.RasterizerState.FrontFace);
            FillMode = MTLFormats.VdToMTLFillMode(description.RasterizerState.FillMode);
            ScissorTestEnabled = description.RasterizerState.ScissorTestEnabled;

            MTLRenderPipelineDescriptor mtlDesc = new MTLRenderPipelineDescriptor();
            foreach (Shader shader in description.ShaderSet.Shaders)
            {
                MTLShader mtlShader = Util.AssertSubtype<Shader, MTLShader>(shader);
                IMTLFunction specializedFunction;

                if (mtlShader.HasFunctionConstants)
                {
                    // Need to create specialized MTLFunction.
                    MTLFunctionConstantValues constantValues = CreateConstantValues(description.ShaderSet.Specializations);
                    specializedFunction = mtlShader.Library.CreateFunction(mtlShader.EntryPoint, constantValues, out NSError? error)!;
                    if (error != null) throw new NSErrorException(error);
                    
                    AddSpecializedFunction(specializedFunction);
                    constantValues.Dispose();
                    // ObjectiveCRuntime.release(constantValues.NativePtr);

                    Debug.Assert(specializedFunction != null, "Failed to create specialized MTLFunction");
                }
                else
                {
                    specializedFunction = mtlShader.Function;
                }

                if (shader.Stage == ShaderStages.Vertex)
                {
                    mtlDesc.VertexFunction = specializedFunction;
                }
                else if (shader.Stage == ShaderStages.Fragment)
                {
                    mtlDesc.FragmentFunction = specializedFunction;
                }
            }

            // Vertex layouts
            VertexLayoutDescription[] vdVertexLayouts = description.ShaderSet.VertexLayouts;
            MTLVertexDescriptor vertexDescriptor = mtlDesc.VertexDescriptor;

            for (uint i = 0; i < vdVertexLayouts.Length; i++)
            {
                uint layoutIndex = ResourceBindingModel == ResourceBindingModel.Improved
                    ? NonVertexBufferCount + i
                    : i;
                MTLVertexBufferLayoutDescriptor mtlLayout = vertexDescriptor.Layouts[(IntPtr)layoutIndex];
                mtlLayout.Stride = (UIntPtr)vdVertexLayouts[i].Stride;
                uint stepRate = vdVertexLayouts[i].InstanceStepRate;
                mtlLayout.StepFunction = stepRate == 0 ? MTLVertexStepFunction.PerVertex : MTLVertexStepFunction.PerInstance;
                mtlLayout.StepRate = (UIntPtr)Math.Max(1, stepRate);
            }

            uint element = 0;
            for (uint i = 0; i < vdVertexLayouts.Length; i++)
            {
                uint offset = 0;
                VertexLayoutDescription vdDesc = vdVertexLayouts[i];
                for (uint j = 0; j < vdDesc.Elements.Length; j++)
                {
                    VertexElementDescription elementDesc = vdDesc.Elements[j];
                    MTLVertexAttributeDescriptor mtlAttribute = vertexDescriptor.Attributes[(IntPtr)element];
                    mtlAttribute.BufferIndex = (UIntPtr)(ResourceBindingModel == ResourceBindingModel.Improved
                        ? NonVertexBufferCount + i
                        : i);
                    mtlAttribute.Format = MTLFormats.VdToMTLVertexFormat(elementDesc.Format);
                    mtlAttribute.Offset = elementDesc.Offset != 0 ? (UIntPtr)elementDesc.Offset : (UIntPtr)offset;
                    offset += FormatSizeHelpers.GetSizeInBytes(elementDesc.Format);
                    element += 1;
                }
            }

            VertexBufferCount = (uint)vdVertexLayouts.Length;

            // Outputs
            OutputDescription outputs = description.Outputs;
            BlendStateDescription blendStateDesc = description.BlendState;
            BlendColor = blendStateDesc.BlendFactor;

            if (outputs.SampleCount != TextureSampleCount.Count1)
            {
                mtlDesc.SampleCount = (UIntPtr)FormatHelpers.GetSampleCountUInt32(outputs.SampleCount);
            }

            if (outputs.DepthAttachment != null)
            {
                PixelFormat depthFormat = outputs.DepthAttachment.Value.Format;
                MTLPixelFormat mtlDepthFormat = MTLFormats.VdToMTLPixelFormat(depthFormat, true);
                mtlDesc.DepthAttachmentPixelFormat = mtlDepthFormat;
                if ((FormatHelpers.IsStencilFormat(depthFormat)))
                {
                    HasStencil = true;
                    mtlDesc.StencilAttachmentPixelFormat = mtlDepthFormat;
                }
            }
            for (uint i = 0; i < outputs.ColorAttachments.Length; i++)
            {
                BlendAttachmentDescription attachmentBlendDesc = blendStateDesc.AttachmentStates[i];
                MTLRenderPipelineColorAttachmentDescriptor colorDesc = mtlDesc.ColorAttachments[(IntPtr)i];
                colorDesc.PixelFormat = MTLFormats.VdToMTLPixelFormat(outputs.ColorAttachments[i].Format, false);
                colorDesc.BlendingEnabled = attachmentBlendDesc.BlendEnabled;
                colorDesc.WriteMask = MTLFormats.VdToMTLColorWriteMask(attachmentBlendDesc.ColorWriteMask.GetValueOrDefault());
                colorDesc.AlphaBlendOperation = MTLFormats.VdToMTLBlendOp(attachmentBlendDesc.AlphaFunction);
                colorDesc.SourceAlphaBlendFactor = MTLFormats.VdToMTLBlendFactor(attachmentBlendDesc.SourceAlphaFactor);
                colorDesc.DestinationAlphaBlendFactor = MTLFormats.VdToMTLBlendFactor(attachmentBlendDesc.DestinationAlphaFactor);

                colorDesc.RgbBlendOperation = MTLFormats.VdToMTLBlendOp(attachmentBlendDesc.ColorFunction);
                colorDesc.SourceRgbBlendFactor = MTLFormats.VdToMTLBlendFactor(attachmentBlendDesc.SourceColorFactor);
                colorDesc.DestinationRgbBlendFactor = MTLFormats.VdToMTLBlendFactor(attachmentBlendDesc.DestinationColorFactor);
            }

            mtlDesc.AlphaToCoverageEnabled = blendStateDesc.AlphaToCoverageEnabled;

            RenderPipelineState = gd.Device.CreateRenderPipelineState(mtlDesc, out NSError? err);
            if (err != null) throw new NSErrorException(err);
            mtlDesc.Dispose();
            // ObjectiveCRuntime.release(mtlDesc.NativePtr);

            if (outputs.DepthAttachment != null)
            {
                // MTLDepthStencilDescriptor depthDescriptor = MTLUtil.AllocInit<MTLDepthStencilDescriptor>(
                //     nameof(MTLDepthStencilDescriptor));
                MTLDepthStencilDescriptor depthDescriptor = new MTLDepthStencilDescriptor();
                depthDescriptor.DepthCompareFunction = MTLFormats.VdToMTLCompareFunction(
                    description.DepthStencilState.DepthComparison);
                depthDescriptor.DepthWriteEnabled = description.DepthStencilState.DepthWriteEnabled;

                bool stencilEnabled = description.DepthStencilState.StencilTestEnabled;
                if (stencilEnabled)
                {
                    StencilReference = description.DepthStencilState.StencilReference;

                    StencilBehaviorDescription vdFrontDesc = description.DepthStencilState.StencilFront;
                    MTLStencilDescriptor front = new MTLStencilDescriptor();
                        // MTLUtil.AllocInit<MTLStencilDescriptor>(nameof(MTLStencilDescriptor));
                    front.ReadMask = stencilEnabled ? description.DepthStencilState.StencilReadMask : 0u;
                    front.WriteMask = stencilEnabled ? description.DepthStencilState.StencilWriteMask : 0u;
                    front.DepthFailureOperation = MTLFormats.VdToMTLStencilOperation(vdFrontDesc.DepthFail);
                    front.StencilFailureOperation = MTLFormats.VdToMTLStencilOperation(vdFrontDesc.Fail);
                    front.DepthStencilPassOperation = MTLFormats.VdToMTLStencilOperation(vdFrontDesc.Pass);
                    front.StencilCompareFunction = MTLFormats.VdToMTLCompareFunction(vdFrontDesc.Comparison);
                    depthDescriptor.FrontFaceStencil = front;

                    StencilBehaviorDescription vdBackDesc = description.DepthStencilState.StencilBack;
                    MTLStencilDescriptor back = new MTLStencilDescriptor();
                        // MTLUtil.AllocInit<MTLStencilDescriptor>(nameof(MTLStencilDescriptor));
                    back.ReadMask = stencilEnabled ? description.DepthStencilState.StencilReadMask : 0u;
                    back.WriteMask = stencilEnabled ? description.DepthStencilState.StencilWriteMask : 0u;
                    back.DepthFailureOperation = MTLFormats.VdToMTLStencilOperation(vdBackDesc.DepthFail);
                    back.StencilFailureOperation = MTLFormats.VdToMTLStencilOperation(vdBackDesc.Fail);
                    back.DepthStencilPassOperation = MTLFormats.VdToMTLStencilOperation(vdBackDesc.Pass);
                    back.StencilCompareFunction = MTLFormats.VdToMTLCompareFunction(vdBackDesc.Comparison);
                    depthDescriptor.BackFaceStencil = back;

                    front.Dispose();
                    back.Dispose();
                    // ObjectiveCRuntime.release(front.NativePtr);
                    // ObjectiveCRuntime.release(back.NativePtr);
                }

                DepthStencilState = gd.Device.CreateDepthStencilState(depthDescriptor)!;
                depthDescriptor.Dispose();
                // ObjectiveCRuntime.release(depthDescriptor.NativePtr);
            }

            DepthClipMode = description.DepthStencilState.DepthTestEnabled ? MTLDepthClipMode.Clip : MTLDepthClipMode.Clamp;
        }

        public MTLPipeline(ref ComputePipelineDescription description, MTLGraphicsDevice gd)
            : base(ref description)
        {
            IsComputePipeline = true;
            ResourceLayouts = new MTLResourceLayout[description.ResourceLayouts.Length];
            for (int i = 0; i < ResourceLayouts.Length; i++)
            {
                ResourceLayouts[i] = Util.AssertSubtype<ResourceLayout, MTLResourceLayout>(description.ResourceLayouts[i]);
            }

            ThreadsPerThreadgroup = new MTLSize(
                (IntPtr) description.ThreadGroupSizeX,
                (IntPtr) description.ThreadGroupSizeY,
                (IntPtr) description.ThreadGroupSizeZ);

            // MTLComputePipelineDescriptor mtlDesc = MTLUtil.AllocInit<MTLComputePipelineDescriptor>(
            //     nameof(MTLComputePipelineDescriptor));
            MTLComputePipelineDescriptor mtlDesc = new MTLComputePipelineDescriptor();
            MTLShader mtlShader = Util.AssertSubtype<Shader, MTLShader>(description.ComputeShader);
            IMTLFunction specializedFunction;
            if (mtlShader.HasFunctionConstants)
            {
                // Need to create specialized MTLFunction.
                MTLFunctionConstantValues constantValues = CreateConstantValues(description.Specializations);
                specializedFunction = mtlShader.Library.CreateFunction(mtlShader.EntryPoint, constantValues, out NSError? error);
                if (error != null) throw new NSErrorException(error);
                
                AddSpecializedFunction(specializedFunction);
                constantValues.Dispose();
                // ObjectiveCRuntime.release(constantValues.NativePtr);

                Debug.Assert(specializedFunction != null, "Failed to create specialized MTLFunction");
            }
            else
            {
                specializedFunction = mtlShader.Function;
            }

            mtlDesc.ComputeFunction = specializedFunction;
            MTLPipelineBufferDescriptorArray buffers = mtlDesc.Buffers;
            uint bufferIndex = 0;
            foreach (MTLResourceLayout layout in ResourceLayouts)
            {
                foreach (ResourceLayoutElementDescription rle in layout.Description.Elements)
                {
                    ResourceKind kind = rle.Kind;
                    if (kind == ResourceKind.UniformBuffer
                        || kind == ResourceKind.StructuredBufferReadOnly)
                    {
                        MTLPipelineBufferDescriptor bufferDesc = buffers[bufferIndex];
                        bufferDesc.Mutability = MTLMutability.Immutable;
                        bufferIndex += 1;
                    }
                    else if (kind == ResourceKind.StructuredBufferReadWrite)
                    {
                        MTLPipelineBufferDescriptor bufferDesc = buffers[bufferIndex];
                        bufferDesc.Mutability = MTLMutability.Mutable;
                        bufferIndex += 1;
                    }
                }
            }

            ComputePipelineState = gd.Device.CreateComputePipelineState(mtlDesc, 
                MTLPipelineOption.None, out _, out NSError? err);
            
            if (err != null) throw new NSErrorException(err);
            mtlDesc.Dispose();
            // ObjectiveCRuntime.release(mtlDesc.NativePtr);
        }

        private unsafe MTLFunctionConstantValues CreateConstantValues(SpecializationConstant[] specializations)
        {
            MTLFunctionConstantValues ret = new MTLFunctionConstantValues();
            if (specializations != null)
            {
                foreach (SpecializationConstant sc in specializations)
                {
                    MTLDataType mtlType = MTLFormats.VdVoMTLShaderConstantType(sc.Type);
                    ret.SetConstantValue((IntPtr) (&sc.Data), mtlType, sc.ID);
                }
            }

            return ret;
        }

        private void AddSpecializedFunction(IMTLFunction function)
        {
            if (_specializedFunctions == null) { _specializedFunctions = new List<IMTLFunction>(); }
            _specializedFunctions.Add(function);
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                if (RenderPipelineState != null)
                {
                    RenderPipelineState.Dispose();
                    // ObjectiveCRuntime.release(RenderPipelineState.NativePtr);
                }
                else
                {
                    Debug.Assert(ComputePipelineState != null);
                    ComputePipelineState.Dispose();
                    // ObjectiveCRuntime.release(ComputePipelineState.NativePtr);
                }

                if (_specializedFunctions != null)
                {
                    foreach (IMTLFunction function in _specializedFunctions)
                    {
                        function.Dispose();
                        // ObjectiveCRuntime.release(function.NativePtr);
                    }
                    _specializedFunctions.Clear();
                }

                _disposed = true;
            }
        }
    }
}
