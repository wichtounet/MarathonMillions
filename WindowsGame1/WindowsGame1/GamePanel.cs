using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using WiimoteLib;

namespace Marathon
{
    public class GamePanel : Panel
    {
        private readonly Wiimote wm;

        private readonly MiniGame[] games;
        private readonly Random random = new Random();

        private int current;

        private int currentLed;

        private readonly Timer startTimer;

        public GamePanel(Wiimote wm)
        {
            this.wm = wm;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            Layout += GameLayout;

            new ClickMe(this, wm);

            games = new MiniGame[]
            {
                //new Labyrinthe(this, wm),
                new ClickMe(this, wm), 
                //new Run(this, wm), 
                //new Moves(this, wm)
            };

            Layout += GameLayout;

            

            startTimer = new Timer {Interval = 3000};
            startTimer.Tick += StartGame;
            startTimer.Start();
        }

        private void StartGame(object sender, EventArgs e)
        {
            startTimer.Stop();

            Console.WriteLine("Stop game " + current);

            if(Controls.Contains(games[current]))
            {
                Controls.Remove(games[current]);
            }

            wm.SetLEDs(true, false, false, false);

            current = random.Next(games.Length);

            current = 0;

            Console.WriteLine("Start game " + current);

            Controls.Add(games[current]);

            games[current].Start();
        }

        public void GameEnded(bool success)
        {
            Console.WriteLine("Game ended " + success);

            if(success)
            {
                ScoreManager.GetInstance().AddMultiplier(0.1);
            } else
            {
                ScoreManager.GetInstance().AddMultiplier(-0.05);
            }

            startTimer.Start();
        }

        public void PimpLed() 
        {
            wm.SetLEDs(currentLed == 0, currentLed == 1, currentLed == 2, currentLed == 3);

            currentLed = (currentLed < 4) ? ++currentLed : 0;
        }

        void GameLayout(object sender, LayoutEventArgs e)
        {
            games[current].SetBounds(0, 0, Width, Height);
        }

        public void WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            games[current].WiimoteChanged(args.WiimoteState);
        }
    }
}
