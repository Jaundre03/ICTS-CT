using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ICTS_CT.Models;
using Microsoft.Maui.Controls;

namespace ICTS_CT.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private string _selectedContribution = string.Empty;


        public ObservableCollection<string> ContributionTypes { get; set; } = new();
        public ObservableCollection<Member> Members { get; set; } = new(); // for internal data
        public ObservableCollection<MemberContributionViewModel> DisplayMembers { get; set; } = new(); // for UI

        public string SelectedContribution
        {
            get => _selectedContribution;
            set
            {
                if (_selectedContribution != value)
                {
                    _selectedContribution = value;
                    OnPropertyChanged();
                    RefreshDisplayMembers();
                }
            }
        }

        public ICommand ShowMemberContributionsCommand { get; }

        public MainPageViewModel()
        {
            LoadInitialData();
            ShowMemberContributionsCommand = new Command<Member>(ShowMemberContributions);
        }

        private void LoadInitialData()
        {
            ContributionTypes.Add("None");
            ContributionTypes.Add("Membership Fee");
            SelectedContribution = ContributionTypes.First();

            RefreshDisplayMembers();
        }

        public void AddContribution(string name, decimal amount)
        {
            if (!ContributionTypes.Contains(name))
            {
                ContributionTypes.Add(name);
            }

            SelectedContribution = name;
        }

        public void RefreshDisplayMembers()
        {
            DisplayMembers.Clear();
            foreach (var member in Members)
            {
                DisplayMembers.Add(new MemberContributionViewModel(member, () => SelectedContribution));
            }
        }

        public void LoadImportedMembers(IEnumerable<Member> imported)
        {
            Members.Clear();
            foreach (var m in imported)
                Members.Add(m);

            RefreshDisplayMembers();
        }

        private async void ShowMemberContributions(Member member)
        {
            if (member == null) return;

            var paid = member.Contributions
                .Where(c => c.Value)
                .Select(c => $"- {c.Key}");

            string message = paid.Any()
                ? string.Join("\n", paid)
                : "No contributions paid.";

            await Shell.Current.DisplayAlert(member.DisplayName, message, "OK");
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null!) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class MemberContributionViewModel : INotifyPropertyChanged
    {
        private readonly Member _member;
        private readonly Func<string> _getSelectedContribution;

        public string DisplayName => _member.DisplayName;
        public Member Member => _member; // Needed for CommandParameter binding in XAML

        public MemberContributionViewModel(Member member, Func<string> getSelectedContribution)
        {
            _member = member;
            _getSelectedContribution = getSelectedContribution;
        }

        public bool IsChecked
        {
            get => _member.GetIsChecked(_getSelectedContribution());
            set
            {
                _member.SetIsChecked(_getSelectedContribution(), value);
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null!) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
