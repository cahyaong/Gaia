﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManager.cs" company="nGratis">
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
// <creation_timestamp>Wednesday, 5 August 2015 1:15:38 PM UTC</creation_timestamp>
// --------------------------------------------------------------------------------------------------------------------

namespace nGratis.Cop.Gaia.Engine
{
    using System;
    using System.Collections.Generic;
    using nGratis.Cop.Gaia.Engine.Core;

    public class SystemManager : ISystemManager
    {
        private readonly IDictionary<Type, ISystem> systemLookup = new Dictionary<Type, ISystem>();

        public void AddSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            Guard.AgainstDefaultArgument(() => system);
            Guard.AgainstInvalidOperation(this.systemLookup.ContainsKey(typeof(TSystem)));

            this.systemLookup.Add(typeof(TSystem), system);
        }

        public void RemoveSystem<TSystem>() where TSystem : ISystem
        {
            Guard.AgainstInvalidOperation(!this.systemLookup.ContainsKey(typeof(TSystem)));

            this.systemLookup.Remove(typeof(TSystem));
        }

        public void AddEntity(IEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity(IEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Clock clock)
        {
            foreach (var system in this.systemLookup.Values)
            {
                system.Update(clock);
            }
        }

        public void Render(Clock clock)
        {
            foreach (var system in this.systemLookup.Values)
            {
                system.Render(clock);
            }
        }
    }
}