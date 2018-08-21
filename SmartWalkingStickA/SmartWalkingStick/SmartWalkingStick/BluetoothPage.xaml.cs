using Plugin.AudioRecorder;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartWalkingStick
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothPage : ContentPage
    {
        IBluetoothLE ble;
        IAdapter adapter;
        ObservableCollection<IDevice> deviceList;
      //  IDevice device;

        //Record section
        AudioRecorderService recorder;
        AudioPlayer player;
        public BluetoothPage()
        {
            InitializeComponent();
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            deviceList = new ObservableCollection<IDevice>();

            //recorder
            recorder = new AudioRecorderService
            {
                StopRecordingOnSilence = true,
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(5)
            };

            player = new AudioPlayer();
            player.FinishedPlaying += Player_FinishedPlaying;

        }

        private void status_Clicked(object sender, EventArgs e)
        {
            var status = ble.State;
            if (ble.State == BluetoothState.Off)
                DisplayAlert("Bluetooth off:", "Turn it On", "ok");
            DisplayAlert("Bluetooth Status", status.ToString(), "OK");
        }
        private void scan_Clicked(object sender, EventArgs e)
        {
            try
            {
                deviceList.Clear();
                adapter.DeviceDiscovered += (s, a) =>
                {
                    deviceList.Add(a.Device);
                };
                if (!adapter.IsScanning)
                {
                    adapter.StartScanningForDevicesAsync();
                    DisplayAlert("TES", adapter.ToString(), "X");
                    var buttons = (from a in adapter.DiscoveredDevices select a.Name).ToArray<string>();
                   DisplayActionSheet("Devices Found", "X", "Stop", buttons);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error!", ex.ToString(), "OK");
            }
        }

   
        private void connect_Clicked(object sender, EventArgs e)
        {
            
        }


        private async void record_Clicked(object sender, EventArgs e)
        {
            await RecordAudio();
        }

        async Task RecordAudio()
            {
                try
                {
                    if (!recorder.IsRecording)
                    {
                        var audioTask = await recorder.StartRecording();
                        record.Text = "Stop";
                        await audioTask;
                        record.Text = "Start";
                    }
                    else
                    {
                        record.Text = "Stop";
                        await recorder.StopRecording();
                        record.Text = "Start";
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Someting went wrong, " + ex.ToString(), "Ok");
                }
            }


        private void play_Clicked(object sender, EventArgs e)
        {
            try
            {
                var filepath = recorder.GetAudioFilePath();
                if (filepath != null)
                {
                    play.Text = "Playing Audio";
                    player.Play(filepath);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Someting went wrong, " + ex.ToString(), "Ok");
            }
        }

        private void Player_FinishedPlaying(object sender, EventArgs e)
        {
            play.Text = "Play";
        }

    }
}