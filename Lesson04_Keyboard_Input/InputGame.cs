using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lesson04_Keyboard_Input;

public class InputGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _arial;
    private string _message = "Hi. It's unseasonably warm these days.";

    private KeyboardState _kbPreviousState;

    public InputGame()
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
        KeyboardState kbCurrentState = Keyboard.GetState();
        _message = "";

        #region arrow keys
        if (kbCurrentState.IsKeyDown(Keys.Down)) //Keys.Down = down arrow
        {
            _message += "Down";
        }
        if (kbCurrentState.IsKeyDown(Keys.Up)) //Keys.Up = up arrow
        {
            _message += "Up";
        }
        if (kbCurrentState.IsKeyDown(Keys.Left)) //Keys.Left = left arrow
        {
            _message += "Left";
        }
        if (kbCurrentState.IsKeyDown(Keys.Right)) //Keys.Right = right arrow
        {
            _message += "Right";
        }
        #endregion

        #region "key down" state

        if (_kbPreviousState.IsKeyUp(Keys.Space) && kbCurrentState.IsKeyDown(Keys.Space))
        {
            _message += "------------------------------------------------------\n";
            _message += "------------------------------------------------------\n";
            _message += "------------------------------------------------------\n";
            _message += "------------------------------------------------------\n";
            _message += "------------------------------------------------------\n";
        }
        #endregion
        #region "key hold" state
        else if (kbCurrentState.IsKeyDown(Keys.Space))
        {
            _message += "Space ";
        }
        #endregion
        #region "key up" state
        else if (_kbPreviousState.IsKeyDown(Keys.Space)) //last call to update, "space" was down
        {
            //"space" is not currently down
            _message += "------------------------------------------------------\n";
            _message += "------------------------------------------------------\n";
            _message += "------------------------------------------------------\n";
            _message += "------------------------------------------------------\n";
            _message += "------------------------------------------------------\n";
        }
        #endregion
        _kbPreviousState = kbCurrentState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Wheat);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(_arial, _message, Vector2.Zero, Color.Chocolate);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
