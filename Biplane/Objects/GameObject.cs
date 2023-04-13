using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Biplane.Objects.Weapon;
using Biplane.Components;

namespace Biplane.Objects
{
    public abstract class GameObject
    {
        public String name = "model";

        public string team = "gray";

        public int score = 0;

        public GameObject parent;

        public int size = 0;

        public long createTime = 0;
        protected long maxAge = 0;

        public BoundingBox bb = new BoundingBox();
        public Vector3 position = new Vector3(0.001f, 0.001f, 0.000f);
        public Vector3 velocity = new Vector3(0.001f, -0.001f, 0.000f);
        public Quaternion modelOrientation = Quaternion.Identity;
        public Vector3 currentDirection = new Vector3(0.0f, 0.0f, 0.0f);

        public Vector3 upDirection = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 downDirection = new Vector3(0.0f, 0.0f, 0.0f);

        public bool destroyed = false;

        /// <summary>
        /// Move Object Forwards (Object's Z Axis)
        /// </summary>
        /// <param name="distance">Distance to Translate Object</param>
        public virtual void MoveForwards(float distance)
        { Translate(new Vector3(0.0f, 0.0f, -distance)); }

        /// <summary>
        /// Move Object Backwards (Object's Z Axis)
        /// </summary>
        /// <param name="distance">Distance to Translate Object</param>
        public virtual void MoveBackwards(float distance)
        { Translate(new Vector3(0.0f, 0.0f, distance)); }

        /// <summary>
        /// Strafe Object Left (Object's X Axis)
        /// </summary>
        /// <param name="distance">Distance to Translate Object</param>
        public virtual void StrafeLeft(float distance)
        { Translate(new Vector3(-distance, 0.0f, 0.0f)); }

        /// <summary>
        /// Strafe Object Right (Object's X Axis)
        /// </summary>
        /// <param name="distance">Distance to Translate Object</param>
        public virtual void StrafeRight(float distance)
        { Translate(new Vector3(distance, 0.0f, 0.0f)); }

        /// <summary>
        /// Strafe Object Up (Object's Y Axis)
        /// </summary>
        /// <param name="distance">Distance to Translate Object</param>
        public virtual void StrafeUp(float distance)
        { Translate(new Vector3(0.0f, distance, 0.0f)); }

        /// <summary>
        /// Strafe Object Down (Object's Y Axis)
        /// </summary>
        /// <param name="distance">Distance to Translate Object</param>
        public virtual void StrafeDown(float distance)
        { Translate(new Vector3(0.0f, -distance, 0.0f)); }

        /// <summary>
        /// Roll Object Left (Object's Z Axis)
        /// </summary>
        /// <param name="angleDegrees">Angle to Rotate Object</param>
        public virtual void RollLeft(float angleDegrees)
        { Rotate(Vector3.Forward, MathHelper.ToRadians(-angleDegrees)); }

        /// <summary>
        /// Roll Object Right (Object's Z Axis)
        /// </summary>
        /// <param name="angleDegrees">Angle to Rotate Object</param>
        public virtual void RollRight(float angleDegrees)
        { Rotate(Vector3.Forward, MathHelper.ToRadians(angleDegrees)); }

        /// <summary>
        /// Pitch Object Down (Object's X Axis)
        /// </summary>
        /// <param name="angleDegrees">Angle to Rotate Object</param>
        public virtual void PitchDown(float angleDegrees)
        { Rotate(Vector3.Left, MathHelper.ToRadians(angleDegrees)); }

        /// <summary>
        /// Pitch Object Up (Object's X Axis)
        /// </summary>
        /// <param name="angleDegrees">Angle to Rotate Object</param>
        public virtual void PitchUp(float angleDegrees)
        { Rotate(Vector3.Left, MathHelper.ToRadians(-angleDegrees)); }

        /// <summary>
        /// Rotate Object Left (Object's Y Axis)
        /// </summary>
        /// <param name="angleDegrees">Angle to Rotate Object</param>
        public virtual void RotateLeft(float angleDegrees)
        { Rotate(Vector3.Up, MathHelper.ToRadians(angleDegrees)); }

        /// <summary>
        /// Turn Object Right (Object's Y Axis)
        /// </summary>
        /// <param name="angleDegrees">Angle to Rotate Object</param>
        public virtual void RotateRight(float angleDegrees)
        { Rotate(Vector3.Up, MathHelper.ToRadians(-angleDegrees)); }

        /// <summary>
        /// Move Object towards Target Vector
        /// </summary>
        /// <param name="target">Target Vector</param>
        /// <param name="factor">Percentage of Translation</param>
        public virtual void MoveTo(Vector3 target, float factor)
        { position = Vector3.Lerp(position, target, factor); }

        /// <summary>
        /// Rotate Object towards Target Quaternion
        /// </summary>
        /// <param name="slerpTarget">Target Quaternion</param>
        /// <param name="factor">Percentage of Rotation</param>
        public virtual void Slerp(Quaternion slerpTarget, float factor)
        { modelOrientation = Quaternion.Slerp(modelOrientation, slerpTarget, factor); }

