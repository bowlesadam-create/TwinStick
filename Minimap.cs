using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TwinStick
{
    internal class Minimap
    {
        public Rectangle ScreenBounds; // Place for the minimap on the screen
        private Texture2D pixelTexture;
        private TiledMap map;

        public Minimap(Texture2D pixelTexture, TiledMap map, Rectangle screenBounds)
        {
            this.pixelTexture = pixelTexture;
            this.map = map;
            ScreenBounds = screenBounds;
        }

        private Vector2 WorldToMinimap(Vector2 worldPosition)
        {
            float mapPixelWidth = map.Width * map.TileWidth;
            float mapPixelHeight = map.Height * map.TileHeight;

            float scaleX = ScreenBounds.Width / mapPixelWidth;
            float scaleY = ScreenBounds.Height / mapPixelHeight;

            float x = ScreenBounds.X + worldPosition.X * scaleX;
            float y = ScreenBounds.Y + worldPosition.Y * scaleY;

            return new Vector2(x, y);
        }

        public void Draw(SpriteBatch sb, Vector2 playerPosition, IEnumerable<Vector2> enemyPositions, IEnumerable<Vector2> spawnerPositions)
        {
            sb.Draw(pixelTexture, ScreenBounds, Color.Black * 0.6f);

            float scaleX = ScreenBounds.Width/ (float)(map.Width * map.TileWidth);
            float scaleY = ScreenBounds.Height / (float)(map.Height * map.TileHeight);

            for(int y=0; y < map.Height; y++)
            {
                for(int x=0; x<map.Width; x++)
                {
                    if (map.IsCollidableAt(x, y))
                    {
                        Vector2 worldPos = new Vector2(x*map.TileWidth, y*map.TileHeight);
                        Vector2 mmPos = WorldToMinimap(worldPos);
                        sb.Draw(pixelTexture, new Rectangle((int)mmPos.X, (int)mmPos.Y, System.Math.Max(1, (int)(map.TileWidth * scaleX)),
                                                                                        System.Math.Max(1, (int)(map.TileHeight * scaleY))), Color.SaddleBrown);
                    }
                }
            }

            foreach (var pos in spawnerPositions)
            {
                Vector2 mmPos = WorldToMinimap(pos);
                DrawDot(sb, mmPos, 4, Color.Purple);
            }

            foreach (var pos in enemyPositions)
            {
                Vector2 mmPos = WorldToMinimap(pos);
                DrawDot(sb, mmPos, 3, Color.Red);
            }

            Vector2 playerMmPos = WorldToMinimap(playerPosition);
            DrawDot(sb, playerMmPos, 5, Color.LightGreen);
        }

        private void DrawDot(SpriteBatch sb, Vector2 center, int size, Color color)
        {
            Rectangle rect = new Rectangle((int)center.X - size / 2, (int)center.Y - size / 2, size, size);
            sb.Draw(pixelTexture, rect, color);
        }
    }
}
