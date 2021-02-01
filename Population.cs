using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Tensorflow.Binding;


namespace TF
{
    public class Population<T>
    {
        GenePool<T> GP;
        List<Genome<T>> Pop;
        List<double> Rating;
        int FullLength;
        public double GetTopRating(int ind = 0)
        {
            if (Rating.Count > 0)
            {
                return Rating[ind];
            }
            else
            {
                return double.PositiveInfinity;
            }
        }
        public double[,] GetTopDataU(int ind = 0)
        {
            if (Pop.Count > 0)
            {

                return Pop[ind].defs;
            }
            else
            {
                return null;
            }

        }
        public string GetTopData(int ind = 0)
        {
            if (Pop.Count > 0)
            {

                return Pop[ind].Addata;
            }
            else
            {
                return "NAN";
            }

        }

        public T[] GetTop(int ind = 0)
        {
            T[] retval = new T[Pop[ind].Genom.Length];
            for (int i = 0; i < Pop[ind].Genom.Length; i++)
            {
                retval[i] = Pop[ind].Genom[i];
            }
            return retval;

        }
        public void Show(int n = 0)
        {
            print("============================");
            if (n == 0)
            {

                foreach (var g in Pop)
                {
                    print(g.Genom);
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    print(Pop[i].Genom);
                }
            }
            print("============================");
        }
        public Population(int size, int genomeLength)
        {
            FullLength = size;
            Rating = new List<double>();
            Pop = new List<Genome<T>>();
            GP = new GenePool<T>();
            for (int i = 0; i < size; i++)
            {
                Pop.Add(new Genome<T>(genomeLength));
            }
        }

        public void AddToGenePool(T GENE)
        {
            GP.AddToPool(GENE);
        }
        public void InitialFillGP()
        {
            Random r = new Random();
            for (int i = 0; i < Pop.Count; i++)
            {

                var Gt = Pop[i].RandomizeInitial(GP, r);
            }
        }
        public void Eval(Func<Genome<T>, int, int, double, double, double, double, double> EvalFunc, int WX, int WY, double vf, double f1, double f2, double f3)
        {
            Rating.Clear();
            /*foreach (var g in Pop)
            {
                double f = EvalFunc(g, WX, WY, vf);
                Rating.Add(f);
            }/**/

            Parallel.ForEach(Pop, (g) =>
            {
                double f = EvalFunc(g, WX, WY, vf,f1,f2,f3);
                g.Rating = f;
                //Rating.Add(f);
            });/**/
            foreach (var g in Pop)
            {
                Rating.Add(g.Rating);
            }
        }
        public void SortAndEliminate(double parity)
        {
            Pop = Pop.OrderBy(d => Rating[Pop.IndexOf(d)]).ToList();
            parity = Math.Max(0.0, Math.Min(1.0, parity));
            int CutInd = (int)Math.Ceiling(parity * Pop.Count);
            Pop.RemoveRange(CutInd, Pop.Count - CutInd);
            Rating.Sort();
        }
        public void Mutate(int parents, int VariateProb = 0,int WX = 0, int WY = 0)
        {
            Random r = new Random();
            int InitialBreed = Pop.Count;
            while (Pop.Count < FullLength)
            {
                int[] pars = new int[InitialBreed];
                // pars[0] = 0;
                for (int i = 0; i < parents; i++)
                {
                    pars[i] = r.Next(Pop.Count);
                }
                Genome<T> g_child = new Genome<T>(Pop[0].Genom.Length);
                for (int i = 0; i < g_child.Genom.Length; i++)
                {
                    g_child.Genom[i] = Pop[pars[0]].Genom[i];

                }
                for (int p = 1; p < parents; p++)
                {
                    int GenomPlace = r.Next(g_child.Genom.Length) / 2 + g_child.Genom.Length / 4;
                    for (int i = 0; i < g_child.Genom.Length; i++)
                    {
                        if (i <= GenomPlace)
                        {
                            g_child.Genom[i] = Pop[pars[p]].Genom[i];
                        }
                        /* if (r.Next(2) == 1)
                         {
                             g_child.Genom[i] = Pop[pars[p]].Genom[i];
                         }/**/
                    }
                }
                if (VariateProb > 0)
                {
                    for (int i = 0; i < g_child.Genom.Length; i++)
                    {
                        if (r.Next(VariateProb) == 1)
                        {
                                g_child.AddToGene(i,(T)GP.GetRandomGene(r));
                         
                        }

                    }
                }
                Pop.Add(g_child);
            }
        }

    };



}
