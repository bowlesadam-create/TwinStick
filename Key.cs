using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinStick
{
    internal class Key
    {
        public Vector2 Position;
        public bool IsCollected = false;
        public Texture2D Texture;

        public Rectangle BoundingBox => new Rectangle(
            (int)Position.X - Texture.Width / 2,
            (int)Position.Y - Texture.Height / 2,
            Texture.Width, Texture.Height);

        public Key(Vector2 position, Texture2D texture)
        {
            Position = position;
            Texture = texture;
        }

        public void CheckPickup(Player player)
        {
            if (IsCollected) return;

            if (BoundingBox.Intersects(player.BoundingBox))
            {
                IsCollected = true;
                player.HasKey = true;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (IsCollected) return;

            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            sb.Draw(Texture, Position, null, Color.Gold, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
