using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson09_Pong;

public class Pong : Game
{
    private const int _Scale = 4;

    private const int _WindowWidth = 250 * _Scale, _WindowHeight = 150 * _Scale;
    
    private const int _BallWidthAndHeight = 3 * _Scale;

    private const int _PaddleWidth = 2 * _Scale, _PaddleHeight = 18 * _Scale;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Rectangle _playAreaBoundingBox;

    private Ball[] _balls;

    private Paddle _paddleLeft, _paddleRight;

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

        _balls = new Ball[4];
        for (int c = 0; c < 4; c++)
        {
            _balls[c] = new Ball();

            Vector2 ballPosition = new Vector2(c + 50 * _Scale, c + 65 * _Scale);
            float ballSpeed = 100 * _Scale;
            Vector2 ballDirection = new Vector2(-1, -1);
            Vector2 ballDimensions = new Vector2(_BallWidthAndHeight, _BallWidthAndHeight);
            _balls[c].Initialize(ballPosition, ballSpeed, ballDirection, ballDimensions, _playAreaBoundingBox);
        }

        _paddleLeft = new Paddle();
        _paddleRight = new Paddle();

        Vector2 paddleLeftPosition = new Vector2(10 * _Scale, 75 * _Scale);
        Vector2 paddleRightPosition = new Vector2(210 * _Scale, 75 * _Scale);
        float paddleSpeed = 100 * _Scale;
        Vector2 paddleDimensions = new Vector2(_PaddleWidth, _PaddleHeight);

        _paddleLeft.Initialize(paddleLeftPosition, paddleSpeed, paddleDimensions, _playAreaBoundingBox);
        _paddleRight.Initialize(paddleRightPosition, paddleSpeed, paddleDimensions, _playAreaBoundingBox);

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
    }

    protected override void Update(GameTime gameTime)
    {
        #region keyboard input to move paddles
        KeyboardState kbState = Keyboard.GetState();
        if (kbState.IsKeyDown(Keys.W))
        {
            _paddleLeft.Direction = new Vector2(0, -1);
        }
        else if (kbState.IsKeyDown(Keys.S))
        {
            _paddleLeft.Direction = new Vector2(0, 1);
        }
        else
        {
            _paddleLeft.Direction = new Vector2(0, 0);
        }

        if (kbState.IsKeyDown(Keys.Up))
        {
            _paddleRight.Direction = new Vector2(0, -1);
        }
        else if (kbState.IsKeyDown(Keys.Down))
        {
            _paddleRight.Direction = new Vector2(0, 1);
        }
        else
        {
            _paddleRight.Direction = new Vector2(0, 0);
        }
        #endregion
        foreach (Ball ball in _balls)
        {
            ball.Update(gameTime);
        }
        _paddleLeft.Update(gameTime);
        _paddleRight.Update(gameTime);
        foreach(Ball ball in _balls)
        {
            ball.ProcessCollision(_paddleLeft.BoundingBox);
            ball.ProcessCollision(_paddleRight.BoundingBox);
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

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}