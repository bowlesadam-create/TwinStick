using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    internal class MeleeAttack
    {
        public float Range = 70f;
        public float ArcDegrees = 90f;
        public float Duration = 0.15f;
        public bool IsActive = false;

        private float timer;
        private Vector2 origin;
        private float facingAngle;

        // tracks which targets this specific swing has already hit, so it can't multi-hit in one activation
        private HashSet<object> hitTargets = new HashSet<object>();

        public void Trigger(Vector2 position, float rotation)
        {
            IsActive = true;
            timer = Duration;
            origin = position;
            facingAngle = rotation;
            hitTargets.Clear();
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= delta;
            if (timer <= 0f)
            {
                IsActive = false;
            }
        }

        public bool IsInArc(Vector2 targetPosition)
        {
            Vector2 toTarget = targetPosition - origin;
            float distance = toTarget.Length();
            if (distance > Range) return false;

            float angleToTarget = (float)System.Math.Atan2(toTarget.Y, toTarget.X);
            float angleDifference = MathHelper.WrapAngle(angleToTarget - facingAngle);
            float halfArcRadians = MathHelper.ToRadians(ArcDegrees / 2f);

            return System.Math.Abs(angleDifference) <= halfArcRadians;
        }

        // returns true only the first time a given target is checked during this swing
        public bool TryRegisterHit(object target)
        {
            return hitTargets.Add(target); // HashSet.Add returns false if already present
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture)
        {
            if (!IsActive) return;

            int segments = 10;
            float halfArc = MathHelper.ToRadians(ArcDegrees / 2f);

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                float angle = facingAngle - halfArc + t * (halfArc * 2f);

                Vector2 direction = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));
                Vector2 endPoint = origin + direction * Range;

                DrawLine(spriteBatch, pixelTexture, origin, endPoint, Color.Cyan);
            }
        }

        private void DrawLine(SpriteBatch spriteBatch, Texture2D pixelTexture, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            float length = edge.Length();
            float angle = (float)System.Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(pixelTexture, start, null, color, angle, Vector2.Zero,
                new Vector2(length, 2f), SpriteEffects.None, 0f);
        }
    }
}