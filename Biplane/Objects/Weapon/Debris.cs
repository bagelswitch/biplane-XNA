using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Biplane.ParticleSystem;

namespace Biplane.Objects.Weapon
{
    public class Debris : Flyer
    {
        private static Dictionary<string, Model> models = new Dictionary<string, Model>();

        public Debris(Game game) : base (game)
        {
        }

        public void setup(Vector3 position, Vector3 velocity, Quaternion orientation, long createTime, String modelName, GameObject parent)
        {
            base.setup();

            this.parent = parent;

            this.size = 500;

            this.damage = 10.0f;

            this.lift = 0.1f;

            this.power = 0.1f;

            this.maxAge = 50000000;
            this.createTime = createTime;

            this.modelName = "Models\\" + modelName;

            this.useXEffects = true;

            this.playerIndex = 0;
            this.useGamePad = false;
            this.useKeyboard = false;
            this.position = position;
            this.velocity = velocity;

            this.modelOrientation = this.parent.modelOrientation;
            this.currentDirection = this.parent.currentDirection;
            this.upDirection = this.parent.upDirection;
            this.downDirection = this.parent.downDirection;

            this.deltaPitch = 0.0f;
            this.deltaRoll = 0.0f;
            this.deltaYaw = 0.0f;
            this.deltaPow = 0.0f;

            this.modelOrientation = orientation;
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

        public override void generateObjects(GameTime gameTime, Vector3 screenVector, List<GameObject> objects)
        {
            // do nothing
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            FireAtLocation(this.position, this.velocity);

            float seconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            RollRight(rotRandom.Next(5));
            RotateRight(rotRandom.Next(5));
            PitchUp(rotRandom.Next(5)); 
        }

        public override void playSounds(SoundBank soundBank)
        {
            // do nothing
        }
    }
}
