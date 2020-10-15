using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skill_Rating_Estimator
{
    class Program
    {
        static void Main(string[] args)
        {
            var pb = new PlayerBase(@"C:\Users\Pichau\Desktop\Almoxarifado\históricos de competitivo\Quake Proleague Playerbase.csv");
            var hist = new History(pb, @"C:\Users\Pichau\Desktop\Almoxarifado\históricos de competitivo\Quake Proleague History.csv");
            pb.RandomizeRanks();
            pb.Print();
            var generatedhist = pb.GenerateHistory(2000);
            pb.ResetRanks();
            pb.BayesRun(generatedhist, 10);
            pb.Print();

            Console.ReadKey();
        }
    }
}
