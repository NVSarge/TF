using System;
using System.Collections.Generic;


namespace TF
{
    public class GenePool<T>
    {
        List<T> Pool;
        public void AddToPool(T Gene)
        {
            if (Pool == null)
            {
                Pool = new List<T>();
            }
            Pool.Add(Gene);
        }
        public T GetRandomGene(Random r)
        {
            int k = r.Next(Pool.Count);
            return Pool[k];
        }
    };



}
