using System;
using System.Collections.Generic;
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
        private string _searchQuery = string.Empty;

        public ObservableCollection<Contribution> ContributionTypes { get; set; } = new();
        public ObservableCollection<Member> Members { get; set; } = new(); // Internal data
        public ObservableCollection<MemberContributionViewModel> DisplayMembers { get; set; } = new(); // For UI

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

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
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
            ContributionTypes.Add(new Contribution("Membership Fee", 70)); // Default 

            SelectedContribution = ContributionTypes.First();

            RefreshDisplayMembers();
        }

        public void AddContribution(string name, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(name) || amount <= 0)
                return;

            if (!ContributionTypes.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                ContributionTypes.Add(new Contribution(name, amount));
            }

            SelectedContribution = ContributionTypes.First(c => c.Name == name);
        }

        public void UpdateContribution(string oldName, string newName, decimal newAmount)
        {
            var contribution = ContributionTypes.FirstOrDefault(c => c.Name == oldName);
            if (contribution != null)
            {
                contribution.Name = newName;
                contribution.Amount = newAmount;

                // Update member contributions keys
                foreach (var member in Members)
                {
                    if (member.Contributions.TryGetValue(oldName, out var value))
                    {
                        member.Contributions.Remove(oldName);
                        member.Contributions[newName] = value;
                    }
                }

                RefreshDisplayMembers();
            }
        }

        public void DeleteContribution(string name)
        {
            if (name == "None") return;

            var contribution = ContributionTypes.FirstOrDefault(c => c.Name == name);
            if (contribution != null)
            {
                ContributionTypes.Remove(contribution);

                foreach (var member in Members)
                {
                    member.Contributions.Remove(name);
                }

                SelectedContribution = ContributionTypes.First();
                RefreshDisplayMembers();
            }
        }

        public void LoadImportedMembers(IEnumerable<Member> imported)
        {
            Members.Clear();
            foreach (var m in imported)
                Members.Add(m);

            RefreshDisplayMembers();
        }

        public void RefreshDisplayMembers()
        {
            DisplayMembers.Clear();

            if (SelectedContribution.Name == "None")
                return;

            var filtered = Members.Where(m =>
                string.IsNullOrWhiteSpace(SearchQuery) ||
                m.DisplayName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

            foreach (var member in filtered)
            {
                DisplayMembers.Add(new MemberContributionViewModel(member, () => SelectedContribution));
            }
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

        // Summary info for current contribution
        public (int totalPaid, decimal totalAmount, List<Member> paidMembers) GetSummary()
        {
            var paidMembers = Members
                .Where(m => m.GetIsChecked(SelectedContribution.Name))
                .ToList();

            var totalAmount = paidMembers.Count * SelectedContribution.Amount;

            return (paidMembers.Count, totalAmount, paidMembers);
        }

        // Unpaid list for export
        public IEnumerable<(string ID, string Reason)> GetUnpaidMembersForCurrentContribution()
        {
            var name = SelectedContribution.Name;

            return Members
                .Where(m => !m.GetIsChecked(name))
                .Select(m => (m.ID, $"Did not pay {name}"));
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
