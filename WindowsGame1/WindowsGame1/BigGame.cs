using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WiimoteLib;
using Microsoft.Xna.Framework.Content;
using System.Speech.Recognition;

namespace Marathon
{
    public class BigGame : MiniGame
    {
        private const float RectangleSize = 25;

        private readonly Random rnd = new Random();
        private readonly Timer timer;
        private readonly Timer viewTimer;

        private float x;
        private float y;
        private System.Drawing.PointF recPos = new System.Drawing.PointF(50, 50);

        private int time;
        private int win;
        private bool won;

        SpriteBatch spriteBatch;
        Texture2D marmotteSprite;
        SpriteFont font;
        ContentManager content;
        Texture2D t;

        private int audioLevel;

        public BigGame(GamePanel panel, Wiimote wm)
            : base(panel, wm)
        {
            Wm.SetRumble(false);

            timer = new Timer { Interval = 1000 };
            timer.Tick += TimerClock;

            viewTimer = new Timer { Interval = 15 };
            viewTimer.Tick += ViewTimerClock;
        }

        internal override void AudioLevelUpdated(AudioLevelUpdatedEventArgs e)
        {
            audioLevel = e.AudioLevel;
        }

        protected override void Initialize()
        {
            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("SpriteFont1");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            marmotteSprite = content.Load<Texture2D>("marmotte");

            font = content.Load<SpriteFont>("SpriteFont1");

            t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData(new[] { Color.Red });
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);

            spriteBatch.Draw(t, new Rectangle((int)x, (int)y, 10, 10), Color.White);

            if (timer.Enabled)
            {
                spriteBatch.Draw(marmotteSprite, new Rectangle((int)recPos.X, (int)recPos.Y, (int)Width / 20, (int)Width / 20), Color.White);
            }
            else
            {
                if (won)
                {
                    spriteBatch.DrawString(font, "GAME WON!", new Vector2(20, 50), Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(font, "GAME OVER!", new Vector2(20, 50), Color.Black);
                }
            }

            spriteBatch.Draw(t, new Rectangle((int)x, (int)y, 5, 5), Color.White);

            spriteBatch.End();
        }

        private void ViewTimerClock(object sender, EventArgs e)
        {
            Invalidate();
        }

        public override void Start()
        {
            Console.WriteLine("Big game started");

            time = 20;
            win = 0;

            won = false;

            x = 0;
            y = 0;

            recPos = new System.Drawing.PointF(50, 50);

            timer.Start();
            viewTimer.Start();
        }

        public override void Stop()
        {
            timer.Stop();
            viewTimer.Stop();
        }

        private void TimerClock(object obj, EventArgs ea)
        {
            if (time == 0)
            {
                won = false;
                GamePanel.BigGameEnded(false);
                Stop();
            }

            if (won)
            {
                GamePanel.BigGameEnded(true);
                Stop();
            }

            if (Wm.WiimoteState.Rumble)
            {
                Wm.SetRumble(false);
            }

            UpdateView();

            time--;
        }

        internal override void WiimoteChanged(WiimoteState ws)
        {
            x = CalibrateX(ws.IRState.Midpoint.X);
            y = CalibrateY(ws.IRState.Midpoint.Y);

            if (timer.Enabled)
            {
                if (x >= recPos.X && x <= recPos.X + RectangleSize && y >= recPos.Y && y <= recPos.Y + RectangleSize && audioLevel > 60)
                {
                    win++;

                    if (win == 10)
                    {
                        won = true;
                    }
                    else
                    {
                        Wm.SetRumble(true);
                    }

                    var randx = (float)(rnd.NextDouble() * Width) - RectangleSize;
                    var randy = (float)(rnd.NextDouble() * Height) - RectangleSize;

                    recPos.X = randx > 0 ? randx : 0;
                    recPos.Y = randy > 0 ? randy : 0;
                }
            }
        }
    }
}
