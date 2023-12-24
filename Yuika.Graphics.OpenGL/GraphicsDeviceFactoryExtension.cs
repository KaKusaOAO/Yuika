namespace Yuika.Graphics.OpenGL;

public static class GraphicsDeviceFactoryExtension
{
    /// <summary>
    /// Creates a new <see cref="GraphicsDevice"/> using OpenGL or OpenGL ES, with a main Swapchain.
    /// </summary>
    /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
    /// <param name="platformInfo">An <see cref="OpenGL.OpenGLPlatformInfo"/> object encapsulating necessary OpenGL context
    /// information.</param>
    /// <param name="width">The initial width of the window.</param>
    /// <param name="height">The initial height of the window.</param>
    /// <returns>A new <see cref="GraphicsDevice"/> using the OpenGL or OpenGL ES API.</returns>
    public static GraphicsDevice CreateOpenGL(
        this GraphicsDeviceFactory self,
        GraphicsDeviceOptions options,
        OpenGLPlatformInfo platformInfo,
        uint width,
        uint height)
    {
        return new OpenGLGraphicsDevice(options, platformInfo, width, height);
    }

    /// <summary>
    /// Creates a new <see cref="GraphicsDevice"/> using OpenGL ES, with a main Swapchain.
    /// This overload can only be used on iOS or Android to create a GraphicsDevice for an Android Surface or an iOS UIView.
    /// </summary>
    /// <param name="options">Describes several common properties of the GraphicsDevice.</param>
    /// <param name="swapchainDescription">A description of the main Swapchain to create.
    /// The SwapchainSource must have been created from an Android Surface or an iOS UIView.</param>
    /// <returns>A new <see cref="GraphicsDevice"/> using the OpenGL or OpenGL ES API.</returns>
    public static GraphicsDevice CreateOpenGLES(
        this GraphicsDeviceFactory self,
        GraphicsDeviceOptions options,
        SwapchainDescription swapchainDescription)
    {
        return new OpenGLGraphicsDevice(options, swapchainDescription);
    }
}