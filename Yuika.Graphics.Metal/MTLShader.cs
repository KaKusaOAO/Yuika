using System.Text;
using CoreFoundation;
using Metal;

namespace Yuika.Graphics.Metal
{
    internal class MTLShader : Shader
    {
        private readonly MTLGraphicsDevice _device;
        private bool _disposed;

        public IMTLLibrary Library { get; private set; }
        public IMTLFunction Function { get; private set; }
        public override string Name { get; set; }
        public bool HasFunctionConstants { get; }
        public override bool IsDisposed => _disposed;

        public unsafe MTLShader(ref ShaderDescription description, MTLGraphicsDevice gd)
            : base(description.Stage, description.EntryPoint)
        {
            _device = gd;

            if (description.ShaderBytes.Length > 4
                && description.ShaderBytes[0] == 0x4d
                && description.ShaderBytes[1] == 0x54
                && description.ShaderBytes[2] == 0x4c
                && description.ShaderBytes[3] == 0x42)
            {
                DispatchQueue queue = DispatchQueue.GetGlobalQueue(DispatchQualityOfService.UserInteractive); 
                // Dispatch.dispatch_get_global_queue(QualityOfServiceLevel.QOS_CLASS_USER_INTERACTIVE, 0);
                
                fixed (byte* shaderBytesPtr = description.ShaderBytes)
                {
                    DispatchData dispatchData = DispatchData.FromByteBuffer(description.ShaderBytes); 
                    dispatchData.SetTargetQueue(queue);
                    
                    // Dispatch.dispatch_data_create(
                    //     shaderBytesPtr,
                    //     (UIntPtr)description.ShaderBytes.Length,
                    //     queue,
                    //     IntPtr.Zero);
                    
                    try
                    {
                        Library = gd.Device.CreateLibrary(dispatchData, out NSError? error);
                        if (error != null) throw new NSErrorException(error);
                    }
                    finally
                    {
                        dispatchData.Dispose();
                        // Dispatch.dispatch_release(dispatchData.NativePtr);
                    }
                }
            }
            else
            {
                string source = Encoding.UTF8.GetString(description.ShaderBytes);
                MTLCompileOptions compileOptions = new MTLCompileOptions();
                Library = gd.Device.CreateLibrary(source, compileOptions, out NSError? error);
                if (error != null) throw new NSErrorException(error);
                compileOptions.Dispose();
                // ObjectiveCRuntime.release(compileOptions);
            }

            Function = Library.CreateFunction(description.EntryPoint);
            if (Function == null)
            {
                throw new VeldridException(
                    $"Failed to create Metal {description.Stage} Shader. The given entry point \"{description.EntryPoint}\" was not found.");
            }

            HasFunctionConstants = Function.FunctionConstants.Count != UIntPtr.Zero;
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                
                Function.Dispose();
                Library.Dispose();

                // ObjectiveCRuntime.release(Function.NativePtr);
                // ObjectiveCRuntime.release(Library.NativePtr);
            }
        }
    }
}
