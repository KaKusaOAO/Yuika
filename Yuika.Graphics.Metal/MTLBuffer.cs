using Metal;

namespace Yuika.Graphics.Metal
{
    internal class MTLBuffer : DeviceBuffer
    {
        private string _name;
        private bool _disposed;

        public override uint SizeInBytes { get; }
        public override BufferUsage Usage { get; }

        public uint ActualCapacity { get; }

        public override string Name
        {
            get => _name;
            set
            {
                NSString nameNSS = new NSString(value);
                DeviceBuffer.AddDebugMarker(nameNSS, new NSRange(0, (IntPtr)SizeInBytes));
                nameNSS.Dispose();
                // ObjectiveCRuntime.release(nameNSS.NativePtr);
                _name = value;
            }
        }

        public override bool IsDisposed => _disposed;

        public IMTLBuffer DeviceBuffer { get; private set; }

        public MTLBuffer(ref BufferDescription bd, MTLGraphicsDevice gd)
        {
            SizeInBytes = bd.SizeInBytes;
            uint roundFactor = (4 - (SizeInBytes % 4)) % 4;
            ActualCapacity = SizeInBytes + roundFactor;
            Usage = bd.Usage;
            DeviceBuffer = gd.Device.CreateBuffer(ActualCapacity, 0)!;
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                DeviceBuffer.Dispose();
                // ObjectiveCRuntime.release(DeviceBuffer.NativePtr);
            }
        }
    }
}
