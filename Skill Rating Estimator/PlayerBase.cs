using System;
using System.Collections.Generic;
using System.IO;

namespace Skill_Rating_Estimator
{
    public class PlayerBase
    {
        public readonly int Max;
        public readonly Player[] Players;
        internal Dictionary<string, Player> Dict = new Dictionary<string, Player>();

        /// <summary>
        /// Create a new representation of a playerbase, based on a csv.
        /// </summary>
        /// <param name="filename"></param>
        public PlayerBase(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                int lenght = int.Parse(sr.ReadLine().Split(';')[0]);
                Players = new Player[lenght];
                Max = lenght;

                for (int i = 0; i < lenght; i++)
                {
                    var row = sr.ReadLine().Split(';');
                    Players[i] = new Player(row[0]);
                    Dict[row[1]] = Players[i];
                }
            }
        }

        /// <summary>
        /// Generate a set of matches with random players, where the winner will respect a certain probabillity.
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        public History GenerateHistory(int matches)
        {
            History hist = new History(matches);
            Random rnd = new Random();
            for (int i = 0; i < matches; i++)
            {
                Player player1 = Players[rnd.Next(0, Max)];
                Player player2;
                while ((player2 = Players[rnd.Next(0, Max)]) == player1) ;
                double prob = player1.Against(player2);

                if (rnd.NextDouble() < prob)
                    hist.results[i] = new Result (player1, player2);
                else
                    hist.results[i] = new Result(player2, player1);
            }
            return hist;
        }

        /// <summary>
        /// Normalizes the ranks of the player base so that the average Lanna Rating is properly 0.
        /// </summary>
        public void Normalize()
        {
            double sum = 0.0;

            for (int i = 0; i < Max; i++)
                sum += Players[i].R;

            double adjust = sum / Max;

            for (int i = 0; i < Max; i++)
                Players[i].R -= adjust;
            Console.WriteLine("Teams normalized.");
        }

        /// <summary>
        /// Displays the data about the playerbase.
        /// </summary>
        public void Print()
        {
            foreach (var player in Players)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{player}:");
                Console.ResetColor();
                Console.WriteLine($"\tLanna Rating: {Math.Round(player.R, 3)}");
                Console.WriteLine($"\tElo/Glicko Rating: {Math.Round(player.Elo)}");
                Console.WriteLine($"\tWinrate against LR0: {Math.Round(player.P0, 2)}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Gives every player a random skill so that its wr against LR0 is beetween 20-80%.
        /// </summary>
        public void RandomizeRanks()
        {
            Random rnd = new Random();
            for (int i = 0; i < Max; i++)
            {
                Players[i].P0 = 0.2 + 0.6 * rnd.NextDouble();
            }
            Normalize();
        }

        /// <summary>
        /// Sets every player's rank to LR0.
        /// </summary>
        public void ResetRanks()
        {
            foreach(Player p in Players)
            {
                p.R = 0.0;
            }
        }

        /// <summary>
        /// Simulate the history respecting the order, such as elo rating does. Each win/lose changes the score only of both players
        /// respecting its likely to happen.
        /// </summary>
        /// <param name="history"></param>
        public void TemporalRun(History history)
        {
            const double stdDelta = 0.35;
            if (history.playerBase != this)
                return;

            foreach(Result result in history.results)
            {
                double prob = result.Winner.Q / (result.Winner.Q + result.Loser.Q);
                double delta = (1 - prob) * stdDelta;
                result.Winner.R += delta;
                result.Loser.R -= delta;
            }
        }

        /// <summary>
        /// Runs the entire history simultaneously, giving a precise aproximation even for fewer games, but with no respect to the
        /// fact that player's skill change on the time.
        /// </summary>
        /// <param name="history"></param>
        /// <param name="interactions"></param>
        public void BayesRun(History history, int interactions)
        {
            for (int i = 0; i < interactions; i++)
            {
                history.Shuffle();
                for (int player_index = 0; player_index < Max; player_index++)
                {
                    Calculus.Function likelyhoodP0 = (x) => history.Likelihood(Players[player_index], Math.Log(x / (1 - x)));
                    double averageP0 = Calculus.Average(likelyhoodP0, 1.0 / interactions, 1.0, interactions);
                    Players[player_index].P0 = averageP0;
                }
            }
            
            Normalize();
        }
    }
}
