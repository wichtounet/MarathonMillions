using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WiimoteLib;
using Color = Microsoft.Xna.Framework.Color;

namespace Marathon
{
    class Moves : MiniGame
    {
        private readonly Timer timer;

        private float distanceRawX;
        private float distanceRawY;
        private float distanceRawZ;

        private State state = State.Started;
        private bool ok;

        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private ContentManager content;

        private enum State
        {
            Started,
            Finished
        }

        private new enum Move
        {
            Up, 
            Down, 
            Left,
            Right
        }

        private int current;

        private readonly Move[] moves = new[]
        {
            Move.Up,Move.Down, Move.Right, Move.Left, Move.Down, Move.Up
        };
        
        public Moves(GamePanel panel, Wiimote wm) : base(panel, wm)
        {
            timer = new Timer {Interval = 5000};
            timer.Tick += TimerClock;
        }

        protected override void Initialize()
        {
            current = 0;

            content = new ContentManager(Services, "Content");
            font = content.Load<SpriteFont>("SpriteFont1");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Start()
        {
            state = State.Started;

            timer.Start();

            UpdateView();

            distanceRawX = distanceRawY = distanceRawZ = 0;
        }

        private void TimerClock(object sender, EventArgs e)
        {
            Console.WriteLine("distance Raw X " + distanceRawX);
            Console.WriteLine("distance Raw Y " + distanceRawY);
            Console.WriteLine("distance Raw Z " + distanceRawZ);

            distanceRawX = distanceRawY = distanceRawZ = 0;

            switch (moves[current])
            {
                case Move.Up:
                    break;
                case Move.Down:
                    break;
                case Move.Left:
                    break;
                case Move.Right:
                    break;
            }

            current++;

            if(current == moves.Length - 1)
            {
                ok = true;

                timer.Stop();

                state = State.Finished;

                GamePanel.GameEnded(true);
            }
            
            UpdateView();
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
            
            if(state == State.Started)
            {
                var currentMove = moves[current];

                switch (currentMove)
                {
                    case Move.Up:
                        spriteBatch.DrawString(font, "UP", new Vector2(Width / 3, Height / 2 - 50), Color.Black);
                        break;
                    case Move.Down:
                        spriteBatch.DrawString(font, "DOWN", new Vector2(Width / 3, Height / 2 - 50), Color.Black);
                        break;
                    case Move.Left:
                        spriteBatch.DrawString(font, "LEFT", new Vector2(Width / 3, Height / 2 - 50), Color.Black);
                        break;
                    case Move.Right:
                        spriteBatch.DrawString(font, "RIGHT", new Vector2(Width / 3, Height / 2 - 50), Color.Black);
                        break;
                }
            } 
            else if(state == State.Finished)
            {
                spriteBatch.DrawString(font, ok ? "Congratulations" : "Game Over", new Vector2(Width / 3, Height/2 - 50), Color.Black);
            }

            spriteBatch.End();
        }

        internal override void WiimoteChanged(WiimoteState ws)
        {
            var accel = ws.AccelState;

            distanceRawX += (accel.RawValues.X - 127);
            distanceRawY += (accel.RawValues.Y - 104);
            distanceRawZ += (accel.RawValues.Z - 129);
        }
    }
}
