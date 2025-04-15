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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreFrontUi.UserControls
{

    public partial class UC_NewItem : UserControl
    {
        public UC_NewItem()
        {
            InitializeComponent();
        }



        public StoreFrontModel.Product NewProducts
        {
            get { return (StoreFrontModel.Product)GetValue(NewProductsProperty); }
            set { SetValue(NewProductsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NewProducts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewProductsProperty =
            DependencyProperty.Register("NewProducts", typeof(StoreFrontModel.Product), typeof(UC_NewItem), new PropertyMetadata(null));


    }
}
