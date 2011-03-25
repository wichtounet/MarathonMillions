namespace Marathon
{
    class ScoreManager
    {
        private static readonly ScoreManager Instance = new ScoreManager();
        
        public delegate void ScoreChangedEventHandler(object sender, ScoreChangedArgs args);
        public event ScoreChangedEventHandler ScoreChanged;

        private ScoreManager()
        {
            Multiplier = 1;
        }

        public static ScoreManager GetInstance()
        {
            return Instance;
        }

        public int Score
        {
            get; private set;
        }

        public double Multiplier
        {
            get; private set;
        }

        public void AddScore(int increment)
        {
            Score += (int)(increment * Multiplier);

            InvokeScoreChanged(new ScoreChangedArgs(Score, Multiplier));
        }

        public void AddMultiplier(double increment)
        {
            Multiplier += increment;

            InvokeScoreChanged(new ScoreChangedArgs(Score, Multiplier));
        }

        public void InvokeScoreChanged(ScoreChangedArgs args)
        {
            var handler = ScoreChanged;

            if (handler != null) handler(this, args);
        }
    }

    public class ScoreChangedArgs
    {
        public ScoreChangedArgs(int score, double multiplier)
        {
            Score = score;
            Multiplier = multiplier;
        }

        public int Score { get; private set; }
        public double Multiplier { get; private set; }
    }
}
