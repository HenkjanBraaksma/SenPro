using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

using System.Threading.Tasks;

using Plugin.BLE;
using System;

namespace SenPro_Application
{
    /// <summary>
    /// This activity provides the user with interactivity to start the measurement
    /// at their leisure and tie a patient's name to a readings session.
    /// </summary>
    [Activity(Label = "SenPro")]
    public class StartUpActivity : Activity
    {

        Button connectButton;
        Button subscribeButton;
        EditText patientName;
        TextView infoText;

        /// <summary>
        /// Basic Oncreate for connecting layout elements to code.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.Main);

            connectButton = FindViewById<Button>(Resource.Id.connect);
            subscribeButton = FindViewById<Button>(Resource.Id.subscribe);
            infoText = FindViewById<TextView>(Resource.Id.info);
            patientName = FindViewById<EditText>(Resource.Id.patientName);

            connectButton.Click += ConnectButton_Click;
            subscribeButton.Click += SubscribeButton_Click;

        }
        /// <summary>
        /// Makes sure the device properly disconnects to the Arduino before
        /// closing the app: Otherwise, the device remains connected while
        /// nothing is done with it.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            var adapter = CrossBluetoothLE.Current.Adapter;
            if(adapter.DiscoveredDevices.Count > 0)
            {
                adapter.DisconnectDeviceAsync(adapter.DiscoveredDevices[0]);
            }
        }

        //Event Handlers
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            AttemptConnection();
        }

        /// <summary>
        /// Starts the Graph activity, and also transmits the input in the
        /// patient name textbox to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubscribeButton_Click(object sender, EventArgs e)
        {
            string patientNameInput = patientName.Text;
            if (patientNameInput.Trim() == "")
                patientNameInput = "Anonymous User";
            var graphActivity = new Intent(this, typeof(GraphActivity));
            graphActivity.PutExtra("Patient Name", patientNameInput);
            StartActivity(graphActivity);
        }

        //Bluetooth Methods
        /// <summary>
        /// Starts the process of looking for devices.
        /// </summary>
        /// <returns></returns>
        async Task AttemptConnection()
        {
            var ble = CrossBluetoothLE.Current;
            var adapter = CrossBluetoothLE.Current.Adapter;
            infoText.Text = "Looking for the SenPro device...";
            adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
            await adapter.StartScanningForDevicesAsync();
        }

        /// <summary>
        /// When a device is found, a check is run to see if the name of the device
        /// matches the name of the Arduino device. If so, proper connection is established.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            var adapter = CrossBluetoothLE.Current.Adapter;
            if (e.Device.Name != "SenProBLE")
                return;

            try
            {
                infoText.Text = "Device found, attempting connection...";
                adapter.ConnectToDeviceAsync(e.Device);
                adapter.DeviceConnected += (s, a) => 
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        connectButton.Enabled = false;
                        subscribeButton.Enabled = true;
                        infoText.Text = "Connection established.";
                    });
                    adapter.StopScanningForDevicesAsync();
                };
            }
            catch (Exception exception)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    infoText.Text = "Failed to establish connection.";
                });
            }
        }
    }
}

