/* Zeit - Arbeitszeiterfassung (Client)
 * 
 * 
 * 
 * Entwickelt von: Christoph Beyer
 * Build Datum: 08/2023 
 * 
 * Copyright (c) 2022 - 2023 Christoph beyer
 */


using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Configuration;

namespace Zeiterfassung
{
  
    public partial class Form1 : Form
    {
        #region Variables

        //File path
        string path = "";
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Zeiterfassung";
        string username = Environment.UserName.ToUpper();
        string usernameFullname, usernameEmail;
        string viewFilePath = @"";
        string logFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ZeifClient_ErrorLog.txt";
        bool syncedSinceLaunch = false;
        string targetFolder = @"";                              //loaded from app.conf file
        string programPath = @"";                               //loaded from app.conf file
        string supportEmail = "christoph.beyer@schindler.com";  //loaded from app.conf file
        int syncmode = 1;                                       //loaded from app.conf file
        int forcedSyncClosing = 0;                              //loaded from app.conf file

        /* sync mode
         * 0 = never
         * 1 = automatic (when excel file is closed)
         * 2 = when last attempt failed
         * 3 = always
         */

        /* forced sync closing
         * 0 = never
         * 1 = if not synced since launch
         * 2 = always
         */

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        #region Tools

        private void GetUsername()
        {
            username =  System.DirectoryServices.AccountManagement.UserPrincipal.Current.Name;
            usernameFullname = System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName;
            usernameEmail = System.DirectoryServices.AccountManagement.UserPrincipal.Current.EmailAddress;

            MessageBox.Show(username + "\n" + usernameFullname + "\n" + usernameEmail);
        }

        private void ReportError()
        {
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo("mailto:" + supportEmail) { UseShellExecute = true });
            }

