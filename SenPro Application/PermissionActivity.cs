using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SenPro_Application
{
    /// <summary>
    /// This activity, set up as the activity that starts when the user boots the app,
    /// checks for necessary permissions and requests them if they are not granted yet.
    /// </summary>
    [Activity(Label = "Checking Permissions", MainLauncher = true)]
    public class PermissionActivity : Activity
    {
        TextView message;

        readonly string[] neededPermissions =
{
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation,
                Manifest.Permission.Bluetooth,
                Manifest.Permission.BluetoothAdmin
            };
        const int permissionsId = 0;

        string permission = Manifest.Permission.BluetoothAdmin;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SingleMessage);
            message = FindViewById<TextView>(Resource.Id.singlemessage);
            message.Text = "Checking and requesting permissions if necessary...";
            GetPermissions();
        }

        /// <summary>
        /// Checks if permissions have been granted, and if not, requests them.
        /// If they aren't granted after the first time, the application shuts itself down.
        /// </summary>
        /// <returns></returns>
        async Task GetPermissions()
        {
            if(CheckAppPermissions())
            {
                var mainActivity = new Intent(this, typeof(StartUpActivity));
                StartActivity(mainActivity); ;
            }
            else
            {
                RequestPermissions(neededPermissions, permissionsId);
                if (CheckAppPermissions())
                {
                    var mainActivity = new Intent(this, typeof(StartUpActivity));
                    StartActivity(mainActivity); ;
                }
            }
            this.Finish();
        }

        /// <summary>
        /// Checks with the Android Permission Manager if the specific permissions
        /// we need have already been granted or not.
        /// </summary>
        /// <returns></returns>
        public bool CheckAppPermissions()
        {
            return (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted);
        }
    }
}