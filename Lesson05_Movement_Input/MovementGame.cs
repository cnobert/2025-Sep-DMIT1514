using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson05_Movement_Input;

public class MovementGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _arial;
    private string _message = "Move me with the arrows!";
    private Vector2 _messagePosition = new Vector2(100, 100);
    private float _messageSpeed = 200f;
    public MovementGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _arial = Content.Load<SpriteFont>("SystemArialFont");
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState kbState = Keyboard.GetState();
        float deltaSeconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

        Vector2 direction = Vector2.Zero;

        //input - vertical movement
        if (kbState.IsKeyDown(Keys.Up))
        {
            direction += new Vector2(0, -1);
        }
        else if (kbState.IsKeyDown(Keys.Down))
        {
            direction += new Vector2(0, 1);
        }
        //input - horizontal movement
        if (kbState.IsKeyDown(Keys.Left))
        {
            direction += new Vector2(-1, 0);
        }
        else if (kbState.IsKeyDown(Keys.Right))
        {
            direction += new Vector2(1, 0);
        }
        _messagePosition += direction * _messageSpeed * deltaSeconds;

        if (_messagePosition.X < 0f)
        {
            _messagePosition.X = 0f;
        }
        //if for going off the right side

        if (_messagePosition.Y < 0f)
        {
            _messagePosition.Y = 0f;
        }
        //if for going off the bottom of the screen
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Brown);

        _spriteBatch.Begin();

        _spriteBatch.DrawString(_arial, _message, _messagePosition, Color.Wheat);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
