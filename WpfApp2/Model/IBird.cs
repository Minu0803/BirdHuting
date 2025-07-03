using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp2.Model
{
    public interface IBird : IObject
    {
        
        int Score { get; set; }

    }
}
