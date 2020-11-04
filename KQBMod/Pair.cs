using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KQBMod
{
    public class Pair<T1, T2>
    {
        public readonly T1 _1;
        public readonly T2 _2;

        public Pair(T1 _1, T2 _2)
        {
            this._1 = _1;
            this._2 = _2;
        }

        public override bool Equals(object obj)
        {
            return obj is Pair<T1, T2> pair &&
                   EqualityComparer<T1>.Default.Equals(_1, pair._1) &&
                   EqualityComparer<T2>.Default.Equals(_2, pair._2);
        }

        public override int GetHashCode()
        {
            int hashCode = -173718679;
            hashCode = hashCode * -1521134295 + EqualityComparer<T1>.Default.GetHashCode(_1);
            hashCode = hashCode * -1521134295 + EqualityComparer<T2>.Default.GetHashCode(_2);
            return hashCode;
        }
    }
}
