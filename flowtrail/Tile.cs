using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace flowtrail
{
    internal class Tile
    {
        public Texture2D _texture { get; set; }

        public Physics2D _physics { get; set; }
        public Tile() 
        {
            _physics = new Physics2D();
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
