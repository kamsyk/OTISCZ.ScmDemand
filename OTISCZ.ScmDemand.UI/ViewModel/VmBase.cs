
using OTISCZ.ScmDemand.Model;
using OTISCZ.ScmDemand.Model.Repository;
using OTISCZ.ScmDemand.UI.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OTISCZ.ScmDemand.UI.ViewModel {
    public class VmBase {
        #region Struct
        public struct DropFile {
            public string FileName;
            public byte[] FileContent;
            public BitmapSource Icon;
        }
        #endregion

        #region Constants
        public const string DRAG_OBJECT_DESCRIPTOR = "Object Descriptor";
        public const string DRAG_FILE_GROUP_DESCRIPTOR = "FileGroupDescriptor";
        public const string DRAG_FILE_DROP = "FileDrop";
        //public const string DRAG_OBJECT_DESCRIPTOR = "Object Descriptor";
        //public const string DRAG_FILE_GROUP_DESCRIPTOR = "FileGroupDescriptor";

        public const string CULTURE_CZ = "cs-CZ";
        public const string CULTURE_EN = "en-US";
        public const string CULTURE_SK = "sk-SK";

        #endregion

        #region Delegates
        public delegate void DlgRefreshDataGrid();
        public delegate void DlgRefreshEditableDataGrid(int newId);
        public delegate void DlgCloseWindow();
        #endregion

        #region Enums
        //public enum AttachmentType {
        //    GeneratedDemand = 1
        //}
        #endregion

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        public bool HasErrors { get; set; } = false;
        //public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private Dispatcher m_Dispatcher = null;
        protected Dispatcher ScmDispatcher {
            get { return m_Dispatcher; }
        }

        private ScmVmUSer m_ScmUser = null;
        public ScmVmUSer CurrentUser {
            get { return m_ScmUser; }
        }


        public string LocLoadingData {
            get {
                return ScmResource.LoadingData;
            }

        }

        private bool m_isBusy = false;
        public bool IsBusy {
            get {
                return m_isBusy;
            }
            set {
                m_isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        private Visibility m_HeaderTitleVisibility = Visibility.Visible;
        public Visibility HeaderTitleVisibility {
            get {
                return m_HeaderTitleVisibility;
            }
            set {
                m_HeaderTitleVisibility = value;
                OnPropertyChanged("HeaderTitleVisibility");
            }
        }

#if RELEASE
#else
        //private WcfScmDemandDebug.ScmDemandClient m_WsScm = null;

        //protected WcfScmDemandDebug.ScmDemandClient WsScm {
        //    get {
        //        if (m_WsScm == null) {
        //            m_WsScm = new WsScmDemandDebug.ScmDemand();

        //        }

        //        return m_WsScm;
        //    }
        //}
#endif

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

        #region Constructor
        public VmBase(ScmUser scmUser, Dispatcher dispatcher) {
            m_ScmUser = new ScmVmUSer();
            m_ScmUser.User = scmUser;
            m_Dispatcher = dispatcher;
        }
        #endregion

        #region Static Methods
        public static byte[] GetBytesFromBitmapSource(BitmapSource bmp) {
            //int width = bmp.PixelWidth;
            //int height = bmp.PixelHeight;
            //int stride = width * ((bmp.Format.BitsPerPixel + 7) / 8);

            //byte[] pixels = new byte[height * stride];

            //bmp.CopyPixels(pixels, stride, 0);

            //return pixels;
            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            var frame = System.Windows.Media.Imaging.BitmapFrame.Create(bmp);
            encoder.Frames.Add(frame);
            var stream = new MemoryStream();

            encoder.Save(stream);
            return stream.ToArray();
        }

        public static BitmapImage LoadImage(byte[] imageData) {

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new System.IO.MemoryStream(imageData);
            image.EndInit();
            return image;

        }

        public static void HandleError(Exception ex, Dispatcher dispatcher) {
            dispatcher.Invoke(() => {
                MessageBox.Show(ScmResource.ErrorMsg + Environment.NewLine + ex.Message, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            });

        }

        public static bool IsMailAddressValid(string emailAddressText) {
            //try {
            //    MailAddress m = new MailAddress(emailaddress);

            //    return true;
            //} catch (FormatException) {
            //    return false;
            //}



            if (string.IsNullOrWhiteSpace(emailAddressText)) {
                return false;
            }

            string[] mailAddresses = emailAddressText.Split(';');

            
            for (int i=0; i<mailAddresses.Length; i++) {
                string emailAddress = mailAddresses[i];

                if (emailAddress.StartsWith(" ") || emailAddress.EndsWith(" ")) {
                    return false;
                }

                try {
                    // Normalize the domain
                    emailAddress = Regex.Replace(emailAddress, @"(@)(.+)$", DomainMapper,
                                              RegexOptions.None, TimeSpan.FromMilliseconds(200));

                    // Examines the domain part of the email and normalizes it.
                    string DomainMapper(Match match) {
                        // Use IdnMapping class to convert Unicode domain names.
                        var idn = new IdnMapping();

                        // Pull out and process domain name (throws ArgumentException on invalid)
                        var domainName = idn.GetAscii(match.Groups[2].Value);

                        string strMap = ( match.Groups[1].Value + domainName);
                        //return match.Groups[1].Value + domainName;
                        return strMap;
                    }
                } catch (RegexMatchTimeoutException e) {
                    return false;
                } catch (ArgumentException e) {
                    return false;
                }

                try {
                    bool isValid = Regex.IsMatch(emailAddress,
                        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

                    if (!isValid) {
                        return false;
                    }

                } catch (RegexMatchTimeoutException) {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Methods
        

        protected void HandleError(Exception ex) {
            try {
                WsMail.OtWsMail wsMail = new WsMail.OtWsMail();
                wsMail.SendMail(
                    "ScmDemand@otis.com",
                    "kamil.sykora@otis.com",
                    null,
                    "SCM Demand Error - " + CurrentUser.User.user_name,
                    ex.ToString(),
                    null,
                    (int)MailPriority.High);
            } catch { }
            ScmDispatcher.Invoke(() => {
                MessageBox.Show(ScmResource.ErrorMsg + Environment.NewLine + ex.Message, ScmResource.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            });

        }

        protected void OnPropertyChanged(string propertyName) {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null) {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Methods
        public static void SetValues(object sourceObject, object targetObject) {
            SetValues(sourceObject, targetObject, null, false);
        }
        public static void SetValues(object sourceObject, object targetObject, List<string> properties) {
            SetValues(sourceObject, targetObject, properties, false);
        }

        public static void SetValues(object sourceObject, object targetObject, List<string> properties, bool isRecursive) {
            Type tSource = sourceObject.GetType();
            Type tTarget = targetObject.GetType();

            PropertyInfo[] sourceAttributes = tSource.GetProperties();
            PropertyInfo[] targetAttributes = tTarget.GetProperties();

            foreach (PropertyInfo sourceAttribute in sourceAttributes) {
                if (properties != null && !properties.Contains(sourceAttribute.Name)) {
                    continue;
                }

                PropertyInfo targetProperty = tTarget.GetProperty(sourceAttribute.Name);
                if (targetProperty == null) {
                    continue;
                }

                if (sourceAttribute.PropertyType.FullName.IndexOf("OTISCZ.ScmDemand.Model") > -1 ||
                    sourceAttribute.PropertyType.FullName.IndexOf("WsScmDemand") > -1) {
                    if (isRecursive) {
                        SetValues(sourceAttribute.GetValue(sourceObject, null), targetProperty.GetValue(targetObject, null), null, true);
                    }
                    continue;
                }

                object oSourceValue = sourceAttribute.GetValue(sourceObject, null);
                targetProperty.SetValue(targetObject, oSourceValue, null);

                targetProperty = tTarget.GetProperty(sourceAttribute.Name + "FieldSpecified");
                if (targetProperty != null) {
                    targetProperty.SetValue(targetObject, true, null);
                }

                targetProperty = tTarget.GetProperty(sourceAttribute.Name + "Specified");
                if (targetProperty != null) {
                    targetProperty.SetValue(targetObject, true, null);
                }
            }

        }

        public static string GetTmpFolder() {
            FileInfo fi = new FileInfo(Assembly.GetEntryAssembly().Location);
            string baseFolder = fi.DirectoryName;
            string baseTmpFolder = Path.Combine(baseFolder, "Tmp");
            if (!Directory.Exists(baseTmpFolder)) {
                Directory.CreateDirectory(baseTmpFolder);
            }

            return baseTmpFolder;
        }

        private async void DeleteTempFilesAsync() {
            try {
                var t = DeleteTempFiles();
                await t;

            } catch (Exception ex) {
                HandleError(ex);
            }
        }

        public static Task DeleteTempFiles() {
            return Task.Run(() => {
                string tmpFolder = GetTmpFolder();

                string[] files = Directory.GetFiles(tmpFolder);
                foreach (string strFile in files) {
                    try {
                        FileInfo fi = new FileInfo(strFile);
                        if (fi.LastWriteTime.AddDays(7) < DateTime.Now) {
                            File.Delete(strFile);
                        }
                    } catch { }
                }

                string[] folders = Directory.GetDirectories(tmpFolder);

                foreach (string folder in folders) {
                    DeleteTempFiles(folder);
                }
            });
        }

        public static bool DeleteTempFiles(string strFolder) {
            string[] files = Directory.GetFiles(strFolder);
            bool isDeletedCompletely = true;
            foreach (string strFile in files) {
                try {
                    FileInfo fi = new FileInfo(strFile);
                    if (fi.LastWriteTime.AddDays(7) < DateTime.Now) {
                        File.Delete(strFile);
                    } else {
                        isDeletedCompletely = false;
                    }
                } catch { }
            }

            string[] folders = Directory.GetDirectories(strFolder);
            foreach (string folder in folders) {
                try {
                    if (!DeleteTempFiles(folder)) {
                        isDeletedCompletely = false;
                    }
                } catch { }
            }

            if (isDeletedCompletely) {
                try {
                    Directory.Delete(strFolder);
                } catch { }
            }

            return isDeletedCompletely;
        }

        protected string GetTmpFileName(string fileName) {
            FileInfo fi = new FileInfo(Assembly.GetEntryAssembly().Location);
            //string baseFolder = fi.DirectoryName;
            //string baseTmpFolder = Path.Combine(baseFolder, "Tmp");
            //if (!Directory.Exists(baseTmpFolder)) {
            //    Directory.CreateDirectory(baseTmpFolder);
            //}

            string baseTmpFolder = GetTmpFolder();

            int iIndex = 0;
            string fullFileName = Path.Combine(baseTmpFolder, fileName);
            while (File.Exists(fullFileName)) {
                iIndex++;
                string tmpFolder = Path.Combine(baseTmpFolder, "Tmp" + iIndex);
                if (!Directory.Exists(tmpFolder)) {
                    Directory.CreateDirectory(tmpFolder);
                }
                fullFileName = Path.Combine(tmpFolder, fileName);
            }

            return fullFileName;
        }

        protected string GetDemandFile(byte[] fileContent) {
            string fileName = "Demand.xlsx";
            FileInfo fi = new FileInfo(Assembly.GetEntryAssembly().Location);
            string baseFolder = fi.DirectoryName;
            string baseTmpFolder = Path.Combine(baseFolder, "Tmp");
            if (!Directory.Exists(baseTmpFolder)) {
                Directory.CreateDirectory(baseTmpFolder);
            }

            
            string fullFileName = Path.Combine(baseTmpFolder, fileName);
            if (!File.Exists(fullFileName)) {
                File.WriteAllBytes(fullFileName, fileContent);
            }

            return fullFileName;
        }

        protected string GetNomenclatureStatusText(int iStatus) {
            if (iStatus == (int)NomenclatureRepository.Status.Loaded) {
                return ScmResource.StatusLoaded;
            }

            if (iStatus == (int)NomenclatureRepository.Status.SentToSupplier) {
                return ScmResource.StatusSentToSupplier;
            }

            if (iStatus == (int)NomenclatureRepository.Status.SupplierReplied) {
                return ScmResource.StatusSupplierReplied;
            }

            if (iStatus == (int)NomenclatureRepository.Status.WaitForApproval) {
                return ScmResource.StatusWaitForApproval;
            }

            if (iStatus == (int)NomenclatureRepository.Status.Approved) {
                return ScmResource.StatusApproved;
            }

            if (iStatus == (int)NomenclatureRepository.Status.Rejected) {
                return ScmResource.StatusRejected;
            }

            if (iStatus == (int)NomenclatureRepository.Status.WithoutDemand) {
                return ScmResource.StatusWoDemand;
            }

            if (iStatus == (int)NomenclatureRepository.Status.PriceSet) {
                return ScmResource.StatusPriceWasSet;
            }

            if (iStatus == (int)NomenclatureRepository.Status.PriceConfirmed) {
                return ScmResource.StatusPriceWasSetConfirmed;
            }

            return ScmResource.StatusUnknown;
        }

        protected string GetDemandStatus(int iStatus) {
            if (iStatus == (int)DemandRepository.Status.Approved) {
                return ScmResource.Approved;
            }

            //if (iStatus == (int)DemandRepository.Status.Closed) {
            //    return ScmResource.Closed;
            //}

            if (iStatus == (int)DemandRepository.Status.WaitForApproval) {
                return ScmResource.StatusWaitForApproval;
            }

            if (iStatus == (int)DemandRepository.Status.Sent) {
                return ScmResource.StatusSentToSupplier;
            }

            if (iStatus == (int)DemandRepository.Status.Replied) {
                return ScmResource.StatusSupplierReplied;
            }

            if (iStatus == (int)DemandRepository.Status.Approved) {
                return ScmResource.StatusApproved;
            }

            if (iStatus == (int)DemandRepository.Status.Rejected) {
                return ScmResource.StatusRejected;
            }

            //if (iStatus == (int)DemandRepository.Status.PriceConfirmed) {
            //    return ScmResource.PriceWasSetConfirmed;
            //}

            return ScmResource.StatusUnknown;
        }

        public List<DropFile> GetDropAttachment(DragEventArgs e) {
            List<DropFile> dropFiles = new List<DropFile>();

            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true) {
                //file
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string tmpFileName in fileNames) {
                    FileInfo fi = new FileInfo(tmpFileName);
                    string fileName = fi.Name;
                    BitmapSource icon = null;

                    using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName)) {
                        icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            sysicon.Handle,
                            System.Windows.Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                    }

                    byte[] fileContent = File.ReadAllBytes(fi.FullName);

                    DropFile dropFile = new DropFile();
                    dropFile.FileName = fileName;
                    dropFile.FileContent = fileContent;
                    dropFile.Icon = icon;

                    dropFiles.Add(dropFile);
                }
            } else if (e.Data.GetDataPresent(DRAG_OBJECT_DESCRIPTOR)) {
                //1 attachment
                foreach (string format in e.Data.GetFormats()) {
                    try {
                        var data = e.Data.GetData(format);
                        if (format == DRAG_FILE_GROUP_DESCRIPTOR) {
                            Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                            for (int i = 1; i <= outlookApp.ActiveExplorer().Selection.Count; i++) {
                                Object temp = outlookApp.ActiveExplorer().Selection[i];
                                if (temp is Microsoft.Office.Interop.Outlook.MailItem) {
                                    Microsoft.Office.Interop.Outlook.MailItem mailitem = (temp as Microsoft.Office.Interop.Outlook.MailItem);
                                    string fileName = GetTmpFileName("ScmMail.msg");
                                    mailitem.SaveAs(fileName);

                                    FileInfo fi = new FileInfo(fileName);
                                    BitmapSource icon = null;

                                    using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName)) {
                                        icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                            sysicon.Handle,
                                            System.Windows.Int32Rect.Empty,
                                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                                    }
                                    byte[] fileContent = File.ReadAllBytes(fi.FullName);

                                    DropFile dropFile = new DropFile();
                                    dropFile.FileName = ReplaceInvalidChars(mailitem.Subject) + ".msg";
                                    dropFile.FileContent = fileContent;
                                    dropFile.Icon = icon;

                                    dropFiles.Add(dropFile);
                                    //m_DlgDisplayMailDetail(mailitem);
                                    //return;
                                }
                            }
                        }
                    } catch (System.Exception ex) {
                        if (ex is COMException) {

                        } else {
                            MessageBox.Show(ex.ToString());
                        }

                    }
                }
            } else if (e.Data.GetDataPresent(DRAG_FILE_GROUP_DESCRIPTOR)) {
                //
                // the first step here is to get the filename
                // of the attachment and
                // build a full-path name so we can store it
                // in the temporary folder
                //

                // set up to obtain the FileGroupDescriptor
                // and extract the file name
                Stream theStream = (Stream)e.Data.GetData("FileGroupDescriptor");
                byte[] fileGroupDescriptor = new byte[512];
                theStream.Read(fileGroupDescriptor, 0, 512);
                // used to build the filename from the FileGroupDescriptor block
                StringBuilder fileName = new StringBuilder("");
                // this trick gets the filename of the passed attached file
                for (int i = 76; fileGroupDescriptor[i] != 0; i++) { fileName.Append(Convert.ToChar(fileGroupDescriptor[i])); }
                theStream.Close();
                string path = Path.GetTempPath();
                // put the zip file into the temp directory
                string theFile = path + fileName.ToString();
                // create the full-path name

                //
                // Second step:  we have the file name.
                // Now we need to get the actual raw
                // data for the attached file and copy it to disk so we work on it.
                //

                // get the actual raw file into memory
                MemoryStream ms = (MemoryStream)e.Data.GetData(
                    "FileContents", true);
                // allocate enough bytes to hold the raw data
                byte[] fileBytes = new byte[ms.Length];
                // set starting position at first byte and read in the raw data
                ms.Position = 0;
                ms.Read(fileBytes, 0, (int)ms.Length);
                // create a file and save the raw zip file to it
                FileStream fs = new FileStream(theFile, FileMode.Create);
                fs.Write(fileBytes, 0, (int)fileBytes.Length);

                fs.Close();  // close the file
                                
                

                FileInfo fi = new FileInfo(theFile);

                BitmapSource icon = null;

                using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName)) {
                    icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        sysicon.Handle,
                        System.Windows.Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                }
                byte[] fileContent = File.ReadAllBytes(fi.FullName);

                DropFile dropFile = new DropFile();
                dropFile.FileName = fileName.ToString();
                dropFile.FileContent = fileContent;
                dropFile.Icon = icon;
                dropFiles.Add(dropFile);

                // always good to make sure we actually created the file
                if (fi.Exists == true) {
                    // for now, just delete what we created
                    fi.Delete();
                } //else { Trace.WriteLine("File was not created!"); }
            }

            //if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true) {
            //    //file
            //    string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            //    foreach (string tmpFileName in fileNames) {
            //        FileInfo fi = new FileInfo(tmpFileName);
            //        string fileName = fi.Name;
            //        BitmapSource icon = null;

            //        using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName)) {
            //            icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
            //                sysicon.Handle,
            //                System.Windows.Int32Rect.Empty,
            //                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            //        }

            //        byte[] fileContent = File.ReadAllBytes(fi.FullName);

            //        DropFile dropFile = new DropFile();
            //        dropFile.FileName = fileName;
            //        dropFile.FileContent = fileContent;
            //        dropFile.Icon = icon;

            //        dropFiles.Add(dropFile);
            //    }

            //} else if (e.Data.GetDataPresent(DRAG_OBJECT_DESCRIPTOR)) {
            //    //1 attachment
            //    Stream attFileNameStream = (Stream)e.Data.GetData(DRAG_FILE_GROUP_DESCRIPTOR);
            //    MemoryStream attFileContentStream = (MemoryStream)e.Data.GetData("FileContents", true);


            //} else if (e.Data.GetDataPresent(DRAG_FILE_GROUP_DESCRIPTOR)) {
            //    //more attachments
            //    Stream attFileNameStream = (Stream)e.Data.GetData(DRAG_FILE_GROUP_DESCRIPTOR);
            //    MemoryStream attFileContentStream = (MemoryStream)e.Data.GetData("FileContents", true);

            //} else {
            //    //foreach (string format in e.Data.GetFormats()) {
            //    //    try {
            //    //        var data = e.Data.GetData(format);
            //    //        if (format == DRAG_FILE_GROUP_DESCRIPTOR) {
            //    //            Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
            //    //            for (int i = 1; i <= outlookApp.ActiveExplorer().Selection.Count; i++) {
            //    //                Object temp = outlookApp.ActiveExplorer().Selection[i];
            //    //                if (temp is Microsoft.Office.Interop.Outlook.MailItem) {
            //    //                    Microsoft.Office.Interop.Outlook.MailItem mailitem = (temp as Microsoft.Office.Interop.Outlook.MailItem);
            //    //                    //m_DlgDisplayMailDetail(mailitem);
            //    //                    break;
            //    //                }
            //    //            }
            //    //        } else if (format == DRAG_FILE_DROP) {
            //    //            string fileName = ((String[])data)[0];
            //    //            Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
            //    //            Microsoft.Office.Interop.Outlook.MailItem mailitem = (Microsoft.Office.Interop.Outlook.MailItem)outlookApp.CreateItemFromTemplate(fileName, Type.Missing);
            //    //            //m_DlgDisplayMailDetail(mailitem);
            //    //            break;
            //    //        }
            //    //    } catch (System.Exception ex) {
            //    //        if (ex is COMException) {

            //    //        } else {
            //    //            HandleError(ex);
            //    //        }

            //    //    }
            //    //}
            //}

            return dropFiles;
        }

        public static string GetAttFileName(Stream attFileNameStream) {
            //File Name
            string fileName = "";
            byte[] fileGroupDescriptor = new byte[512];
            attFileNameStream.Read(fileGroupDescriptor, 0, 512);
            StringBuilder sbFileName = new StringBuilder("");

            ArrayList byteList = new ArrayList();
            for (int i = 76; fileGroupDescriptor[i] != 0; i++) {
                byteList.Add(fileGroupDescriptor[i]);
                //sbFileName.Append(Convert.ToChar(fileGroupDescriptor[i]));
            }

            byte[] fileNameBytes = new byte[byteList.Count];
            for (int i = 0; i < byteList.Count; i++) {
                fileNameBytes[i] = (byte)byteList[i];
            }
            Encoding enc1250 = Encoding.GetEncoding(1250);
            fileName = enc1250.GetString(fileNameBytes);

            return fileName;
        }

        public static void GetAttFile(
            Stream attFileNameStream, 
            MemoryStream attFileContentStream,
            out string fileName, 
            out byte[] fileContent) {
            
            fileName = GetAttFileName(attFileNameStream);
            
            fileContent = new byte[attFileContentStream.Length];
            attFileContentStream.Position = 0;
            attFileContentStream.Read(fileContent, 0, (int)attFileContentStream.Length);
            
        }

        public string ReplaceInvalidChars(string filename) {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static string GetCulture(string strLang) {
            switch (strLang) {
                case "SK":
                    return CULTURE_SK;

                case "EN":
                    return CULTURE_EN;

                default:
                    return CULTURE_CZ;

            }

            
        }

        //private string SaveDragDropMailMessage(DragEventArgs e) {
        //    OtOutlook otOutlook = new OtOutlook();
        //    object activeExloreItem = otOutlook.GetActiveExplorerSelection();

        //    string filePath = otOutlook.SaveMailMessage(activeExloreItem, GetTmpFolder());

        //    return filePath;
        //}
        #endregion
    }

    public class ScmVmUSer {
        public ScmUser User;

        public bool IsAdministrator {
            get {
                if (User.Role == null) {
                    return false;
                }

                foreach (var role in User.Role) {
                    if (role.id == UserRepository.USER_ROLE_ADMINISTRATOR) {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsScmReferent {
            get {
                if (User.Role == null) {
                    return false;
                }

                foreach (var role in User.Role) {
                    if (role.id == UserRepository.USER_ROLE_REFERNT) {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsApproveManager {
            get {
                if (User.Role == null) {
                    return false;
                }

                foreach (var role in User.Role) {
                    if (role.id == UserRepository.USER_ROLE_APP_MAN) {
                        return true;
                    }
                }

                return false;
            }
        }

        

    }

    public class FilterField {
        public string FieldName = null;
        public string FilterText = null;
        public VmBaseGrid.FilterFromTo FromTo;
        public string SqlFilter = null;

        public FilterField(string fieldName, string filterText, VmBaseGrid.FilterFromTo fromTo, string sqlFilter) {
            FieldName = fieldName;
            FilterText = filterText;
            FromTo = fromTo;
            SqlFilter = sqlFilter;
        }
    }

    
}
