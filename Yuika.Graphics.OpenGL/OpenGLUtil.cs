using System.Diagnostics;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace Yuika.Graphics.OpenGL;

internal static class OpenGLUtil
{
    private static int? MaxLabelLength;

    [Conditional("DEBUG")]
    [DebuggerNonUserCode]
    internal static void CheckLastError()
    {
        ErrorCode error = GL.GetError();
        if (error != 0)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            throw new VeldridException("GL.GetError indicated an error: " + error);
        }
    }

    internal static unsafe void SetObjectLabel(ObjectLabelIdentifier identifier, uint target, string name)
    {
        if ((OpenGLGraphicsDevice._getProcAddress?.Invoke("glObjectLabel") ?? 0) != 0)
        {
            int byteCount = Encoding.UTF8.GetByteCount(name);
            if (MaxLabelLength == null)
            {
                int maxLabelLength = -1;
                GL.GetInteger(GetPName.MaxLabelLength, &maxLabelLength);
                CheckLastError();
                MaxLabelLength = maxLabelLength;
            }
            if (byteCount >= MaxLabelLength)
            {
                name = name.Substring(0, MaxLabelLength.Value - 4) + "...";
                byteCount = Encoding.UTF8.GetByteCount(name);
            }
            
            GL.ObjectLabel(identifier, target, byteCount, name);
            CheckLastError();
        }
    }

    internal static TextureTarget GetTextureTarget(OpenGLTexture glTex, uint arrayLayer)
    {
        if ((glTex.Usage & TextureUsage.Cubemap) == TextureUsage.Cubemap)
        {
            switch (arrayLayer % 6)
            {
                case 0:
                    return TextureTarget.TextureCubeMapPositiveX;
                case 1:
                    return TextureTarget.TextureCubeMapNegativeX;
                case 2:
                    return TextureTarget.TextureCubeMapPositiveY;
                case 3:
                    return TextureTarget.TextureCubeMapNegativeY;
                case 4:
                    return TextureTarget.TextureCubeMapPositiveZ;
                case 5:
                    return TextureTarget.TextureCubeMapNegativeZ;
            }
        }

        return glTex.TextureTarget;
    }
}