using System.Collections.ObjectModel;
using System.ComponentModel;
using ICTS_CT.Models;

namespace ICTS_CT.ViewModels
{
    public class SummaryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Contribution SelectedContribution { get; set; }
        public ObservableCollection<Member> Members { get; set; }

        public SummaryViewModel(Contribution contribution, ObservableCollection<Member> members)
        {
            SelectedContribution = contribution;
            Members = members;

            foreach (var member in Members)
            {
                member.PropertyChanged += OnMemberPropertyChanged;
            }

            Members.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Member newMember in e.NewItems)
                    {
                        newMember.PropertyChanged += OnMemberPropertyChanged;
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (Member oldMember in e.OldItems)
                    {
                        oldMember.PropertyChanged -= OnMemberPropertyChanged;
                    }
                }

                OnPropertyChanged(nameof(TotalCollected));
                OnPropertyChanged(nameof(TotalNotCollected));
            };
        }

        private void OnMemberPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Member.IsChecked))
            {
                OnPropertyChanged(nameof(TotalCollected));
                OnPropertyChanged(nameof(TotalNotCollected));
            }
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public int TotalCollected =>
            Members.Count(m => m.IsChecked && m.Contributions.Any(c => c.Key == SelectedContribution.Name));

        public int TotalNotCollected =>
            Members.Count(m => !m.IsChecked || !m.Contributions.Any(c => c.Key == SelectedContribution.Name));
    }
}
