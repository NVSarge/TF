﻿using System;


namespace TF
{
    public class Genome<T> : BaseGenome<T>
    {
        public string Addata;
        public double[,] defs;
        public Genome(int L = 100):base(L)
        {
            
            Addata = "";            
        }
        public T[] Initial(T t)
        {
            for (int i = 0; i < Genom.Length; i++)
            {

                Genom[i] = t;
            }
            return Genom;
        }
        public T[] RandomizeInitial(GenePool<T> GP, Random r = null)
        {

            if (r == null)
            {
                r = new Random();
            }

            for (int i = 0; i < Genom.Length; i++)
            {

                    Genom[i] = GP.GetRandomGene(r);
            }
            return Genom;
        }

    };



}
