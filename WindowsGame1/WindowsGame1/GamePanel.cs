using System;
using System.Speech.Recognition;
using System.Windows.Forms;
using WiimoteLib;

namespace Marathon
{
    public class GamePanel : Panel
    {
        private readonly Wiimote wm;

        private readonly MiniGame[] games;
        private readonly MiniGame bigGame;
        private readonly Random random = new Random();

        private int current = -1;
        private bool big;

        private int currentLed;

        private readonly Timer startTimer;

        private QuestionPanel questionsPanel;

        public GamePanel(Wiimote wm)
        {
            this.wm = wm;
            
            Layout += GameLayout;

            bigGame = new BigGame(this, wm);

            games = new MiniGame[]
            {
                new Labyrinth(this, wm),
                new ClickMe(this, wm), 
                new Run(this, wm), 
                new Buttons(this, wm)
            };

            Layout += GameLayout;

            startTimer = new Timer {Interval = 3000};
            startTimer.Tick += StartGame;
            startTimer.Start();
        }

        private void StartGame(object sender, EventArgs e)
        {
            startTimer.Stop();

            if(current >= 0 && Controls.Contains(games[current]))
            {
                Controls.Remove(games[current]);
            }

            wm.SetLEDs(true, false, false, false);

            current = random.Next(games.Length);

            current = 1;
            
            Controls.Add(games[current]);

            games[current].Start();
        }

        public void GameEnded(bool success)
        {
            if(success)
            {
                ScoreManager.GetInstance().AddMultiplier(0.1);
            } else
            {
                ScoreManager.GetInstance().AddMultiplier(-0.05);
            }

            startTimer.Start();
        }

        public void TerminateGame()
        {
            games[current].Stop();

            if (current >= 0 && Controls.Contains(games[current]))
            {
                Controls.Remove(games[current]);
            }

            current = -1;
        }

        public void StartBigGame(QuestionPanel questionPanel)
        {
            questionsPanel = questionPanel;

            big = true;

            Controls.Add(bigGame);

            bigGame.Start();
        }

        public void BigGameEnded(bool success)
        {
            big = false;

            Controls.Remove(bigGame);

            if (success)
            {
                ScoreManager.GetInstance().AddMultiplier(0.2);
            }
            else
            {
                ScoreManager.GetInstance().AddMultiplier(-0.1);
            }

            questionsPanel.Restart();

            startTimer.Start();
        }

        public void PimpLed() 
        {
            wm.SetLEDs(currentLed == 0, currentLed == 1, currentLed == 2, currentLed == 3);

            currentLed = (currentLed < 4) ? ++currentLed : 0;
        }

        void GameLayout(object sender, LayoutEventArgs e)
        {
            if(current >= 0)
            {
                games[current].SetBounds(0, 0, Width, Height);
            }

            if(big)
            {
                bigGame.SetBounds(0, 0, Width, Height);
            }
        }

        public void WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            if(current >= 0)
            {
                games[current].WiimoteChanged(args.WiimoteState);
            }

            if (big)
            {
                bigGame.WiimoteChanged(args.WiimoteState);
            }
        }

        public void AudioLevelUpdated(AudioLevelUpdatedEventArgs e)
        {
            if (current >= 0)
            {
                games[current].AudioLevelUpdated(e);
            }

            if (big)
            {
                bigGame.AudioLevelUpdated(e);
            }
        }
    }
}