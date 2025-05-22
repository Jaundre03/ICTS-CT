using System;
using Microsoft.Maui.Controls;

namespace ICTS_CT.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnLoginClicked(object sender, EventArgs e)
        {
            // Null-check to avoid CS8602 warnings
            if (usernameEntry?.Text == null || passwordEntry?.Text == null)
            {
                errorLabel.Text = "Please enter both username and password.";
                errorLabel.IsVisible = true;
                return;
            }

            string username = usernameEntry.Text.Trim();
            string password = passwordEntry.Text;

            // Simple hardcoded login logic (replace with real auth later)
            if (username == "admin" && password == "123")
            {
                // Navigate to AppShell (safe null-forgiving operator used)
                Application.Current!.MainPage = new AppShell();
            }
            else
            {
                errorLabel.Text = "Invalid username or password.";
                errorLabel.IsVisible = true;
            }
        }
    }
}
