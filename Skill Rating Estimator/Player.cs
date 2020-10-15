using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skill_Rating_Estimator
{
    public class Player
    {
        public double R
        {
            get => r;
            set
            {
                r = value;
                q = Math.Exp(value);
                p0 = 1 / (1 + Math.Exp(-value));
            }
        }
        public double Q
        {
            get => q;
            set
            {
                q = value;
                r = Math.Log(value);
                p0 = value / (1 + value);
            }
        }
        public double P0
        {
            get => p0;
            set
            {
                p0 = value;
                q = value / (1 - value);
                r = Math.Log(q);
            }
        }
        public double Elo
        {
            get => R * 173.72 + 1500;
        }

        public readonly string Name;

        private double q = 1.0;
        private double r = 0.0;
        private double p0 = 0.5;

        public Player(string name)
        {
            this.Name = name;
        }

        public double Against(Player p) => q / (q + p.q);

        public override string ToString()
        {
            return this.Name;
        }
    }
}
