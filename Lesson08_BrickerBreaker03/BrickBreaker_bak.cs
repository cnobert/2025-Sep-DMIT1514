using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using SpriteFontPlus; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
using System.IO; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
using System.Collections; 
using System.Collections.Generic;

namespace Lesson08_BrickBreaker03;

public class BrickBreaker : Game
{
    private const int _WindowWidth = 840, _WindowHeight = 600;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    private KeyboardState _kbState, _kbPreviousState;

    private SpriteFont _font;

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
    private const int _BallSize = 10;
    private Ball _ball;

    #endregion

    #region Bricks (parallel arrays)

    private const int _BrickRows = 5;
    private const int _BrickCols = 10;
    private const int _BrickCount = _BrickRows * _BrickCols;

    private Rectangle[] _brickRectangles = new Rectangle[_BrickCount];
    private Color[] _brickColors = new Color[_BrickCount];
    private bool[] _brickAlive = new bool[_BrickCount];

    // Brick layout
    private const int _BrickWidth = 70;
    private const int _BrickHeight = 24;
    private const int _BrickTopOffset = 70;
    private const int _BrickLeftOffset = 20;
    private const int _BrickHorizontalGap = 10;
    private const int _BrickVerticalGap = 8;
    #endregion

    #region derived properties
    private Rectangle PaddleRectangle => new Rectangle(
        (int)_paddlePosition.X,
        (int)_paddlePosition.Y,
        _PaddleWidth,
        _PaddleHeight
    );

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
    private const float _TrailSampleSpacing = 0.01f; // seconds between samples
    private float _trailTimer = 0.0f;
    #endregion

    #region Background Flash
    private readonly Color _bgBaseColor = Color.DarkGreen;
    private readonly Color _bgFlashColor = new Color(0, 0, 0);
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

        Vector2 ballPosition = new Vector2(
            _paddlePosition.X + (_PaddleWidth / 2) - (_BallSize / 2),
            _paddlePosition.Y - _BallSize - 5
        );

        _ball.Initialize(ballPosition, _BallSize);
        
