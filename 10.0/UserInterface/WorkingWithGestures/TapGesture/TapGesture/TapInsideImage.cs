using System.Diagnostics;

namespace TapGesture
{
	public class TapInsideImage : ContentPage
	{
		int tapCount;
		readonly Label label;

		public TapInsideImage()
		{
			var image = new Image
			{
				Source = "tapped.jpg",

				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};


			var tapGestureRecognizer = new TapGestureRecognizer();
//			tapGestureRecognizer.NumberOfTapsRequired = 2; // double-tap
			tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;
			image.GestureRecognizers.Add(tapGestureRecognizer);


			label = new Label
			{
				Text = "tap the photo!",
				FontSize = 18,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			Content = new StackLayout
			{
				Padding = new Thickness(20, 100),

				Children =
				{
					image,
					label
				}
			};
		}

		void OnTapGestureRecognizerTapped(object sender, EventArgs args)
		{
			tapCount++;
			label.Text = String.Format("{0} tap{1} so far!",
				tapCount,
				tapCount == 1 ? "" : "s");

			var imageSender = (Image)sender;

			// watch the monkey go from color to black&white!
			if (tapCount % 2 == 0) {
				imageSender.Source = "tapped.jpg";
			} else {
				imageSender.Source = "tapped_bw.jpg";
			}
			Debug.WriteLine ("image tapped: " + ((FileImageSource)imageSender.Source).File);
		}
	}
}

