namespace ICTS_CT.Models
{
    public class Contribution
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0;

        // ✅ Add this constructor
        public Contribution(string name, decimal amount)
        {
            Name = name;
            Amount = amount;
        }

        // ✅ Add a parameterless constructor if needed by serialization/binding
        public Contribution() { }
    }
}
