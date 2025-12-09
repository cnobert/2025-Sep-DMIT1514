using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lesson13_Platforms;

public class Player
{
    private const int _Speed = 150;
    private const int _JumpForce = -150;

    private enum State
    {
        Idle,
        Walking,
        Jumping
    }

    private State _state;
    private bool _facingRight = true;

    private SimpleAnimation _idleAnimation, _jumpAnimation, _walkAnimation;
    private SimpleAnimation _currentAnimation;

    private Vector2 _position, _velocity, _dimensions;
    
    internal Vector2 Velocity { get => _velocity; set => _velocity = value; }

    private Rectangle _gameBoundingBox;

    internal Rectangle BoundingBox
    {
        get {return new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X, (int)_dimensions.Y);}
    }

    public Player(Vector2 position, Rectangle gameBoundingBox)
    {
        _position = position;
        _gameBoundingBox = gameBoundingBox;
        _dimensions = new Vector2(36, 35);
    }

    internal void Initialize()
    {
        _state = State.Idle;
        _currentAnimation = _idleAnimation;
        if (_currentAnimation != null)
        {
            _currentAnimation.Reset();
        }
    }

    internal void LoadContent(ContentManager content)
    {
        // Idle: cells 30 px wide, 1/8 s per frame => 8 fps
        Texture2D idleTexture = content.Load<Texture2D>("Idle");
        int idleFrameWidth = 30;
        int idleFrameHeight = idleTexture.Height;
        int idleFrameCount = idleTexture.Width / idleFrameWidth;
        _idleAnimation = new SimpleAnimation(idleTexture, idleFrameWidth, idleFrameHeight, idleFrameCount, 8f);

        // Walk: cells 35 px wide, 1/8 s per frame => 8 fps
        Texture2D walkTexture = content.Load<Texture2D>("Walk");
        int walkFrameWidth = 35;
        int walkFrameHeight = walkTexture.Height;
        int walkFrameCount = walkTexture.Width / walkFrameWidth;
        _walkAnimation = new SimpleAnimation(walkTexture, walkFrameWidth, walkFrameHeight, walkFrameCount, 8f);

        // Jump: cells 30 px wide, 1/8 s per frame => 8 fps
        Texture2D jumpTexture = content.Load<Texture2D>("JumpOne");
        int jumpFrameWidth = 30;
        int jumpFrameHeight = jumpTexture.Height;
        int jumpFrameCount = jumpTexture.Width / jumpFrameWidth;
        _jumpAnimation = new SimpleAnimation(jumpTexture, jumpFrameWidth, jumpFrameHeight, jumpFrameCount, 8f);

        // After loading, make sure Initialize will have something to use
        _currentAnimation = _idleAnimation;
    }

    internal void Update(GameTime gameTime)
    {
        if (_currentAnimation != null)
        {
            _currentAnimation.Update(gameTime);
        }
        
        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
        _velocity.Y += PlatformerGame._Gravity * dt;

        _position += _velocity * dt;

        //are we moving up or down faster than gravity?
        if(Math.Abs(_velocity.Y) > PlatformerGame._Gravity * dt)
        {
            _state = State.Jumping;
            _currentAnimation = _jumpAnimation;
        }

        switch (_state)
        {
            case State.Jumping:
                break;
            case State.Idle:
                break;
            case State.Walking:
                break;
        }
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        switch (_state)
        {
            case State.Jumping:
            case State.Idle:
            case State.Walking:
                if (_currentAnimation != null)
                {
                    SpriteEffects effects = SpriteEffects.None;
                    if (!_facingRight)
                    {
                        effects = SpriteEffects.FlipHorizontally;
                    }

                    _currentAnimation.Draw(spriteBatch, _position, effects);
                }
                break;
        }
    }

    internal void MoveHorizontally(float direction)
    {
        _velocity.X = direction * _Speed;

        if (_velocity.X > 0)
        {
            _facingRight = true;
        }
        else if (_velocity.X < 0)
        {
            _facingRight = false;
        }

        if (_state != State.Jumping)
        {
            _currentAnimation = _walkAnimation;
            if (_currentAnimation != null)
            {
                _currentAnimation.Reset();
            }
            _state = State.Walking;
        }
    }

    internal void MoveVertically(float direction)
    {
        _velocity.Y = direction * _Speed;
    }

    internal void Stop()
    {
        _velocity.X = 0;
        if (_state == State.Walking)
        {
            _state = State.Idle;
            _currentAnimation = _idleAnimation;
            if (_currentAnimation != null)
            {
                _currentAnimation.Reset();
            }
        }
    }
    internal void Land(Rectangle whatILandedOn)
    {
        if(_state  == State.Jumping)
        {
            //set our position exactly on top of the collider, but
            //sink us one pixel into it
            _position.Y = whatILandedOn.Top - _dimensions.Y + 1;
            _velocity.Y = 0;
            _state = State.Walking;
        }
    }
    internal void StandOn(Rectangle whatILandedOn, float dt)
    {
        _velocity.Y -= PlatformerGame._Gravity * dt;
    }
    internal void Jump()
    {
        if(_state != State.Jumping)
        {
            _velocity.Y = _JumpForce;
        }
    }
}