﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SdkRenderManager.cs" company="nGratis">
//  The MIT License (MIT)
//
//  Copyright (c) 2014 - 2015 Cahya Ong
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
// </copyright>
// <author>Cahya Ong - cahya.ong@gmail.com</author>
// <creation_timestamp>Wednesday, 29 July 2015 12:54:01 PM UTC</creation_timestamp>
// --------------------------------------------------------------------------------------------------------------------

namespace nGratis.Cop.Gaia.Wpf.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    internal class SdkRenderManager : IRenderManager
    {
        private readonly CubePrimitive cube;

        private GraphicsDevice graphicsDevice;

        public SdkRenderManager()
        {
            this.cube = new CubePrimitive();
        }

        public void Draw()
        {
            if (this.graphicsDevice == null)
            {
                return;
            }

            this.graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

            var worldMatrix = Matrix.CreateFromYawPitchRoll(0.5F, 0.5F, 0) * Matrix.CreateTranslation(new Vector3());
            var viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 2.5F), Vector3.Zero, Vector3.Up);
            var projectionMatrix = Matrix.CreatePerspectiveFieldOfView(1, this.graphicsDevice.Viewport.AspectRatio, 1, 10);

            this.cube.Draw(worldMatrix, viewMatrix, projectionMatrix, Color.CornflowerBlue);
        }

        public void SetRenderTarget(RenderTarget2D renderTarget)
        {
            if (renderTarget == null)
            {
                this.graphicsDevice = null;
                return;
            }

            this.graphicsDevice = renderTarget.GraphicsDevice;

            if (this.graphicsDevice != null)
            {
                this.cube.Initialize(this.graphicsDevice);
            }
        }
    }
}