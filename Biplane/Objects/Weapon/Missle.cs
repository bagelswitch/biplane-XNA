using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Biplane.Objects.Weapon;
using Biplane.ParticleSystem;

namespace Biplane.Objects.Weapon
{
    public class Missle : Flyer
    {
        private static Random rotRand = new Random();

        private static Model model;

        public Missle(Game game) : base(game)
        {
            this.trailEmitter = new ParticleEmitter(((BiplaneGame)game).vaporTrailParticles, this.trailParticlesPerSecond, this.position);

            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");
            this.debris.Add("pea_proj");

            modelName = "Models\\missle";
        }

        public void setup(Vector3 position, Vector3 velocity, Quaternion orientation, long createTime, GameObject parent)
        {
            base.setup();

            this.parent = parent;

            this.trailParticlesPerSecond = 300;
            this.numExplosionParticles = 50;
            this.numExplosionSmokeParticles = 50;

            this.size = 250;

            this.range = 5000.0f;

            this.power = 1500.0f;

            this.ceiling = 500000.0f;

            this.lift = 0.25f;
       
            this.drag = 0.25f;

            this.damage = 50.0f;

            this.maxAge = 25000000;
            this.createTime = createTime;

            this.pitchRate = 0.10f;
            this.yawRate = 0.10f;
            this.rollRate = 0.10f;

            this.minDis = 0;

            modelName = "Models\\missle";

            this.playerIndex = 0;
            this.useGamePad = false;
            this.useKeyboard = false;
            this.position = position;
            this.velocity = velocity;

            this.modelOrientation = orientation;
        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects)
        {
            if (this.target != null && !this.target.destroyed)
            {
                float distance = Vector3.Distance(this.target.position, this.position);
                if ((gameTime.TotalRealTime.Ticks - createTime) > maxAge || (distance < range && (gameTime.TotalRealTime.Ticks - createTime) > (maxAge / 2)))
                {
                    this.destroyed = true;
                    for (int i = 0; i < numExplosionParticles; i++)
                    {
                        Vector3 jitter = new Vector3(rotRand.Next(3000) - 1500, rotRand.Next(3000) - 1500, rotRand.Next(3000) - 1500);
                        ((BiplaneGame)game).explosionParticles.AddParticle(position + jitter, Vector3.Zero);
                    }

                    for (int i = 0; i < numExplosionSmokeParticles; i++)
                    {
                        Vector3 jitter = new Vector3(rotRand.Next(3000) - 1500, rotRand.Next(3000) - 1500, rotRand.Next(3000) - 1500);
                        ((BiplaneGame)game).explosionSmokeParticles.AddParticle(position + jitter, Vector3.Zero);
                    }
                }
            }
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            turnTowardsTarget(gameTime);

            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (this.target == null || Vector3.Distance(this.position, this.target.position) > range)
            {
                int chanceRot = rotRand.Next(100);
                if (chanceRot > 80)
                {
                    RotateLeft(this.yawRate * seconds * this.velocity.Length() / 50.0f * (rotRand.Next(8)-4));
                    PitchDown(this.pitchRate * seconds * this.velocity.Length() / 50.0f * (rotRand.Next(8)-4));
                }
            }
            this.deltaPow = this.power * seconds;

            base.update(gameTime, screenVector);
        }

        public override void playSounds(SoundBank soundBank)
        {
            if (destroyed)
            {
                soundBank.PlayCue("bombexpl");
            }
        }
    }
}
