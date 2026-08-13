using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwinStick
{
    internal class SpawnerManager
    {
        private List<Spawner> spawners = new List<Spawner>();
        private Texture2D spawnerTexture;

        public SpawnerManager(Texture2D spawnerTexture)
        {
            this.spawnerTexture = spawnerTexture;
        }

        public void AddSpawner(Vector2 position)
        {
            spawners.Add(new Spawner(spawnerTexture, position));  
        }

        public void Update(GameTime gameTime, EnemyManager enemyManager)
        {
            foreach (var spawner in spawners) 
            {
                spawner.Update(gameTime, enemyManager);
            }

            spawners.RemoveAll(s => !s.IsActive);
        }

        public void CheckProjectileCollisions(List<Projectile> projectiles)
        {
            foreach (var spawner in spawners)
            {
                foreach (var projectile in projectiles)
                {
                    if (projectile.IsActive && spawner.IsActive && spawner.BoundingBox.Intersects(projectile.BoundingBox))
                    {
                        spawner.TakeDamage(10);
                        projectile.IsActive = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var spawner in spawners)
            {
                spawner.Draw(sb);
            }
        }
    }
}
