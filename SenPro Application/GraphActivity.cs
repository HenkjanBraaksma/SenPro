using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Plugin.BLE;

using Xamarin.Forms;

using OxyPlot.Xamarin.Android;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SenPro_Application
{
    /// <summary>
    /// This is the activity class dealing with the graph screen, and as such,
    /// the heart of the application.
    /// </summary>
    [Activity(Label = "SenPro Graph")]
    public class GraphActivity : Activity
    {
        bool currentlyListening;

        string deviceId;
        int notificationService = 2;
        int notificationCharacteristic = 1;

        PlotView plot;
        SaMi samiClient;
        Android.Widget.Button quitButton;

        /// <summary>
        /// This method gets called the moment the activity is called. It sets up all
        /// the basics in regards to the graph plotter, the connection to SaMi, and
        /// finishes by starting the process of listening to the BLE Shield's notifications.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        async protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Forms.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.graphlayout);

            var adapter = CrossBluetoothLE.Current.Adapter;
            adapter.DeviceConnectionLost += Adapter_ConnectionLost;

            plot = FindViewById<PlotView>(Resource.Id.plotview);
            plot.Model = CreatePlotModel();

            string patientName = Intent.GetStringExtra("Patient Name") ?? "ERROR: User Unknown";
            samiClient = new SaMi(patientName);

            quitButton = FindViewById<Android.Widget.Button>(Resource.Id.stopButton);
            quitButton.Click += QuitButton_Click;

            ManageSubscription(true);
        }

        /// <summary>
        /// This method is called whenever the user returns to the activity, and is
        /// there to ensure that the system resubscribes after a lost connection when
        /// necessary.
        /// </summary>
        protected override void OnRestart()
        {
            base.OnRestart();
            if (!currentlyListening)
            {
                ManageSubscription(true);
                currentlyListening = true;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            samiClient.SaveResults();
            ManageSubscription(false);

        }

        //Plot-related Methods

        /// <summary>
        /// This creates the necessary PlotModel object for the graph to function
        /// </summary>
        /// <returns></returns>
        private PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = "Sensors" };

            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom,
                Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(2)),
                Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(-8)),
                TextColor = OxyColors.White,
                AxislineColor = OxyColors.White,
                TitleColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                StringFormat = "HH:mm:ss",
                IsZoomEnabled = false,
                IsPanEnabled = false
            });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,
                Maximum = 1023,
                Minimum = 0,
                TextColor = OxyColors.White,
                AxislineColor = OxyColors.White,
                TitleColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                IsZoomEnabled = false,
                IsPanEnabled = false});


            var series0 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Red
            };


            var series1 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Green
            };


            var series2 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Blue
            };


            var series3 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Yellow
            };

            var series4 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Black
            };

            var series5 = new LineSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.White
            };
            plotModel.Series.Add(series0);
            plotModel.Series.Add(series1);
            plotModel.Series.Add(series2);
            plotModel.Series.Add(series3);
            plotModel.Series.Add(series4);
            plotModel.Series.Add(series5);


            return plotModel;
        }

        //Bluetooth Methods

        /// <summary>
        /// Locates the characteristic that sends the messages from Arduino, and
        /// sets up a handler that fires up whenever the characteristic's value changes.
        /// For the RedBearLabs BLE shield, the correct characteristic index is Service[2], Characteristic[1].
        /// If "add" is true, the application subscribes. If it is false, the application unsubscribes.
        /// </summary>
        /// <returns></returns>
        async private void ManageSubscription(bool add)
        {
            var adapter = CrossBluetoothLE.Current.Adapter;
            var device = adapter.ConnectedDevices[0];
            deviceId = device.Id.ToString();
            var services = await device.GetServicesAsync();
            var service = services[notificationService];
            var characteristics = await service.GetCharacteristicsAsync();
            var characteristic = characteristics[notificationCharacteristic];

            if (add)
            {
                characteristic.ValueUpdated += Characteristic_ValueUpdated;
                await characteristic.StartUpdatesAsync();
                currentlyListening = true;
            }
            else
            {
                characteristic.ValueUpdated -= Characteristic_ValueUpdated;
                await characteristic.StopUpdatesAsync();
                currentlyListening = false;
            }

        }

        /// <summary>
        /// This handler fires when the application loses connection with the Arduino,
        /// and fires up the Reconnect activity.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_ConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            currentlyListening = false;
            var reconnectActivity = new Intent(this, typeof(ReconnectActivity));
            reconnectActivity.PutExtra("GUID", deviceId);
            StartActivity(reconnectActivity);
        }

        /// <summary>
        /// This is the event handler that runs every time the Arduino sends a new message.
        /// It parses the message to an array and sends the values to both the graph
        /// and the SaMi object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Characteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            string valueString = Encoding.Default.GetString(e.Characteristic.Value);
            int count = valueString.Length - valueString.Replace(",", "").Length;
            if (count != 5 || valueString[0] == ',')
                return;
            double[] values;

            /*This part is wrapped in a try/catch because BLE messages can be a bit jumbled when
            connection has just been established. Jumbled message won't parse, of course,
            so they get skipped.*/
            try
            {
                values = Array.ConvertAll(valueString.Split(','), Double.Parse);
            }
            catch
            {
                return;
            }
            SenProReading newReading = new SenProReading { DateTime = DateTime.Now, Values = values };
            double dateTimePlot = DateTimeAxis.ToDouble(newReading.DateTime);

            (plot.Model.Series[0] as LineSeries).Points.Add(new DataPoint(dateTimePlot, newReading.Values[0]));
            (plot.Model.Series[1] as LineSeries).Points.Add(new DataPoint(dateTimePlot, newReading.Values[1]));
            (plot.Model.Series[2] as LineSeries).Points.Add(new DataPoint(dateTimePlot, newReading.Values[2]));
            (plot.Model.Series[3] as LineSeries).Points.Add(new DataPoint(dateTimePlot, newReading.Values[3]));
            (plot.Model.Series[4] as LineSeries).Points.Add(new DataPoint(dateTimePlot, newReading.Values[4]));
            (plot.Model.Series[5] as LineSeries).Points.Add(new DataPoint(dateTimePlot, newReading.Values[5]));

            if ((plot.Model.Series[0] as LineSeries).Points.Count > 15)
            {
                (plot.Model.Series[0] as LineSeries).Points.RemoveAt(0);
                (plot.Model.Series[1] as LineSeries).Points.RemoveAt(0);
                (plot.Model.Series[2] as LineSeries).Points.RemoveAt(0);
                (plot.Model.Series[3] as LineSeries).Points.RemoveAt(0);
                (plot.Model.Series[4] as LineSeries).Points.RemoveAt(0);
                (plot.Model.Series[5] as LineSeries).Points.RemoveAt(0);
            }

            plot.Model.Axes[0].Maximum = DateTimeAxis.ToDouble(newReading.DateTime.AddSeconds(2));
            plot.Model.Axes[0].Minimum = DateTimeAxis.ToDouble(newReading.DateTime.AddSeconds(-8));

            samiClient.AddMeasurement(newReading);

            if(samiClient.MeasurementMinuteCheck())
            {
                samiClient.SaveResults();
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                plot.InvalidatePlot();
            });
        }
        //This event handler fires when someone presses the quit button.
        //It saves all current measurements and closes the graph.
        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Finish();
        }
    }
}