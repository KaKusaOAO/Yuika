namespace Yuika.Graphics.Metal;

public static class GraphicsDeviceFactoryExtension
{
    /// <summary>
        /// Creates a new <see cref="GraphicsDevice"/> using Metal.
        /// </summary>
        /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
        /// <returns>A new <see cref="GraphicsDevice"/> using the Metal API.</returns>
        public static GraphicsDevice CreateMetal(this GraphicsDeviceFactory self,
            GraphicsDeviceOptions options)
        {
            return new MTLGraphicsDevice(options, null);
        }

        /// <summary>
        /// Creates a new <see cref="GraphicsDevice"/> using Metal, with a main Swapchain.
        /// </summary>
        /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
        /// <param name="swapchainDescription">A description of the main Swapchain to create.</param>
        /// <returns>A new <see cref="GraphicsDevice"/> using the Metal API.</returns>
        public static GraphicsDevice CreateMetal(this GraphicsDeviceFactory self,
            GraphicsDeviceOptions options, SwapchainDescription swapchainDescription)
        {
            return new MTLGraphicsDevice(options, swapchainDescription);
        }

#if __MACOS__
        /// <summary>
        /// Creates a new <see cref="GraphicsDevice"/> using Metal, with a main Swapchain.
        /// </summary>
        /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
        /// <param name="nsWindow">A pointer to an NSWindow object, which will be used to create the Metal device's swapchain.
        /// </param>
        /// <returns>A new <see cref="GraphicsDevice"/> using the Metal API.</returns>
        public static GraphicsDevice CreateMetal(this GraphicsDeviceFactory self,
            GraphicsDeviceOptions options, NSWindow nsWindow)
        {
            SwapchainDescription swapchainDesc = new SwapchainDescription(
                new NSWindowSwapchainSource(nsWindow.Handle.Handle),
                0, 0,
                options.SwapchainDepthFormat,
                options.SyncToVerticalBlank,
                options.SwapchainSrgbFormat);

            return new MTLGraphicsDevice(options, swapchainDesc);
        }
#endif
}