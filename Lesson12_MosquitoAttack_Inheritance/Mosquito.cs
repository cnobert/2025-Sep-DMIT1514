using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lesson12_MosquitoAttack_Inheritance;

public class Mosquito
{
    private const float _MaxSpeed = 120f, _FireBallSpeed = 100;
    private const float _DyingDuration = 1f;
    private const int _UpperRandomFiringRange = 500;
    private const int _NumFireBalls = 10;

    private const float _SwoopDuration = 2.0f;

    private SimpleAnimation _animation, _poofAnimation;

    private float _dyingTimer = 0;
    private Vector2 _position, _direction;
    private Rectangle _gameBoundingBox;
    internal Vector2 Direction { set => _direction = value; }

    private Random randomNumberGenerator = new Random();
    private FireBall[] _fireBalls;

    private float _timeUntilNextSwoop;
    private float _swoopElapsed;
    private bool _isSwooping;

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

        _direction.Y = 0f;
        _timeUntilNextSwoop = GetRandomSwoopDelay();
        _swoopElapsed = 0f;
        _isSwooping = false;

        _fireBalls = new FireBall[_NumFireBalls];
        for(int c = 0; c < _NumFireBalls; c++)
        {
            _fireBalls[c] = new FireBall();
            _fireBalls[c].Initialize(gameBoundingBox);
        }
    }

    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Mosquito");

        _animation = new SimpleAnimation(texture, 46, texture.Height, texture.Width / 46, 8f);
        _animation.Paused = false;

        Texture2D poofTexture = content.Load<Texture2D>("Poof");
        _poofAnimation = new SimpleAnimation(poofTexture, 16, poofTexture.Height, poofTexture.Width / 16, 2f);
        _poofAnimation.Paused = false;

        foreach (FireBall fb in _fireBalls)
        {
            fb.LoadContent(content);
        }
    }

    internal void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        switch(_state)
        {
            case State.Alive:
#region swooping
                if (_isSwooping)
                {
                    _swoopElapsed += deltaTime;
                    float halfDuration = _SwoopDuration * 0.5f;

                    if (_swoopElapsed < halfDuration)
                    {
                        _direction.Y = 1f;
                    }
                    else if (_swoopElapsed < _SwoopDuration)
                    {
                        _direction.Y = -1f;
                    }
                    else
                    {
                        _direction.Y = 0f;
                        _isSwooping = false;
                        _timeUntilNextSwoop = GetRandomSwoopDelay();
                        _swoopElapsed = 0f;
                    }
                }
                else
                {
                    _timeUntilNextSwoop -= deltaTime;
                    if (_timeUntilNextSwoop <= 0f)
                    {
                        _isSwooping = true;
                        _swoopElapsed = 0f;
                    }
                }
#endregion

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
                if(randomNumberGenerator.Next(1, _UpperRandomFiringRange) == 1)
                {
                    this.Shoot(new Vector2(0, 1));
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
        foreach (FireBall fb in _fireBalls)
        {
            fb.Update(gameTime);
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
        foreach (FireBall fb in _fireBalls)
        {
            fb.Draw(spriteBatch);
        }
    }
    internal void Shoot(Vector2 direction)
    {
        int fireBallIndex = 0;
        while(fireBallIndex < _NumFireBalls)
        {
            //Console.WriteLine(cannonBallIndex);
            if(_fireBalls[fireBallIndex].Shootable())
            {
                 //the cannonball is not quite centered on the barrel of the Cannon
                //this is left for you to figure out in the assignment
                Vector2 shootingPosition = new Vector2(BoundingBox.Center.X, BoundingBox.Bottom);
                _fireBalls[fireBallIndex].Shoot(shootingPosition, direction, _FireBallSpeed);

                //I found one that I can shoot! Don't forget to exit the loop.
                fireBallIndex = _NumFireBalls;
            }
            fireBallIndex++;
        }
    }
    internal bool ProcessCollision(Rectangle boundingBox)
    {
        bool hit = false;
        int c = 0;
        while(!hit && c < _fireBalls.Length)
        {
            hit = _fireBalls[c].ProcessCollision(boundingBox);
            c++;
        }
        return hit;
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

    //////////////////CHANGED///////////////
    private float GetRandomSwoopDelay()
    {
        int milliseconds = randomNumberGenerator.Next(2000, 5001);
        float delay = milliseconds / 1000f;
        return delay;
    }
    //////////////////CHANGED///////////////
}