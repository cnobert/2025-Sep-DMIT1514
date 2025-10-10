using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using SpriteFontPlus; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
using System.IO; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
using System.Collections; 
using System.Collections.Generic;

namespace Lesson07_BrickBreaker02;

public class BrickBreaker : Game
{
    private const int _WindowWidth = 800, _WindowHeight = 600;

    private SpriteFont _font;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    private KeyboardState _kbState, _kbPreviousState;

    #region Debug Overlay and Pause
    private bool _showDebug = false;
    private bool _paused = false;
    #endregion
    #region Paddle
    private Vector2 _paddlePosition;
    private float _paddleSpeed = 400.0f;
    private const int _PaddleWidth = 120, _PaddleHeight = 18;
    #endregion
    #region Ball
    private Vector2 _ballPosition;
    private Vector2 _ballVelocity;
    private bool _ballLaunched = false; //this is our first game object state
    private const int _BallSize = 10;
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

    #region Ball Trail
    private Queue<Vector2> _trail = new Queue<Vector2>();
    private const int _TrailMaxPoints = 10;
    private const float _TrailSampleSpacing = 0.01f;//seconds between trail samples
    private float _trailTimer = 0;
    #endregion
    #region Background Flash
    private readonly Color _bgBaseColor = Color.CornflowerBlue;
    private readonly Color _bgFlashColor = new Color(255, 240, 180);
    private float _bgFlashTimer = 0.0f;
    private const float _BgFlashDuration = 0.15f; // seconds
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

        #region ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
        byte[] fontBytes = File.ReadAllBytes("Content/Tahoma.ttf");
        _font = TtfFontBaker.Bake(fontBytes, 18, 1024, 1024,
                                    new[] { CharacterRange.BasicLatin })
                            .CreateSpriteFont(GraphicsDevice);
        #endregion
    }

    protected override void Update(GameTime gameTime)
    {
        _kbPreviousState = _kbState;
        _kbState = Keyboard.GetState();

        #region debug / pause
        if (Pressed(Keys.Tab))
        {
            _showDebug = !_showDebug;
        }
        if (Pressed(Keys.P))
        {
            _paused = !_paused;
        }
        #endregion

        if (!_paused)
        {

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            #region Flash Timer
            if (_bgFlashTimer > 0.0f)
            {
                _bgFlashTimer -= deltaTime;
                if (_bgFlashTimer < 0.0f)
                {
                    _bgFlashTimer = 0.0f;
                }
            }
            #endregion
            #region Paddle
            if (_kbState.IsKeyDown(Keys.Left) || _kbState.IsKeyDown(Keys.A))
            {
                _paddlePosition.X -= _paddleSpeed * deltaTime;
            }
            if (_kbState.IsKeyDown(Keys.Right) || _kbState.IsKeyDown(Keys.D))
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

                if (_kbState.IsKeyDown(Keys.Space))
                {
                    _ballLaunched = true;
                }
            }
            else //ball is in play!
            {
                _ballPosition += _ballVelocity * deltaTime;

                #region Trail
                _trailTimer += deltaTime;
                if (_trailTimer >= _TrailSampleSpacing)
                {
                    _trailTimer = 0;
                    //Vector2 that represents the current center of the ball
                    Vector2 centre = new Vector2(
                        _ballPosition.X + _BallSize * 0.5f,
                        _ballPosition.Y + _BallSize * 0.5f
                    );
                    _trail.Enqueue(centre);
                    if (_trail.Count > _TrailMaxPoints)
                    {
                        _trail.Dequeue();
                    }
                }
                #endregion
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
                    _trail.Clear();
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

                    //start the background flash timer ("flashing" will be happening for the duration)
                    _bgFlashTimer = _BgFlashDuration;
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
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        #region Flash
        //as the timer goes on, we lerp the background colour
        float tRaw = 0.0f;
        if (_bgFlashTimer > 0.0f)
        {
            tRaw = _bgFlashTimer / _BgFlashDuration; // 1 -> 0
        }
        Color bg = LerpColor(_bgFlashColor, _bgBaseColor, 1.0f - tRaw);
        GraphicsDevice.Clear(bg);
        #endregion
        _spriteBatch.Begin();

        #region debug stuff
        if (_showDebug)
        {
            _spriteBatch.DrawString(_font, $"Ball Pos: {_ballPosition}", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(_font, $"Ball Vel: {_ballVelocity}", new Vector2(10, 30), Color.White);
            //other possible data to dump: bricks left, game state data
            double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            int fps = 0;
            if (elapsedSeconds > 0.0)
            {
                fps = (int)(1.0 / elapsedSeconds);
            }
            _spriteBatch.DrawString(_font, $"FPS: {fps}", new Vector2(10, 50), Color.White);
        }
        #endregion

        #region Trail
        //loop through the queue and draw each segment
        int index = 0;
        foreach (Vector2 segment in _trail)
        {

            Rectangle segmentRectangle = new Rectangle(
                (int)(segment.X - _BallSize * 0.5f),
                (int)(segment.Y - _BallSize * 0.5f),
                _BallSize,
                _BallSize
            );

            float alpha = 1f - (float)index / _trail.Count;
            Color c = new Color(255, 180, 120) * (alpha * 0.5f);
            _spriteBatch.Draw(_pixel, segmentRectangle, c);
        }
        #endregion

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
        if (choice == 0)
        {
            dirX = -0.9f;
        }
        _ballVelocity = new Vector2(dirX, -1f);
        _ballVelocity *= _BallSpeed;
    }

    private bool Pressed(Keys key)
    {
        return _kbState.IsKeyDown(key) && _kbPreviousState.IsKeyUp(key);
    }

    // Linearly interpolates between two colors (a → b) based on blend factor t.
    // t = 0 returns color a, t = 1 returns color b, and values in between blend the two.
    // Each RGBA channel is interpolated separately and clamped to ensure valid range.
    // Used here to smoothly transition the background color after a brick is hit.
    private static Color LerpColor(Color a, Color b, float t)
    {
        t = Math.Clamp(t, 0.0f, 1.0f);

        int r = (int)(a.R + (b.R - a.R) * t);
        int g = (int)(a.G + (b.G - a.G) * t);
        int bch = (int)(a.B + (b.B - a.B) * t);
        int ach = (int)(a.A + (b.A - a.A) * t);
        Color c = new Color(r, g, bch, ach);
        return c;
    }
}