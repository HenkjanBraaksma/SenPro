using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

using System.Threading.Tasks;

using Plugin.BLE;
using System;

namespace SenPro_Application
{
    [Activity(Label = "SenPro")]
    public class StartUpActivity : Activity
    {

        Button connectButton;
        Button subscribeButton;
        EditText patientName;
        TextView infoText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            connectButton = FindViewById<Button>(Resource.Id.connect);
            subscribeButton = FindViewById<Button>(Resource.Id.subscribe);
            infoText = FindViewById<TextView>(Resource.Id.info);
            patientName = FindViewById<EditText>(Resource.Id.patientName);

            connectButton.Click += ConnectButton_Click;
            subscribeButton.Click += SubscribeButton_Click;

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            var adapter = CrossBluetoothLE.Current.Adapter;
            adapter.DisconnectDeviceAsync(adapter.DiscoveredDevices[0]);
        }

        //Event Handlers
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            AttemptConnection();
        }

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
        async Task AttemptConnection()
        {
            var ble = CrossBluetoothLE.Current;
            var adapter = CrossBluetoothLE.Current.Adapter;
            infoText.Text = "Looking for the SenPro device...";
            adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
            await adapter.StartScanningForDevicesAsync();
        }

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

