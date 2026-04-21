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

namespace Race_Condition.Views
{
    /// <summary>
    /// Interaction logic for MinhHoaView.xaml
    /// </summary>
    public partial class MinhHoaView : UserControl
    {
        public MinhHoaView()
        {
            InitializeComponent();
            this.DataContext = new Race_Condition.ViewModels.MinhHoaViewModel();
        }
    }
}
