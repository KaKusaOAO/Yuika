// See https://aka.ms/new-console-template for more information

#if __MACOS__
using System.Runtime.InteropServices;
using MetalFX;
using ObjCRuntime;
using Yuika.Graphics;
using Yuika.Graphics.Metal;
#endif

Console.WriteLine("Hello, world");

#if __MACOS__

try
{
    var device = GraphicsDeviceFactory.Shared.CreateMetal(new GraphicsDeviceOptions(false));
    var factory = device.ResourceFactory;

    var info = (device.BackendInfo as BackendInfoMetal)!;
    var desc = info.CreateTemporalScalerDescriptor();
    var cl = factory.CreateCommandList();
    var scaler = info.CreateScaler(device, desc);
    info.ExecuteMetalFX(cl, scaler);
}
catch
{
    Console.WriteLine("Oh no");
}

var arr = Array.Empty<int>();

#endif