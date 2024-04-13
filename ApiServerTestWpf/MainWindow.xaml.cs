using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace ApiServerTestWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            tbUrlApi.Text = "https://localhost:44369/UploadFiles";
        }

        private void btnStartUpload_Click(object sender, RoutedEventArgs e)
        {

            var url = tbUrlApi.Text;
            Uri uri = null;
            if (string.IsNullOrEmpty(url) && !Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return;
            }


            var dlg = new OpenFileDialog { CheckFileExists = true, InitialDirectory = Environment.CurrentDirectory};

                if (dlg.ShowDialog() == true && !string.IsNullOrEmpty(dlg.FileName))
                {
                    var fileName = dlg.FileName;
                try
                {
                    using (HttpClientHandler hdl = new HttpClientHandler())
                        {
                            using (HttpClient clnt = new HttpClient(hdl))
                            {
                                using (MultipartFormDataContent content = new MultipartFormDataContent())
                                {
                                    using (var fileContent = new ByteArrayContent(File.ReadAllBytes(fileName)))
                                    {
                                        fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                                        {
                                            FileName = Path.GetFileName(fileName),
                                            Name = "file"
                                        };

                                        content.Add(fileContent);

                                        var response = clnt.PostAsync(url, content).Result;

                                        var text = response.Content.ReadAsStringAsync().Result;
                                        MessageBox.Show(text);
                                    }
                                }
                            }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                }

            
            
        }
    }
}
