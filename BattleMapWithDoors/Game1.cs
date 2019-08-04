using System.Collections.Generic;
using System.Security.Policy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;

namespace BattleMap
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics { get; }
        private SpriteBatch _spriteBatch;


        private TiledMap _tiledMap;
        private TiledMapRenderer _mapRenderer;

        // Alle Variablen für die linken und rechten Türen. Die linke Tür an Position 0 passt 
        // zur rechten Tür an position 0.
        private List<Rectangle> _leftDoorPosition = new List<Rectangle>();
        private Texture2D _leftDoorTextureClosed;
        private Texture2D _leftDoorTextureOpened;
        private List<bool> _leftDoorClosed = new List<bool>();

        private List<Rectangle> _rightDoorPosition = new List<Rectangle>();
        private Texture2D _rightDoorTextureClosed;
        private Texture2D _rightDoorTextureOpened;
        private List<bool> _rightDoorClosed = new List<bool>();

        // Das 2d-array: Wenn ein Feld true ist, ist diese Position gesperrt, also kollidierbar.
        private bool[,] _blocked;

        // _rectanglePlayer wird benötigt, damit die Kollision mehr Sinn ergibt. Man kollidiert ja 
        // nicht mit einem Punkt der Figur, sondern mit einer "Box".
        private Texture2D _player;
        private Rectangle _rectanglePlayer;

        // Wird für die Steuerung benutzt.
        private MouseState mMouseState;


        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            base.Initialize();
            _mapRenderer = new TiledMapRenderer(GraphicsDevice);

            // Erstellt das 2d-Array. Man muss z.b. _tiledMap.Width * 32 berechnen, da Width die Kachelbreite der TiledMaps
            // zurück gibt, aber eine Kachel ist 32x32 Pixel groß, deshalb "* 32" um die tatsächliche Breite berrechnen zu können.
            var collisionLayer = _tiledMap.GetLayer<TiledMapObjectLayer>("CollisionObjects");
            _blocked = new bool[_tiledMap.Width * 32, _tiledMap.Height * 32];
            // Berechnung des 2D Arrays ist aufwendig, aber ich denke für das weitere Spielgeschehen einfacher, da man
            // zur Collisionskontrolle nur eine einfache Abfrage machen muss.
            foreach (var collLayer in collisionLayer.Objects)
            {
                for (int x = (int)collLayer.Position.X; x <= (int)collLayer.Size.Width + collLayer.Position.X; x++)
                {
                    for (int y = (int)collLayer.Position.Y; y <= (int)collLayer.Size.Height + collLayer.Position.Y; y++)
                    {
                        _blocked[x, y] = true;
                    }
                }
            }
            // Neben dem "CollisionLayer" habe ich in der TiledMap noch einen ObjectsLayer. 
            // Mit dem kann man Punkte/ Gebiete direkt auf der TiledMap festlegen, die man nicht sieht und nicht für die Kollision
            // benötigt werden, aber sehr Vorteilhaft sind z.b. Spawnpunkte oder vielleicht
            // die spätere Endzone für alle Charaktere um die Map zu beenden.
            var objectLayer = _tiledMap.GetLayer<TiledMapObjectLayer>(layerName: "Objects");
            objectLayer.IsVisible = false;
            foreach (var i in objectLayer.Objects)
            {
                if (i.Name == "Spawn")
                {
                    _rectanglePlayer.X = (int)i.Position.X;
                    _rectanglePlayer.Y = (int)i.Position.Y;
                    _rectanglePlayer.Height = _player.Height / 5;
                    _rectanglePlayer.Width = _player.Width / 5;
                }
            }
            // Es gibt zwei weitere ObjektLayer, nämlich einen für alle linken Türen und eine für die rechten.
            // Wichtig ist, dass beim erstellen der Layer in Tiled die Paare an Türen in den jeweiligen Objektlayern an der gleichen
            // Stelle stehen.
            var doorLayer = _tiledMap.GetLayer<TiledMapObjectLayer>("LeftDoors");
            doorLayer.IsVisible = false;
            var position = 0;
            foreach (var i in doorLayer.Objects)
            {
                _leftDoorPosition.Insert(position, new Rectangle((int)i.Position.X, (int)i.Position.Y, (int)i.Size.Width, (int)i.Size.Height));
                _leftDoorClosed.Insert(position, true);
                position++;
            }
            doorLayer = _tiledMap.GetLayer<TiledMapObjectLayer>("RightDoors");
            doorLayer.IsVisible = false;
            position = 0;
            foreach (var i in doorLayer.Objects)
            {
                _rightDoorPosition.Insert(position, new Rectangle((int)i.Position.X, (int)i.Position.Y, (int)i.Size.Width, (int)i.Size.Height));
                _rightDoorClosed.Insert(position, true);
                position++;
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(graphicsDevice: GraphicsDevice);
            _tiledMap = Content.Load<TiledMap>(assetName: "BattleMap1");
            _player = Content.Load<Texture2D>(assetName: "Player1");

            _leftDoorTextureClosed = Content.Load<Texture2D>("LeftDoor");
            _rightDoorTextureClosed = Content.Load<Texture2D>("RightDoor");
            _leftDoorTextureOpened = Content.Load<Texture2D>("LeftDoorOpened");
            _rightDoorTextureOpened = Content.Load<Texture2D>("RightDoorOpened");
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _mapRenderer.Update(_tiledMap, gameTime);
            KeyboardUpdate(gameTime);
            MouseUpdate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            _mapRenderer.Draw(_tiledMap);
            _spriteBatch.Draw(_player, _rectanglePlayer, Color.White);
            // Zeichnet je nachdem, ob die Türen offen oder geschlossen sind, jeweils die passende Textur.
            for (int i = 0; i < _leftDoorPosition.Count; i++)
            {
                if (_leftDoorClosed[i])
                {
                    _spriteBatch.Draw(_leftDoorTextureClosed, _leftDoorPosition[i], Color.White);
                }
                else
                {
                    _spriteBatch.Draw(_leftDoorTextureOpened, _leftDoorPosition[i], Color.White);
                }
            }
            for (int i = 0; i < _rightDoorPosition.Count; i++)
            {
                if (_rightDoorClosed[i])
                {
                    _spriteBatch.Draw(_rightDoorTextureClosed, _rightDoorPosition[i], Color.White);
                }
                else
                {
                    _spriteBatch.Draw(_rightDoorTextureOpened, _rightDoorPosition[i], Color.White);
                }
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void MouseUpdate()
        {

            MouseState newMouseState = Mouse.GetState();
            if ((newMouseState.LeftButton == ButtonState.Pressed) &&
                (mMouseState.LeftButton == ButtonState.Released))
            {
                Rectangle mouseRectangle = new Rectangle(mMouseState.X, mMouseState.Y, mMouseState.Y, mMouseState.X);
                for (int i = 0; i < _leftDoorPosition.Count; i++)
                {
                    if (_leftDoorPosition[i].Intersects(mouseRectangle))
                    {
                        if (_leftDoorClosed[i])
                        {
                            _leftDoorClosed[i] = false;
                        }
                        else
                        {
                            _leftDoorClosed[i] = true;
                        }
                    }

                    else if (_rightDoorPosition[i].Intersects(mouseRectangle))
                    {
                        if (_rightDoorClosed[i])
                        {
                            _rightDoorClosed[i] = false;
                        }
                        else
                        {
                            _rightDoorClosed[i] = true;
                        }
                    }
                }
            }
            mMouseState = newMouseState;
        }

        bool CheckForDoorCollision(Rectangle player)
        {
            for (int i = 0; i < _leftDoorPosition.Count; i++)
            {
                if (_leftDoorPosition[i].Intersects(player))
                {
                    if (_leftDoorClosed[i])
                    {
                        return true;
                    }
                }

                if (_rightDoorPosition[i].Intersects(player))
                {
                    if (_rightDoorClosed[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private void KeyboardUpdate(GameTime gameTime)
        {
            var newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.Up))
            {
                // Jenachdem in welche Richtung man läuft, werden unterschiedliche Punkte von dem Charakter
                // als Kollisionspunkt gesehen z.b. wenn man nach links läuft, soll er mit seiner linken Seite
                // an der Mauer kollidieren, aber wenn er nach rechts läuft mit der rechten Seite.

                if (_blocked[_rectanglePlayer.X, _rectanglePlayer.Height + _rectanglePlayer.Y - 3] is false)
                {
                    _rectanglePlayer.Y -= 3;
                    if (CheckForDoorCollision(_rectanglePlayer))
                    {
                        _rectanglePlayer.Y += 3;
                    }
                }
            }
            if (newKeyboardState.IsKeyDown(Keys.Down))
            {
                if (_blocked[_rectanglePlayer.X, _rectanglePlayer.Height + _rectanglePlayer.Y + 3] is false)
                {
                    _rectanglePlayer.Y += 3;
                    if (CheckForDoorCollision(_rectanglePlayer))
                    {
                        _rectanglePlayer.Y -= 3;
                    }
                }
            }
            if (newKeyboardState.IsKeyDown(Keys.Left))
            {
                if (_blocked[_rectanglePlayer.X - 3, _rectanglePlayer.Height + _rectanglePlayer.Y] is false)
                {
                    _rectanglePlayer.X -= 3;
                    if (CheckForDoorCollision(_rectanglePlayer))
                    {
                        _rectanglePlayer.Y += 3;
                    }
                }
            }
            if (newKeyboardState.IsKeyDown(Keys.Right))
            {
                if (_blocked[_rectanglePlayer.X + 3 + _rectanglePlayer.Width, _rectanglePlayer.Height + _rectanglePlayer.Y + 3] is false)
                {
                    _rectanglePlayer.X += 3;
                    if (CheckForDoorCollision(_rectanglePlayer))
                    {
                        _rectanglePlayer.Y -= 3;
                    }
                }
            }
        }
    }
}
