using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_App.ViewModels
{
    public class SelectionItem<T>
    {
        public T Value { get; set; } = default!;

        public string Display { get; set; } = string.Empty;

        public override string ToString()
        {
            return Display;
        }
    }
}
