using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp2.Model;

namespace WpfApp2.Util
{
    public class ObjectTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BirdTemplate { get; set; }
        public DataTemplate HunterTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IBird) return BirdTemplate;
            if (item is Hunter) return HunterTemplate;
            return base.SelectTemplate(item, container); // return null을 함
        }
    }

}
