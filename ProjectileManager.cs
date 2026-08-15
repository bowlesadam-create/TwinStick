using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinStick
{
    internal class ProjectileManager
    {
        private List<Projectile> projectiles = new List<Projectile>();
        private Texture2D projectileTexture;
        private float fireCooldown = 0f;
        private float fireRate = 0.3f;

        private MouseState previousMouseState;

        public List<Projectile> Projectiles => projectiles;

        public ProjectileManager(Texture2D projectileTexture)
        {
            this.projectileTexture = projectileTexture;
        }

        public void Update(GameTime gameTime, Player player, Rectangle roomBounds)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            fireCooldown -= delta;

            MouseState mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && fireCooldown <= 0f)
            {
                Fire(player);
                fireCooldown = fireRate;
            }

            previousMouseState = mouse;

            foreach (var projectile in projectiles)
            {
                projectile.Update(gameTime, roomBounds);
            }

            projectiles.RemoveAll(p => !p.IsActive);
        }

        private void Fire(Player player) 
        {
            Projectile newProjectile = new Projectile(projectileTexture, player.Position, player.Rotation);
            projectiles.Add(newProjectile);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(var projectile in projectiles)
            {
                projectile.Draw(sb);
            }
        }
    }
}
