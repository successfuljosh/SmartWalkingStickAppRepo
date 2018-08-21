using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.CurrentActivity;

namespace SmartWalkingStick.Droid
{
    [Activity(Label = "SmartWalkingStick", Icon = "@mipmap/icon", Theme = "@style/MainTheme",
        MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
          //  CrossCurrentActivity.Current.Activity.inInit(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            LoadApplication(new App());

            //if(ContextCompat.CheckSelfPermission(this,Manifes       t.Permission.RecordAudio) != Permission.Granted)
            //{
            //    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.RecordAudio }, 1);
            //}

            //if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.LocationHardware) != Permission.Granted)
            //{
            //    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.LocationHardware }, 1);
     

        }          //}
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

