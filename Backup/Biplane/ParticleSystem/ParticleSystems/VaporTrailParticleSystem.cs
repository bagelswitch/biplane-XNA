#region File Description
//-----------------------------------------------------------------------------
// ProjectileTrailParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Biplane.ParticleSystem
{
    /// <summary>
    /// Custom particle system for leaving smoke trails behind the rocket projectiles.
    /// </summary>
    class VaporTrailParticleSystem : ParticleSystem
    {
        public VaporTrailParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "ParticleSystem\\vapor";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(3);

            settings.DurationRandomness = 1.5f;

            settings.EmitterVelocitySensitivity = 0.1f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1;

            settings.MinVerticalVelocity = -1;
            settings.MaxVerticalVelocity = 1;

            settings.MinColor = new Color(255, 255, 255, 255);
            settings.MaxColor = new Color(255, 255, 255, 0);

            settings.MinRotateSpeed = -4;
            settings.MaxRotateSpeed = 4;

            settings.MinStartSize = 200;
            settings.MaxStartSize = 400;

            settings.MinEndSize = 500;
            settings.MaxEndSize = 1500;
        }
    }
}
