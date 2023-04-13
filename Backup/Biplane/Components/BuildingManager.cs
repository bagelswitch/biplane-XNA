using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Biplane.Objects;
using Biplane.Objects.Target;
using Biplane.Objects.Weapon;

namespace Biplane.Components
{
    class BuildingManager : GameComponent
    {
        private static List<Building> deadBuildings;

        public BuildingManager(Game game)
            : base(game)
        {
            deadBuildings = new List<Building>();
        }

        private static Building newBuilding(Game game, GameTime gameTime)
        {
            Building building = null;
            if (deadBuildings.Count > 0)
            {
                building = deadBuildings[0];
                deadBuildings.Remove(building);
            }
            else
            {
                building = new Building(game);
            }
            building.setup(gameTime);
            return building;
        }

        public override void Update(GameTime gameTime)
        {
            List<GameObject> objects = ((BiplaneGame)Game).gameState.objects;

            int buildingCount = 0;
            bool foundDebris = false;

            foreach (GameObject gameObject in objects)
            {
                if (gameObject is Building)
                {
                    buildingCount += 1;

                    if (gameObject.destroyed)
                    {
                        deadBuildings.Add((Building)gameObject);
                    }
                }
                else if (gameObject is Debris)
                {
                    foundDebris = true;
                }
            }
            while (buildingCount < 5 && !foundDebris)
            {
                objects.Add(newBuilding(this.Game, gameTime));
                buildingCount++;
            }
            base.Update(gameTime);
        }
    }
}
