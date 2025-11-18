using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson11_MosquitoAttack;

public class Cannon
{
    private const float _MaxSpeed = 250;

    private SimpleAnimation _animation;
    private Vector2 _position, _direction;

    private Rectangle _gameBoundingBox;

    internal Vector2 Direction { set => _direction = value; }
    
    public Rectangle BoundingBox
    {
        get
        {
            return new Rectangle(
                (int)_position.X, 
                (int)_position.Y, 
                (int)_animation.FrameDimensions.X, 
                (int)_animation.FrameDimensions.Y);
        }
    }
    
    internal void Initialize(Vector2 position, Rectangle gameBoundingBox)
    {
        _position = position;
        _gameBoundingBox = gameBoundingBox;
    }
    
    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Cannon");
        _animation = new SimpleAnimation(texture, texture.Width / 4, texture.Height, 4, 2f);
        _animation.Paused = false;
    }
    internal void Update(GameTime gameTime)
    {
        _position += _direction * _MaxSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        //change animation depending on direction (or not moving)
        if(_direction.X != 0)
        {
            if(_direction.X == -1)
            {
                _animation.Reverse = true;
            }
            else
            {
                _animation.Reverse = false;
            }
            _animation.Update(gameTime);
        }
    
        if(BoundingBox.Left < _gameBoundingBox.Left)
        {
            _position.X = _gameBoundingBox.Left;
        }
        else if(BoundingBox.Right > _gameBoundingBox.Right)
        {
            _position.X = _gameBoundingBox.Right - BoundingBox.Width;
        }
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        if(_animation != null)
        {
            _animation.Draw(spriteBatch, _position, SpriteEffects.None);
        }
    }
}