using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WiimoteLib;
using System.Windows.Forms;

namespace Marathon
{
    class Labyrinth : MiniGame
    {
        private Timer timer;

        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private ContentManager content;

        private Texture2D startSprite;
        private Texture2D endSprite;
        private Texture2D ballSprite;
        private Texture2D wallSprite;
        private Texture2D simpleTexture;

        private float x, y;
        private int fps;
        private int state;
        private int currentMatrice;
        private static int nbLabyrinth = 3;
        private static int labyrinthSize = 20;

        private Random rand = new Random();

        enum S
        {
            Wall, Empty, Start, End
        };

        private readonly S[, ,] matrice = new S[,,] { 
            {                                                                                     //10
                {S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Start, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall ,S.Wall , S.Wall },
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
       /*10*/   {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.End  ,S.End  , S.End  },
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.End  ,S.End  , S.End  }
            }, 
            {                                                                                     //10
                {S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.End  , S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall ,S.Wall , S.Wall },
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
       /*10*/   {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Start, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Empty,S.Empty, S.Empty}
            },
            {                                                                                     //10
                {S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.End  , S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall ,S.Wall , S.Wall },
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
       /*10*/   {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Wall , S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Empty, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Empty,S.Start, S.Empty},
                {S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Empty, S.Wall , S.Wall , S.Wall , S.Wall , S.Empty,S.Empty, S.Empty}
            }
        };

        public Labyrinth(GamePanel panel, Wiimote wm) : base(panel, wm)
        {
            timer = new Timer { Interval = 10 };
            timer.Tick += TimerClock;
        }
        private void TimerClock(object sender, EventArgs e)
        {            
            if (Width > 0)
                CheckMatrice();
            UpdateView();
        }
        protected override void Initialize()
        {
            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("SpriteFont1");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            startSprite = content.Load<Texture2D>("human");
            endSprite   = content.Load<Texture2D>("end");
            ballSprite  = content.Load<Texture2D>("ball");
            wallSprite  = content.Load<Texture2D>("wall");

            simpleTexture = new Texture2D(GraphicsDevice, 2, 2);
            simpleTexture.SetData(new[] { Color.Black, Color.Black, Color.Black, Color.Black });
            //simpleTexture.SetData(new[] { 0xFFFFFF }, 0, simpleTexture.Width * simpleTexture.Height);
        }
        
        public override void Start()
        {
            x = 0;
            y = 0;
            fps = 0;
            state = 0;

            currentMatrice = 0;//rand.Next(nbLabyrinth);
            UpdateView();
            timer.Start();
        }
        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);

            var width = Width / labyrinthSize;
            var height = Height / labyrinthSize;

            string text;
            switch (state)
            {
                case 0:
                    spriteBatch.Draw(endSprite, new Rectangle((int)x, (int)y, 4, 4), Color.Black);
                    break;
                case 2:
                    text = "You loose!";
                    spriteBatch.DrawString(font, text, new Vector2(Width / 2, Height / 2) - (font.MeasureString(text) / 2), Color.RoyalBlue); 
                    break;
                case 3:
                    text = "You win!";
                    spriteBatch.DrawString(font, text, new Vector2(Width / 2, Height / 2) - (font.MeasureString(text) / 2), Color.RoyalBlue); 
                    break;
            }

            if (state != 2 && state != 3)
            {
                for (var i = 0; i < labyrinthSize; i++)
                {
                    for (var j = 0; j < labyrinthSize; j++)
                    {
                        var xPos = width * j;
                        var yPos = height * i;

                        switch (matrice[currentMatrice, i, j])
                        {
                            case S.Empty:

                                break;
                            case S.Wall:
                                spriteBatch.Draw(wallSprite, new Rectangle(xPos, yPos, width, height), Color.White);
                                break;
                            case S.Start:
                                switch (state)
                                {
                                    case 0:
                                        spriteBatch.Draw(startSprite, new Rectangle(xPos, yPos, width, height), Color.White);
                                        break;
                                    case 1:
                                        spriteBatch.Draw(startSprite, new Rectangle((int)x, (int)y, width, height), Color.White);
                                        break;
                                }
                                break;
                            case S.End:
                                spriteBatch.Draw(endSprite, new Rectangle(xPos, yPos, width, height), Color.White);
                                break;
                        }
                    }
                }
            }


            spriteBatch.End();
        }

        internal override void WiimoteChanged(WiimoteState ws)
        {
            x = CalibrateX(ws.IRState.Midpoint.X);
            y = CalibrateY(ws.IRState.Midpoint.Y);

            if (fps++ == 1)
            {
                fps = 0;
                

                UpdateView();
            }
        }

        public void CheckMatrice()
        {
            var xPos = (int)(x * labyrinthSize / Width) % labyrinthSize;
            var yPos = (int)(y * labyrinthSize / Height) % labyrinthSize;

            //Console.WriteLine("position x:" + xPos + " y:" + yPos + "state:" + matrice[currentMatrice, yPos, xPos] + " width:" + Width + " height:" + Height);

            if (state == 0 && matrice[currentMatrice,yPos,xPos] == S.Start)
            {
                state = 1;
            }
            else if (state == 1 && matrice[currentMatrice, yPos, xPos] == S.Wall)
            {
                state = 2;
                timer.Stop();
                GamePanel.GameEnded(false);
            }
            else if (state == 1 && matrice[currentMatrice, yPos, xPos] == S.End)
            {
                state = 3;
                timer.Stop();
                GamePanel.GameEnded(true);
            }

            UpdateView();
        }
    }
}
