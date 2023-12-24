# Yuika

> Just a fork of the Veldrid library.

## Features

- [x] MetalFX
  - [ ] Provide wrapper for access to MetalFX
    -  Uses MetalFX framework in Xamarin SDK for now, which is not intended
- [x] Creation of `GraphicsDevice` are moved to a new class `GraphicsDeviceFactory`
    - [x] We can use extension class to add more types of backends
    - [ ] Rewrite `GraphicsBackend` so it would be extendable
- [ ] Still incomplete