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
using Microsoft.Win32;

namespace Windows_Telemetry_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            //write if telemetry is enabled
            statusLabel.Content = "Telemetry Enabled: " + telemetryEnabled().ToString();

            //sets color of text depending on if telemetry is enabled
            if (telemetryEnabled())
            {
                statusLabel.Foreground = Brushes.Red;
            }
            else {statusLabel.Foreground = Brushes.Green;}
        }

        
        /*Todo: add another way of disabling telemetry because registry disabling doesn't work on windows 10 and 11 home editions
        check if there is hopefully not a half-assed services api and disable telemetry service. Taskscheduler also works to*/
        
        //check if telemetry is enabled using registry key
        private bool telemetryEnabled()
        {
            if (Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry", null) == null)
            {
                return true;
            }
            else { return false; }
        }
    }
}
