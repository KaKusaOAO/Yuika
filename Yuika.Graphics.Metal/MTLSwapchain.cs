using CoreAnimation;
using Metal;
using ObjCRuntime;

namespace Yuika.Graphics.Metal
{
    internal class MTLSwapchain : Swapchain
    {
        private readonly MTLSwapchainFramebuffer _framebuffer;
        private CAMetalLayer _metalLayer;
        private readonly MTLGraphicsDevice _gd;
#if __IOS__ || __TVOS__
        private UIView _uiView; // Valid only when a UIViewSwapchainSource is used.
#endif
        private bool _syncToVerticalBlank;
        private bool _disposed;

        private ICAMetalDrawable? _drawable;

        public override Framebuffer Framebuffer => _framebuffer;
        public override bool SyncToVerticalBlank
        {
            get => _syncToVerticalBlank;
            set
            {
                if (_syncToVerticalBlank != value)
                {
                    SetSyncToVerticalBlank(value);
                }
            }
        }

        public override string Name { get; set; }

        public override bool IsDisposed => _disposed;

        public ICAMetalDrawable? CurrentDrawable => _drawable;

        public MTLSwapchain(MTLGraphicsDevice gd, ref SwapchainDescription description)
        {
            _gd = gd;
            _syncToVerticalBlank = description.SyncToVerticalBlank;

            uint width;
            uint height;

            SwapchainSource source = description.Source;
            
            #if __MACOS__
            if (source is NSWindowSwapchainSource nsWindowSource)
            {
                NSWindow nswindow = new NSWindow(nsWindowSource.NSWindow);
                NSView contentView = nswindow.ContentView;
                CGSize windowContentSize = contentView.Frame.Size;
                width = (uint)windowContentSize.Width;
                height = (uint)windowContentSize.Height;

                if (contentView.Layer is CAMetalLayer metalLayer)
                {
                    _metalLayer = metalLayer;
                }
                else {
                    _metalLayer = new CAMetalLayer();
                    contentView.WantsLayer = true;
                    contentView.Layer = _metalLayer;
                }
                
                // if (!CAMetalLayer.TryCast(contentView.Layer, out _metalLayer))
                // {
                //     _metalLayer = new CAMetalLayer();
                //     contentView.WantsLayer = true;
                //     contentView.Layer = _metalLayer;
                // }
            }
            else if (source is NSViewSwapchainSource nsViewSource)
            {
                NSView contentView = Runtime.GetNSObject<NSView>(nsViewSource.NSView)!; // new NSView(nsViewSource.NSView);
                CGSize windowContentSize = contentView.Frame.Size;
                width = (uint)windowContentSize.Width;
                height = (uint)windowContentSize.Height;

                if (contentView.Layer is CAMetalLayer metalLayer)
                {
                    _metalLayer = metalLayer;
                }
                else
                {
                    _metalLayer = new CAMetalLayer();
                    contentView.WantsLayer = true;
                    contentView.Layer = _metalLayer;
                }
                
                // if (!CAMetalLayer.TryCast(contentView.layer, out _metalLayer))
                // {
                //     _metalLayer = CAMetalLayer.New();
                //     contentView.wantsLayer = true;
                //     contentView.layer = _metalLayer.NativePtr;
                // }
            }
            else 
            #endif
            
            #if __IOS__ || __TVOS__
            if (source is UIViewSwapchainSource uiViewSource)
            {
                UIScreen mainScreen = UIScreen.MainScreen;
                nfloat nativeScale = mainScreen.NativeScale;

                _uiView = Runtime.GetNSObject<UIView>(uiViewSource.UIView)!; // new UIView(uiViewSource.UIView);
                CGSize viewSize = _uiView.Frame.Size;
                width = (uint)(viewSize.Width * nativeScale);
                height = (uint)(viewSize.Height * nativeScale);

                if (_uiView.Layer is CAMetalLayer metalLayer)
                {
                    _metalLayer = metalLayer;
                }
                else
                {
                    _metalLayer = new CAMetalLayer();
                    _metalLayer.Frame = _uiView.Frame;
                    _metalLayer.Opaque = true;
                    _uiView.Layer.AddSublayer(_metalLayer);
                }
                
                // if (!CAMetalLayer.TryCast(_uiView.layer, out _metalLayer))
                // {
                //     _metalLayer = CAMetalLayer.New();
                //     _metalLayer.frame = _uiView.frame;
                //     _metalLayer.opaque = true;
                //     _uiView.layer.addSublayer(_metalLayer.NativePtr);
                // }
            }
            else
            #endif
            
            {
                throw new VeldridException($"A Metal Swapchain can only be created from an NSWindow, NSView, or UIView.");
            }

            PixelFormat format = description.ColorSrgb
                ? PixelFormat.B8_G8_R8_A8_UNorm_SRgb
                : PixelFormat.B8_G8_R8_A8_UNorm;

            _metalLayer.Device = _gd.Device;
            _metalLayer.PixelFormat = MTLFormats.VdToMTLPixelFormat(format, false);
            _metalLayer.FramebufferOnly = true;
            _metalLayer.DrawableSize = new CGSize(width, height);

            SetSyncToVerticalBlank(_syncToVerticalBlank);

            GetNextDrawable();

            _framebuffer = new MTLSwapchainFramebuffer(
                gd,
                this,
                width,
                height,
                description.DepthFormat,
                format);
        }

        public void GetNextDrawable()
        {
            _drawable?.Dispose();
            // ObjectiveCRuntime.release(_drawable.NativePtr);

            using (new NSAutoreleasePool())
            {
                _drawable = _metalLayer.NextDrawable();
                // ObjectiveCRuntime.retain(_drawable.NativePtr);
            }
        }

        public override void Resize(uint width, uint height)
        {
#if __IOS__ || __TVOS__
            if (_uiView != null)
            {
                UIScreen mainScreen = UIScreen.MainScreen;
                nfloat nativeScale = mainScreen.NativeScale;
                width = (uint)(width * nativeScale);
                height = (uint)(height * nativeScale);

                _metalLayer.Frame = _uiView.Frame;
            }
#endif

            _framebuffer.Resize(width, height);
            _metalLayer.DrawableSize = new CGSize(width, height);
#if __IOS__ || __TVOS__
            if (_uiView != null)
            {
                _metalLayer.Frame = _uiView.Frame;
            }
#endif
            GetNextDrawable();
        }

        private void SetSyncToVerticalBlank(bool value)
        {
            _syncToVerticalBlank = value;
            #if __MACOS__ || __MACCATALYST__
            _metalLayer.DisplaySyncEnabled = value;
            #endif
        }

        public override void Dispose()
        {
            _drawable?.Dispose();
            // ObjectiveCRuntime.objc_msgSend(_drawable.NativePtr, "release");
            _framebuffer.Dispose();
            _metalLayer.Dispose();
            // ObjectiveCRuntime.release(_metalLayer.NativePtr);

            _disposed = true;
        }
    }
}
