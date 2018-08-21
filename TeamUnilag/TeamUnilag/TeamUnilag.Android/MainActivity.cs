using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TeamUnilag.Droid
{
    [Activity(Label = "Smart Walking Stick App", Icon = "@mipmap/logo", Theme = "@style/MainTheme",
        MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            LoadApplication(new App());
        
            this.RequestPermissions(new string[]
{
       Manifest.Permission.RecordAudio,
       Manifest.Permission.BluetoothPrivileged,
       Manifest.Permission.BluetoothAdmin,
       Manifest.Permission.Bluetooth,
       Manifest.Permission.LocationHardware,
       Manifest.Permission.AccessFineLocation,
       Manifest.Permission.AccessCoarseLocation,
       Manifest.Permission.AccessLocationExtraCommands,
       Manifest.Permission.AccessMockLocation
}, 0);
            
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

