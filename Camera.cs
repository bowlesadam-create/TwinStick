using Microsoft.Xna.Framework;


namespace TwinStick
{
    internal class Camera
    {
        public Vector2 Position;
        public int ViewportWidth;
        public int ViewportHeight;
        public Rectangle RoomBounds;

        public Camera(int viewportWidth, int viewportHeight, Rectangle roomBounds)
        {
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
            RoomBounds = roomBounds;
        }

        public void Follow(Vector2 target)
        {
            Position = target - new Vector2(ViewportWidth / 2, ViewportHeight / 2);

            Position.X = MathHelper.Clamp(Position.X, RoomBounds.Left, RoomBounds.Right);
            Position.Y = MathHelper.Clamp(Position.Y, RoomBounds .Top, RoomBounds .Bottom);
        }

        public Matrix GetTransformMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0));
        }

        public Vector2 ScreenToWorld(Vector2 screenPos) 
        {
            return screenPos + Position;
        }

        public Vector2 WorldToScreen(Vector2 worldPos) 
        {
            return worldPos - Position;
        }
    }
}
