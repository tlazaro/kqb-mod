using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KQBMod
{
    static class Utils
    {
        public static T[] GetEnumValues<T>() where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("GetValues<T> can only be called for types derived from System.Enum", "T");
            }
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
