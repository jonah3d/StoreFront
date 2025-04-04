﻿using StoreFrontModel;
using StoreFrontUi.Utils;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace StoreFrontUi
{
    public partial class MainWindow : Window
    {


        public User CurrentUser
        {
            get { return (User)GetValue(CurrentUserProperty); }
            set { SetValue(CurrentUserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentUser.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentUserProperty =
            DependencyProperty.Register("CurrentUser", typeof(User), typeof(MainWindow), new PropertyMetadata(null));



        public MainWindow()
        {
            InitializeComponent();
            MainFramePage.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            NavigateToMainPage();
            Popup_Login.StaysOpen = false;

            this.DataContext = this;
        }

        public void NavigateToMainPage()
        {
            MainFramePage.Navigate(new Pages.MainPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFramePage.CanGoBack)
            {
                MainFramePage.GoBack();
            }
        }

        private void MainFramePage_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            BackButton.GetBindingExpression(Button.VisibilityProperty)?.UpdateTarget();
        }

        public void NavigateToCreateUserPage()
        {
            MainFramePage.Navigate(new Pages.CreateUserPage());
        }


        private void Btn_UserAcc_MouseEnter(object sender, MouseEventArgs e)
        {
            if (CurrentUser != null && CurrentUser.ProfileImage != null)
            {
             
                Popup_loggedInUser.IsOpen = true;
                Popup_Login.IsOpen = false;
            }
            else
            {
                Popup_Login.IsOpen = true;
                Popup_loggedInUser.IsOpen = false;
            }
        }


        private void Btn_UserAcc_MouseLeave(object sender, MouseEventArgs e)
        {
            if (CurrentUser != null)
            {
               
                if (!Popup_loggedInUser.IsMouseOver && !Btn_UserAcc.IsMouseOver)
                {
                    Popup_loggedInUser.IsOpen = false;
                }
            }
            else
            {
             
                if (!Popup_Login.IsMouseOver && !Btn_UserAcc.IsMouseOver)
                {
                    Popup_Login.IsOpen = false;
                }
            }
        }

        private void Popup_Login_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Btn_UserAcc.IsMouseOver && !Popup_Login.IsMouseOver)
            {
                Popup_Login.IsOpen = false;
            }
        }

        private void Popup_loggedInUser_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Btn_UserAcc.IsMouseOver && !Popup_loggedInUser.IsMouseOver)
            {
                Popup_loggedInUser.IsOpen = false;
            }
        }

        public void SetUpCurrentUser()
        {
            if (CurrentUser != null && CurrentUser.ProfileImage != null)
            {
                Btn_UserAcc.Background = ConvertImageToBrush.ConvertToImageBrush(CurrentUser.ProfileImage);
                Btn_Tb_Icon.Visibility = Visibility.Collapsed;
            }
            else
            {
                Btn_UserAcc.Background = new SolidColorBrush(Colors.Transparent);
                Btn_Tb_Icon.Visibility = Visibility.Visible;
            }

            
        }
    }
}
