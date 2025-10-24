using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson09a_Pong_and_AI;

public class Pong : Game
{
    private const int _Scale = 4;

    private const int _WindowWidth = 250 * _Scale;
    private const int _WindowHeight = 150 * _Scale;

    private const int _BallWidthAndHeight = 3 * _Scale;

    private const int _PaddleWidth = 2 * _Scale;
    private const int _PaddleHeight = 18 * _Scale;

    private const int _MaxBalls = 100;
    private const int _InitialBallCount = 1;
    private const int _StartingLives = 3;

    private static readonly Random _rng = new Random();

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Rectangle _playAreaBoundingBox;

    private readonly List<Ball> _balls = new List<Ball>(_InitialBallCount);

    private Paddle _paddleLeft, _paddleRight;

    private int _livesLeftPlayer = _StartingLives;
    private int _livesRightPlayer = _StartingLives;
    private bool _gameOver = false;

    private KeyboardState _kbState, _kbPreviousState;

    // Reusable 1x1 pixel for UI (lives)
    private Texture2D _uiPixel;

    public Pong()
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

        _playAreaBoundingBox = new Rectangle(0, 0, _WindowWidth, _WindowHeight);

        _paddleLeft = new Paddle();
        _paddleRight = new Paddle();

        Vector2 paddleLeftPosition = new Vector2(10 * _Scale, 75 * _Scale);
        Vector2 paddleRightPosition = new Vector2(210 * _Scale, 75 * _Scale);
        float paddleSpeed = 100 * _Scale;
        Vector2 paddleDimensions = new Vector2(_PaddleWidth, _PaddleHeight);

        // Fun multi-colour stripes for each paddle
        Color[] leftColors = new Color[] { Color.Green, Color.LimeGreen, Color.ForestGreen, Color.DarkGreen };
        Color[] rightColors = new Color[] { Color.OrangeRed, Color.Orange, Color.Gold, Color.Yellow };

        _paddleLeft.Initialize(paddleLeftPosition, paddleSpeed, paddleDimensions, _playAreaBoundingBox, leftColors);
        _paddleRight.Initialize(paddleRightPosition, paddleSpeed, paddleDimensions, _playAreaBoundingBox, rightColors);

        // Start with a configurable number of balls
        int startCount = Math.Clamp(_InitialBallCount, 1, _MaxBalls);
        for (int i = 0; i < startCount; i++)
        {
            AddBallRandom();
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        foreach (Ball ball in _balls)
        {
            ball.LoadContent(GraphicsDevice);
        }

        _paddleLeft.LoadContent(GraphicsDevice);
        _paddleRight.LoadContent(GraphicsDevice);

        // Create a single reusable pixel texture for UI elements (lives)
        _uiPixel = new Texture2D(GraphicsDevice, 1, 1);
        _uiPixel.SetData(new[] { Color.White });
    }

