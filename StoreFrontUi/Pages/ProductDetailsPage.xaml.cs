﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using StoreFrontModel;
using StoreFrontRepository;

namespace StoreFrontUi.Pages
{

   
        public partial class ProductDetailsPage : Page
    {
        public ObservableCollection<ProductVariant> Variant { get; set; }
        private int _currentImageIndex = 0;
        private SizeStock _selectedSize;

        public Product StoreProduct
        {
            get { return (Product)GetValue(StoreProductProperty); }
            set { SetValue(StoreProductProperty, value); }
        }

        public static readonly DependencyProperty StoreProductProperty =
            DependencyProperty.Register("StoreProduct", typeof(Product), typeof(ProductDetailsPage), new PropertyMetadata(null));

        public ProductVariant SelectedVariant
        {
            get { return (ProductVariant)GetValue(SelectedVariantProperty); }
            set { SetValue(SelectedVariantProperty, value);
                _currentImageIndex = 0;

                if (value != null && value.Sizes != null)
                {
                    AvailableSizes = new ObservableCollection<SizeStock>(value.Sizes);

                    SelectedSize = value.Sizes.FirstOrDefault(s => s.Stock > 0);
                }



                Product_Img?.GetBindingExpression(Image.SourceProperty)?.UpdateTarget();

                if(HasDiscount)
                {
                   Tb_discountPrice.Text = string.Format("{0:C}", DiscountedPrice);
                
                }
                else
                {
                    Tb_discountPrice.TextDecorations = null;
                    Tb_discountPrice.Text = string.Empty;
                }
                Tb_Price.GetBindingExpression(TextBlock.TextDecorationsProperty)?.UpdateTarget();


            }
        }

        public static readonly DependencyProperty SelectedVariantProperty =
            DependencyProperty.Register("SelectedVariant", typeof(ProductVariant), typeof(ProductDetailsPage), new PropertyMetadata(null));


        public ObservableCollection<SizeStock> AvailableSizes
        {
            get { return (ObservableCollection<SizeStock>)GetValue(AvailableSizesProperty); }
            set { SetValue(AvailableSizesProperty, value); }
        }

        public static readonly DependencyProperty AvailableSizesProperty =
            DependencyProperty.Register("AvailableSizes", typeof(ObservableCollection<SizeStock>), typeof(ProductDetailsPage), new PropertyMetadata(null));



        public SizeStock SelectedSize
        {
            get { return (SizeStock)GetValue(SelectedSizeProperty); }
            set { SetValue(SelectedSizeProperty, value); }
        }

        public static readonly DependencyProperty SelectedSizeProperty =
            DependencyProperty.Register("SelectedSize", typeof(SizeStock), typeof(ProductDetailsPage), new PropertyMetadata(null));



        public string CurrentImageUrl
        {
            get
            {
                if (SelectedVariant?.Photos != null && SelectedVariant.Photos.Count > _currentImageIndex)
                {
                    return SelectedVariant.Photos[_currentImageIndex].Url;
                }
                return null;
            }
        }
        public bool HasDiscount
        {
            get
            {
                return SelectedVariant?.Discount != null &&
                       SelectedVariant.Discount.Percentage > 0 &&
                       (SelectedVariant.Discount.ValidFrom == null || DateTime.Now >= SelectedVariant.Discount.ValidFrom) &&
                       (SelectedVariant.Discount.ValidTo == null || DateTime.Now <= SelectedVariant.Discount.ValidTo);
            }
        }

  
        public decimal DiscountedPrice
        {
            get
            {
                if (HasDiscount && SelectedVariant != null)
                {
                    return SelectedVariant.Price * (1 - (decimal)SelectedVariant.Discount.Percentage / 100);
                }
                return 0;
            }
        }

        private MainWindow parentWindow;
        private IStoreFront storeFront;

