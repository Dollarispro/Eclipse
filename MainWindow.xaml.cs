using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication3
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Bo2All.About s = new Bo2All.About();
            s.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LABEL_1.Content = "Welcome " + Environment.UserName + " to Eclipse's Launcher!";
            LABEL_2.Content = "This is was developed by iMaes";
            LABEL_3.Content = "Thanks SkeezR for HUD Offsets (Lazy to update it)";
            LABEL_4.Content = "Thanks TehPoisonOne for HUD Client error help";
            LABEL_5.Content = "Please visit se7ensins.com & thetechgame.com";
        }
    }
}
