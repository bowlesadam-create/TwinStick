using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwinStick
{
    internal class EnemyManager
    {
        private List<Enemy> enemies = new List<Enemy>();
        private Texture2D enemyTexture;

        public IEnumerable<Vector2> GetEnemyPositions()
        {
            foreach (var enemy in enemies)
                if (enemy.IsActive) yield return enemy.Position;
        }

        public EnemyManager(Texture2D enemyTexture)
        {
            this.enemyTexture = enemyTexture;
        }

        public void SpawnEnemy(Vector2 position, Spawner originSpawner = null)
        {
            Enemy enemy = new Enemy(enemyTexture, position);
            enemy.OriginSpawner = originSpawner;
            enemies.Add(enemy);
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

        public void CheckMeleeCollisions(MeleeAttack melee, Vector2 meleeOrigin)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.IsActive && melee.IsInArc(enemy.Position))
                {
                    if (melee.TryRegisterHit(enemy))
                    {
                        enemy.TakeDamage(20);

                        Vector2 knockbackDirection = enemy.Position - meleeOrigin;
                        enemy.ApplyKnockback(knockbackDirection, 400f);
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
