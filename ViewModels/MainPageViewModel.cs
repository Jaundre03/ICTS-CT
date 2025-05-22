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
        private Contribution _selectedContribution = new Contribution("None", 0);

        public ObservableCollection<Contribution> ContributionTypes { get; set; } = new();
        public ObservableCollection<Member> Members { get; set; } = new(); // for internal data
        public ObservableCollection<MemberContributionViewModel> DisplayMembers { get; set; } = new(); // for UI

        public Contribution SelectedContribution
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
            ContributionTypes.Add(new Contribution("None", 0));
            ContributionTypes.Add(new Contribution("Membership Fee", 100)); // Default sample amount

            SelectedContribution = ContributionTypes.First();

            RefreshDisplayMembers();
        }

        public void AddContribution(string name, decimal amount)
        {
            if (!ContributionTypes.Any(c => c.Name == name))
            {
                ContributionTypes.Add(new Contribution(name, amount));
            }

            SelectedContribution = ContributionTypes.First(c => c.Name == name);
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
        private readonly Func<Contribution> _getSelectedContribution;

        public string DisplayName => _member.DisplayName;
        public Member Member => _member;

        public MemberContributionViewModel(Member member, Func<Contribution> getSelectedContribution)
        {
            _member = member;
            _getSelectedContribution = getSelectedContribution;
        }

        public bool IsChecked
        {
            get => _member.GetIsChecked(_getSelectedContribution().Name);
            set
            {
                _member.SetIsChecked(_getSelectedContribution().Name, value);
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null!) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
