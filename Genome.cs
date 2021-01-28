using System;


namespace TF
{
    public class Genome:BaseGenome<double>
    {
        public string Addata;
        public Genome(int L = 100)
        {
            Addata = "";
        }
        public double[] RandomizeInitial(double lb, double ub, GenePool<double> GP = null, Random r = null)
        {

            if (r == null)
            {
                r = new Random();
            }

            for (int i = 0; i < Genom.Length; i++)
            {
                if (GP == null)
                {
                    Genom[i] = r.NextDouble() * (ub - lb) + lb;
                }
                else
                {
                    Genom[i] = GP.GetRandomGene(r);
                }
            }
            return Genom;
        }


        public void FillByInvert(double[] G, double GMax)
        {
          
            for (int i = 0; i < Genom.Length; i++)
            {
                Genom[i] = GMax - G[i];
            }/**/
        }

    };



}
