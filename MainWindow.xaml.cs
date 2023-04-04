using System;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

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
            if (!(Environment.OSVersion.Version.Major >= 10))
            {
                MessageBox.Show("This program only runs on Windows 10 or higher", "Windows Telemetry Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
        
        //check if telemetry is enabled using service api
        private bool telemetryEnabled()
        {
            try
            {   
                // check if the service is running. If it is return true; else return false
                if (TelemetryService.getService().Status == ServiceControllerStatus.Running) { return true;} else if (TelemetryService.getService().Status == ServiceControllerStatus.Stopped){ return false;} else { MessageBox.Show("Unable to tell if telemetry service is running. Assuming it is"); return true; }
            }
            // show messagebox error if something happens and return false
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Windows Telemetry Manager", MessageBoxButton.OK, MessageBoxImage.Hand); return true; }
        }

        // attempts to disable telemetry using sc since we can't do it using services api (buggy try running it as administrator)
        private void disableTelemetry()
        {
            try
            {
                // create process with start info to run as admin 
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = "config \"DiagTrack\" start=disabled";
                p.StartInfo.RedirectStandardInput = false;
                p.Start();

                // show messagebox and rewrite status label and start service
                stopStartService("DiagTrack", "stop");
                MessageBox.Show("Telemetry Sucessfully Disabled");
                reloadStatusLabel();
            }
            catch (System.ComponentModel.Win32Exception) { MessageBox.Show("Operation Canceled", "Windows Telemetry Manager", MessageBoxButton.OK, MessageBoxImage.Exclamation); reloadStatusLabel(); }
            catch (Exception ex) {MessageBox.Show(ex.ToString()); reloadStatusLabel(); }
        }

        //attempts to disable telemetry using commands. Still a bit buggy (try running it as administrator)
        private void enableTelemetry()
        {
            try
            {
                // create process with start info to run as admin
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Verb = "runas";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = "config \"DiagTrack\" start=auto";
                p.StartInfo.RedirectStandardInput = false;
                p.Start();

                // show messagebox and rewrite status label and start service
                stopStartService("DiagTrack", "start");
                MessageBox.Show("Telemetry Sucessfully Enabled");
                reloadStatusLabel();
            }
            catch (System.ComponentModel.Win32Exception) { MessageBox.Show("Operation Canceled", "Windows Telemetry Manager", MessageBoxButton.OK, MessageBoxImage.Exclamation); reloadStatusLabel(); }
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
                MessageBox.Show("Telemetry already enabled", "Windows Telemetry Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else {enableTelemetry(); reloadStatusLabel(); }
        }

        private void disableButton_Click(object sender, RoutedEventArgs e)
        {
            if (telemetryEnabled() == false)
            {
                MessageBox.Show("Telemetry already disabled", "Windows Telemetry Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            } else { disableTelemetry(); reloadStatusLabel(); }
        }

        // stop or start service
        private void stopStartService(string name, string mode)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo();
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.Verb = "runas";
            p.StartInfo.FileName = "net";
            p.StartInfo.Arguments = mode + " " + name;
            p.Start();

        }
    }

    // static class for accessing telemetry service variable anywhere
    public static class TelemetryService
    {
        // method to return service
        public static ServiceController getService()
        {
            try
            {
                ServiceController telemetryService = new ServiceController("Connected User Experiences and Telemetry", Environment.MachineName);
                return telemetryService;
            }
            catch (Exception) { MessageBox.Show("Unable to Find Telemetry Service", "Windows Telemetry Manager", MessageBoxButton.OK, MessageBoxImage.Error); Environment.Exit(0); return new ServiceController(); }
        }
        
    }
}
