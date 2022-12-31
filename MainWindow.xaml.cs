using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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

namespace EncryptTest
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSelectFileForEncrypt_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("clicked", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void SCR_Main_Load(object sender, RoutedEventArgs e)
        {
            lblFilepath.Content = @"C:\work\EncryptWork\フルーツ.xlsx";
        }

        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            StreamEncrypter objEncrypter = new StreamEncrypter();
            string strAfterFileName = $@"\暗号化後_{System.IO.Path.GetFileName(@lblFilepath.Content.ToString())}";
            string strAfterFilePath = System.IO.Path.GetDirectoryName(@lblFilepath.Content.ToString())+strAfterFileName;

            using (var inStream = File.Open(@lblFilepath.Content.ToString(), FileMode.Open))
            using (var outStream = File.Open(@strAfterFilePath, FileMode.Create,FileAccess.Write))
            {
                objEncrypter.Encrypt(inStream, outStream);
                Console.WriteLine($"Key:{Convert.ToBase64String(objEncrypter.Key)}");
                Console.WriteLine($"IV:{Convert.ToBase64String(objEncrypter.IV)}");

            }

            lblFilePathforDecrypt.Content = @strAfterFilePath;
            lblKey.Content = Convert.ToBase64String(objEncrypter.Key);
            lblIV.Content = Convert.ToBase64String(objEncrypter.IV);
            MessageBox.Show("暗号完了", "Info", MessageBoxButton.OK, MessageBoxImage.Information);


            using (FileStream fs = new FileStream(strAfterFilePath, FileMode.Open, FileAccess.Write))
            {
                //初期ベクターをファイルの先頭に出力
                fs.Write(objEncrypter.IV, 0, objEncrypter.IV.Length);
            }
                




        }

        private void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            var key = lblKey.Content.ToString(); //"3k93BexTujSrdlwQB0DyNx8cSuIxA22douhCa+eJqFE=";
            var iv = lblIV.Content.ToString(); // "rmcWf+Wfv/B8MPUjjLsv2Q==";
            

            var objEncrypter = new StreamEncrypter(Convert.FromBase64String(key));
            using (var inStream = File.Open(@lblFilePathforDecrypt.Content.ToString(), FileMode.Open))
            using (var outStream = File.Open(@"C:\work\EncryptWork\復号化後_フルーツ.xlsx", FileMode.Create))
            {
                // ファイルの最初の16バイトから初期化ベクターを読み込みます。
                byte[] temp_iv = new byte[16];
                inStream.Read(temp_iv, 0, temp_iv.Length);
                objEncrypter.Decrypt(inStream, outStream, temp_iv);
            }

            MessageBox.Show("復号完了", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
