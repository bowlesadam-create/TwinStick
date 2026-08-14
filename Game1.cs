using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.Serialization.Formatters;

namespace TwinStick
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Camera camera;
        private Player player;
        private Texture2D playerTexture;
        private Texture2D pixelTexture;

        private ProjectileManager projectileManager;
        private Texture2D projectileTexture;

        private TiledMap tiledMap;
        private Texture2D floorTexture;
        private Texture2D wallTexture;

        private EnemyManager enemyManager;
        private Texture2D enemyTexture;

        private SpawnerManager spawnerManager;
        private Texture2D spawnerTexture;

        private Minimap minimap;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Rectangle roomBounds = new Rectangle(0,0,1920, 1080);
            camera = new Camera(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, roomBounds);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            playerTexture = new Texture2D(GraphicsDevice, 32, 32);
            Color[] data = new Color[32 * 32];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Color.White;
            }
            playerTexture.SetData(data);

            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            player = new Player(playerTexture, pixelTexture, new Vector2(960, 540));

            projectileTexture = new Texture2D(GraphicsDevice, 8, 8);
            Color[] projData = new Color[8 * 8];
            for (int i = 0; i < projData.Length;i++) projData[i] = Color.White;
            projectileTexture.SetData(projData);

            projectileManager = new ProjectileManager(projectileTexture);

            floorTexture = new Texture2D(GraphicsDevice, 32, 32);
            Color[] floorData = new Color[32 * 32];
            for (int i = 0; i < floorData.Length; i++) floorData[i] = Color.DarkSlateGray;
            floorTexture.SetData(floorData);

            wallTexture = new Texture2D(GraphicsDevice, 32, 32);
            Color[] wallData = new Color[32 * 32];
            for (int i = 0; i < wallData.Length; i++) wallData[i] = Color.SaddleBrown;
            wallTexture.SetData(wallData);
            
            tiledMap = TiledMap.Load("Content/Tilesets/testMap.tmx");

            enemyTexture = new Texture2D(GraphicsDevice, 28, 28);
            Color[] enemyData = new Color[28 * 28];
            for (int i = 0; i < enemyData.Length; i++) enemyData[i] = Color.White;
            enemyTexture.SetData(enemyData);

            enemyManager = new EnemyManager(enemyTexture);

            // temporary hardcoded spawn for testing, before object-layer parsing exists
            enemyManager.SpawnEnemy(new Vector2(960, 200));
            enemyManager.SpawnEnemy(new Vector2(1200, 700));

            spawnerTexture = new Texture2D(GraphicsDevice, 36, 36);
            Color[] spawnerData = new Color[36 * 36];
            for (int i = 0; i < spawnerData.Length; i++) spawnerData[i] = Color.White;
            spawnerTexture.SetData(spawnerData);

            spawnerManager = new SpawnerManager(spawnerTexture);
            spawnerManager.AddSpawner(new Vector2(700, 400));

            Rectangle minimapBounds = new Rectangle(
                GraphicsDevice.Viewport.Width - 220, 20,
                200, 200 * tiledMap.Height / tiledMap.Width);
            minimap = new Minimap(pixelTexture, tiledMap, minimapBounds);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            player.Update(gameTime, camera, tiledMap);
            camera.Follow(player.Position);
            projectileManager.Update(gameTime, player, camera.RoomBounds);
            enemyManager.Update(gameTime, player.Position, tiledMap);
            enemyManager.CheckProjectileCollision(projectileManager.Projectiles);
            spawnerManager.Update(gameTime, enemyManager);
            spawnerManager.CheckProjectileCollisions(projectileManager.Projectiles);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(transformMatrix: camera.GetTransformMatrix());
            tiledMap.Draw(_spriteBatch, floorTexture,wallTexture);
            DrawDebugGrid(_spriteBatch, 64, camera.RoomBounds);
            player.Draw(_spriteBatch);
            projectileManager.Draw(_spriteBatch);
            enemyManager.Draw(_spriteBatch);
            spawnerManager.Draw(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin();
            minimap.Draw(_spriteBatch, player.Position, enemyManager.GetEnemyPositions(), spawnerManager.GetSpawnerPositions());
            _spriteBatch.End();

            base.Draw(gameTime); 
        }

        private void DrawDebugGrid(SpriteBatch sb, int cellSize, Rectangle bounds)
        {
            for (int x = bounds.Left; x <= bounds.Right; x += cellSize)
            {
                sb.Draw(pixelTexture, new Rectangle(x, bounds.Top, 1, bounds.Height), Color.Gray);
            }

            for (int y = bounds.Top; y <= bounds.Bottom; y += cellSize)
            {
                sb.Draw(pixelTexture, new Rectangle(bounds.Left, y, bounds.Width, 1), Color.Gray);
            }
        }
    }
}
