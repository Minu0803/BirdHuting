using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp2.Model
{
    public interface IObject
    {
        string Name { get; set; }
        BitmapImage Image { get; set; }
        int Row { get; set; }
        int Column { get; set; }
    }
}
