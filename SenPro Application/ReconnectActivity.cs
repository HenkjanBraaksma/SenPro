using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Plugin.BLE;

namespace SenPro_Application
{
    /// <summary>
    /// This activity automatically keeps trying to reconnect to the Arduino
    /// after connection is lost.
    /// </summary>
    [Activity(Label = "ReconnectActivity")]
    public class ReconnectActivity : Activity
    {
        TextView message;

        async protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SingleMessage);
            message = FindViewById<TextView>(Resource.Id.singlemessage);
            message.Text = "Lost connection, attempting to reconnect...";

            string deviceId = Intent.GetStringExtra("GUID") ?? "Data not available";
            var adapter = CrossBluetoothLE.Current.Adapter;
            adapter.DeviceConnected += (s, e) => { this.Finish(); };
            await adapter.ConnectToKnownDeviceAsync(new Guid(deviceId));
        }
    }
}