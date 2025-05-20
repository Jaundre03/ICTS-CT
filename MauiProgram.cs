using Microsoft.Extensions.Logging;
using ICTS_CT.Views;
using ICTS_CT.ViewModels;
using ICTS_CT.Services;
using OfficeOpenXml;


namespace ICTS_CT;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		ExcelPackage.License.SetNonCommercialPersonal("Jhon Carlo Obor");

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register Views
		builder.Services.AddSingleton<MainPage>();

		// Register ViewModels
		builder.Services.AddSingleton<MainPageViewModel>();

		// Register Services
		builder.Services.AddSingleton<ExcelService>();
		builder.Services.AddSingleton<DataService>();

		return builder.Build();
	}
}
