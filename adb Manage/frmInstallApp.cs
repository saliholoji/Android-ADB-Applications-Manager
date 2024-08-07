using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApkNet;
using ApkNet.ApkReader;
using ICSharpCode.SharpZipLib.Zip;

namespace adb_Manage
{
    public partial class frmInstallApp : Form
    {
        public frmInstallApp()
        {
            InitializeComponent();
            pictureBox1.AllowDrop = true;
            pictureBox1.DragEnter += PictureBox1_DragEnter;
            pictureBox1.DragDrop += PictureBox1_DragDrop;
        }
        Thread thr;
        string dosyayolu = "";
        #region Methods

        void WaitForDevices()
        {
            MessageBox.Show("Bağlı bir Android cihaz yoksa cihazı bağlayana kadar beklenecektir, devam etmek için lütfen \"Tamam\" butonuna tıklayın!", "Cihaz Taranıyor...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            lstPaths.Invoke(() =>
            {
                lstPaths.Items[0].SubItems[1].Text = "Waiting...";
                lstPaths.Items[0].ForeColor = Color.Maroon;
            });
            ADB.Shell(" wait-for-device");
            GetConnectedDevices();
        }


        void GetConnectedDevices()
        {
            string Data = ADB.Shell(" devices");
            string[] AllData = Data.Split('\n');
            for (int i = 1; i < AllData.Length; i++)
            {
                Thread thr2 = new Thread(InstallApps);
                thr2.IsBackground = true;
                thr2.Start();
            }
        }

        void InstallApps()
        {
            List<string> Paths = new List<string>();
            lstPaths.Invoke(() =>
            {

                Paths.Add(dosyayolu);

            });
            for (int i = 0; i < Paths.Count; i++)
            {
                lstPaths.Invoke(() =>
                {
                    lstPaths.Items[i].SubItems[1].Text = "Preparing...";
                    lstPaths.Items[0].ForeColor = Color.Yellow;
                });
                File.Copy(Paths[i], "apkfile.apk", true);
                lstPaths.Invoke(() =>
                {
                    lstPaths.Items[i].SubItems[1].Text = "Installing...";
                    lstPaths.Items[i].ForeColor = Color.Blue;
                });
                string Result = ADB.Shell(" install apkfile.apk"); ;
                lstPaths.Invoke(() =>
                {
                    lstPaths.Items[i].SubItems[1].Text = Result;
                    if (Result != "Success")
                    {
                        MessageBox.Show(".APK yüklenirken sorun olmuş olabilir, gelen sonuç: " + Result, "İşlem sonuçlandı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    lstPaths.Items[i].ForeColor = Color.Green;
                });
            }
            btnInstall.Invoke(() => { btnInstall.Enabled = true; });
            dosyayolu = "";

        }
        #endregion
       
        ListViewItem itm = new ListViewItem();
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "APK dosyaları (*.apk)|*.apk";
            openFileDialog.Title = "APK Dosyası Seçin";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string apkDosyaYolu = openFileDialog.FileName;
                dosyayolu = openFileDialog.FileName;

                itm = new ListViewItem();
                lstPaths.Items.Clear();

                itm.Text = Path.GetFileName(dosyayolu);
                itm.SubItems.Add("Ready");
                lstPaths.Items.Add(itm);

                ApkInfo infos = GetApkIcon(apkDosyaYolu);
                if (infos == null)
                {
                    pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\images\noicon.png");
                    MessageBox.Show("APK dosyasından ikon alınamadı. Bu bir sorun değil, sadece bilgilendirmedir!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    Bitmap icon = GetApkIconSon(apkDosyaYolu, infos.iconFileName[0]);
                    if (icon != null)
                    {
                        pictureBox1.Image = icon;
                    }
                }
                catch
                {
                    pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\images\noicon.png");
                    MessageBox.Show("APK dosyasından ikon alınamadı. Bu bir sorun değil, sadece bilgilendirmedir!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
        }


        private Bitmap GetApkIconSon(string apkDosyaYolu, string yol)
        {
            try
            {
                using (var zipArchive = System.IO.Compression.ZipFile.OpenRead(apkDosyaYolu))
                {
                    var iconEntry = zipArchive.GetEntry(yol);
                    if (iconEntry != null)
                    {
                        using (var stream = iconEntry.Open())
                        {
                            return new Bitmap(stream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\images\noicon.png");
                MessageBox.Show("İkon alınırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        private ApkInfo GetApkIcon(string path)
        {

            try
            {
                byte[] manifestData = null;
                byte[] resourcesData = null;
                using (var zip = new ZipInputStream(File.OpenRead(path)))
                {
                    using (var filestream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        var zipfile = new ICSharpCode.SharpZipLib.Zip.ZipFile(filestream);
                        ICSharpCode.SharpZipLib.Zip.ZipEntry item;
                        while ((item = zip.GetNextEntry()) != null)
                        {
                            if (item.Name.ToLower() == "androidmanifest.xml")
                            {
                                manifestData = new byte[50 * 1024];
                                using (var strm = zipfile.GetInputStream(item))
                                {
                                    strm.Read(manifestData, 0, manifestData.Length);
                                }

                            }
                            if (item.Name.ToLower() == "resources.arsc")
                            {
                                using (var strm = zipfile.GetInputStream(item))
                                {
                                    using (var s = new BinaryReader(strm))
                                    {
                                        resourcesData = s.ReadBytes((int)s.BaseStream.Length);
                                    }
                                }
                            }
                        }
                    }
                }

                var apkReader = new ApkReader();
                var info = apkReader.extractInfo(manifestData, resourcesData);

                return info;
            }
            catch
            {
                return null;
            }
        }
        private void PictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            // Sadece dosyaları kabul edelim
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Mouse ikonunu değiştirelim
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void PictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            // Sürüklenen dosyaları alalım
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // İlk dosyanın yolunu MessageBox ile gösterelim
            string firstFilePath = files[0];

            // İlk dosyanın uzantısını alalım
            string fileExtension = Path.GetExtension(firstFilePath);

            // Uzantı .apk ise işlem yapalım
            if (fileExtension.Equals(".apk", StringComparison.OrdinalIgnoreCase))
            {
                string apkDosyaYolu = firstFilePath;

                dosyayolu = firstFilePath;

                itm = new ListViewItem();
                lstPaths.Items.Clear();

                itm.Text = Path.GetFileName(dosyayolu);
                itm.SubItems.Add("Ready");
                lstPaths.Items.Add(itm);

                ApkInfo infos = GetApkIcon(apkDosyaYolu);
                if (infos == null)
                {
                    pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\images\noicon.png");
                    MessageBox.Show("APK dosyasından ikon alınamadı. Bu bir sorun değil, sadece bilgilendirmedir!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    Bitmap icon = GetApkIconSon(apkDosyaYolu, infos.iconFileName[0]);
                    if (icon != null)
                    {
                        pictureBox1.Image = icon;
                    }
                }
                catch
                {
                    pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\images\noicon.png");
                    MessageBox.Show("APK dosyasından ikon alınamadı. Bu bir sorun değil, sadece bilgilendirmedir!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Bırakılan dosya bir APK dosyası değildir!", "APK Dosyası Bulunamadı");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dosyayolu != "")
            {
                thr = new Thread(WaitForDevices);
                thr.IsBackground = true;
                thr.Start();
                btnInstall.Enabled = false;
            }
        }
    }
}
