﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameManager.cs" company="nGratis">
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
// <creation_timestamp>Thursday, 30 July 2015 10:36:23 AM UTC</creation_timestamp>
// --------------------------------------------------------------------------------------------------------------------

namespace nGratis.Cop.Gaia.Client.Mono
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using nGratis.Cop.Core.Contract;
    using nGratis.Cop.Gaia.Client.Mono.Core;
    using nGratis.Cop.Gaia.Engine;
    using nGratis.Cop.Gaia.Engine.Common;
    using nGratis.Cop.Gaia.Engine.Core;
    using nGratis.Cop.Gaia.Engine.Data;

    internal class GameManager : Game, IGameManager
    {
        private readonly IColor backgroundColor = new RgbColor(37, 37, 38);

        private readonly ITemplateManager templateManager;

        private readonly IEntityManager entityManager;

        private readonly ISystemManager systemManager;

        private readonly IProbabilityManager probabilityManager;

        private readonly Size tileSize;

        private readonly Size mapSize;

        private IDrawingCanvas drawingCanvas;

        private IFontManager fontManager;

        public GameManager()
            : this(new TemplateManager(), new EntityManager(), new SystemManager(), new ProbabilityManager())
        {
        }

        public GameManager(
            ITemplateManager templateManager,
            IEntityManager entityManager,
            ISystemManager systemManager,
            IProbabilityManager probabilityManager)
        {
            Guard.AgainstNullArgument(() => templateManager);
            Guard.AgainstNullArgument(() => entityManager);
            Guard.AgainstNullArgument(() => systemManager);
            Guard.AgainstNullArgument(() => probabilityManager);

            this.templateManager = templateManager;
            this.entityManager = entityManager;
            this.systemManager = systemManager;
            this.probabilityManager = probabilityManager;

            var graphicsDeviceManager = new GraphicsDeviceManager(this)
                {
                    PreferredBackBufferWidth = 1280,
                    PreferredBackBufferHeight = 720
                };

            this.tileSize = new Size(10, 10);

            this.mapSize = new Size(
                graphicsDeviceManager.PreferredBackBufferWidth / this.tileSize.Width,
                graphicsDeviceManager.PreferredBackBufferHeight / this.tileSize.Height);

            graphicsDeviceManager.ApplyChanges();

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            this.fontManager = new FontManager(this.Content);
            this.drawingCanvas = new MonoDrawingCanvas(this.GraphicsDevice, this.fontManager);

            this.InitializeTemplateManager();
            this.InitializeEntityManager();
            this.InitializeSystemManager();

            this.InitializeGame();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            this.systemManager.Update(gameTime.ToCopClock());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.drawingCanvas.BeginBatch();
            this.drawingCanvas.Clear(this.backgroundColor);
            this.systemManager.Render(gameTime.ToCopClock());
            this.drawingCanvas.EndBatch();

            base.Draw(gameTime);
        }

        private void InitializeTemplateManager()
        {
            this.templateManager.InitializeCreatureTemplates();
        }

        private void InitializeEntityManager()
        {
            this.entityManager.RegisterComponentType<StatisticComponent>();
            this.entityManager.RegisterComponentType<ConstitutionComponent>();
            this.entityManager.RegisterComponentType<TraitComponent>();
            this.entityManager.RegisterComponentType<PlacementComponent>();

            Observable
                .FromEventPattern<EntityChangedEventArgs>(this.entityManager, "EntityCreated")
                .Subscribe(pattern => this.systemManager.AddEntity(pattern.EventArgs.Entity));

            Observable
                .FromEventPattern<EntityChangedEventArgs>(this.entityManager, "EntityDestroyed")
                .Subscribe(pattern => this.systemManager.RemoveEntity(pattern.EventArgs.Entity));
        }

        private void InitializeSystemManager()
        {
            this.systemManager.AddSystem(new MovementSystem(this.entityManager, this.templateManager, this.probabilityManager, mapSize));
            this.systemManager.AddSystem(new RenderSystem(this.drawingCanvas, this.entityManager, this.templateManager, tileSize));

#if DEBUG
            this.systemManager.AddSystem(new DiagnosticSystem(this.drawingCanvas, this.entityManager, this.templateManager));
#endif
        }

        private void InitializeGame()
        {
            var template = this.templateManager.FindTemplate("Character");

            var entities = Enumerable
                .Range(0, 250)
                .Select(_ => this.entityManager.CreateEntity(template));

            var constitutionBucket = this.entityManager.FindComponentBucket<ConstitutionComponent>();
            var placementBucket = this.entityManager.FindComponentBucket<PlacementComponent>();

            foreach (var entity in entities)
            {
                var constitutionComponent = constitutionBucket.FindComponent(entity);
                constitutionComponent.HitPoint = this.probabilityManager.Roll(0, 100);

                var placementComponent = placementBucket.FindComponent(entity);

                placementComponent.Position = new nGratis.Cop.Gaia.Engine.Data.Point(
                    this.probabilityManager.Roll(0, this.mapSize.Width),
                    this.probabilityManager.Roll(0, this.mapSize.Height));

                placementComponent.Direction = new Vector(
                    this.probabilityManager.Roll(-1, 1),
                    this.probabilityManager.Roll(-1, 1));

                placementComponent.Speed = this.probabilityManager.Roll(0, 3);
            }
        }
    }
}