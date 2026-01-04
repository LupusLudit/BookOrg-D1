using System.Windows;
using System.Windows.Controls;

namespace BookOrg.Src.UI.DBInteraction.Controls.Menus
{
    public partial class MainMenuControl : UserControl
    {
        private readonly Action<string> navigationCallback;

        public MainMenuControl(Action<string> navigationCallback)
        {
            InitializeComponent();
            this.navigationCallback = navigationCallback;
        }

        private void NavigateClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string destination)
            {
                navigationCallback?.Invoke(destination);
            }
        }
    }
}