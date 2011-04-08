using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WiimoteLib;

namespace Marathon
{
    class Run : MiniGame
    {
        private float ComputerSpeed = 3.0f;
        private float StopLine = 320.0f;

        private readonly Timer timer;

        private int fps;

        private float distance;
        private float speed;
        private int count;

        private float xComputer;
        private float xPlayer;
        
        private State state = State.Starting;
        private int startingTime;

        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private ContentManager content;
        private Texture2D humanSprite;
        private Texture2D ballSprite;
        private Texture2D simpleTexture;
        private Texture2D background;

        private enum State
        {
            Starting,
            Started,
            Finished
        }

        public Run(GamePanel panel, Wiimote wm) : base(panel, wm)
        {
            timer = new Timer { Interval = 100 };
            timer.Tick += TimerClock;
        }

        protected override void Initialize()
        {
            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("SpriteFont1");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            humanSprite = content.Load<Texture2D>("greenCar");
            ballSprite = content.Load<Texture2D>("redCar");
            background = content.Load<Texture2D>("road");

            simpleTexture = new Texture2D(GraphicsDevice, 1, 1);
            simpleTexture.SetData(new[] { Color.Black });
        }

        public override void Start()
        {
            fps = 0;
            speed = 0;
            count = 0;

            state = State.Starting;

            startingTime = 0;

            distance = 0;

            xComputer = 0;
            xPlayer = 0;

            timer.Start();
        }

        public override void Stop()
        {
            timer.Stop();
        }

        private void TimerClock(object sender, EventArgs e){

            StopLine = Width - 100;
            ComputerSpeed = StopLine / 100;

            if(state == State.Starting)
            {
                startingTime += 1;

                if(startingTime == 30)
                {
                    state = State.Started;
                }
            } 
            else
            {
                xComputer += ComputerSpeed;
                xPlayer += (speed* StopLine/320);

                if (xComputer >= StopLine)
                {
                    timer.Stop();

                    state = State.Finished;

                    GamePanel.GameEnded(false);
                }
                else if (xPlayer >= StopLine)
                {
                    timer.Stop();

                    state = State.Finished;

                    GamePanel.GameEnded(true);
                }

                UpdateView();
            }
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(); //SpriteSortMode.BackToFront, BlendState.NonPremultiplied

            spriteBatch.Draw(background, new Rectangle(0, 0, Width, Height), Color.White);
            
            if (state == State.Starting)
            {
                string sTime = "" + (3 - startingTime / 10);
                spriteBatch.DrawString(font, sTime, new Vector2(Width / 2, Height / 2) - (font.MeasureString(sTime) / 2), Color.Black);
            }
            else if (state == State.Started)
            {
                spriteBatch.Draw(ballSprite, new Rectangle((int)xComputer, (int)(Height * 0.598) - ballSprite.Height / 2, ballSprite.Width, ballSprite.Height), Color.White);
                spriteBatch.Draw(humanSprite, new Rectangle((int)xPlayer, (int)(Height * 0.854) - humanSprite.Height / 2, humanSprite.Width, humanSprite.Height), Color.White);

                //spriteBatch.Draw(simpleTexture, new Rectangle(0, Height / 2, (int) StopLine, 2), Color.White);
                spriteBatch.Draw(simpleTexture, new Rectangle((int) StopLine, 0, 2, Height), Color.White);
            }
            else if (state == State.Finished)
            {
                if (xComputer >= StopLine)
                {
                    string text = "Computer wins";
                    spriteBatch.DrawString(font, text, new Vector2(Width / 2, Height / 2) - (font.MeasureString(text) / 2), Color.Red);
                }
                else if (xPlayer >= StopLine)
                {
                    string text = "Player wins";
                    spriteBatch.DrawString(font, text, new Vector2(Width / 2, Height / 2) - (font.MeasureString(text) / 2), Color.Black);
                }
            }

            spriteBatch.End();
        }

        internal override void WiimoteChanged(WiimoteState ws)
        {
            var state = ws.AccelState;

            if(0 > state.Values.Y)
            {
                distance += 0 - state.Values.Y;
            } else
            {
                distance += state.Values.Y - 0;
            }

            if(count++ == 20)
            {
                speed = distance / 20;
                distance = 0;

                count = 0;
            }

            if (fps++ == 10)
            {
                fps = 0;

                UpdateView();
            }
        }
    }
}
