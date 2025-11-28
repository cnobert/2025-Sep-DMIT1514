using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson11_MosquitoAttack;

public class Mosquito
{
    private const float _MaxSpeed = 120f;
    private const float _DyingDuration = 1f;

    private SimpleAnimation _animation, _poofAnimation;

    private float _dyingTimer = 0;

    private Vector2 _position, _direction;

    private Rectangle _gameBoundingBox;

    internal Vector2 Direction { set => _direction = value; }

    private enum State { Alive, Dying, Dead }

    private State _state;

    internal bool IsAlive
    {
        get
        {
            return _state == State.Alive;
        }
    }

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
        _state = State.Alive;
    }

    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Mosquito");

        int frameWidth = 46;
        int frameHeight = texture.Height;
        int frameCount = texture.Width / frameWidth;
        float framesPerSecond = 8f;
        _animation = new SimpleAnimation(texture, frameWidth, frameHeight, frameCount, framesPerSecond);
        _animation.Paused = false;

        Texture2D poofTexture = content.Load<Texture2D>("Poof");
        frameWidth = 16;
        frameHeight = poofTexture.Height;
        frameCount = poofTexture.Width / frameWidth;
        framesPerSecond = 8f;
        _poofAnimation = new SimpleAnimation(poofTexture, frameWidth, frameHeight, frameCount, framesPerSecond);
    }

    internal void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        switch(_state)
        {
            case State.Alive:
                _position += _direction * _MaxSpeed * deltaTime;

                if (BoundingBox.Left < _gameBoundingBox.Left)
                {
                    _direction.X *= -1;
                    _animation.Reverse = false;
                }
                else if (BoundingBox.Right > _gameBoundingBox.Right)
                {
                    _direction.X *= -1;
                    _animation.Reverse = true;
                }
                _animation.Update(gameTime);
                break;
            case State.Dying:
                _dyingTimer += deltaTime;
                if(_dyingTimer >= _DyingDuration)
                {
                    _state = State.Dead;
                }
                _animation.Update(gameTime);
                break;
            case State.Dead:
                break;
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        switch(_state)
        {
            case State.Alive:
            case State.Dying:
                if (_animation != null)
                {
                    _animation.Draw(spriteBatch, _position, SpriteEffects.None);
                }
                break;
            case State.Dead:
                break;
        }
    }
    internal void Die()
    {
        if(_state == State.Alive)
        {
            _state = State.Dying;
            _dyingTimer = 0;
            _poofAnimation.Paused = false;
            _animation = _poofAnimation;
            
        }
    }
}
