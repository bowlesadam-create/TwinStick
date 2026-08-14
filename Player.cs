using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TwinStick
{
    internal class Player
    {
        public Vector2 Position;
        public float Rotation;
        public float Speed = 200f; // px/s
        public Texture2D Texture;
        public Texture2D DebugLineTexture;
        public float FacingLineLength = 40f;
        public float DashSpeed = 600f;
        public float DashDuration = 0.15f;
        public float DashCooldown = 0.6f;

        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;
        private Vector2 dashDirection;
        private bool isDashing = false;




        private KeyboardState previousKeyboardState;

        public Rectangle BoundingBox => new Rectangle(
            (int)Position.X - Texture.Width /2,
            (int)Position.Y - Texture.Height/2,
            Texture.Width, Texture.Height);

        public Player(Texture2D texture, Texture2D debugLineTexture, Vector2 startPosition)
        {
            Texture = texture;
            DebugLineTexture = debugLineTexture;
            Position = startPosition;
        }

        public void Update(GameTime gameTime, Camera camera, TiledMap map)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleDashInput(delta);

            if (isDashing)
            {
                Vector2 dashMovement = dashDirection * DashSpeed * delta;
                TryMove(dashMovement, map);
                dashTimer -= delta;
                if (dashTimer <= 0f) isDashing = false;
            }
            else
            {
                HandleMovement(delta, map);
            }
            
            HandleAim(camera);

            previousKeyboardState = Keyboard.GetState();
        }

        private void HandleDashInput(float delta)
        {
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= delta;
            }

            KeyboardState keyboard = Keyboard.GetState();
            bool spacePressed = keyboard.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space);

            if (spacePressed && !isDashing && dashCooldownTimer <= 0f)
            {
                Vector2 moveDirection = GetCurrentMoveDirection(keyboard);

                if (moveDirection == Vector2.Zero)
                    moveDirection = new Vector2((float)System.Math.Cos(Rotation), (float)System.Math.Sin(Rotation));

                dashDirection = moveDirection;
                isDashing = true;
                dashTimer = DashDuration;
                dashCooldownTimer = DashCooldown;
            }
        }

        private Vector2 GetCurrentMoveDirection(KeyboardState keyboard) 
        {
            Vector2 direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W)) direction.Y -= 1;
            if (keyboard.IsKeyDown(Keys.S)) direction.Y += 1;
            if (keyboard.IsKeyDown(Keys.A)) direction.X -= 1;
            if (keyboard.IsKeyDown(Keys.D)) direction.X += 1;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            return direction;
        }

        private void HandleMovement(float delta, TiledMap map) {
            KeyboardState keyboard = Keyboard.GetState();
            Vector2 direction = GetCurrentMoveDirection(keyboard);

            if (direction != Vector2.Zero)
            {
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

        private void HandleAim(Camera camera) 
        {
            MouseState mouse = Mouse.GetState();
            Vector2 mouseScreenPos = new Vector2(mouse.X, mouse.Y);
            Vector2 mouseWorldPos = camera.ScreenToWorld(mouseScreenPos);

            Vector2 aimDirection = mouseWorldPos - Position;
            if (aimDirection != Vector2.Zero)
            {
                Rotation = (float)System.Math.Atan2(aimDirection.Y, aimDirection.X);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2);
            sb.Draw(Texture, Position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);

            DrawFacingLine(sb);
        }

        private void DrawFacingLine(SpriteBatch sb)
        {
            Vector2 lineOrigin = new Vector2(0f, 0.5f);
            sb.Draw(DebugLineTexture, Position, null, Color.Red,Rotation,lineOrigin,new Vector2(FacingLineLength, 3f),SpriteEffects.None, 0f);
        }
    }
}
