﻿namespace Yuika.Graphics.OpenGL;

internal interface OpenGLDeferredResource
{
    bool Created { get; }
    void EnsureResourcesCreated();
    void DestroyGLResources();
}