        /// <summary>
        /// Rotate Object towards Target Vector
        /// </summary>
        /// <param name="target">Target Vector</param>
        /// <param name="factor">Percentage of Rotation</param>
        public virtual void LookAt(Vector3 target, float factor)
        {
            Vector3 tminusp = target - position;
            Vector3 ominusp = Vector3.Forward;

            tminusp.Normalize();

            float theta = (float)System.Math.Acos(Vector3.Dot(tminusp, ominusp));
            Vector3 cross = Vector3.Cross(ominusp, tminusp);
            //string x = cross.X.ToString();
            //string y = cross.Y.ToString();
            //string z = cross.Z.ToString();

            cross.Normalize();
            //if (cross.X.ToString().Contains("NaN")) cross.X = 0.0f;
            //if (cross.Y.ToString().Contains("NaN")) cross.Y = 0.0f;
            //if (cross.Z.ToString().Contains("NaN")) cross.Z = 0.0f;

            Quaternion targetQ = Quaternion.CreateFromAxisAngle(cross, theta);
            Slerp(targetQ, factor);
        }

        public virtual void LookAtYawToRoll(Vector3 target, float factor)
        {
            Vector3 tminusp = target - position;
            Vector3 ominusp = Vector3.Forward;

            tminusp.Normalize();

            float theta = (float)System.Math.Acos(Vector3.Dot(tminusp, ominusp));
            Vector3 cross = Vector3.Cross(ominusp, tminusp);
            //string x = cross.X.ToString();
            //string y = cross.Y.ToString();
            //string z = cross.Z.ToString();

            cross.Normalize();
            //if (cross.X.ToString().Contains("NaN")) cross.X = 0.0f;
            //if (cross.Y.ToString().Contains("NaN")) cross.Y = 0.0f;
            //if (cross.Z.ToString().Contains("NaN")) cross.Z = 0.0f;

            Quaternion targetQ = Quaternion.CreateFromAxisAngle(cross, theta);

            Quaternion slerp = Quaternion.Slerp(modelOrientation, targetQ, factor);
            float ydiff = slerp.Y - modelOrientation.Y;
            modelOrientation = slerp;
            if (ydiff < 0)
            {
                RollRight(1.0f);
            }
            else
            {
                RollLeft(1.0f);
            }
        }

        /// <summary>
        /// Force Object to Orient to UP Vector (Y)
        /// </summary>
        /// <param name="factor">Percentage of Rotation</param>
        public void ConstrainRoll(float factor)
        {
            Quaternion Q = new Quaternion(0, modelOrientation.Y, modelOrientation.Z, modelOrientation.W);
            Slerp(Q, factor);
        }

        public void ConstrainYaw(float factor)
        {
            float dir = Math.Sign(this.currentDirection.X);
            if (dir > 0)
            {
                Quaternion Q = new Quaternion(modelOrientation.X, -0.5f, modelOrientation.Z, modelOrientation.W);
                Slerp(Q, factor);
            }
            else
            {
                Quaternion Q = new Quaternion(modelOrientation.X, 0.5f, modelOrientation.Z, modelOrientation.W);
                Slerp(Q, factor);
            }
            
        }


        /// <summary>
        /// Rotate Object around specific Axis
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        private void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(modelOrientation));
            modelOrientation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * modelOrientation);
        }

        /// <summary>
        /// Translate Object along specific Axis
        /// </summary>
        /// <param name="distance"></param>
        private void Translate(Vector3 distance)
        { position += Vector3.Transform(distance, Matrix.CreateFromQuaternion(modelOrientation)); }

        public void setup()
        {
            this.destroyed = false;

            bb = new BoundingBox();
            //position = new Vector3(0.0f, 0.0f, 0.0f);
            //velocity = new Vector3(0.0f, 0.0f, 0.0f);
            //modelOrientation = Quaternion.Identity;
            //currentDirection = new Vector3(0.0f, 0.0f, 0.0f);

            //upDirection = new Vector3(0.0f, 0.0f, 0.0f);
            //downDirection = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public virtual void update(GameTime gameTime, Vector3 screenVector)
        {
            this.bb.Min = this.position - (Vector3.One * this.size / 2);
            this.bb.Max = this.position + (Vector3.One * this.size / 2);

            if (this.maxAge > 0.0f && (gameTime.TotalRealTime.Ticks - this.createTime) > this.maxAge)
            {
                this.destroyed = true;
            }

            /*DebugLineManager.DebugLine(this.position, this.position + (this.currentDirection * 10000.0f), 0xffffffff);
            DebugLineManager.DebugLine(this.position, this.position + (this.velocity * 10.0f), 0xff0000ff);
            DebugLineManager.DebugLine(this.position, this.position + (this.upDirection * 10000.0f), 0xffff00ff);
            DebugLineManager.DebugLine(this.position, this.position + (this.downDirection * 10000.0f), 0xff00ffff);*/
        }
    }
}
