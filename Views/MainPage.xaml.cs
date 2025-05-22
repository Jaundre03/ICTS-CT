using ICTS_CT.ViewModels;
using ICTS_CT.Services;
using ICTS_CT.Models;


namespace ICTS_CT.Views;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnAddContributionClicked(object sender, EventArgs e)
    {
        string name;
        while (true)
        {
            name = await DisplayPromptAsync("New Contribution", "Enter contribution name:");
            if (name == null) return; // user canceled

            if (!string.IsNullOrWhiteSpace(name))
                break;

            await DisplayAlert("Invalid Input", "Contribution name cannot be empty.", "OK");
        }

        string amountStr;
        decimal amount;
        while (true)
        {
            amountStr = await DisplayPromptAsync("New Contribution", "Enter amount:");
            if (amountStr == null) return; // user canceled

            if (decimal.TryParse(amountStr, out amount) && amount > 0)
                break;

            await DisplayAlert("Invalid Input", "Please enter a valid positive number for amount.", "OK");
        }

        _viewModel.AddContribution(name.Trim(), amount);
        await DisplayAlert("Success", "Contribution added successfully!", "OK");
    }



    private async void OnImportMembersClicked(object sender, EventArgs e)
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xlsx" } },
                { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                { DevicePlatform.iOS, new[] { "com.microsoft.excel.xlsx" } },
                { DevicePlatform.MacCatalyst, new[] { "com.microsoft.excel.xlsx" } }
            });

            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select Excel File",
                FileTypes = customFileType
            });


            if (result != null)
            {
                var excelService = new ExcelService();
                var stream = await result.OpenReadAsync();

                // Save stream to temporary file
                var tempPath = Path.Combine(FileSystem.CacheDirectory, result.FileName);
                using (var fileStream = File.Create(tempPath))
                    await stream.CopyToAsync(fileStream);

                var importedMembers = excelService.ImportMembersFromExcel(tempPath);
                _viewModel.LoadImportedMembers(importedMembers);

                _viewModel.Members.Clear();
                foreach (var m in importedMembers)
                    _viewModel.Members.Add(m);

                await DisplayAlert("Success", "Members imported successfully", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to import: {ex.Message}", "OK");
        }
    }
    private async void OnSummaryButtonClicked(object sender, EventArgs e)
    {
        if (_viewModel == null || _viewModel.SelectedContribution.Name == "None")
        {
            await DisplayAlert("Invalid", "Please select a valid contribution type.", "OK");
            return;
        }

        var contribution = new Contribution
        {
            Name = _viewModel.SelectedContribution.Name
        };

        await Navigation.PushAsync(new SummaryPage(contribution, _viewModel.Members));
    }

    private async void OnExportUnpaidClicked(object sender, EventArgs e)
    {
        try
        {
            if (_viewModel == null || _viewModel.SelectedContribution.Name == "None")
            {
                await DisplayAlert("Invalid", "Please select a valid contribution type.", "OK");
                return;
            }

            // Get only unpaid members for the currently selected contribution
            var unpaidMembers = _viewModel.GetUnpaidMembersForCurrentContribution(); // IEnumerable<(string ID, string Reason)>
            
            if (!unpaidMembers.Any())
            {
                await DisplayAlert("No Unpaid", "Everyone has paid for this contribution.", "OK");
                return;
            }

            // Prompt user for export path
            var exportPath = await DisplayPromptAsync("Export Unpaid Members", "Enter export file path:", initialValue: Path.Combine(FileSystem.CacheDirectory, "UnpaidMembers.xlsx"));
            // Export using updated method
            var excelService = new ExcelService();
            excelService.ExportUnpaidListToExcel(unpaidMembers, exportPath);

            await DisplayAlert("Export Successful", $"File saved to:\n{exportPath}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Export Failed", $"An error occurred:\n{ex.Message}", "OK");
        }
    }
}
