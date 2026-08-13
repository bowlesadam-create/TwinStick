using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinStick
{
    internal class Enemy
    {
        public Vector2 Position;
        public float Speed = 100f;
        public int Health = 30;
        public bool IsActive = true;
        public Texture2D Texture;

        public Rectangle BoundingBox => new Rectangle(
            (int)Position.X - Texture.Width / 2,
            (int)Position.Y - Texture.Height / 2,
            Texture.Width, Texture.Height);

        public Enemy(Texture2D texture, Vector2 startPosition)
        {
            Texture = texture;
            Position = startPosition;
        }

        public void Update(GameTime gameTime, Vector2 playerPosition, TiledMap map)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction = playerPosition - Position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                Vector2 movement = direction * Speed * delta;
                TryMove(movement, map);
            }
        }

        private void TryMove(Vector2 movement, TiledMap map)
        {
            Vector2 newPositionX = Position + new Vector2(movement.X, 0);
            if (!IsCollidingAt(newPositionX, map))
            {
                Position = newPositionX;
            }

            Vector2 newPositionY = Position + new Vector2(0, movement.Y);
            if (!IsCollidingAt(newPositionY, map))
            {
                Position = newPositionY;
            }
        }

        private bool IsCollidingAt(Vector2 position, TiledMap map)
        {
            Rectangle box = new Rectangle(
                (int)position.X - Texture.Width / 2,
                (int)position.Y - Texture.Height / 2,
                Texture.Width, Texture.Height);

            int leftTile = box.Left / map.TileWidth;
            int rightTile = (box.Right - 1) / map.TileWidth;
            int topTile = box.Top / map.TileHeight;
            int bottomTile = (box.Bottom - 1) / map.TileHeight;

            return map.IsCollidableAt(leftTile, topTile) ||
                   map.IsCollidableAt(rightTile, topTile) ||
                   map.IsCollidableAt(leftTile, bottomTile) ||
                   map.IsCollidableAt(rightTile, bottomTile);
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
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            sb.Draw(Texture, Position, null, Color.Crimson, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
