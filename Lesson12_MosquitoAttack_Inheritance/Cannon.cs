using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lesson12_MosquitoAttack_Inheritance;

public class Cannon
{
    private const float _MaxSpeed = 250;
    private const int _NumCannonBalls = 10;
    private const float _DyingDuration = 1f;

    private SimpleAnimation _animation, _poofAnimation;

    private float _dyingTimer = 0;

    private Vector2 _position, _direction;

    private Rectangle _gameBoundingBox;

    private CannonBall [] _cannonBalls;

    internal Vector2 Direction { set => _direction = value; }
    
    private enum State { Alive, Dying, Dead }

    private State _state;
    private bool _noCannonBalls;
    internal bool NeedReload => _noCannonBalls;

    internal bool IsAlive
    {
        get
        {
            return _state == State.Alive;
        }
    }
    internal bool Dead => _state == State.Dead;
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

        _cannonBalls = new CannonBall[_NumCannonBalls];
        for(int c = 0; c < _NumCannonBalls; c++)
        {
            _cannonBalls[c] = new CannonBall();
            _cannonBalls[c].Initialize(gameBoundingBox);
        }        
    }
    
    internal void LoadContent(ContentManager content)
    {
        Texture2D texture = content.Load<Texture2D>("Cannon");

         int frameWidth = texture.Width / 4;
        int frameHeight = texture.Height;
        int frameCount = texture.Width / frameWidth;
        float framesPerSecond = 8f;

        _animation = new SimpleAnimation(texture, frameWidth, texture.Height, frameCount, 2f);

        _animation.Paused = false;
        foreach(CannonBall c in _cannonBalls)
        {
            c.LoadContent(content);
        }

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
                }
            
                if(BoundingBox.Left < _gameBoundingBox.Left)
                {
                    _position.X = _gameBoundingBox.Left;
                }
                else if(BoundingBox.Right > _gameBoundingBox.Right)
                {
                    _position.X = _gameBoundingBox.Right - BoundingBox.Width;
                }
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
        _noCannonBalls = true;
        foreach(CannonBall c in _cannonBalls)
        {
            c.Update(gameTime);
            if(c.Shootable())
            {
                _noCannonBalls = false;
            }
        }
        
    }
    internal void Draw(SpriteBatch spriteBatch)
    {
        if(_animation != null)
        {
            _animation.Draw(spriteBatch, _position, SpriteEffects.None);
        }
        foreach(CannonBall c in _cannonBalls)
        {
            c.Draw(spriteBatch);
        }
    }

    internal void Shoot()
    {
        int cannonBallIndex = 0;
        while(cannonBallIndex < _NumCannonBalls)
        {
            //Console.WriteLine(cannonBallIndex);
            if(_cannonBalls[cannonBallIndex].Shootable())
            {
                 //the cannonball is not quite centered on the barrel of the Cannon
                //this is left for you to figure out in the assignment
                _cannonBalls[cannonBallIndex].Shoot(new Vector2(BoundingBox.Center.X, BoundingBox.Top), new Vector2(0, -1));

                //I found one that I can shoot! Don't forget to exit the loop.
                cannonBallIndex = _NumCannonBalls;
            }
            cannonBallIndex++;
        }
    }
    internal bool ProcessCollision(Rectangle boundingBox)
    {
        bool hit = false;
        int c = 0;
        while(!hit && c < _cannonBalls.Length)
        {
            hit = _cannonBalls[c].ProcessCollision(boundingBox);
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

    internal void Reload()
    {
        foreach(CannonBall cannonBall in _cannonBalls)
        {
            cannonBall.MakeShootable();
        }
    }
}