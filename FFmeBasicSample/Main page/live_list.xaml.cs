using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FFmePlayer_snu
{
    /// <summary>
    /// live_list.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class live_list : UserControl
    {
        public live_list()
        {
            InitializeComponent();
            button_create();
        }

        List<Button> Buttons = new List<Button>();
        
        private void button_create()
        {

        }

    }
}