            catch
            {
                MessageBox.Show("Senden Sie eine Mail mit ihrer Fehlerbeschreibung an folgende Adresse:" +
                    "\n\n" + supportEmail, "Fehler melden", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void ERROR(string functionName, string errMsg)
        {
            try
            {
                timer.Stop();

                lblSync.ForeColor = System.Drawing.Color.Red;

                string dt = GetDateTime();
                string addStream = dt + ": [" + functionName + "] \"" + errMsg + "\"";

                if (File.Exists(logFilePath))
                {
                    string fileStream = File.ReadAllText(logFilePath);
                    File.WriteAllText(logFilePath, fileStream + "\n" + addStream);
                }

                else
                {
                    File.WriteAllText(logFilePath, addStream);
                }

                DialogResult dr = MessageBox.Show("Es ist ein Fehler aufgetreten und die Zeitentabelle wurde möglicherweise nicht Synchronisiert (siehe Datumsstempel).\n\nWenn dieses Problem wiederholt auftritt, wenden Sie sich bitte an den lokalen Support.\n\nMöchten Sie den Fehler jetzt melden?", "Fehler", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (dr == DialogResult.Yes) ReportError();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim erstellen der Logdatei. Wenden Sie sich an ihren lokalen IT-Support:\n\n" + supportEmail + "\n\nError: " + ex.Message, "Fataler Fehler!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReadConfig()
        {
            try
            {
                programPath = Properties.Settings.Default.ProgramPath;
                if (Properties.Settings.Default.ProgramPathUser != string.Empty) programPath = Properties.Settings.Default.ProgramPathUser;
                targetFolder = Properties.Settings.Default.TargetFolderPath;
                supportEmail = Properties.Settings.Default.SupportEmail;
                syncmode = Properties.Settings.Default.syncmode;
                viewFilePath = Properties.Settings.Default.ViewFilePath;
                forcedSyncClosing = Properties.Settings.Default.ForcedSyncClosing;
            }

            catch (Exception ex)
            {
                ERROR("ReadConfFile", ex.Message);
            }
        }
        private bool CheckProgramExists()
        {
            if (File.Exists(programPath)) return true;
            else
            {
                MessageBox.Show("Der Programpfad \"" + programPath + "\" wurde nicht gefunden.\n\nGeben Sie in der Konfigurationsdatei den korrekten Programm Dateipfad an." +
                    "\n\n\nSie können die Anwendung weiterhin nutzen, jedoch ihre Excel Tabelle nicht öffnen.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ClearCreateFolder()
        {
            try
            {
                if (File.Exists(folder)) Directory.Delete(folder, true);
                Directory.CreateDirectory(folder);
            }

            catch (Exception ex)
            {
                ERROR("ClearFolder", ex.Message);
            }
        }

        public static string GetCellValue(string fileName,
        string sheetName,
        string addressName)
        {
           try
            {
                string value = null;

                
                using (SpreadsheetDocument document =
                    SpreadsheetDocument.Open(fileName, false))
                {
                    
                    WorkbookPart wbPart = document.WorkbookPart;

                    Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().
                      Where(s => s.Name == sheetName).FirstOrDefault();

                    if (theSheet == null)
                    {
                        throw new ArgumentException("sheet1");
                    }

                    WorksheetPart wsPart =
                        (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                    Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                      Where(c => c.CellReference == addressName).FirstOrDefault();

                    if (theCell.InnerText.Length > 0)
                    {
                        value = theCell.CellValue.Text.ToString();

                        if (theCell.DataType != null)
                        {
                            switch (theCell.DataType.Value)
                            {
                                case CellValues.SharedString:

                                    var stringTable =
                                        wbPart.GetPartsOfType<SharedStringTablePart>()
                                        .FirstOrDefault();

                                    if (stringTable != null)
                                    {
                                        value =
                                            stringTable.SharedStringTable
                                            .ElementAt(int.Parse(value)).InnerText;
                                    }
                                    break;

                                case CellValues.Boolean:
                                    switch (value)
                                    {
                                        case "0":
                                            value = "FALSE";
                                            break;
                                        default:
                                            value = "TRUE";
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                return value;
            }

            catch
            {
                return null;
            }
        }

        private double getHoursFromExcel()
        {
            try
            {
                int month = DateTime.Now.Month;
                int calcCellNumb = month + 5;
                double inCell;

                string value = GetCellValue(path, "Übersicht", "F" + calcCellNumb.ToString());

                if (value != null) inCell = Convert.ToDouble(value.Replace(".", ",")) * 24;
                else return 99999;

                return Math.Ceiling(inCell);
            }

            catch (Exception ex)
            {
                ERROR("getHoursFromExcel", ex.Message);
                return 99999;
            }
        }

        private string getNameFromExcel()
        {
            try
            {
                string nameFromExcel = GetCellValue(path, "Übersicht", "B3");
                if (nameFromExcel != string.Empty) return nameFromExcel;
                else return "(leer)";
            }

            catch (Exception ex)
            {
                ERROR("getNameFromExcel", ex.Message);
                return "Fehler";
            }
        }

        private string getTableName()
        {
            return GetCellValue(path, "Übersicht", "B1");
        }

        private void CreateViewFile()
        {
            try
            {
                string hoursString, month = "";

                if (Properties.Settings.Default.RetrieveHoursFromExcel)
                {
                    double hours = getHoursFromExcel();
                    if (hours >= 10000 ^ hours <= -10000) hoursString = "Fehler";
                    else hoursString = hours.ToString();
                    month = "\n" + DateTime.Now.Month.ToString();

                    switch (DateTime.Now.Month)
                    {
                        case 1:
                            month = "\nJanuar";
                            break;
                        case 2:
                            month = "\nFebruar";
                            break;
                        case 3:
                            month = "\nMärz";
                            break;
                        case 4:
                            month = "\nApril";
                            break;
                        case 5:
                            month = "\nMai";
                            break;
                        case 6:
                            month = "\nJuni";
                            break;
                        case 7:
                            month = "\nJuli";
                            break;
                        case 8:
                            month = "\nAugust";
                            break;
                        case 9:
                            month = "\nSeptember";
                            break;
                        case 10:
                            month = "\nOktober";
                            break;
                        case 11:
                            month = "\nNovember";
                            break;
                        case 12:
                            month = "\nDezember";
                            break;
                        default:
                            month = "\n";
                            break;
                    }
                }

                else hoursString = "Funktion deaktiviert";

                string filename = @"\" + username + ".szt";

                string textStream = username + "\n" + getNameFromExcel() + "\n" + GetDateTime() + "\n" + hoursString + month + "\n" + getTableName();

                File.WriteAllText(folder + filename, (Encrypt(textStream, CreateKey(), CreateIV())));
            }

            catch (Exception ex)
            {
                ERROR("CreateViewFile", ex.Message);
            }
        }

        private string GetDateTime()
        {
            DateTime dt = DateTime.Now;
            return dt.ToString();
        }

        private void LoadSavedSettings()
        {
            //loads userinfo from conf file
            try
            {
                path = Properties.Settings.Default.TablePath;
                lblSync.Text = "Letzte Synchronisierung: " + Properties.Settings.Default.LastSync;
                if (Properties.Settings.Default.LastSync == String.Empty) lblSync.Text = "Letzte Synchronisierung: (noch nicht Synchronisiert)";
            }

            catch (Exception ex)
            {
                ERROR("LoadSavedSettings", ex.Message);
            }
        }

        private void OpenTable()
        {
            //opens the timetable file
            try
            {
                if (CheckProgramExists())
                {
                    Process.Start(programPath, "\"" + path + "\"");
                    btnOpen.Enabled = false;
                    btnOpen.Text = "Geöffnet...";
                    btnSync.Enabled = false;

                    timer.Start();
                }
            }

            catch (Exception ex)
            {
                ERROR("OpenTable", ex.Message);
            }
        }

        private bool CheckSuccessSync(string filename)
        {
            if (File.Exists(filename))
            {
                Properties.Settings.Default.LastSyncSuccess = true;
                Properties.Settings.Default.Save();
                return true;
            }
            else return false;
        }

        private void EncryptAndSync()
        {
            try
            {
                string dateTime = GetDateTime();
                //string dateTimeModified = GetDateTime().Replace(".", "").Replace(":", "").Replace(" ", "").Substring(0, 12);
                string filename = @"\" + username + ".zt";
                string filename2 = @"\" + username + ".szt";

                Byte[] bytes = File.ReadAllBytes(path);
                String file = Convert.ToBase64String(bytes);

                Byte[] bytes2 = Convert.FromBase64String(Encrypt(file, CreateKey(), CreateIV()));
                File.WriteAllBytes(folder + filename, bytes2);

                CreateViewFile();

                //copy file to network location
                File.Copy(folder + filename, targetFolder + filename, File.Exists(targetFolder + filename));
                File.Copy(folder + filename2, viewFilePath + filename2, File.Exists(viewFilePath + filename2));

                if (CheckSuccessSync(targetFolder + filename) && CheckSuccessSync(viewFilePath + filename2))
                {
                    Properties.Settings.Default.LastSync = dateTime;
                    Properties.Settings.Default.Save();

                    lblSync.Text = "Letzte Synchronisierung: " + dateTime;
                    lblSync.ForeColor = System.Drawing.Color.Green;
                    syncedSinceLaunch = true;
                }

                else
                {
                    lblSync.Text = "Synchronisierung fehlgeschlagen! Letzte erfolgreiche Sync.: " + Properties.Settings.Default.LastSync;
                    lblSync.ForeColor = System.Drawing.Color.Red;
                }
            }

            catch (Exception ex)
            {
                ERROR("EncryptAndSync", ex.Message);
            }
        }

        private bool SelectFilePath(string currentFilename)
        {
            if (currentFilename != String.Empty) openFileDialog.FileName = currentFilename;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                llblPath.Text = openFileDialog.FileName;

                Properties.Settings.Default.TablePath = openFileDialog.FileName;
                Properties.Settings.Default.Save();

                return true;
            }

            return false;
        }

        protected virtual bool FileInUse(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Encryption

        private string Encrypt(string decrypted, string key, string iv)
        {
            //encrypts the timetable file
            try
            {
                byte[] textbytes = ASCIIEncoding.ASCII.GetBytes(decrypted);
                AesCryptoServiceProvider encdec = new AesCryptoServiceProvider();
                encdec.BlockSize = 128;
                encdec.KeySize = 128;
                encdec.Key = ASCIIEncoding.ASCII.GetBytes(key);
                encdec.IV = ASCIIEncoding.ASCII.GetBytes(iv);
                encdec.Padding = PaddingMode.PKCS7;
                encdec.Mode = CipherMode.CBC;

                ICryptoTransform icrypt = encdec.CreateEncryptor(encdec.Key, encdec.IV);

                byte[] enc = icrypt.TransformFinalBlock(textbytes, 0, textbytes.Length);
                icrypt.Dispose();

                return Convert.ToBase64String(enc);
            }

            catch (Exception ex)
            {
                //ERROR("Encrypt", ex.Message);
                return null;
            }
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private String CreateKey()
        {
            string key = GetHashString(username + username.Length).Substring(0, 32);
            return key;
        }

        private String CreateIV()
        {
            return GetHashString(username).Substring(0, 16);
        }

        #endregion

        #region GUI

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSavedSettings();
            ClearCreateFolder();
            ReadConfig();

            //username in form title
            this.Text = this.Text += " (" + username + ")";
            llblPath.Text = path;

            //copyright year
            if (DateTime.Now.Year > 2023)
            {
                lblCopyright.Text = "Copyright © 2022 - " + DateTime.Now.Year + " Christoph Beyer, Schindler AG";
            }

            //check if last sync was successfull, if not, sync new
            if (File.Exists(path) && syncmode == 2 && Properties.Settings.Default.LastSyncSuccess == false) EncryptAndSync();
            else if (File.Exists(path) && syncmode == 3) EncryptAndSync();
        }
 

        private void btnOpen_Click(object sender, EventArgs e)
        {
            lblSync.ForeColor = System.Drawing.Color.Black;

            if (path != String.Empty && File.Exists(path))
            {
                OpenTable();
            }

            else
            {
                if (SelectFilePath(path)) OpenTable();
            }

            //update info about last sync success
            Properties.Settings.Default.LastSyncSuccess = false;
            Properties.Settings.Default.Save();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Thread.Sleep(1000);
            FileInfo fInf = new FileInfo(path);

            if (!FileInUse(fInf))
            {
                if (syncmode > 0) EncryptAndSync();
                timer.Stop();

                btnOpen.Text = "Ö&ffnen";
                btnOpen.Enabled = true;
                btnSync.Enabled = true;
            }
        }

        private void llblPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SelectFilePath(path);
        }

        private void btnReportError_Click(object sender, EventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                if (File.Exists(logFilePath)) Process.Start("notepad.exe", logFilePath);

                else MessageBox.Show("Keine Log-Datei vorhanden");
            }

            else if (ModifierKeys.HasFlag(Keys.Shift) && ModifierKeys.HasFlag(Keys.Alt))
            {
                try
                {
                    Properties.Settings.Default.LastSync = Properties.Settings.Default.LastSync;
                    Properties.Settings.Default.LastSyncSuccess = Properties.Settings.Default.LastSyncSuccess;
                    Properties.Settings.Default.ProgramPathUser = Properties.Settings.Default.ProgramPathUser;
                    Properties.Settings.Default.TablePath = Properties.Settings.Default.TablePath;
                    Properties.Settings.Default.syncmode = Properties.Settings.Default.syncmode;
                    Properties.Settings.Default.EnableScaling = Properties.Settings.Default.EnableScaling;
                    Properties.Settings.Default.Save();
                    Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath));
                }

                catch (Exception ex)
                {
                    ERROR("OpenAppDataFolder(*)", ex.Message);
                }
            }

            else ReportError();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            FormAbout frmAbt = new FormAbout();
            frmAbt.ShowDialog();
        }

        private void btnReportError_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (forcedSyncClosing == 1 && !syncedSinceLaunch) EncryptAndSync();
            else if (forcedSyncClosing == 2) EncryptAndSync();
        }

        private void llblPath_TextChanged(object sender, EventArgs e)
        {
            string llblPathText = llblPath.Text;

            if (llblPathText.Length < 75)
            {
                llblPath.Text = llblPathText;
                toolTip.SetToolTip(llblPath, "Dateipfad der Excel Tabelle ändern");
            }
            else
            {
                llblPath.Text = llblPathText.Substring(0, 74) + "...";
                toolTip.SetToolTip(llblPath, "Dateipfad der Excel Tabelle ändern:\n" + llblPathText);
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            lblSync.ForeColor = System.Drawing.Color.Black;

            if (path != String.Empty && File.Exists(path))
            {
                EncryptAndSync();
            }

            else if (path != String.Empty && !File.Exists(path))
            {
                lblSync.ForeColor = System.Drawing.Color.Red;

                MessageBox.Show("Die Datei \"" + path + "\" konnte nicht gefunden werden.\n\nKlicken sie auf 'Öffnen' oder auf den Pfad um eine Datei auszuwählen." +
                    "\n\n\nBenötigen Sie Hilfe? Wenden sie sich an folgende Adresse: " + supportEmail, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else if (path == String.Empty) SelectFilePath(path);
        }

        #endregion
    }
}