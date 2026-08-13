using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinStick
{
    internal class Spawner
    {
        public Vector2 Position;
        public int Health = 60;
        public bool IsActive = true;
        public Texture2D Texture;

        public float SpawnInterval = 0.5f;
        private float spawnTimer;

        public int MaxAliveFromThisSpawner = 10; // Some way to stop the level from getting too flooded
        private int aliveCount = 0;

        public Rectangle BoundingBox => new Rectangle(
            (int)Position.X - Texture.Width / 2,
            (int)Position.Y - Texture.Height / 2,
            Texture.Width, Texture.Height);

        public Spawner(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            spawnTimer = SpawnInterval; // Spawner starts on cooldown
        }

        public void Update(GameTime gameTime, EnemyManager enemyManager)
        {
            if (!IsActive)
                return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            spawnTimer -= delta;

            if (spawnTimer <= 0f && aliveCount < MaxAliveFromThisSpawner)
            {
                enemyManager.SpawnEnemy(Position, this);
                aliveCount++;
                spawnTimer = SpawnInterval;
            }
        }

        public void NotifyEnemyDied()
        {
            aliveCount--;
            if (aliveCount < 0) aliveCount = 0;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            sb.Draw(Texture, Position, null, Color.Purple, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