    protected override void UnloadContent()
    {
        _uiPixel?.Dispose();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        _kbPreviousState = _kbState;
        _kbState = Keyboard.GetState();

        if (_kbState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        HandleBallCountHotkeys();

        if (!_gameOver)
        {
            HandlePaddleInput();

            foreach (Ball ball in _balls)
            {
                ball.Update(gameTime);
            }

            _paddleLeft.Update(gameTime);
            _paddleRight.Update(gameTime);

            foreach (Ball ball in _balls)
            {
                ball.ProcessCollision(_paddleLeft.BoundingBox);
                ball.ProcessCollision(_paddleRight.BoundingBox);
            }

            HandleLivesAndResets();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        foreach (Ball ball in _balls)
        {
            ball.Draw(_spriteBatch);
        }

        _paddleLeft.Draw(_spriteBatch);
        _paddleRight.Draw(_spriteBatch);

        DrawLives();

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void HandlePaddleInput()
    {
        if (_kbState.IsKeyDown(Keys.W))
        {
            _paddleLeft.Direction = new Vector2(0f, -1f);
        }
        else if (_kbState.IsKeyDown(Keys.S))
        {
            _paddleLeft.Direction = new Vector2(0f, 1f);
        }
        else
        {
            _paddleLeft.Direction = Vector2.Zero;
        }

        if (_kbState.IsKeyDown(Keys.Up))
        {
            _paddleRight.Direction = new Vector2(0f, -1f);
        }
        else if (_kbState.IsKeyDown(Keys.Down))
        {
            _paddleRight.Direction = new Vector2(0f, 1f);
        }
        else
        {
            _paddleRight.Direction = Vector2.Zero;
        }
    }

    private void HandleBallCountHotkeys()
    {
        // Plus: either main-keyboard OEM Plus (Shift+'=') or Numpad Add
        bool plusPressed = EdgePressed(Keys.OemPlus) || EdgePressed(Keys.Add);

        // Minus: either main-keyboard OEM Minus or Numpad Subtract
        bool minusPressed = EdgePressed(Keys.OemMinus) || EdgePressed(Keys.Subtract);

        if (plusPressed && _balls.Count < _MaxBalls)
        {
            AddBallRandom();
            _balls[^1].LoadContent(GraphicsDevice);
        }
        else if (minusPressed && _balls.Count > 1)
        {
            _balls.RemoveAt(_balls.Count - 1);
        }
    }

    private bool EdgePressed(Keys key)
    {
        bool pressed = false;

        if (_kbState.IsKeyDown(key) && _kbPreviousState.IsKeyUp(key))
        {
            pressed = true;
        }

        return pressed;
    }

    private void AddBallRandom()
    {
        Ball b = new Ball();

        Vector2 center = new Vector2(_WindowWidth / 2f - _BallWidthAndHeight / 2f, _WindowHeight / 2f - _BallWidthAndHeight / 2f);
        float speed = 100 * _Scale;

        // Random direction with a reasonable vertical component
        float dirX = _rng.Next(0, 2) == 0 ? -1f : 1f;
        float dirY = (float)(_rng.NextDouble() * 1.2 - 0.6); // between -0.6 and +0.6
        dirY = MathF.Abs(dirY) < 0.2f ? MathF.CopySign(0.2f, dirY == 0f ? 1f : dirY) : dirY;

        Vector2 direction = new Vector2(dirX, dirY);
        direction.Normalize();

        Vector2 dimensions = new Vector2(_BallWidthAndHeight, _BallWidthAndHeight);

        // Small random offset so multiple balls do not perfectly overlap
        Vector2 jitter = new Vector2(_rng.Next(-10, 11), _rng.Next(-10, 11));
        b.Initialize(center + jitter, speed, direction, dimensions, _playAreaBoundingBox);

        _balls.Add(b);
    }

    private void HandleLivesAndResets()
    {
        for (int i = 0; i < _balls.Count; i++)
        {
            Ball ball = _balls[i];

            bool outLeft = ball.IsOutLeft();
            bool outRight = ball.IsOutRight();

            if (outLeft || outRight)
            {
                if (outLeft)
                {
                    _livesLeftPlayer = Math.Max(0, _livesLeftPlayer - 1);
                }
                else
                {
                    _livesRightPlayer = Math.Max(0, _livesRightPlayer - 1);
                }

                if (_livesLeftPlayer == 0 || _livesRightPlayer == 0)
                {
                    _gameOver = true;
                }

                Vector2 center = new Vector2(_WindowWidth / 2f - _BallWidthAndHeight / 2f, _WindowHeight / 2f - _BallWidthAndHeight / 2f);

                float dirX = outLeft ? -1f : 1f; // send it back toward the side that just scored

                float dirY = (float)(_rng.NextDouble() * 1.2 - 0.6);
                dirY = MathF.Abs(dirY) < 0.2f ? MathF.CopySign(0.2f, dirY == 0f ? 1f : dirY) : dirY;

                Vector2 direction = new Vector2(dirX, dirY);
                direction.Normalize();

                ball.Reset(center, direction);
            }
        }
    }

    private void DrawLives()
    {
        // Tiny squares showing remaining lives for each side
        // Left lives at top-left, right lives at top-right
        int box = 6 * _Scale / 2; // scale a bit with window
        int pad = 4 * _Scale / 2;

        for (int i = 0; i < _livesLeftPlayer; i++)
        {
            Rectangle r = new Rectangle(pad + i * (box + pad), pad, box, box);
            _spriteBatch.Draw(_uiPixel, r, new Color(50, 220, 50));
        }

        for (int i = 0; i < _livesRightPlayer; i++)
        {
            int x = _WindowWidth - pad - box - i * (box + pad);
            Rectangle r = new Rectangle(x, pad, box, box);
            _spriteBatch.Draw(_uiPixel, r, new Color(255, 180, 0));
        }
    }
}