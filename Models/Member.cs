public class Member
{
    public string ID { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsChecked { get; set; }

    // NEW: Track payments per contribution name
    public Dictionary<string, bool> Contributions { get; set; } = new();

    // Helper for UI binding (manages selected contribution state)
    public bool GetIsChecked(string contribution)
    {
        if (Contributions.TryGetValue(contribution, out bool paid))
            return paid;
        return false;
    }

    public void SetIsChecked(string contribution, bool value)
    {
        Contributions[contribution] = value;
    }
}
