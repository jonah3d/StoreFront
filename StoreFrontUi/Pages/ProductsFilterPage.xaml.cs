﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using StoreFrontModel;
using StoreFrontRepository;

namespace StoreFrontUi.Pages
{

    public partial class ProductsFilterPage : Page
    {
        private IStoreFront storeFront;
        private MainWindow parentWindow;
        private string selectedCategory;
        public ObservableCollection<Product> Shoes { get; set; }

        public ProductsFilterPage(string category)
        {
            InitializeComponent();
            selectedCategory = category;
            parentWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            storeFront = new StoreFrontRepository.StoreFrontRepository();
            this.DataContext = this;

            // Use this pattern for async calls from constructor
            Loaded += async (s, e) => {
                await NavigatedFilter();
            };
        }

        public async Task NavigatedFilter()
        {

            switch (selectedCategory)
            {
                case "men_cat":
                    ckb_men.IsChecked = true;
                    await LoadMenShoes();
                    break;
                case "women_cat":
                    ckb_women.IsChecked = true;
                    break;
                case "children_cat":
                    ckb_children.IsChecked = true;
                    break;

            }
        }

        public async Task LoadMenShoes()
        {
            try
            {
                LoadingProgressBar.Visibility = Visibility.Visible;
                LoadingProgressBar.IsIndeterminate = true;

                var menShoes = await storeFront.GetAllMenProduct();

                Shoes = new ObservableCollection<Product>(menShoes);
                ProductsGrid.ItemsSource = Shoes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading shoes: {ex.Message}");
            }
            finally
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

    }
}

