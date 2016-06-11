using Blotter.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Blotter.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var vm = DataContext as ShellViewModel;
                var row = e.Row.DataContext as TradeViewModel;
                if (vm != null && row != null)
                {
                    vm.CommitRow(e.Row.DataContext as TradeViewModel);
                }
            }
        }
    }
}
