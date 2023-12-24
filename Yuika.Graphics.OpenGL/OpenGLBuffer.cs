using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using static Yuika.Graphics.OpenGL.OpenGLUtil;

namespace Yuika.Graphics.OpenGL;

internal unsafe class OpenGLBuffer : DeviceBuffer, OpenGLDeferredResource
{
    private readonly OpenGLGraphicsDevice _gd;
    private uint _buffer;
    private bool _dynamic;
    private bool _disposeRequested;

    private string _name;
    private bool _nameChanged;

    public override string Name { get => _name; set { _name = value; _nameChanged = true; } }

    public override uint SizeInBytes { get; }
    public override BufferUsage Usage { get; }

    public uint Buffer => _buffer;

    public bool Created { get; private set; }

    public override bool IsDisposed => _disposeRequested;

    public OpenGLBuffer(OpenGLGraphicsDevice gd, uint sizeInBytes, BufferUsage usage)
    {
        _gd = gd;
        SizeInBytes = sizeInBytes;
        _dynamic = (usage & BufferUsage.Dynamic) == BufferUsage.Dynamic;
        Usage = usage;
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
                SetObjectLabel(ObjectLabelIdentifier.Buffer, _buffer, _name);
            }
        }
    }

    public void CreateGLResources()
    {
        Debug.Assert(!Created);

        if (_gd.Extensions.ARB_DirectStateAccess)
        {
            uint buffer;
            GL.CreateBuffers(1, &buffer);
            CheckLastError();
            _buffer = buffer;

            GL.NamedBufferData(
                _buffer,
                (IntPtr) SizeInBytes,
                0,
                _dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
            CheckLastError();
        }
        else
        {
            GL.GenBuffers(1, out _buffer);
            CheckLastError();

            GL.BindBuffer(BufferTarget.CopyReadBuffer, _buffer);
            CheckLastError();

            GL.BufferData(
                BufferTarget.CopyReadBuffer,
                (IntPtr)SizeInBytes,
                0,
                _dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
            CheckLastError();
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
        uint buffer = _buffer;
        GL.DeleteBuffers(1, ref buffer);
        CheckLastError();
    }
}