using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using VisualHEX.Models;

namespace VisualHEX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string strPath = @"C:\Users\posta\Desktop\PIC18\Pic18.X\dist\default\production";
        string strFileName = "Pic18.X.production.hex";

        ZoneHEX zone { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ApriFile(System.IO.Path.Combine(strPath, strFileName));
        }

        private void btnApri_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = strPath;

            if (openFileDialog.ShowDialog() == true)
                ApriFile(openFileDialog.FileName);
        }

        private void ApriFile(string fileName)
        {
            try
            {
                //zone = new ZoneHEX(System.IO.Path.Combine(strPath, strFileName));
                zone = new ZoneHEX(fileName);
                Title = fileName;
                dgDati1.ItemsSource = zone;
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.Message);
            }
        }

        private void dgDati_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ZonaHEX z = dgDati1.SelectedItem as ZonaHEX;
            if( z != null )
            {
                Records rr = z.Records;
                if( rr != null )
                {
                    dgDati2.ItemsSource = null;
                    dgDati2.ItemsSource = rr;


                }
            }
        }

        private void dgDati2_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            RecordHEX r = e.Row.Item as RecordHEX;
            if( r!= null )
            {
                switch( r.Tipo)
                {
                    case RecordType.Data:
                        e.Row.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xD3, 0xD3));
                        break;

                    case RecordType.ExtLinearAddr:
                        e.Row.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xF9, 0xD1));
                        break;

                }
            }
        }
    }
}
