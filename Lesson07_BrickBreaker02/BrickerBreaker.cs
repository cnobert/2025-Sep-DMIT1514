using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Lesson07_BrickBreaker02;

public class BrickBreaker : Game
{
    private const int _WindowWidth = 800, _WindowHeight = 600;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    #region Paddle
        private Vector2 _paddlePosition;
        private float _paddleSpeed = 400.0f;
        private const int _PaddleWidth = 120, _PaddleHeight = 18;
    #endregion
    #region Ball
    private Vector2 _ballPosition;
    private Vector2 _ballVelocity;
    private bool _ballLaunched = false; //this is our first game object state
    private const int _BallSize = 16;
    private const float _BallSpeed = 500;
    #endregion
    #region Brick
    private Rectangle _brickRectangle;
    private Color _brickColour = Color.SandyBrown;
    private bool _brickAlive = true;
    private const int _BrickWidth = 90, _BrickHeight = 28;
    
    #endregion
    #region derived properties
    private Rectangle PaddleRectangle => new Rectangle(
        (int)_paddlePosition.X,
        (int)_paddlePosition.Y,
        _PaddleWidth,
        _PaddleHeight
    );
    //read-only, derived property
    private Rectangle BallRectangle => new Rectangle(
        (int)_ballPosition.X, 
        (int)_ballPosition.Y, 
        _BallSize, 
        _BallSize
    );
    #endregion

    public BrickBreaker()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = _WindowWidth;
        _graphics.PreferredBackBufferHeight = _WindowHeight;
        _graphics.ApplyChanges();

        _paddlePosition = new Vector2((_WindowWidth - _PaddleWidth) / 2, _WindowHeight - 50);

        // Place ball above paddle
        _ballPosition = new Vector2(
            _paddlePosition.X + (_PaddleWidth / 2) - (_BallSize / 2),
            _paddlePosition.Y - _BallSize - 5
        );

        int brickX = (_WindowWidth - _BrickWidth) / 2;
        int brickY = 100;
        _brickRectangle = new Rectangle(brickX, brickY, _BrickWidth, _BrickHeight);

        ResetBallOnPaddle();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1); //create 1-by-1 pixel Texture2D

        Color[] colours = new Color[1];
        colours[0] = Color.White;
        _pixel.SetData(colours);

        //or, we could just use _pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState kb = Keyboard.GetState();
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        #region Paddle
        if (kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.A))
        {
            _paddlePosition.X -= _paddleSpeed * deltaTime;
        }
        if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D))
        {
            _paddlePosition.X += _paddleSpeed * deltaTime;
        }
        //fix any screen overrunds (pin to one side or the other if we're exiting the screen)
        //right side
        if (_paddlePosition.X > _WindowWidth - _PaddleWidth)
        {
            _paddlePosition.X = _WindowWidth - _PaddleWidth;
        }
        //left side
        if (_paddlePosition.X < 0.0f)
        {
            _paddlePosition.X = 0.0f;
        }
        #endregion

        if (!_ballLaunched)
        {
            //keep ball resting above paddle
            _ballPosition.X = _paddlePosition.X + (_PaddleWidth / 2) - (_BallSize / 2);
            _ballPosition.Y = _paddlePosition.Y - _BallSize - 5;

            if (kb.IsKeyDown(Keys.Space))
            {
                _ballLaunched = true;
            }
        }
        else //ball is in play!
        {
            _ballPosition += _ballVelocity * deltaTime;

            #region Walls collisions
            if (_ballPosition.X <= 0.0f)
            {
                _ballPosition.X = 0.0f;
                _ballVelocity.X *= -1;
            }
            if (_ballPosition.X + _BallSize >= _WindowWidth)
            {
                _ballPosition.X = _WindowWidth - _BallSize;
                _ballVelocity.X *= -1;
            }
            //top wall
            if (_ballPosition.Y <= 0.0f)
            {
                _ballPosition.Y = 0.0f;
                _ballVelocity.Y *= -1;
            }
            //bottom of window
            if (_ballPosition.Y > 1.2f * _WindowHeight)
            {
                ResetBallOnPaddle();
            }
            #endregion
            #region Paddle collisions
            //collide with Paddle, if needed
            if (BallRectangle.Intersects(PaddleRectangle))
            {
                _ballPosition.Y = _paddlePosition.Y - _BallSize;
                _ballVelocity.Y *= -1; //change y direction

                //divides the paddle into 5 regions, and returns a number depending on which region was hit
                float hitRatio = (
                    (_ballPosition.X + _BallSize / 2f) -
                    (_paddlePosition.X + _PaddleWidth / 2f)
                ) / (_PaddleWidth / 2f); //returns -1.0, -0.5, 0, +0.5, +1.0

                _ballVelocity.X = MathHelper.Clamp(hitRatio, -1f, 1f) * _BallSpeed; //change x direction

                _ballVelocity.Normalize();//strip the speed
                _ballVelocity *= _BallSpeed; //add back the speed
            }
            #endregion
            #region Brick collisions

            //in assignment #1, this if statement will be in a loop
            if (_brickAlive && BallRectangle.Intersects(_brickRectangle))
            {
                _brickAlive = false;
                //make the ball bounce off the brick using the shallowest-overlap bounce technique

                float overlapLeft = BallRectangle.Right - _brickRectangle.Left;
                float overlapRight = _brickRectangle.Right - BallRectangle.Left;
                float overlapTop = BallRectangle.Bottom - _brickRectangle.Top;
                float overlapBottom = _brickRectangle.Bottom - BallRectangle.Top;

                float minXOverlap = Math.Min(overlapLeft, overlapRight);
                float minYOverlap = Math.Min(overlapTop, overlapBottom);

                //did it a side?
                if (minXOverlap < minYOverlap)
                {
                    //it hit the left side
                    if (overlapLeft < overlapRight)
                    {
                        _ballPosition.X -= overlapLeft;
                    }
                    else // hit the right side
                    {
                        _ballPosition.X += overlapRight;
                    }
                    _ballVelocity.X *= -1.0f;
                }
                else //did it hit the top or bottom?
                {
                    //it hit the top
                    if (overlapTop < overlapBottom)
                    {
                        _ballPosition.Y -= overlapTop;
                    }
                    else //hit the bottom
                    {
                        _ballPosition.Y += overlapBottom;
                    }
                    _ballVelocity.Y *= -1.0f;
                }
            }
            #endregion
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
;
        _spriteBatch.Draw(_pixel, PaddleRectangle, Color.Brown);
        _spriteBatch.Draw(_pixel, BallRectangle, Color.OrangeRed);

        //in assignment #1, loop through the brick arrays here
        if (_brickAlive)
        {
            _spriteBatch.Draw(_pixel, _brickRectangle, _brickColour);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
    private void ResetBallOnPaddle()
    {
        _ballLaunched = false;
        // Direction points generally upward; X may be left or right
        Random rando = new Random();
        float dirX = 0.9f;
        int choice = rando.Next(2);
        if(choice == 0)
        {
            dirX = -0.9f;
        }
        _ballVelocity = new Vector2(dirX, -1f);
        _ballVelocity *= _BallSpeed;
    }
}