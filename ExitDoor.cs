using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace TwinStick
{
    internal class ExitDoor
    {
        public Rectangle TriggerArea;
        public bool IsTriggered = false;
        public Texture2D Texture;

        public ExitDoor(Texture2D texture, Rectangle triggerArea)
        {
            Texture = texture;
            TriggerArea = triggerArea;
        }

        public void Update(Player player)
        {
            if (IsTriggered) return;

            // When this happens the player has cleared the room
            if (TriggerArea.Intersects(player.BoundingBox) && player.HasKey)
            {
                IsTriggered = true;

                // do some room transition...
            }
        }

        public void Draw(SpriteBatch sb, Player player)
        {
            Color color = player.HasKey ? Color.LightGreen : Color.IndianRed;
            sb.Draw(Texture, TriggerArea, color);
        }
    }
}
