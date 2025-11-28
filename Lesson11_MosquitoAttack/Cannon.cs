using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Lesson11_MosquitoAttack;

public class Cannon
{
    private const float _MaxSpeed = 250;
    private const int _NumCannonBalls = 10;

    private SimpleAnimation _animation;
    private Vector2 _position, _direction;

    private Rectangle _gameBoundingBox;

    private CannonBall [] _cannonBalls;

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
        _animation = new SimpleAnimation(texture, texture.Width / 4, texture.Height, 4, 2f);
        _animation.Paused = false;
        foreach(CannonBall c in _cannonBalls)
        {
            c.LoadContent(content);
        }
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
        foreach(CannonBall c in _cannonBalls)
        {
            c.Update(gameTime);
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
}