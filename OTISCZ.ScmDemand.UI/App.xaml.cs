using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OTISCZ.ScmDemand.UI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            //this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        //void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        //    // Process unhandled exception
        //    MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);

        //    // Prevent default unhandled exception processing
        //    e.Handled = true;
        //}

        //private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
        //    MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    e.Handled = true;

        //}

        //void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
        //    string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
        //    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    // OR whatever you want like logging etc. MessageBox it's just example
        //    // for quick debugging etc.
        //    e.Handled = true;
        //}

        //protected override void OnStartup(StartupEventArgs e) {
        //    // define application exception handler
        //    Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;

        //    // defer other startup processing to base class
        //    base.OnStartup(e);
        //}

        //private void Application_Startup(object sender, StartupEventArgs e) {
        //    Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);
        //}

        //private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        //    string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
        //    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    // OR whatever you want like logging etc. MessageBox it's just example
        //    // for quick debugging etc.
        //    e.Handled = true;
        //}

        //private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        //    string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
        //    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    // OR whatever you want like logging etc. MessageBox it's just example
        //    // for quick debugging etc.
        //    e.Handled = true;
        //}

        private void Application_Startup(object sender, StartupEventArgs e) {
            this.Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            //throw new NotImplementedException();
            MessageBox.Show("Chyba", "Došlo k chybě. Kontaktujte administrátora.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            //throw new NotImplementedException();
            MessageBox.Show(e.Exception.Message, "Došlo k chybě. Kontaktujte administrátora.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show(e.Exception.Message, "Došlo k chybě. Kontaktujte administrátora.", MessageBoxButton.OK, MessageBoxImage.Error);
            //if (this.DoHandle) {
            //    //Handling the exception within the UnhandledExcpeiton handler.
            //    MessageBox.Show(e.Exception.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            //} else {
            //    //If you do not set e.Handled to true, the application will close due to crash.
            //    MessageBox.Show("Application is going to close! ", "Uncaught Exception");
            //    e.Handled = false;
            //}
        }

        private void Grid_GiveFeedback(object sender, GiveFeedbackEventArgs e) {

        }
    }
 }
