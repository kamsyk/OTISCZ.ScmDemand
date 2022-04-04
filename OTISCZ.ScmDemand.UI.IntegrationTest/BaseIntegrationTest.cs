using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OTISCZ.ScmDemand.UI.IntegrationTest {
    public abstract class BaseIntegrationTest {
        #region Constructor
        public BaseIntegrationTest() {
            TestInitialize();
        }
        #endregion

        #region Properties
        private static WindowsDriver<WindowsElement> m_WindowsDriver = null;
        protected static WindowsDriver<WindowsElement> WindowsDriver {
            get {
                if (m_WindowsDriver == null) {
                    m_WindowsDriver = GetWindowsDriver();
                }

                return m_WindowsDriver;
            }
        }
        #endregion

        #region Methods
        private void SetWinAppDriver() {
            var processes = Process.GetProcesses();
            foreach (var process in processes) {
                if (process.ProcessName.IndexOf("WinAppDriver") > -1) {
                    //WinAppDriver is running
                    return;
                }
            }

            Process.Start(@"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe");
            Thread.Sleep(1000);
        }

        [TestInitialize]
        private void TestInitialize() {
#if !TEST
            throw new Exception("Switch Configuration Manager to Test");
#else

            SetWinAppDriver();
#endif
        }

        protected static WindowsDriver<WindowsElement> GetWindowsDriver() {
            return GetWindowsDriver(false);
        }

        protected static WindowsDriver<WindowsElement> GetWindowsDriver(bool isNew) {
            if (m_WindowsDriver == null || isNew) {
                AppiumOptions options = new AppiumOptions();
                options.AddAdditionalCapability("app", @"d:\Develop\CSharp\OTISCZ.ScmDemand\OTISCZ.ScmDemand.UI\bin\Test\OTISCZ.ScmDemand.UI.exe");
                m_WindowsDriver = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), options);
            }
            return m_WindowsDriver;
        }

        protected void SetTestUser(int userId) {

        }
#endregion
    }
}
