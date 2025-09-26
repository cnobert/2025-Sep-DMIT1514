using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson06_BrickBreaker01;

public class BrickBreaker : Game
{
    private const int _WindowWidth = 800, _WindowHeight = 600;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    //Paddle
    private Vector2 _paddlePosition;
    private float _paddleSpeed = 400.0f;
    private const int _PaddleWidth = 120, _PaddleHeight = 18;

    //Ball
    private Vector2 _ballPosition;
    private Vector2 _ballVelocity;
    private bool _ballLaunched = false; //this is our first game object state
    private const int _BallSize = 16;
    private const float _BallSpeed = 500;

    //read-only, derived property
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
        _ballLaunched = false;
        //first give it direction only
        _ballVelocity = new Vector2(0.9f, -1.0f);
        //now give it a magnitude (speed)
        _ballVelocity *= _BallSpeed;

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
            //left and right walls
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

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
