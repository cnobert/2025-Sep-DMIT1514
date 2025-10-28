using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using SpriteFontPlus; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
using System.IO; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
using System.Collections;
using System.Collections.Generic;



namespace Lesson10_BrickerBreaker_OO;
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
    private const int _PaddleWidth = 120, _PaddleHeight = 18;
    private const float _PaddleSpeed = 400;
    private Paddle _paddle;
    
    #endregion

    #region Ball
    private const int _BallSize = 10;
    private const float _BallSpeed = 320.0f;
    private Ball _ball;

    #endregion

    private Rectangle[] _brickRectangles = new Rectangle[_BrickCount];
    private Color[] _brickColors = new Color[_BrickCount];
    private bool[] _brickAlive = new bool[_BrickCount];

    #region Brick constants
    private const int _BrickWidth = 70;
    private const int _BrickHeight = 24;
    private const int _BrickTopOffset = 70;
    private const int _BrickLeftOffset = 20;
    private const int _BrickHorizontalGap = 10;
    private const int _BrickVerticalGap = 8;

    private const int _BrickRows = 5;
    private const int _BrickCols = 10;
    private const int _BrickCount = _BrickRows * _BrickCols;
    #endregion

    #region Background Flash
    private readonly Color _bgBaseColor = Color.DarkGreen;
    private readonly Color _bgFlashColor = new Color(0, 0, 0);
    private float _bgFlashTimer = 0.0f;
    private const float _BgFlashDuration = 0.15f;
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

        Vector2 paddleStartPosition = new Vector2((_WindowWidth - _PaddleWidth) / 2, _WindowHeight - 50);

        Vector2 ballStartPosition = new Vector2(
            paddleStartPosition.X + (_PaddleWidth / 2) - (_BallSize / 2),
            paddleStartPosition.Y - _BallSize - 5
        );

        Rectangle gameAreaBoundingBox = new Rectangle(0, 0, _WindowWidth, _WindowHeight);

        _ball = new Ball();
        _ball.Initialize(ballStartPosition, _BallSize, _BallSpeed, gameAreaBoundingBox);

        _paddle = new Paddle();
        _paddle.Initialize(paddleStartPosition, new Vector2(_PaddleWidth, _PaddleHeight), _PaddleSpeed, Color.Brown, gameAreaBoundingBox);
        
        BuildLevel();
        ResetBallOnPaddle(_paddle);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _paddle.LoadContent(GraphicsDevice);

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

        if (Pressed(Keys.Tab))
        {
            _showDebug = !_showDebug;
        }

        if (Pressed(Keys.P))
        {
            _paused = !_paused;
        }

        if (!_paused)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_bgFlashTimer > 0.0f)
            {
                _bgFlashTimer -= deltaTime;
                if (_bgFlashTimer < 0.0f)
                {
                    _bgFlashTimer = 0.0f;
                }
            }
            // Paddle
            if (_kbState.IsKeyDown(Keys.Left) || _kbState.IsKeyDown(Keys.A))
            {
                _paddle.Move(-1);
            }
            else if (_kbState.IsKeyDown(Keys.Right) || _kbState.IsKeyDown(Keys.D))
            {
                _paddle.Move(1);
            }
            else
            {
                _paddle.Move(0);
            }

            _paddle.Update(gameTime);

            // Ball
            if (!_ball.Launched)
            {
                _ball.CentreOn(_paddle.BoundingBox);

                if (_kbState.IsKeyDown(Keys.Space))
                {
                    _ball.Launch();
                }
            }
            else
            {
                _ball.Update(gameTime);

                if (_ball.OutsideOfBounds()) //add 20% to _WindowHeight as a buffer to avoid early resets
                {
                    ResetBallOnPaddle(_paddle);
                }

                if(_ball.CollideWith(_paddle.BoundingBox))
                {
                    _ball.BounceOffTopAccordingToImpactLocation(_paddle.BoundingBox);
                }
                ResolveBrickCollision();
            }

            base.Update(gameTime);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkBlue);

        _spriteBatch.Begin();

        _ball.Draw(_spriteBatch, _pixel, Color.OrangeRed, new Color(255, 180, 120));
        
        _paddle.Draw(_spriteBatch);

        for (int i = 0; i < _BrickCount; i++)
        {
            if (_brickAlive[i])
            {
                _spriteBatch.Draw(_pixel, _brickRectangles[i], _brickColors[i]);
            }
        }

        if (_showDebug)
        {
            _spriteBatch.DrawString(_font, $"Ball Pos: {_ball.Position}", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(_font, $"Ball Vel: {_ball.Velocity}", new Vector2(10, 30), Color.White);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void ResetBallOnPaddle(Paddle paddle)
    {
        _ball.Launched = false;
        _ball.ClearTrail();
        _ball.CentreOn(paddle.BoundingBox);
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
                if (row == 1) { c = Color.LightGreen; }
                else if (row == 2) { c = Color.Khaki; }
                else if (row == 3) { c = Color.SandyBrown; }
                else if (row == 4) { c = Color.LightCoral; }

                _brickColors[index] = c;
                _brickAlive[index] = true;
                index++;
            }
        }
    }

    private void ResolveBrickCollision()
    {
        for (int i = 0; i < _BrickCount; i++)
        {
            if (_brickAlive[i])
            {
                if (_ball.CollideWith(_brickRectangles[i]))
                {
                    _brickAlive[i] = false;
                    _bgFlashTimer = _BgFlashDuration;
                    i = _BrickCount; // exit after first collision
                }
            }
        }
    }
}