using System;
using System.Drawing;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;

namespace Marathon
{
    public class QuestionPanel : Panel
    {
        private readonly SpeechSynthesizer synthesizer;

        private State state = State.Started;
        private int goodAnswer;
        private int userAnswer = -1;
        private int countDown;
        private int wait;

        private String title, answer1, answer2, answer3, answer4;

        private readonly Timer timer;

        private enum State
        {
            Started,
            QuestionAsked, 
            WaitConfirmation,
            Wait,
            Fail,
            Win,
            BigGame
        }

        private readonly String[][][] questions = new[]
        {
            new[]{ //Level 0
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 1
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 2
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 3
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 4
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 5
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 6
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 7
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 8
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 9
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 10
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 11
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 12
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            },
            new[]{ //Level 13
                    new[] {"Qu'est-ce qui tourne autour du soleil ?", "La terre", "La lune", "Un haricot", "Le soleil", "1"},
                    new[] {"Qui est Thor ? ", "Un dieu viking", "Un schtroumpf", "Un haricot", "Un hobbit", "1"},
                    new[] {"Qui sont les vikings ? ", "Des grands malades", "Des schtroumpfs", "Des haricots", "Des hobbits", "1"},
                    new[] {"Que mettre sur les patates ? ", "Mayo, aromat, beurre", "Du ketchup", "Des haricots", "Des frites", "1"}
            } 
        };

        private int current;
        private readonly int[] scores = new[] {800, 1500, 3000, 6000, 12000, 24000, 48000, 72000, 100000, 150000, 300000, 1000000, 2500000, 5000000};
        private readonly GamePanel gamePanel;

        public QuestionPanel(GamePanel gamePanel)
        {
            this.gamePanel = gamePanel;
            Paint += PaintView;

            synthesizer = new SpeechSynthesizer();

            timer = new Timer();
            timer.Tick += TimerClock;
            timer.Interval = 1000;
        }

        public void Start()
        {
            timer.Start();
        }

        private void TimerClock(object obj, EventArgs ea)
        {
            switch (state)
            {
                case State.Started:
                    AskRandom();

                    break;
                case State.QuestionAsked:
                case State.WaitConfirmation:
                    countDown--;

                    if(countDown > 0 && countDown <= 10)
                    {
                        synthesizer.SpeakAsync("" + countDown);
                    }

                    if(countDown == 0)
                    {
                        synthesizer.SpeakAsync("Game over !");

                        state = State.Fail;

                        gamePanel.TerminateGame();
                        
                        timer.Stop();
                    }

                    Refresh();

                    gamePanel.PimpLed();

                    break;
                case State.Wait:
                    wait--;

                    if(wait == 0)
                    {
                        AskRandom();
                    }

                    break;
            }
        }

        private void AskRandom()
        {
            var r = new Random().Next(0, questions[current].Length - 1);

            AskQuestion(questions[current][r][0], questions[current][r][1], questions[current][r][2], questions[current][r][3], questions[current][r][4],
                        Convert.ToInt16(questions[current][r][5]));
        }

        private void PaintView(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.Clear(Color.White);

            if (state == State.Started)
            {
                return;
            }

            if (state == State.Fail)
            {
                g.DrawString("Game over", new Font("Verdana", 32), new SolidBrush(Color.Black), 5, 5);

                return;
            }

            if (state == State.Win)
            {
                g.DrawString("Congratulations", new Font("Verdana", 32), new SolidBrush(Color.Black), 5, 5);

                return;
            }

            if (state == State.BigGame)
            {
                g.DrawString("Play the right game", new Font("Verdana", 32), new SolidBrush(Color.Black), 5, 5);

                return;
            }

            if(state == State.Wait)
            {
                g.DrawString("...", new Font("Verdana", 32), new SolidBrush(Color.Black), 5, 5);

                return;
            }

            g.DrawString(title, new Font("Verdana", 16), new SolidBrush(Color.Black), 5, 5);

            g.DrawString("1. " + answer1, new Font("Verdana", 15), new SolidBrush(userAnswer == 1 ? Color.Red : Color.Black), 20, 50);
            g.DrawString("2. " + answer2, new Font("Verdana", 15), new SolidBrush(userAnswer == 2 ? Color.Red : Color.Black), 20, 90);
            g.DrawString("3. " + answer3, new Font("Verdana", 15), new SolidBrush(userAnswer == 3 ? Color.Red : Color.Black), 20, 130);
            g.DrawString("4. " + answer4, new Font("Verdana", 15), new SolidBrush(userAnswer == 4 ? Color.Red : Color.Black), 20, 170);

            g.DrawString("" + countDown, new Font("Verdana", 20), new SolidBrush(Color.Red), 100, 250);
        }

        internal void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (state)
            {
                case State.QuestionAsked :
                    userAnswer = GetAnswer(e.Result.Text);
                    
                    if (userAnswer != -1)
                    {
                        state = State.WaitConfirmation;

                        Refresh();

                        synthesizer.SpeakAsync("Are you sure ?");
                    }

                    break;
                case State.WaitConfirmation :
                    var text = e.Result.Text;

                    if (text.Equals("Yes"))
                    {
                        if (userAnswer == goodAnswer)
                        {
                            synthesizer.SpeakAsync("Great job !");

                            ScoreManager.GetInstance().AddScore(scores[current]);

                            current++;

                            if(current == scores.Length)
                            {
                                state = State.Win;

                                gamePanel.TerminateGame();

                                timer.Stop();
                            } 
                            else if (current != 0 && current % 2 == 0)
                            {
                                state = State.Wait;
                                timer.Stop();

                                gamePanel.TerminateGame();
                                gamePanel.StartBigGame(this);

                                state = State.BigGame;
                            } else
                            {
                                wait = 5;
                                state = State.Wait;
                            }
                        } else
                        {
                            synthesizer.SpeakAsync("Game over !");

                            state = State.Fail;
                        }

                        Refresh();
                    }
                    else if (text.Equals("No"))
                    {
                        synthesizer.SpeakAsync("Give another answer");

                        state = State.QuestionAsked;
                    }

                    break;
            }
        }

        public void Restart()
        {
            wait = 5;
            state = State.Wait;

            timer.Start();

            Refresh();
        }

        private void AskQuestion(String title, String answer1, String answer2, String answer3, String answer4, int answer)
        {
            this.title = title;
            this.answer1 = answer1;
            this.answer2 = answer2;
            this.answer3 = answer3;
            this.answer4 = answer4;

            userAnswer = -1;
            goodAnswer = answer;
            state = State.QuestionAsked;
            countDown = 25;

            Refresh();

            synthesizer.SpeakAsync("Answer the question");
        }

        private static int GetAnswer(string text)
        {
            switch (text)
            {
                case "1": return 1;
                case "2": return 2;
                case "3": return 3;
                case "4": return 4;
            }

            return -1;
        }
    }
}
