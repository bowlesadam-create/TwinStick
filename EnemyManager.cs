using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwinStick
{
    internal class EnemyManager
    {
        private List<Enemy> enemies = new List<Enemy>();
        private Texture2D enemyTexture;

        public EnemyManager(Texture2D enemyTexture)
        {
            this.enemyTexture = enemyTexture;
        }

        public void SpawnEnemy(Vector2 position)
        {
            enemies.Add(new Enemy(enemyTexture, position));
        }

        public void Update(GameTime gameTime, Vector2 playerPosition, TiledMap map)
        {
            foreach (var enemy in enemies) 
            {
                enemy.Update(gameTime, playerPosition, map);
            }

            enemies.RemoveAll(e => !e.IsActive);
        }

        public void CheckProjectileCollision(List<Projectile> projectiles)
        {
            foreach (var enemy in enemies)
            {
                foreach (var projectile in projectiles)
                {
                    if (projectile.IsActive && enemy.IsActive && enemy.BoundingBox.Intersects(projectile.BoundingBox))
                    {
                        enemy.TakeDamage(10);
                        projectile.IsActive = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(var enemy in enemies)
            {
                enemy.Draw(sb);
            }
        }
    }
}
