﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using MongoDB.Driver;
using StoreFrontModel;
using StoreFrontRepository;
using StoreFrontUi.Utils;

namespace StoreFrontUi
{

    public partial class UserInvoicesWindow : Window
    {

        private IStoreFront storeFront;
        public User User { get; set; }

        public UserInvoicesWindow(User user)
        {
            InitializeComponent();
            User = user;
            storeFront = new StoreFrontRepository.StoreFrontRepository();

             LoadInvoicesAsync();

            this.DataContext = this;
        }


        private async void LoadInvoicesAsync()
        {
          var invoices = await storeFront.GetUserInvoice(User.Id);
            if(invoices == null || invoices.Count == 0)
            {
                MessageBox.Show("No invoices found.");
                return;
            }

            InvoiceListView.ItemsSource = invoices;
        }

        public Invoice SelectedInvoice { get; set; }

        private void InvoiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InvoiceListView.SelectedItem is Invoice selectedInvoice)
            {

                SelectedInvoice = selectedInvoice;
                MessageBox.Show($"Download {selectedInvoice.InvoiceNumber}");
                
            }
        }

        private async void Btn_Download_Click(object sender, RoutedEventArgs e)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("serversettings.json")
                .Build();


            var jasperUrl = configuration["JasperSettings:ServerUrl"];
            var jasperUser = configuration["JasperSettings:Username"];
            var jasperPassword = configuration["JasperSettings:Password"];
            var ReportPath = configuration["JasperSettings:ReportPath"];
            var ParameterName = configuration["JasperSettings:ParameterName"];


            if (SelectedInvoice == null)
            {
                MessageBox.Show("Please select an invoice to download.");
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = "Save Invoice PDF",
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"{SelectedInvoice.InvoiceNumber}.pdf",
                DefaultExt = ".pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var downloader = new InvoiceDownloader(
                        jasperUrl,
                       jasperUser,
                        jasperPassword);
                bool reportDownloaded = await downloader.DownloadReport(
                                    reportUri: ReportPath,
                                    outputPath: @$"C:\Users\Public\Documents\{SelectedInvoice.InvoiceNumber}.pdf",
                                    paramName: ParameterName,
                                    paramValue: SelectedInvoice.InvoiceNumber
                                );

                if (reportDownloaded)
                {
                    MessageBox.Show($"Invoice {SelectedInvoice.InvoiceNumber} downloaded successfully to:\n{saveDialog.FileName}");
                }
                else
                {
                    MessageBox.Show($"Failed to download invoice {SelectedInvoice.InvoiceNumber}.");
                }
            }
        }
    }
}