        public ProductDetailsPage(Product product)
        {
            InitializeComponent();
            storeFront = new StoreFrontRepository.StoreFrontRepository();
            StoreProduct = product;
            parentWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            Variant = new ObservableCollection<ProductVariant>(product.Variants);


            if (product.Variants != null && product.Variants.Count > 0)
            {
                SelectedVariant = product.Variants[0];

                if (SelectedVariant.Sizes != null && SelectedVariant.Sizes.Count > 0)
                {
                    SelectedSize = SelectedVariant.Sizes.FirstOrDefault(s => s.Stock > 0);
                }

                this.Loaded += async (s, e) =>
                {
                    await WebView_Description.EnsureCoreWebView2Async(null);
                    if (!string.IsNullOrWhiteSpace(StoreProduct.Description))
                    {
                        WebView_Description.NavigateToString($"<html><body style='font-family:sans-serif;color:gray;font-size:14px'>{StoreProduct.Description}</body></html>");
                    }
                };
                this.DataContext = this;
            }

        }
            private void LB_ProductVariants_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
   
                if (LB_ProductVariants.SelectedItem is ProductVariant selectedVariant)
                {
                    SelectedVariant = selectedVariant;
                    _currentImageIndex = 0; // Reset image index when variant changes
   
                }
            }
        private void LB_Sizes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (LB_Sizes.SelectedItem is SizeStock size)
            {
                SelectedSize = size;
            }
        }


        private void PreviousImage_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedVariant?.Photos != null && SelectedVariant.Photos.Count > 0)
            {
                _currentImageIndex--;
                if (_currentImageIndex < 0)
                {
                    _currentImageIndex = SelectedVariant.Photos.Count - 1;
                }
                UpdateImageSource();
            }
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedVariant?.Photos != null && SelectedVariant.Photos.Count > 0)
            {
                _currentImageIndex++;
                if (_currentImageIndex >= SelectedVariant.Photos.Count)
                {
                    _currentImageIndex = 0;
                }
                UpdateImageSource();
            }
        }

        private void UpdateImageSource()
        {

            Product_Img.GetBindingExpression(Image.SourceProperty)?.UpdateTarget();
        }

        private void Btn_AddToCart_Click(object sender, RoutedEventArgs e)
        {

            if (parentWindow.CurrentUser == null)
            {
                MessageBox.Show("You must be logged in to add items to the cart.", "Not Logged In", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var product = StoreProduct.Id;
                var variant = SelectedVariant.Id;
                var size = SelectedSize.Size;



                if (StoreProduct != null && SelectedVariant != null && SelectedSize != null)
                {
                    var cartItem = new CartItem
                    {
                        ProductId = product,
                        VariantId = variant,
                        Size = size,
                        Quantity = 1
                    };
                    
                    parentWindow.StoreCart.AddItem(cartItem);
                    parentWindow.StoreCart.UserId = parentWindow.CurrentUser.Id;
                    parentWindow.StoreCart.CreatedAt = DateTime.UtcNow;
                    parentWindow.StoreCart.UpdatedAt = DateTime.UtcNow;
                    parentWindow.StoreCart.Purchased = false;

                    storeFront.AddOrUpdateCart(parentWindow.StoreCart,parentWindow.CurrentUser.Id);



                    MessageBox.Show("Item Successfully Added");
                    /*  MessageBox.Show($"Item added to cart successfully! \n ProductID: {cartItem.ProductId}," +
                          $" VariantId {cartItem.VariantId}, SelectedSize {cartItem.Size}");*/

                    /*     foreach(var item in parentWindow.StoreCart.Items)
                         {
                             //Console.WriteLine($"\nProductID: {item.ProductId}, VariantId: {item.VariantId}, Size: {item.Size}, Quantity: {item.Quantity}");
                             File.AppendAllText("debug_log.txt", $"\nProductID: {item.ProductId}, VariantId: {item.VariantId}, Size: {item.Size}, Quantity: {item.Quantity}\n");

                         }*/

                }
                else
                {
                    MessageBox.Show("Please select a variant and size before adding to cart.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the item to the cart: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        }
    }
