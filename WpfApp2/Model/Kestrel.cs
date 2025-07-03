using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfApp2.Model
{
    public class Kestrel : IBird
    {
        public string Name { get; set; }
        public BitmapImage Image { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int Score { get; set; }

    }
}
