// See https://aka.ms/new-console-template for more information

#if __MACOS__
using MetalFX;
using ObjCRuntime;
using Yuika.Graphics;
using Yuika.Graphics.Metal;
#endif

Console.WriteLine("");

#if __MACOS__

var device = GraphicsDeviceFactory.Shared.CreateMetal(new GraphicsDeviceOptions(false));
var factory = device.ResourceFactory;

var info = (device.BackendInfo as BackendInfoMetal)!;
var desc = info.CreateTemporalScalerDescriptor();
factory.CreateCommandList();

#endif