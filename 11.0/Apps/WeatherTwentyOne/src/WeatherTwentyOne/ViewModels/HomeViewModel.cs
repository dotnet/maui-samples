using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeatherTwentyOne.Models;

namespace WeatherTwentyOne.ViewModels;

public class HomeViewModel : INotifyPropertyChanged
{
    public List<Forecast> Week { get; set; }

    public List<Forecast> Hours { get; set; }

    public Command QuitCommand { get; set; } = new Command(() => {
        Application.Current.Quit();
    });

    public Command AddLocationCommand { get; set; } = new Command(() => {
        // nav to modal form
    });

    public Command<string> ChangeLocationCommand { get; set; } = new Command<string>((location) => {
        // change primary location
    });

    public Command RefreshCommand { get; set; } = new Command(() => {
        // fake a refresh call
    });

    private Command toggleModeCommand;

    public Command ToggleModeCommand {
        get {
            return toggleModeCommand;
        }
        set {
            toggleModeCommand = value;
            OnPropertyChanged();
        }
    }

    public HomeViewModel()
    {
        InitData();

        ToggleModeCommand = new Command(() => {
            App.Current.UserAppTheme = App.Current.UserAppTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
        });
    }

    private void InitData()
    {
        Week = new List<Forecast>
            {
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(1),
                    Day = new Day{ Phrase = "fluent_weather_sunny_high_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 52 }, Maximum = new Maximum { Unit = "F", Value = 77 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(2),
                    Day = new Day{ Phrase = "fluent_weather_partly_cloudy" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 61 }, Maximum = new Maximum { Unit = "F", Value = 82 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(3),
                    Day = new Day{ Phrase = "fluent_weather_rain_showers_day_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 62 }, Maximum = new Maximum { Unit = "F", Value = 77 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(4),
                    Day = new Day{ Phrase = "fluent_weather_thunderstorm_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 57 }, Maximum = new Maximum { Unit = "F", Value = 80 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(5),
                    Day = new Day{ Phrase = "fluent_weather_thunderstorm_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 49 }, Maximum = new Maximum { Unit = "F", Value = 61 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(6),
                    Day = new Day{ Phrase = "fluent_weather_partly_cloudy" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 49 }, Maximum = new Maximum { Unit = "F", Value = 68 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(7),
                    Day = new Day{ Phrase = "fluent_weather_rain_showers_day_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 47 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(1),
                    Day = new Day{ Phrase = "fluent_weather_sunny_high_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 52 }, Maximum = new Maximum { Unit = "F", Value = 77 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(2),
                    Day = new Day{ Phrase = "fluent_weather_partly_cloudy" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 61 }, Maximum = new Maximum { Unit = "F", Value = 82 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(3),
                    Day = new Day{ Phrase = "fluent_weather_rain_showers_day_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 62 }, Maximum = new Maximum { Unit = "F", Value = 77 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(4),
                    Day = new Day{ Phrase = "fluent_weather_thunderstorm_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 57 }, Maximum = new Maximum { Unit = "F", Value = 80 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(5),
                    Day = new Day{ Phrase = "fluent_weather_thunderstorm_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 49 }, Maximum = new Maximum { Unit = "F", Value = 61 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(6),
                    Day = new Day{ Phrase = "fluent_weather_partly_cloudy" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 49 }, Maximum = new Maximum { Unit = "F", Value = 68 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Today.AddDays(7),
                    Day = new Day{ Phrase = "fluent_weather_rain_showers_day_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 47 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                }
            };

        Hours = new List<Forecast>
            {
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(1),
                    Day = new Day{ Phrase = "fluent_weather_rain_showers_day_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 47 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(2),
                    Day = new Day{ Phrase = "fluent_weather_rain_showers_day_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 47 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                }
                ,
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(3),
                    Day = new Day{ Phrase = "fluent_weather_rain_showers_day_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 48 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                }
                ,
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(4),
                    Day = new Day{ Phrase = "fluent_weather_rain_showers_day_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 49 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                }
                ,
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(5),
                    Day = new Day{ Phrase = "fluent_weather_cloudy_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 52 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(6),
                    Day = new Day{ Phrase = "fluent_weather_cloudy_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 53 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(7),
                    Day = new Day{ Phrase = "fluent_weather_cloudy_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 58 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(8),
                    Day = new Day{ Phrase = "fluent_weather_sunny_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 63 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(9),
                    Day = new Day{ Phrase = "fluent_weather_sunny_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 64 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(10),
                    Day = new Day{ Phrase = "fluent_weather_sunny_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 65 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(11),
                    Day = new Day{ Phrase = "fluent_weather_sunny_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 68 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(12),
                    Day = new Day{ Phrase = "fluent_weather_sunny_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 68 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(13),
                    Day = new Day{ Phrase = "fluent_weather_sunny_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 68 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(14),
                    Day = new Day{ Phrase = "fluent_weather_sunny_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 65 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(15),
                    Day = new Day{ Phrase = "fluent_weather_sunny_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 63 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(16),
                    Day = new Day{ Phrase = "fluent_weather_sunny_20_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 60 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(17),
                    Day = new Day{ Phrase = "fluent_weather_moon_16_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 58 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(18),
                    Day = new Day{ Phrase = "fluent_weather_moon_16_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 54 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(19),
                    Day = new Day{ Phrase = "fluent_weather_moon_16_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 53 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(20),
                    Day = new Day{ Phrase = "fluent_weather_moon_16_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 52 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(21),
                    Day = new Day{ Phrase = "fluent_weather_moon_16_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 50 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(22),
                    Day = new Day{ Phrase = "fluent_weather_moon_16_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 47 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                },
                new Forecast
                {
                    DateTime = DateTime.Now.AddHours(23),
                    Day = new Day{ Phrase = "fluent_weather_moon_16_filled" },
                    Temperature = new Temperature{ Minimum = new Minimum{ Unit = "F", Value = 47 }, Maximum = new Maximum { Unit = "F", Value = 67 } },
                }
            };
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
    }

}
