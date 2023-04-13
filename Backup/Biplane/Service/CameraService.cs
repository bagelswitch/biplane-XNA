using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Biplane.Objects;

namespace Biplane.Service
{
    class CameraService : GameComponent, CameraProvider
    {
        // initial camera position
        Vector3 cameraPosition = new Vector3(0.0f, 8500.0f, 20000.0f);
        Vector3 lookAt = new Vector3(0.0f, 5500.0f, 0.0f);
        public Vector3 camera1 = new Vector3(0.0f, 5500.0f, 15500.0f);
        public Vector3 camera2 = new Vector3(0.0f, 5500.0f, 15500.0f);

        Queue cameraList = new Queue();

        public CameraService(Game game)
            : base(game)
        {
            ((BiplaneGame)Game).gameState.viewMatrix = Matrix.CreateLookAt(cameraPosition, lookAt, Vector3.Up);
        }

        public void AddCamera(Vector3 position, Boolean force)
        {
            if (force || (!((BiplaneGame)Game).gameState.player1Active && !((BiplaneGame)Game).gameState.player1Active))
            {
                cameraList.Enqueue(position);
            }
        }

        public override void Update(GameTime gameTime)
        {
            BiplaneGame game = ((BiplaneGame)Game);

            Vector3 newCamera = Vector3.Zero;
            
            float minX = 1000000;
            float minY = 1000000;
            float minZ = 1000000;
            float maxX = -1000000;
            float maxY = -1000000;
            float maxZ = -1000000;

            int camCount = cameraList.Count;

            if (camCount == 0)
            {
                newCamera = Vector3.UnitY;
                camCount = 1;
            }

            while (cameraList.Count > 0)
            {
                Vector3 cpos = (Vector3) cameraList.Dequeue();
                if (cpos.X < minX) minX = cpos.X;
                if (cpos.X > maxX) maxX = cpos.X;
                if (cpos.Y < minY) minY = cpos.Y;
                if (cpos.Y > maxY) maxY = cpos.Y;
                if (cpos.Z < minZ) minZ = cpos.Z;
                if (cpos.Z > maxZ) maxZ = cpos.Z;

                newCamera += cpos;
            }

            newCamera = newCamera / (camCount);
            //if(newCamera.X.ToString().Equals("NaN")) newCamera.X = 0.0f;
            //if (newCamera.Y.ToString().Equals("NaN")) newCamera.Y = 1.0f;
            //if (newCamera.Z.ToString().Equals("NaN")) newCamera.Z = 0.0f;

            float distance = Vector3.Distance(new Vector3(minX, 0.0f, minZ), new Vector3(maxX, 0.0f, maxZ));

            distance *= 1.3f;

            if (distance < 30000.0f) distance = 30000.0f;

            lookAt.X = (lookAt.X * 0.95f) + ((newCamera.X) * 0.05f);
            lookAt.Y = (lookAt.Y * 0.95f) + ((newCamera.Y) * 0.05f);
            lookAt.Z = (lookAt.Z * 0.95f) + ((newCamera.Z) * 0.05f);
            cameraPosition.X = (cameraPosition.X * 0.95f) + ((newCamera.X) * 0.05f);
            cameraPosition.Y = (cameraPosition.Y * 0.95f) + ((newCamera.Y) * 0.05f);
            cameraPosition.Z = (cameraPosition.Z * 0.95f) + ((distance + maxZ) * 0.05f);
            if (cameraPosition.Z < -500000.0f) cameraPosition.Z = -500000.0f;
            if (cameraPosition.Z > 500000.0f) cameraPosition.Z = 500000.0f;
            if (cameraPosition.Y < -100000.0f) cameraPosition.Y = -100000.0f;
            //if (((BiplaneGame)Game).gameState.zMotion) cameraPosition.Y += 500.0f;

            //cameraPosition = new Vector3(0.0f, 500000.0f, -2000000.0f);

            ((BiplaneGame)Game).gameState.viewMatrix = Matrix.CreateLookAt(cameraPosition, lookAt, Vector3.Up);
        }
    }
}
