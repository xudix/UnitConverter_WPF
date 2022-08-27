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

namespace UnitConverter
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
            viewModel = DataContext as MainWindow.MainWindowViewModel;
        }

        private MainWindow.MainWindowViewModel viewModel;

        private void Unit_Input_PreviewTextInput(object sender, TextCompositionEventArgs e) =>
            ((ComboBox)sender).IsDropDownOpen = true;

        private void Result_Prefix_Changed(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Console.WriteLine(e);
        }

        private void Result_Prefix_Changed(object sender, TextCompositionEventArgs e)
        {
            
        }

        private void Result_Prefix_Changed(object sender, SelectionChangedEventArgs e)
        {
            if(e.OriginalSource.GetType() == typeof(ComboBox))
            {
                viewModel.UpdatePrefix((sender as DataGrid).SelectedIndex, (e.OriginalSource as ComboBox).SelectedItem as string);
            }
        }
    }
}
