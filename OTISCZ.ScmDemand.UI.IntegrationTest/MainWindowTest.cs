using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OTISCZ.ScmDemand.Model.Repository;

namespace OTISCZ.ScmDemand.UI.IntegrationTest {
    [TestClass]
    public class MainWindowTest : BaseIntegrationTest {
        [TestMethod]
        public void LanguageSK_Switch() {
            //Arrange
            bool isOk = true;

            //Act
            var cmbCulture = WindowsDriver.FindElementByAccessibilityId("cmbCulture");
            cmbCulture.Click();
            Thread.Sleep(1000);
            var cmbSk = cmbCulture.FindElementByAccessibilityId("txtLangSK");
            cmbSk.Click();
            var mniTxtSetting = (WindowsElement)WindowsDriver.FindElementByAccessibilityId("mniTxtSetting");
            string skText = mniTxtSetting.Text;
            isOk = (skText == "Nastavenie");

            if (isOk) {
                cmbCulture = WindowsDriver.FindElementByAccessibilityId("cmbCulture");
                cmbCulture.Click();
                Thread.Sleep(1000);
                var cmbCz = cmbCulture.FindElementByAccessibilityId("txtLangCZ");
                cmbCz.Click();
                mniTxtSetting = (WindowsElement)WindowsDriver.FindElementByAccessibilityId("mniTxtSetting");
                var czText = mniTxtSetting.Text;
                isOk = (czText == "Nastavení");
            }

            //Assert
            Assert.IsTrue(isOk);
        }

        [TestMethod]
        public void UserHasNoSettings_DefaultLangSet() {
            //Arrange
            int userId = 0;
            UserRepository userRepository = new UserRepository();
            userRepository.RemoveUserSettings(userId);

            //Act
            GetWindowsDriver(true);

            //Asset
            var mniTxtSetting = (WindowsElement)WindowsDriver.FindElementByAccessibilityId("mniTxtSetting");
            var czText = mniTxtSetting.Text;
            Assert.IsTrue(czText == "Nastavení");
        }

        //mniProdisImport
        [TestMethod]
        public void ProdisImport_MenuItemClick() {
            //Arrange
            
            var mniImport = WindowsDriver.FindElementByAccessibilityId("mniImport");
            mniImport.Click();

            Thread.Sleep(1000);
            
            var mniProdisImport = WindowsDriver.FindElementByAccessibilityId("mniProdisImport");
            mniProdisImport.Click();

        }
    }
}
