using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace flowtrail
{
    internal class Player
    {
        public Player()
        {
            _jumpStrength = 1.0f;
            _jumpResetDuration = 3.0f;
            _jumpDuration = 3.0f;
            _jumping = false;
            _canJump = true;

            _physics = new Physics2D(true);
        }
        public Player(Vector2 position) 
        {
            _jumpStrength = 1.0f;
            _jumpResetDuration = 3.0f;
            _jumpDuration = 3.0f;
            _jumping = false;
            _canJump = true;

            _physics = new Physics2D(true);
        }

        public Texture2D _texture { get; set; }

        public Physics2D _physics { get; set; }

        public float _jumpStrength {  get; set; }

        public float _jumpDecay { get; set; }

        public float _jumpResetDuration { get; set; }

        public bool _jumping { get; set; }  

        public float _jumpDuration { get; set; }

        public bool _canJump { get; set; }

        public void Jump()
        {
            if (_canJump)
            {
                _jumping = true;            
            }
        }

        public void StopJump()
        {
            _jumping = false;
        }

        public void Dash()
        {
            // need to overhaul movement limitations  
            //_physics.Move(_physics._direction, MathF.Abs(_physics._velocity.X) * 2.0f);
        }

        public void Update(float deltaTime)
        {
            _canJump = _jumpDuration > 0.0f;

            if (_jumping)
            {
                _jumpDuration -= deltaTime * 10.0f;
                _physics.Move(Game1.Up, _jumpStrength);
            }
            else
            {
                _canJump = false;
            }

            if (_jumpDuration < 0.0f)
            {
                _jumping = false;
            }

            // need to consider when the player bumps into a platform from below
            if (_physics._velocity.Y == 0 && !_canJump)
            {
                _jumping = false;
                _canJump = true;
                _jumpDuration = _jumpResetDuration;
            }

            _physics.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture
                , _physics._position
                , null
                , Color.White
                , 0.0f
                , new Vector2(_texture.Width / 2, _texture.Height / 2)
                , Vector2.One
                , SpriteEffects.None
                , 0.0f);
        }
    }
}
