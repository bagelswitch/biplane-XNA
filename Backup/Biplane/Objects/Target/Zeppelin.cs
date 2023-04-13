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
using Biplane.Objects.Static;
using Biplane.ParticleSystem;

namespace Biplane.Objects.Target
{
    public class Zeppelin : Sitter
    {
        private static Model model;

        private float ystart;

        private static Random locationRand = new Random();

        public Zeppelin(Game game)
            : base(game)
        {
            this.debris.Add("debrisz1");
            this.debris.Add("debrisz2");
            this.debris.Add("debrisz3");
            this.debris.Add("debrisz4");
            this.debris.Add("debrisz5");
            this.debris.Add("debrisz6");

            modelName = "Models\\zepp";
        }

        public void setup(GameTime gameTime)
        {
            base.setup();

            this.target = null;

            this.size = 4000;

            this.damage = 200.0f;

            this.floor = -80000;

            this.ceiling = 250000;

            this.power = 50.0f;

            this.minDis = 6000.0f;

            modelName = "Models\\zepp";

            this.name = "zeppelin";

            this.bombCount = 100;
            this.missleCount = 0;

            this.useXEffects = true;
            this.scale = 3.0f;

            this.playerIndex = 0;
            this.useGamePad = false;
            this.useKeyboard = false;

            int randCenter = (locationRand.Next(40) - 20);
            if (randCenter == 0) randCenter = 1;
            int center = randCenter * 20000;
            int randZCenter = (locationRand.Next(40000) - 20000);
            this.ystart = (locationRand.Next(300) / 100.0f * 10000.0f) + 10000.0f;
            this.position = new Vector3((float)center, this.ystart, (float)randZCenter);
            this.velocity = new Vector3(0.001f, 0.001f, 0.000f);

            this.createTime = gameTime.TotalRealTime.Ticks;

            this.destroyed = false;

            float pitch = 0.0f;
            float yaw = 0.0f;
            float roll = 0.0f;

            this.modelOrientation = Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(pitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(yaw)) * Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(roll));
        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> weapons)
        {
            this.firingGuns = false;

            if (this.target != null)
            {
                float distance = Vector3.Distance(this.target.position, this.position);
                // OK, so this doesn't really give the angle, but we can tell above/below/ahead from the Vector cross-product
                Vector3 direction = (this.target.position - this.position);
                direction.Normalize();
                
                float angleFromTarget = Vector3.Cross(direction, currentDirection).Z;

                if (!this.target.destroyed && distance < range && (gameTime.TotalRealTime.Ticks - lastFire) > fireRate && this.target.position.Y < this.position.Y)
                {
                    this.firingGuns = true;
                    lastFire = gameTime.TotalRealTime.Ticks;
                    Vector3 thisPos = new Vector3(this.position.X, this.position.Y - 3000.0f, this.position.Z);
                    Vector3 targetPos = new Vector3(this.target.position.X, this.target.position.Y - 3000.0f, this.target.position.Z) - thisPos;
                    targetPos.Normalize();
                    weapons.Add(newBullet(new Vector3(this.position.X, this.position.Y - 4000.0f, 0.0f), targetPos * 500.0f, gameTime.TotalRealTime.Ticks, this, game));
                }

                // directly below you! bombs away!
                if (!this.target.destroyed && angleFromTarget >= 0.9f && (gameTime.TotalRealTime.Ticks - this.lastBomb) > this.bombRate)
                {
                    this.lastBomb = gameTime.TotalRealTime.Ticks;
                    weapons.Add(newBomb(this.position + (downDirection * 4000) + (currentDirection * 500), this.velocity + (downDirection * this.velocity.Length() * 0.5f), this.modelOrientation, gameTime.TotalRealTime.Ticks, this, game));
                    this.firingBombs = false;
                }
            }
            if (this.damage <= 50)
            {
                ((BiplaneGame)game).projectileTrailParticles.AddParticle(this.position + (currentDirection * -2500.0f), Vector3.Zero);
            }
            else if (this.damage <= 30)
            {
                ((BiplaneGame)game).smokePlumeParticles.AddParticle(this.position + (currentDirection * -2500.0f), Vector3.Zero);
            }
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            this.deltaPow = this.power * seconds;

            if (this.target != null && !this.target.destroyed)
            {
                yawToFaceTarget(gameTime);
            }
            else
            {
                yawTowardsHome(gameTime);
            }

            float ground = Terrain.GetHeight(this.position);

            this.position.Y = this.ystart + ground; 
        }

        public override void playSounds(SoundBank soundBank)
        {
            if (!this.destroyed && !(this.target == null) && !this.target.destroyed && firingGuns)
            {
                soundBank.PlayCue("targfire");
            }

            base.playSounds(soundBank);
        }
    }
}
