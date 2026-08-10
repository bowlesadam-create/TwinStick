using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace TwinStick
{
    internal class Projectile
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Speed = 500f;
        public bool IsActive = true;
        public Texture2D Texture;
        public int Radius = 4;

        public Rectangle BoundingBox => new Rectangle(
            (int)Position.X - Radius,
            (int)Position.Y - Radius,
            Radius * 2, Radius * 2);

        public Projectile(Texture2D texture, Vector2 startPosition, float rotation)
        {
            Texture = texture;
            Position = startPosition;
            Velocity = new Vector2((float)System.Math.Cos(rotation), (float)System.Math.Sin(rotation)) * Speed;
        }

        public void Update(GameTime gameTime, Rectangle roomBounds)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * delta;

            if (!roomBounds.Contains(Position)) IsActive = false;
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            sb.Draw(Texture, Position, null, Color.Yellow, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
