using OTISCZ.ScmDemand.Interface.WsScmDemand;
using OTISCZ.ScmDemand.UI.Resource;
using OTISCZ.ScmDemand.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OTISCZ.ScmDemand.UI {
    /// <summary>
    /// Interaction logic for ScmDemandStart.xaml
    /// </summary>
    public partial class ScmDemandStart : Window {
        #region Constants
        private const string APP_PROCESS_NAME = "OTISCZ.ScmDemand.UI";
        #endregion

        #region Properties
        private System.Windows.Forms.NotifyIcon m_notifyIcon = new System.Windows.Forms.NotifyIcon();
        //private System.Windows.Forms.ContextMenu m_ContectMenu = null;
        private System.Windows.Forms.ContextMenuStrip m_ContectMenu = new System.Windows.Forms.ContextMenuStrip();
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

#if RELEASE
        private WcfScmDemand.ScmDemandClient m_WcfScm = null;

        protected WcfScmDemand.ScmDemandClient WcfScm {
            get {
                if (m_WcfScm == null) {
                    m_WcfScm = new WcfScmDemand.ScmDemandClient();

                }

                return m_WcfScm;
            }
        }
#else
        private WcfScmDemandDebug.ScmDemandClient m_WcfScm = null;

        protected WcfScmDemandDebug.ScmDemandClient WcfScm {
            get {
                if (m_WcfScm == null) {
                    m_WcfScm = new WcfScmDemandDebug.ScmDemandClient();

                }

                return m_WcfScm;
            }
        }
#endif
        #endregion

        private MainWindow m_MainWindow = new MainWindow();
        public MainWindow MainWindow {
            get { return m_MainWindow; }
        }

        public ScmDemandStart() {
            InitializeComponent();

            CloseAllOlderRunningInstances();

            object oScmDemandIco = ScmResource.ResourceManager.GetObject("ScmDemand");
            System.Drawing.Icon icoDemand = (System.Drawing.Icon)oScmDemandIco;

            //m_notifyIcon.Icon = new System.Drawing.Icon(@"ScmDemand.ico");
            m_notifyIcon.Icon = icoDemand;
            m_notifyIcon.BalloonTipClicked += M_notifyIcon_BalloonTipClicked;

            m_ContectMenu = new System.Windows.Forms.ContextMenuStrip();

            var miDashboard = m_ContectMenu.Items.Add("Dashboard");
            object oDahboard = ScmResource.ResourceManager.GetObject("Dashboard");
            miDashboard.Image = (System.Drawing.Image)oDahboard;
            miDashboard.Click += MiDashboard_Click; ;

            var miExit = m_ContectMenu.Items.Add("Exit");
            object oExitApp = ScmResource.ResourceManager.GetObject("AppExit"); //Return an object from the image chan1.png in the project
            miExit.Image = (System.Drawing.Image)oExitApp; //Set the Image property of channelPic to the returned object as Image
            //miExit.Image = Properties.Resources;// Bitmap.FromFile("c:\\NewItem.bmp");
            miExit.Click += MiExit_Click;

            m_notifyIcon.ContextMenuStrip = m_ContectMenu;
            m_notifyIcon.Click += M_notifyIcon_Click;

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(1, 0, 0);
            dispatcherTimer.Start();

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            m_MainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            m_MainWindow.Show();
        }

        private void MiDashboard_Click(object sender, EventArgs e) {
            OpenDashboard();
        }

        private void M_notifyIcon_BalloonTipClicked(object sender, EventArgs e) {
            OpenDashboard();
        }

        private void CloseAllOlderRunningInstances() {
            Process[] runProcesses = Process.GetProcessesByName(APP_PROCESS_NAME);

            //get the latest one
            DateTime latestStart = DateTime.MinValue;
            foreach (Process p in runProcesses) {
                if (latestStart < p.StartTime) {
                    latestStart = p.StartTime;
                }
            }

            foreach (Process p in runProcesses) {
                if (p.StartTime != latestStart) {
                    p.Kill();
                }
            }


        }

        private void OpenDashboard() {
            if (!m_MainWindow.IsClosed) {
                if (m_MainWindow.WindowState == WindowState.Minimized) {
                    m_MainWindow.WindowState = WindowState.Normal;
                }
                m_MainWindow.Activate();
                return;
            }

            m_MainWindow = new MainWindow();
            m_MainWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            m_MainWindow.Show();
        }

        private void MiExit_Click(object sender, EventArgs e) {
            m_notifyIcon.Visible = false;
            this.Close();
            Environment.Exit(0);
        }

        private void M_notifyIcon_Click(object sender, EventArgs e) {
            if (e is System.Windows.Forms.MouseEventArgs) {
                System.Windows.Forms.MouseEventArgs mouseEvent = (System.Windows.Forms.MouseEventArgs)e;
                if (mouseEvent.Button != System.Windows.Forms.MouseButtons.Left) {
                    return;
                }
            }

            
            MethodInfo mi = typeof(System.Windows.Forms.NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(m_notifyIcon, null);
                        
        }

        private int GetPendingDemands() {
            //var m_ScmUser = new ScmVmUSer();

            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;


            string pureUserName = GetPureUserName(userName);
            var wsScmUser = WcfScm.GetUserData(pureUserName);

            if (wsScmUser == null) {
                return -1;
            }

            var scmUser = new ScmUser();
            VmBase.SetValues(wsScmUser, scmUser);

            int pendingDemands = WcfScm.GetPendingDemandsNumber(scmUser.id);

            return pendingDemands;
        }

        private string GetPureUserName(string userName) {
            string[] items = userName.Split('\\');
            return items[1];
        }

        private void DisplayNotifyBallon() {
            int pendingDemands = GetPendingDemands();
            if (pendingDemands > 0) {
                m_notifyIcon.ShowBalloonTip(30000, "SCM Demand", ScmResource.PendingDemands + ": " + pendingDemands, System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            m_notifyIcon.Visible = true;
            //Thread.Sleep(5000);
            this.Visibility = System.Windows.Visibility.Collapsed;

            DisplayNotifyBallon();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            DisplayNotifyBallon();
        }
    }
}
