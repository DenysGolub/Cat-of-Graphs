﻿using Main.Classes;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       
    }

    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var proc = Process.GetCurrentProcess();
            var processName = proc.ProcessName.Replace(".vshost", "");
            var runningProcess = Process.GetProcesses()
                .FirstOrDefault(x => (x.ProcessName == processName ||
                                x.ProcessName == proc.ProcessName ||
                                x.ProcessName == proc.ProcessName + ".vshost") && x.Id != proc.Id);

            if (runningProcess == null)
            {
                var app = new App();
                app.InitializeComponent();
                var window = new MainWindow();
                MainWindow.HandleParameter(args);
                app.Run(window);

                MainWindow.HandleParameter(args);
                return; // In this case we just proceed on loading the program
            }

            if (args.Length > 0)
                UnsafeNative.SendMessage(runningProcess.MainWindowHandle, string.Join(" ", args));
        }
    }

}
