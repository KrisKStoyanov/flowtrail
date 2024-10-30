using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace flowtrail
{
    public class Game1 : Game
    {
        public static Vector2 Left     = new Vector2(-1.0f, 0.0f);
        public static Vector2 Right    = new Vector2(1.0f, 0.0f);
        public static Vector2 Up       = new Vector2(0.0f, -1.0f);
        public static Vector2 Down     = new Vector2(0.0f, 1.0f);

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private static int PhysicsID = 0;

        private SpriteFont _spriteFont;

        private int _playerLives;
        private int _playerScore;

        private bool _gamePaused;
        private bool _acceptPauseInput;

        private bool _gameLost;

        private Player _player;
        private Tile _tile1;
        private Tile _tile2;
        private Tile _tile3;

        private List<Tile> _tiles;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280; // GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = 720; // GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();

            _player = new Player();

            _player._physics._position = new Vector2(
                _graphics.PreferredBackBufferWidth / 2
                , _graphics.PreferredBackBufferHeight / 2 - 280.0f);

            
            _playerLives = 3;
            _playerScore = 0;

            _acceptPauseInput = true;

            _gamePaused = false;
            _gameLost = false;

            _tile1 = new Tile();
            _tile1._physics._position = new Vector2(
                _graphics.PreferredBackBufferWidth / 2
                , _graphics.PreferredBackBufferHeight / 2);

            _tile2 = new Tile();
            _tile2._physics._position = new Vector2(
                _graphics.PreferredBackBufferWidth / 2 + 100.0f
                , _graphics.PreferredBackBufferHeight / 2 - 30.0f);
            
            _tile3 = new Tile();
            _tile3._physics._position = new Vector2(
                _graphics.PreferredBackBufferWidth / 2 + 100.0f
                , _graphics.PreferredBackBufferHeight / 2 - 280.0f);

            _player._physics.ID = PhysicsID++;
            _tile1._physics.ID = PhysicsID++;
            _tile2._physics.ID = PhysicsID++;
            _tile3._physics.ID = PhysicsID++;

            _tiles = new List<Tile>();
            _tiles.Add(_tile1);
            _tiles.Add(_tile2);
            _tiles.Add(_tile3);

            for(int i = 0; i < 8; i++)
            {
                Tile tile = new Tile();
                tile._physics._position = new Vector2(
                    64.0f + i * 64.0f, _graphics.PreferredBackBufferHeight - 64.0f);
                tile._physics.ID = PhysicsID++;
                _tiles.Add(tile);
            }

            for (int i = 0; i < 8; i++)
            {
                Tile tile = new Tile();
                tile._physics._position = new Vector2(
                    _graphics.PreferredBackBufferWidth - 64.0f - i * 64.0f, _graphics.PreferredBackBufferHeight - 64.0f);
                tile._physics.ID = PhysicsID++;
                _tiles.Add(tile);
            }

            for (int i = 0; i < 2; i++)
            {
                Tile tile = new Tile();
                tile._physics._position = new Vector2(_graphics.PreferredBackBufferWidth / 4, 64.0f + i * 64.0f);
                tile._physics.ID = PhysicsID++;
                _tiles.Add(tile);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _player._texture = Content.Load<Texture2D>("player");
            _spriteFont = Content.Load<SpriteFont>("gui");

            Texture2D tileTexture = Content.Load<Texture2D>("tile");
            for(int i = 0; i < _tiles.Count; i++)
            {
                _tiles[i]._texture = tileTexture;
                _tiles[i]._physics._radiusOfBoundingSphere = tileTexture.Width / 2; 
            }

            // these need to be moved to initialize 
            _player._physics._radiusOfBoundingSphere = _player._texture.Width / 2;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)
                && _acceptPauseInput && !_gameLost)
            {
                _gamePaused = !_gamePaused;
                _acceptPauseInput = false;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                _acceptPauseInput = true;
            }

            if (_gamePaused || _gameLost)
            {
                return;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
         
            if (keyboardState.IsKeyDown(Keys.A))
            {
                _player._physics._direction = Left;
                _player._physics.Move(Left, 2.0f);
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                _player._physics._direction = Right;
                _player._physics.Move(Right, 2.0f);
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                _player._physics.Move(Up, 2.0f);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                _player._physics.Move(Down, 2.0f);
            }

            if(keyboardState.IsKeyDown(Keys.R))
            {
                _player._physics._position = new Vector2(
                    _graphics.PreferredBackBufferWidth / 2
                    , _graphics.PreferredBackBufferHeight / 2 - 280.0f);

                _player._physics._velocity = Vector2.Zero;
            }

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                _player.Jump();
            }

            if(keyboardState.IsKeyUp(Keys.Space))
            {
                _player.StopJump();
            }

            if(keyboardState.IsKeyDown(Keys.LeftShift))
            {
                _player.Dash();
            }

            int playerCollisionRadius = _player._texture.Height / 2;
            int tileCollisionRadius = _tile1._texture.Height / 2;

            foreach(Tile tile in _tiles)
            {
                bool existingCollision = false;

                for(int i = 0; i < _player._physics._collisions.Count; i++)
                {
                    if (_player._physics._collisions[i].ID == tile._physics.ID)
                    {
                        existingCollision = true;
                        break;
                    }
                }

                if(!existingCollision)
                {
                    bool newCollision = true; // Physics2D.GetCollision(_player._physics, tile._physics);

                    if (newCollision)
                    {
                        _player._physics._collisions.Add(tile._physics);

                        // this is not being used yet and won't be for a while (i think)
                        // a 2D physics engine would support it though but i need to
                        // implement mass as a value that affects objects
                        //tile._physics._collisions.Add(_player._physics);
                    }
                }
            }

            _gameLost = (_playerLives == 0);
            if (!_gameLost)
            {
                _player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AliceBlue);

            _spriteBatch.Begin();

            if(!_gameLost)
            {
                _player.Draw(_spriteBatch);
            }

            foreach(Tile tile in _tiles)
            {
                tile.Draw(_spriteBatch);
            }

            _spriteBatch.DrawString(
                _spriteFont
                , "X: " + _player._physics._position.X + "\nY: " + _player._physics._position.Y
                , new Vector2(15.0f, _spriteFont.LineSpacing / 2 + _spriteFont.LineSpacing * 2)
                , Color.Orange);

            _spriteBatch.DrawString(
                _spriteFont
                , "minX: " + _player._physics._minX + "\nminY: " + _player._physics._minY
                , new Vector2(15.0f, _spriteFont.LineSpacing / 2 + _spriteFont.LineSpacing * 4)
                , Color.Orange);

            _spriteBatch.DrawString(
                _spriteFont
                , "maxX: " + _player._physics._maxX + "\nmaxY: " + _player._physics._maxY
                , new Vector2(15.0f, _spriteFont.LineSpacing / 2 + _spriteFont.LineSpacing * 6)
                , Color.Orange);

            _spriteBatch.DrawString(
                _spriteFont
                , "velocity X: " + _player._physics._velocity.X + "\nvelocity Y: " + _player._physics._velocity.Y
                , new Vector2(15.0f, _spriteFont.LineSpacing / 2 + _spriteFont.LineSpacing * 8)
                , Color.Orange);

            _spriteBatch.DrawString(
                _spriteFont
                , "Test1: " + _player._physics._testFlag1,
                new Vector2(15.0f, _spriteFont.LineSpacing / 2 + _spriteFont.LineSpacing * 10)
                , Color.Orange);
            _spriteBatch.DrawString(
                _spriteFont
                , "Test2: " + _player._physics._testFlag2,
                new Vector2(15.0f, _spriteFont.LineSpacing / 2 + _spriteFont.LineSpacing * 11)
                , Color.Orange);
            _spriteBatch.DrawString(
                _spriteFont
                , "Test3: " + _player._physics._testFlag3,
                new Vector2(15.0f, _spriteFont.LineSpacing / 2 + _spriteFont.LineSpacing * 12)
                , Color.Orange);
            _spriteBatch.DrawString(
                _spriteFont
                , "Test4: " + _player._physics._testFlag4,
                new Vector2(15.0f, _spriteFont.LineSpacing / 2 + _spriteFont.LineSpacing * 13)
                , Color.Orange);

            _spriteBatch.DrawString(
                _spriteFont
                , "Lives: " + _playerLives, 
                new Vector2(15.0f, _spriteFont.LineSpacing / 2)
                , Color.Teal);

            _spriteBatch.DrawString(
                _spriteFont
                , "Score: " + _playerScore,
                new Vector2(15.0f, _spriteFont.LineSpacing / 2 + _spriteFont.LineSpacing)
                , Color.Teal);

            if(_gamePaused)
            {
                _spriteBatch.DrawString(
                    _spriteFont
                    , "Paused"
                    , new Vector2(
                        _graphics.PreferredBackBufferWidth / 2 
                        , _graphics.PreferredBackBufferHeight / 2)
                    , Color.Teal
                    , 0.0f
                    , new Vector2(
                        _spriteFont.MeasureString("Paused").X / 2
                        , (_spriteFont.MeasureString("Paused").Y / 2)) 
                    , Vector2.One
                    , SpriteEffects.None
                    , 0.0f);
            }

            if(_gameLost)
            {
                _spriteBatch.DrawString(
                    _spriteFont
                    , "Game Over",
                    new Vector2(
                        _graphics.PreferredBackBufferWidth / 2
                        , _graphics.PreferredBackBufferHeight / 2)
                    , Color.Teal
                    , 0.0f
                    , new Vector2(
                        _spriteFont.MeasureString("Game Over").X / 2
                        , (_spriteFont.MeasureString("Game Over").Y / 2))
                    , Vector2.One
                    , SpriteEffects.None
                    , 0.0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
