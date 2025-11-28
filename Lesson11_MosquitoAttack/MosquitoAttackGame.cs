using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using SpriteFontPlus; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)
using System.IO; //ONLY CONRAD NEEDS THIS CODE (MACOS THINGS)


namespace Lesson11_MosquitoAttack;

public class MosquitoAttackGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int _WindowWidth = 550;
    private const int _WindowHeight = 400;

    private Texture2D _background;


    private enum State
    {
        Level01, Start, Paused, Over
    }
    private State _gameState;
    
    private Cannon _cannon;
    private Mosquito[] _mosquitoes;

    private Random _random;
    private SpriteFont _font;
    private string _message;

    public MosquitoAttackGame()
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

        _random = new Random();

        Rectangle gameBounds = new Rectangle(0, 0, _WindowWidth, _WindowHeight);

        _cannon = new Cannon();
        _cannon.Initialize(
            new Vector2(50, 325),
            gameBounds
        );

        _mosquitoes = new Mosquito[10];

        for (int i = 0; i < _mosquitoes.Length; i++)
        {
            Mosquito m = new Mosquito();

            float x = _random.Next(40, _WindowWidth - 47);
            float y = _random.Next(20, _WindowHeight - 200);
            Vector2 startPos = new Vector2(x, y);

            m.Initialize(startPos, gameBounds);

            float dirX = (float)(_random.NextDouble() * 2 - 1);
            float dirY = 0;
            Vector2 dir = new Vector2(dirX, dirY);

            dir.Normalize();

            m.Direction = dir;
            _mosquitoes[i] = m;
        }
        _gameState = State.Start;
        _message = "Welcome to Mosquito Attack. Press space to play.";
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _background = Content.Load<Texture2D>("Background");

        _cannon.LoadContent(Content);

        byte[] fontBytes = File.ReadAllBytes("Content/Tahoma.ttf");
        _font = TtfFontBaker.Bake(
                    fontBytes,
                    18,
                    1024,
                    1024,
                    new[] { CharacterRange.BasicLatin })
                .CreateSpriteFont(GraphicsDevice);

        foreach (Mosquito m in _mosquitoes)
        {
            m.LoadContent(Content);
        }
    }
    KeyboardState _kbPreviousState;
    KeyboardState _kbState;
    protected override void Update(GameTime gameTime)
    {
        _kbState = Keyboard.GetState();

        switch(_gameState)
        {
            
            case State.Level01:
                if(Pressed(Keys.P))
                {
                    _gameState = State.Paused;
                    _message = "Game paused. Press P to resume.";
                }
                if (_kbState.IsKeyDown(Keys.A))
                {
                    _cannon.Direction = new Vector2(-1, 0);
                }
                else if (_kbState.IsKeyDown(Keys.D))
                {
                    _cannon.Direction = new Vector2(1, 0);
                }
                else
                {
                    _cannon.Direction = Vector2.Zero;
                }

                if(_kbState.IsKeyDown(Keys.Space) && _kbPreviousState.IsKeyUp(Keys.Space))
                {
                    _cannon.Shoot();
                }
                _cannon.Update(gameTime);

                foreach (Mosquito mosquito in _mosquitoes)
                {
                    mosquito.Update(gameTime);
                    if(mosquito.IsAlive && _cannon.ProcessCollision(mosquito.BoundingBox))
                    {
                        mosquito.Die();
                    }
                    //!!!! count mosquitoes that are alive - if they're all gone, set the message appropriately and move to State.Over
                    //!!!! check if cannon is dead - if so, do the same
                }
                break;
            case State.Start:
                if(Pressed(Keys.Space))
                {
                    _gameState = State.Level01;
                }
                break;
            case State.Paused:
                if(Pressed(Keys.P))
                {
                    _gameState = State.Level01;
                }
                break;
            case State.Over:
                 if(Pressed(Keys.Space))
                {
                    _gameState = State.Level01;
                }
                break;

        }
        
        _kbPreviousState = _kbState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        switch(_gameState)
        {
            case State.Level01:
                _spriteBatch.Draw(_background, Vector2.Zero, Color.White);
                _cannon.Draw(_spriteBatch);

                foreach (Mosquito m in _mosquitoes)
                {
                    m.Draw(_spriteBatch);
                }
                break;
            case State.Start:
            case State.Paused:
            case State.Over:
                _spriteBatch.DrawString(_font, _message, Vector2.Zero, Color.White);
                break;

        }
        
       

        _spriteBatch.End();

        base.Draw(gameTime);
    }
    private bool Pressed(Keys key)
    {
        return _kbState.IsKeyDown(key) && _kbPreviousState.IsKeyUp(key);
    }
}