        BuildLevel();
        ResetBallOnPaddle();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        #region ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
        byte[] fontBytes = File.ReadAllBytes("Content/Tahoma.ttf");
        _font = TtfFontBaker.Bake(
                    fontBytes,
                    18,
                    1024,
                    1024,
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
            if (_paddlePosition.X > _WindowWidth - _PaddleWidth)
            {
                _paddlePosition.X = _WindowWidth - _PaddleWidth;
            }
            if (_paddlePosition.X < 0.0f)
            {
                _paddlePosition.X = 0.0f;
            }
            #endregion

            #region Ball update
            if (!_ball.Launched)
            {
                // keep ball resting above paddle
                _ball.Position = new Vector2(
                        _paddlePosition.X + (_PaddleWidth / 2) - (_BallSize / 2),
                        _paddlePosition.Y - _BallSize - 5
                );

                if (_kbState.IsKeyDown(Keys.Space))
                {
                    _ball.Launched = true;
                }
            }
            else
            {
                _ball.Update(gameTime);
                

                #region Trail sample
                _trailTimer += deltaTime;
                if (_trailTimer >= _TrailSampleSpacing)
                {
                    _trailTimer = 0.0f;
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
                if (_ball.Position.X <= 0.0f)
                {
                    _ball.Position = new Vector2(0.0f, _ball.Position.Y);
                    _ball.Velocity = new Vector2(-1.0f, _ball.Velocity.Y);
                }
                if (_ballPosition.X + _BallSize >= _WindowWidth)
                {
                    _ballPosition.X = _WindowWidth - _BallSize;
                    _ballVelocity.X *= -1.0f;
                }
                if (_ballPosition.Y <= 0.0f)
                {
                    _ballPosition.Y = 0.0f;
                    _ballVelocity.Y *= -1.0f;
                }
                if (_ballPosition.Y > 1.2f * _WindowHeight)
                {
                    ResetBallOnPaddle();
                    _trail.Clear();
                }
                #endregion

                #region Paddle collisions
                if (BallRectangle.Intersects(PaddleRectangle))
                {
                    _ballPosition.Y = _paddlePosition.Y - _BallSize;
                    _ballVelocity.Y *= -1.0f;

                    float hitRatio = (
                        (_ballPosition.X + _BallSize / 2.0f) -
                        (_paddlePosition.X + _PaddleWidth / 2.0f)
                    ) / (_PaddleWidth / 2.0f);

                    _ballVelocity.X = MathHelper.Clamp(hitRatio, -1.0f, 1.0f) * _BallSpeed;
                    _ballVelocity.Normalize();
                    _ballVelocity *= _BallSpeed;
                }
                #endregion

                #region Brick collisions
                ResolveBrickCollision();
                #endregion
            }
            #endregion
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        #region Flash
        float tRaw = 0.0f;
        if (_bgFlashTimer > 0.0f)
        {
            tRaw = _bgFlashTimer / _BgFlashDuration; // 1 -> 0 over time
        }
        Color bg = Color.DarkBlue; //LerpColor(_bgFlashColor, _bgBaseColor, 1.0f - tRaw);
        GraphicsDevice.Clear(bg);
        #endregion

        _spriteBatch.Begin();

        #region debug stuff
        if (_showDebug)
        {
            _spriteBatch.DrawString(_font, $"Ball Pos: {_ballPosition}", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(_font, $"Ball Vel: {_ballVelocity}", new Vector2(10, 30), Color.White);

            int bricksRemaining = 0;
            for (int i = 0; i < _BrickCount; i++)
            {
                if (_brickAlive[i])
                {
                    bricksRemaining++;
                }
            }
            _spriteBatch.DrawString(_font, $"Bricks: {bricksRemaining}", new Vector2(10, 50), Color.White);

            double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            int fps = 0;
            if (elapsedSeconds > 0.0)
            {
                fps = (int)(1.0 / elapsedSeconds);
            }
            _spriteBatch.DrawString(_font, $"FPS: {fps}", new Vector2(10, 70), Color.White);
        }
        #endregion

        #region Trail
        int index = 0;
        foreach (Vector2 segment in _trail)
        {
            Rectangle segmentRectangle = new Rectangle(
                (int)(segment.X - _BallSize * 0.5f),
                (int)(segment.Y - _BallSize * 0.5f),
                _BallSize,
                _BallSize
            );

            float alpha = 1.0f - (float)index / Math.Max(1, _trail.Count);
            Color c = new Color(255, 180, 120) * (alpha * 0.5f);
            _spriteBatch.Draw(_pixel, segmentRectangle, c);
            index++;
        }
        #endregion

        // Paddle and ball
        _spriteBatch.Draw(_pixel, PaddleRectangle, Color.Brown);
        _spriteBatch.Draw(_pixel, BallRectangle, Color.OrangeRed);

        // Bricks
        for (int i = 0; i < _BrickCount; i++)
        {
            if (_brickAlive[i])
            {
                _spriteBatch.Draw(_pixel, _brickRectangles[i], _brickColors[i]);
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void ResetBallOnPaddle()
    {
        _ballLaunched = false;

        Random rando = new Random();
        float dirX = 0.9f;
        int choice = rando.Next(2);
        if (choice == 0)
        {
            dirX = -0.9f;
        }

        _ballVelocity = new Vector2(dirX, -1.0f);
        _ballVelocity.Normalize();
        _ballVelocity *= _BallSpeed;
    }

    private bool Pressed(Keys key)
    {
        bool pressed = _kbState.IsKeyDown(key) && _kbPreviousState.IsKeyUp(key);
        return pressed;
    }

    private void BuildLevel()
    {
        int index = 0;

        for (int row = 0; row < _BrickRows; row++)
        {
            for (int col = 0; col < _BrickCols; col++)
            {
                int x = _BrickLeftOffset + col * (_BrickWidth + _BrickHorizontalGap);
                int y = _BrickTopOffset + row * (_BrickHeight + _BrickVerticalGap);

                _brickRectangles[index] = new Rectangle(x, y, _BrickWidth, _BrickHeight);

                Color c = Color.LightSkyBlue;
                if (row == 1)
                {
                    c = Color.LightGreen;
                }
                else if (row == 2)
                {
                    c = Color.Khaki;
                }
                else if (row == 3)
                {
                    c = Color.SandyBrown;
                }
                else if (row == 4)
                {
                    c = Color.LightCoral;
                }

                _brickColors[index] = c;
                _brickAlive[index] = true;

                index++;
            }
        }
    }

    private void ResolveBrickCollision()
    {
        Rectangle b = BallRectangle;

        for (int i = 0; i < _BrickCount; i++)
        {
            if (_brickAlive[i])
            {
                Rectangle r = _brickRectangles[i];

                if (b.Intersects(r))
                {
                    float overlapLeft = b.Right - r.Left;
                    float overlapRight = r.Right - b.Left;
                    float overlapTop = b.Bottom - r.Top;
                    float overlapBottom = r.Bottom - b.Top;

                    float minXOverlap = Math.Min(overlapLeft, overlapRight);
                    float minYOverlap = Math.Min(overlapTop, overlapBottom);

                    if (minXOverlap < minYOverlap)
                    {
                        if (overlapLeft < overlapRight)
                        {
                            _ballPosition.X -= overlapLeft;
                        }
                        else
                        {
                            _ballPosition.X += overlapRight;
                        }
                        _ballVelocity.X *= -1.0f;
                    }
                    else
                    {
                        if (overlapTop < overlapBottom)
                        {
                            _ballPosition.Y -= overlapTop;
                        }
                        else
                        {
                            _ballPosition.Y += overlapBottom;
                        }
                        _ballVelocity.Y *= -1.0f;
                    }

                    _brickAlive[i] = false;

                    // start background flash on hit
                    _bgFlashTimer = _BgFlashDuration;

                    // resolve only one brick per frame
                    i = _BrickCount;
                }
            }
        }
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