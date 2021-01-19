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
using System.Windows.Shapes;
using WPF_MindMap.Resources.scripts;
namespace WPF_MindMap
{
    /// <summary>
    /// CreateWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CreateWindow : Window
    {
        public CreateWindow(int point_x = 0, int point_y = 0)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.WindowState = WindowState.Maximized;
            this.Loaded += PageLoaded;
            InitializeComponent();

        }

        public void PageLoaded(Object sender, RoutedEventArgs e)
        {
            MindTextBox main_mindbox1 = new MindTextBox(draw_panel, "환영합니다!", true);
            draw_panel.Children.Add(main_mindbox1);

            main_mindbox1.SetActualPosition((draw_panel.ActualWidth - main_mindbox1.Width) / 2, (draw_panel.ActualHeight - main_mindbox1.Height) / 2);
        }



    }
}
