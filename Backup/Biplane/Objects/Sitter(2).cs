using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Biplane.Objects.Weapon;
using Biplane.Objects.Target;
using Biplane.Objects.Static;

namespace Biplane.Objects
{
    public abstract class Sitter : Targeter
    {
        protected static Cue gunsSound = null;

        protected float range = 10000.0f;
        protected float minDis = 5000.0f;

        protected long fireRate = 1000000;
        protected long bombRate = 200000000;
        protected long missleRate = 30000000;
        protected long lastFire = 0;
        protected long lastBomb = 0;
        protected long lastMissle = 0;

        public Sitter(Game game)
            : base(game)
        {
            this.hasShadow = true;
        }

        public void setup()
        {
            base.setup();
        }

        public override void update(GameTime gameTime, Vector3 screenVector)
        {
            base.update(gameTime, screenVector);

            this.velocity = this.powerVector;

            this.position += this.velocity;

            //this.position.Z = 0.0f;

            if (this.position.Y > this.ceiling)
            {
                this.position.Y = this.ceiling;
            }

            if (this.position.Y < this.floor)
            {
                this.position.Y = this.floor;
            }

            if (Math.Abs(this.position.X) > 500000)
            {
                this.destroyed = true;
            }

        }

        public override void playSounds(SoundBank soundBank)
        {
            if (destroyed)
            {
                soundBank.PlayCue("targexpl");
            }
        }

        public override void checkTargeters(List<GameObject> targeters)
        {
            foreach (GameObject targeter in targeters)
            {
                if (targeter != null && targeter is Targeter && !(targeter.team.Equals(this.team)) && !this.Equals(targeter) && !this.Equals(targeter.parent) && !(this is Building && targeter is Building))
                {
                    if (((Targeter)targeter).target == null || ((Targeter)targeter).target.destroyed)
                    {
                        ((Targeter)targeter).target = this;
                    }
                    else
                    {
                        if (Vector3.Distance(targeter.position, ((Targeter)targeter).target.position) > Vector3.Distance(targeter.position, this.position))
                        {
                            ((Targeter)targeter).target = this;
                        }
                    }
                }
            }
        }
    }
}
