using PointOfSale.Pages.Handheld;

namespace PointOfSale;

public partial class AppShellMobile : Shell
{
	public AppShellMobile()
	{
		InitializeComponent();

		InitRoutes();
	}

	private void InitRoutes()
	{
		Routing.RegisterRoute(nameof(OrderDetailsPage), typeof(OrderDetailsPage));
		Routing.RegisterRoute(nameof(TipPage), typeof(TipPage));
		Routing.RegisterRoute(nameof(PayPage), typeof(PayPage));
		Routing.RegisterRoute(nameof(SignaturePage), typeof(SignaturePage));
		Routing.RegisterRoute(nameof(ReceiptPage), typeof(ReceiptPage));
        Routing.RegisterRoute(nameof(ScanPage), typeof(ScanPage));
    }
}
