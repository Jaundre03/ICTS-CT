using System.Collections.ObjectModel;
using System.ComponentModel;
using ICTS_CT.Models;

namespace ICTS_CT.ViewModels;

public class SummaryViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Member> Members { get; set; }
    public Contribution SelectedContribution { get; set; }

    public string ContributionName => SelectedContribution?.Name ?? "N/A";

    public decimal TotalCollected =>
        Members.Count(m => m.GetIsChecked(SelectedContribution.Name)) * SelectedContribution.Amount;

    public decimal TotalNotCollected =>
        Members.Count(m => !m.GetIsChecked(SelectedContribution.Name)) * SelectedContribution.Amount;

    public event PropertyChangedEventHandler? PropertyChanged;


    public SummaryViewModel(Contribution contribution, ObservableCollection<Member> members)
    {
        SelectedContribution = contribution;
        Members = members;
    }
}
