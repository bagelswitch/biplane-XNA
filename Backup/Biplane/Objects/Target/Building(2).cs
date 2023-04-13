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
using Biplane.Objects.Static;

namespace Biplane.Objects.Target
{
    public class Building : Sitter
    {
        private static Random locationRand = new Random();

        private static Dictionary<string, Model> models = new Dictionary<string, Model>();

        public Building(Game game) : base(game)
        {
            this.modelName = "Models\\factory";
            this.name = "factory";
            this.debris.Add("debrisb1");
            this.debris.Add("debrisb2");
            this.debris.Add("debrisb3");
            this.debris.Add("debrisb4");
            this.debris.Add("debrisb5");
            this.debris.Add("debrisb6");
        }

        public override void loadModel()
        {
            if (!models.ContainsKey(this.modelName))
            {
                Model newmodel = game.Content.Load<Model>(modelName);
                models.Add(this.modelName, newmodel);
            }
            this.model = models[this.modelName];
        }

        public void setup(GameTime gameTime)
        {
            base.setup();

            this.target = null;

            this.size = 8000;

            this.useXEffects = true;
            this.scale = 2.0f;

            this.playerIndex = 0;
            this.useGamePad = false;
            this.useKeyboard = false;

            this.range = 20000.0f;

            this.fireRate = 2000000;
            this.lastFire = 0;

            int randCenter = (locationRand.Next(40) - 20);
            if (randCenter == 0) randCenter = 1;
            int center = (Math.Sign(randCenter) * 30000) + (randCenter * 30000);
            int randZCenter = (locationRand.Next(40000) - 20000);
            this.position = new Vector3((float)center, 0.0f, (float)randZCenter);

            this.velocity = Vector3.Zero;

            this.damage = 200.0f;

            this.createTime = gameTime.TotalRealTime.Ticks;

            this.destroyed = false;

            float pitch = 0.0f;
            float yaw = -90.0f;
            float roll = 90.0f;

            this.modelOrientation = Quaternion.CreateFromAxisAngle(Vector3.Backward, MathHelper.ToRadians(pitch)) * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(yaw)) * Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(roll));

        }

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> weapons)
        {
            this.firingGuns = false;
            if (this.target != null)
            {
                float distance = Vector3.Distance(this.target.position, this.position);
                if (!this.target.destroyed && distance < range && (this.position.Y + 3000.0f) < this.target.position.Y && (gameTime.TotalRealTime.Ticks - lastFire) > fireRate)
                {
                    this.firingGuns = true;
                    lastFire = gameTime.TotalRealTime.Ticks;
                    Vector3 thisPos = new Vector3(this.position.X, this.position.Y + 3000.0f, this.position.Z);
                    Vector3 targetPos = new Vector3(this.target.position.X, this.target.position.Y + 3000.0f, this.target.position.Z) - thisPos;
                    targetPos.Normalize();
                    weapons.Add(newBullet(new Vector3(this.position.X, this.position.Y + 6000.0f, 0.0f), targetPos * 500.0f, gameTime.TotalRealTime.Ticks, this, game));
                }
            }
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            float ground = Terrain.GetHeight(this.position);
            this.position.Y = ground;

            if (this.name == "factory")
            {
                if (locationRand.Next(10) >= 9)
                {
                    ((BiplaneGame)game).vaporPlumeParticles.AddParticle(this.position + new Vector3(-800.0f, 8000.0f, -1500.0f), new Vector3(0.0f, 1000.0f, 0.0f));
                }
                if (locationRand.Next(10) >= 9)
                {
                    ((BiplaneGame)game).vaporPlumeParticles.AddParticle(this.position + new Vector3(1300.0f, 8000.0f, -1500.0f), new Vector3(0.0f, 1000.0f, 0.0f));
                }
                if (locationRand.Next(10) >= 9)
                {
                    ((BiplaneGame)game).vaporPlumeParticles.AddParticle(this.position + new Vector3(3500.0f, 8000.0f, -1500.0f), new Vector3(0.0f, 1000.0f, 0.0f));
                }
            }
            else
            {
                if (locationRand.Next(10) >= 9)
                {
                    ((BiplaneGame)game).vaporPlumeParticles.AddParticle(this.position + new Vector3(5500.0f, 7500.0f, -1500.0f), new Vector3(0.0f, 1000.0f, 0.0f));
                }
            }

            if (ground <= Water.waterLevel && this.name == "factory")
            {
                // make a boat!
                modelName = "Models\\battleship";
                this.name = "battleship";
                this.debris.Clear();
                this.debris.Add("debrisbs1");
                this.debris.Add("debrisbs2");
                this.debris.Add("debrisbs3");
                this.debris.Add("debrisbs3");
                this.debris.Add("debrisbs4");
                this.debris.Add("debrisbs5");
            }
            else if (ground > Water.waterLevel && this.name == "battleship")
            {
                // a building goes on land
                modelName = "Models\\factory";
                this.name = "factory";
                this.debris.Clear();
                this.debris.Add("debrisb1");
                this.debris.Add("debrisb2");
                this.debris.Add("debrisb3");
                this.debris.Add("debrisb4");
                this.debris.Add("debrisb5");
                this.debris.Add("debrisb6");
            }
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
