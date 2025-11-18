using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson11_MosquitoAttack;

public class Mosquito
{
    private const float _MaxSpeed = 120f;

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
        Texture2D texture = content.Load<Texture2D>("Mosquito");

        int frameWidth = 46;
        int frameHeight = texture.Height;
        int frameCount = texture.Width / frameWidth;
        float framesPerSecond = 8f;
        _animation = new SimpleAnimation(
            texture,
            frameWidth,
            frameHeight,
            frameCount,
            framesPerSecond
        );

        _animation.Paused = false;
    }

    internal void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _position += _direction * _MaxSpeed * deltaTime;

        if (BoundingBox.Left < _gameBoundingBox.Left)
        {
            _direction.X *= -1;
        }
        else if (BoundingBox.Right > _gameBoundingBox.Right)
        {
            _direction.X *= -1;
        }
        _animation.Update(gameTime);

    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        if (_animation != null)
        {
            _animation.Draw(spriteBatch, _position, SpriteEffects.None);
        }
    }
}
