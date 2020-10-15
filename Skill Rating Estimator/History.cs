using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Skill_Rating_Estimator
{
    public struct Result
    {
        public readonly Player Winner;
        public readonly Player Loser;
        
        public double Likely => Winner.Against(Loser);

        public Result (Player winner, Player loser)
        {
            Winner = winner;
            Loser = loser;
        }
    }

    public class History
    {
        public readonly PlayerBase playerBase;
        internal readonly Result[] results;

        internal History(int size)
        {
            results = new Result[size];
        }

        public History(PlayerBase pb, string filename)
        {
            this.playerBase = pb;
            var dict = pb.Dict;
            using (StreamReader sr = new StreamReader(filename))
            {
                int lenght = int.Parse(sr.ReadLine().Split(';')[0]);
                results = new Result[lenght];

                for (int i = 0; i < lenght; i++)
                {
                    var row = sr.ReadLine().Split(';');
                    results[i] = new Result(dict[row[0]], dict[row[1]]);
                }
            }
        }

        public void Shuffle()
        {
            Random rnd = new Random();
            for (int i = 0; i < results.Length; i++)
            {
                int flip = rnd.Next(i, results.Length);
                var temp = results[i];
                results[i] = results[flip];
                results[flip] = temp;
            }
        }

        public void Print()
        {
            foreach (var item in results)
            {
                Console.WriteLine("[" + item.Winner + " wins " + item.Loser + "] " + item.Winner.Against(item.Loser));
            }
        }

        internal double Likelihood(Player player, double score)
        {
            double likelihood = 1;
            double q = Math.Exp(score);

            for (int match = 0; match < results.Length; match++)
            {
                if (results[match].Loser == player)
                    likelihood *= results[match].Winner.Q / (q + results[match].Winner.Q);
                else if (results[match].Winner == player)
                    likelihood *= q / (q + results[match].Loser.Q);
            }
            return likelihood;
        }
    }
}
