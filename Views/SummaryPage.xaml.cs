using ICTS_CT.Models;
using ICTS_CT.ViewModels;
using System.Collections.ObjectModel;


namespace ICTS_CT.Views;

public partial class SummaryPage : ContentPage
{
    public SummaryPage(Contribution selectedContribution, ObservableCollection<Member> members)
    {
        InitializeComponent();
        BindingContext = new SummaryViewModel(selectedContribution, members);
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
