using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Member : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _id = string.Empty;
    public string ID
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _displayName = string.Empty;
    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }

    private Dictionary<string, bool> _contributions = new();
    public Dictionary<string, bool> Contributions
    {
        get => _contributions;
        set => SetProperty(ref _contributions, value);
    }

    public bool GetIsChecked(string contribution)
    {
        if (Contributions.TryGetValue(contribution, out bool paid))
            return paid;
        return false;
    }

    public void SetIsChecked(string contribution, bool value)
    {
        Contributions[contribution] = value;
        OnPropertyChanged(nameof(Contributions));
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
