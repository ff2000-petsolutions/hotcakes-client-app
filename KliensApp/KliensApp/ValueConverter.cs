using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KliensApp
{
    public class ValueConverter
    {
        public object Convert(string text, Type targetType)
        {
            if (targetType == typeof(string))
                return text;
            if (targetType == typeof(decimal))
                return decimal.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
            if (targetType == typeof(int))
                return int.Parse(text);
            if (targetType == typeof(bool))
                return bool.Parse(text);
            if (targetType == typeof(DateTime))
                return DateTime.Parse(text);

            throw new ArgumentException($"Nem támogatott típus: {targetType.Name}");
        }
    }
}
