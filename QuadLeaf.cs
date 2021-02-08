using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TF
{
    class QuadLeaf<T>
    {
        public QuadLeaf<T>[] Siblings;
        private T nodeValue;
        private Rect bB;
        private bool isLast;
        public QuadLeaf()
        {
            Siblings = new QuadLeaf<T>[4];
            bB = new Rect(0, 0, 0, 0);
            nodeValue = default(T);
        }

        public QuadLeaf(T nodeValue)
        {
            NodeValue = nodeValue;
            IsLast = false;
        }

        public void SubDivide(T NewValue=default(T))
        {
            if (BB.Width > 1.0 && BB.Height > 1.0)
            {
                for (int i = 0; i < Siblings.Length; i++)
                {
                    Siblings[i] = new QuadLeaf<T>();
                    Rect C = BB;
                    C.Width /= 2;
                    C.Height /= 2;
                    C.X = C.X + i % 2 * C.Width;
                    C.Y = C.Y + i / 2 * C.Height;
                    Siblings[i].BB = C;
                    if (!EqualityComparer<T>.Default.Equals(NewValue,default(T)))
                    {
                        Siblings[i].NodeValue = NewValue;
                    }
                    else
                    {
                        Siblings[i].NodeValue = NodeValue;
                    }
                }
            }
            else
            {
                IsLast = true;
            }
        }

        public T NodeValue { get => nodeValue; set => nodeValue = value; }
        public Rect BB { get => bB; set => bB = value; }
        public bool IsLast { get => isLast; set => isLast = value; }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Siblings.Length; i++)
            {
                yield return Siblings[i];
            }
        }

    }

}
