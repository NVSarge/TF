using System;
using System.Collections.Generic;

namespace TF
{
    class QuadTree<T>
    {
        public QuadLeaf<T> Root;

        public QuadTree()
        {
            Root = new QuadLeaf<T>();
        }

        public void CollapseLeafs()
        {
            List<QuadLeaf<T>> Pack = new List<QuadLeaf<T>>();
            Pack.Add(Root);
            QuadLeaf<T> Curleaf = Pack[0];
            while (Pack.Count > 0)
            {
                Curleaf = Pack[0];
                T Nv = Curleaf.Siblings[0].NodeValue;
                bool isEqualPack = true;
                foreach (var q in Curleaf.Siblings)
                {
                    if (q != null)
                    {
                        Pack.Add(q);
                        if(!q.NodeValue.Equals(Nv))
                        {
                            isEqualPack = false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if(isEqualPack)
                {
                    Curleaf.Siblings = null;
                    Pack.Clear();
                    Pack.Add(Root);
                }
                Pack.RemoveAt(0);
            }
        }
        public List<T> GetLeafsValues()
        {
            List<T> retval = new List<T>();
            var Lfs = getLeafs();
            foreach (var q in Lfs)
            {
                retval.Add(q.NodeValue);
            }
            return retval;
        }
        public List<QuadLeaf<T>> getLeafs()
        {
            List<QuadLeaf<T>> retval = new List<QuadLeaf<T>>();
            List<QuadLeaf<T>> Pack = new List<QuadLeaf<T>>();
            Pack.Add(Root);
            QuadLeaf<T> Curleaf = Pack[0];
            while (Pack.Count > 0)
            {
                Curleaf = Pack[0];
                foreach (var q in Curleaf.Siblings)
                {
                    if (q != null)
                    {
                        Pack.Add(q);
                    }else
                    {
                        retval.Add(Curleaf);
                        break;
                    }
                }
                Pack.RemoveAt(0);
            }

            return retval;
        }
    }

}
