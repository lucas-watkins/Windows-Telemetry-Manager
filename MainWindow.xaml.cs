using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
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
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

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
            reloadStatusLabel();

            // Check if running on windows 10 or higher and shows messagebox if not
            if (!(Environment.OSVersion.Version.Major >= 6))
            {
                MessageBox.Show("This program only runs on Windows 10 or higher.");
                Environment.Exit(0);
            }
        }
        
        //check if telemetry is enabled using service api
        private bool telemetryEnabled()
        {
            try
            {   
                // check if the service is running. If it is return true; else return false
                if (TelemetryService.telemetryService.Status == ServiceControllerStatus.Running) { return true;} else if (TelemetryService.telemetryService.StartType == ServiceStartMode.Disabled){ return false;} else { MessageBox.Show("Unable to tell telemetry service start type assuming true"); return true; }
            }
            // show messagebox error if something happens and return false
            catch (Exception ex) { MessageBox.Show(ex.ToString()); return true; }
        }

        // attempts to disable telemetry using sc since we can't do it using services api (buggy try running it as administrator)
        private void disableTelemetry()
        {
            try
            {
                // create process with start info 
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = "config \"Connected User Experiences and Telemetry\" start=disabled";
                p.StartInfo.RedirectStandardInput = true;
                p.Start();

                // show messagebox and rewrite status label
                MessageBox.Show("Telemetry Sucessfully Disabled");
                reloadStatusLabel();
            } catch (Exception ex) {MessageBox.Show(ex.ToString()); reloadStatusLabel(); }
        }

        //attempts to disable telemetry using commands. Still a bit buggy (try running it as administrator)
        private void enableTelemetry()
        {
            try
            {
                // create process with start info 
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = "config \"Connected User Experiences and Telemetry\" start=auto";
                p.StartInfo.RedirectStandardInput = true;
                p.Start();

                // show messagebox and rewrite status label
                MessageBox.Show("Telemetry Sucessfully Enabled");
                reloadStatusLabel();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); reloadStatusLabel(); }
        }

        // writes telemetry status to status label
        private void reloadStatusLabel()
        {
            //write if telemetry is enabled
            statusLabel.Content = "Telemetry Enabled: " + telemetryEnabled().ToString();

            //sets color of text depending on if telemetry is enabled
            if (telemetryEnabled())
            {
                statusLabel.Foreground = Brushes.Red;
            }
            else { statusLabel.Foreground = Brushes.Green; }
        }

        private void enableButton_Click(object sender, RoutedEventArgs e)
        {
            if (telemetryEnabled())
            {
                MessageBox.Show("Telemetry already enabled");
            }
            else {enableTelemetry();}
        }

        private void disableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!telemetryEnabled())
            {
                MessageBox.Show("Telemetry already disabled");
            } else { disableTelemetry();}
        }
    }

    // static class for accessing telemetry service variable anywhere
    public static class TelemetryService
    {
        // initalize service
        public static ServiceController telemetryService = new ServiceController("Connected User Experiences and Telemetry", Environment.MachineName);
    }
}
