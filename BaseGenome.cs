namespace TF
{
    public class BaseGenome<T>
    {
        public double Rating;
        public T[] Genom;
        public BaseGenome(int L = 100)
        {
            Rating = double.PositiveInfinity;
            Genom = new T[L];
        }
        public void AddToGene(int i, T G)
        {
            if(Genom.Length>i)
            {
                Genom[i] = G;
            }
        }


    };



}
