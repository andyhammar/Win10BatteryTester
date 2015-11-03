using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Devices.Power;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BatteryTester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _timer;

        public MainPage()
        {
            this.InitializeComponent();
            _textBlock.Text = ApplicationData.Current.LocalSettings.Values["batteryLog"] as string ?? string.Empty;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(10000);
                _timer.Tick += _timer_Tick;
                    _timer.Start();
            }

            await HandleTimerTick();
        }

        private async void _timer_Tick(object sender, object e)
        {
            await HandleTimerTick();
        }

        private async Task HandleTimerTick()
        {
            var report = Battery.AggregateBattery.GetReport();
            if (report.Status == Windows.System.Power.BatteryStatus.NotPresent)
            {
                _timer.Stop();
                await new MessageDialog("no battery present").ShowAsync();
                return;
            }
            var batteryLevel = 100 * (double)report.RemainingCapacityInMilliwattHours / report.FullChargeCapacityInMilliwattHours;
            var acStatus = report.Status;
            var logLine = $"{DateTime.Now.ToString(new CultureInfo("sv-se"))} [{acStatus}]{batteryLevel}";
            _textBlock.Text = logLine + Environment.NewLine + _textBlock.Text;
            ApplicationData.Current.LocalSettings.Values["batteryLog"] = _textBlock.Text;
        }
    }
}
