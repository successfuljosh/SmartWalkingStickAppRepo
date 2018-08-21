using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TeamUnilag
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
        Plugin.Geolocator.Abstractions.Position position;
        AudioRecorderService recorder;
        //   AudioPlayer player;
        Direction direction;

        public HomePage()
        {
            InitializeComponent();
            staticControls.Children.Remove(connectingLayout);
            position = null;
            direction = new Direction();

            recorder = new AudioRecorderService
            {
                StopRecordingOnSilence = true, //will stop recording after 2 seconds (default)
                StopRecordingAfterTimeout = true,  //stop recording after a max timeout (defined below)
                TotalAudioTimeout = TimeSpan.FromSeconds(15) //audio will stop recording after 15 seconds
            };

        }

        //record AUdio
        async Task RecordAudio()
        {
            try
            {
                if (!recorder.IsRecording)
                {

                    var recordTask = await recorder.StartRecording();
                    destinationBtn.Text = "Stop Recording";
                    await recordTask;
                }
                else
                {
                    await recorder.StopRecording();
                    destinationBtn.Text = "My Destination";

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
                await CrossTextToSpeech.Current.Speak("An Error Occured, Please Try again or contact team Achievers ");
            }
        }



        private void connectBtn_Clicked(object sender, EventArgs e)
        {
            //  if bluetooth connection successful 
            //bluetooth connection
            CrossTextToSpeech.Current.Speak("Bluetooth Connecting... ");
            CrossTextToSpeech.Current.Speak("Unable to connect... Please Try again or contact team Achievers... ");
            //  modeTxt.Text = "Mode: Online";
            //   modeTxt.TextColor = Color.Green;
            staticControls.Children.Remove(homeTxtLayout);
            staticControls.Children.Add(connectingLayout, Constraint.RelativeToParent((parent) =>
            {
                return parent.Width * 0.35;
            }),
             Constraint.RelativeToParent((parent) => { return parent.Height * 0.35; })
             );

        }


        bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;
            return CrossGeolocator.Current.IsGeolocationAvailable;
        }

        private async void locationBtn_Clicked(object sender, EventArgs e)
        {
            staticControls.Children.Remove(connectingLayout);
            staticControls.Children.Add(homeTxtLayout, Constraint.RelativeToParent((parent) =>
            {
                return parent.Width * 0.35;
            }),
             Constraint.RelativeToParent((parent) => { return parent.Height * 0.35; })
             );

            try
            {
                if (IsLocationAvailable())
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 100;

                    position = await locator.GetLastKnownLocationAsync();
                    //got a cahched position, so let's use it.
                    if (position == null)
                    {

                        if (locator.IsGeolocationAvailable || locator.IsGeolocationEnabled)
                        {
                            //not available or enabled
                            position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10), null, true);
                            await CrossTextToSpeech.Current.Speak("Position Found!!");
                        }
                    }


                    await DisplayAlert("MoreDetails", $"Time: {position.Timestamp} \nLat: {position.Latitude} \nLong: {position.Longitude} \nAltitude: {position.Altitude} \nAltitude Accuracy:" +
                          $" {position.AltitudeAccuracy} \nAccuracy: {position.Accuracy} \nHeading: {position.Heading} \nSpeed: {position.Speed}", "OK");

                    var address = await CrossGeolocator.Current.GetAddressesForPositionAsync(position);
                    await CrossTextToSpeech.Current.Speak("You are at: " + address.First().FeatureName + ". OK");

                    await DisplayAlert("Feature Name", address.First().FeatureName, "OK");

                    //maps
                    var map = new Map(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude), Xamarin.Forms.Maps.Distance.FromMeters(10)))
                    {
                        IsEnabled = true,
                        IsShowingUser = true,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                    };

                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude),
                        Label = "Current Location",
                        Address = address.First().FeatureName
                    };
                    map.Pins.Add(pin);
                    mapLayout.Children.Clear();
                    mapLayout.Children.Add(map);

                    //foreach (var item in address)
                    //{
                    //    await DisplayAlert("Locality", item.Locality, "OK");
                    //    await DisplayAlert("AdminArea", item.AdminArea, "OK");
                    //    await DisplayAlert("SubAdminArea", item.SubAdminArea, "OK");
                    //    await DisplayAlert("SubLocality", item.SubLocality, "OK");
                    //    await DisplayAlert("throughfare", item.Thoroughfare, "OK");
                    //    await DisplayAlert("subthroughfare", item.SubThoroughfare, "OK");
                    //    await DisplayAlert("postcal code", item.PostalCode, "OK");
                    //    await DisplayAlert("Country Name", item.CountryName, "OK");
                    //    await DisplayAlert("country code", item.CountryCode, "OK");
                    //    await DisplayAlert("Feature Name", item.FeatureName, "OK");
                    //}
                }


            }
            catch (Exception ex)
            {
                await CrossTextToSpeech.Current.Speak("An Error Occured, Please Try again or contact team Achievers ");
                await DisplayAlert("Error", ex.ToString(), "OK");


            }
        }

        private async void destinationBtn_Clicked(object sender, EventArgs e)
        {
            //collect audio from user

            //record audio
            await RecordAudio();

            if (destinationBtn.Text == "Stop Recording")
            {
                if (recorder.GetAudioFilePath() != null)
                {
                    ///speech to text
                    var destinationText = "text from audio";
                    var positionDestination = await CrossGeolocator.Current.GetPositionsForAddressAsync(destinationText);
                    var distanceApart = 0d;
                    foreach (var item in positionDestination)
                    {//compare destination with current location
                        distanceApart = item.CalculateDistance(position, GeolocatorUtils.DistanceUnits.Kilometers) * 1000; //convert to meters
                                                                                                                           //get info from bluetooth
                        if (distanceApart > 10) //checking if close to 10m to destination
                        {
                            distanceApart = await StartListening(item);
                            await DisplayAlert("Continue going", $"you are {distanceApart}m from your destination", "OK");
                            //check for obstacle from bluetooth
                            //get direction from maps or api....
                        }
                        else
                        {
                            await StopListening();
                            await DisplayAlert("Success", "Destination reached", "OK");
                        }
                    }
                }
                else { await DisplayAlert("Error", "Recording file is Null", "OK"); }
            }

            //      await CrossTextToSpeech.Current.Speak("This is a demo location guide from your current position to University of Lagos, Lagos.");
            await CrossTextToSpeech.Current.Speak("Please wait, while i calculate your distance..", null, 10, null, 100);
            if (position == null)
                position = await CrossGeolocator.Current.GetPositionAsync(new TimeSpan(0, 0, 3));
            if (position != null)
            {
                var direct = await direction.getDirection(position, new Plugin.Geolocator.Abstractions.Position(6.51, 3.4));
                await CrossTextToSpeech.Current.Speak("Direction is" + direct.Routes[0].summary + ". OK");
                await DisplayAlert("Direction", direct.Routes[0].summary, "OK");
                foreach (var item in direct.Routes[0].legs)
                {
                    await DisplayAlert("Leg distance", item.distance.text, "ok");
                    foreach (var step in item.steps)
                    {
                        var st = step.html_instructions.Replace("<b>", "").Replace("</b>", "");
                        directionlabel.Text += @"
" + st + ", then, " +
    "";
                        await CrossTextToSpeech.Current.Speak(st + ", then, ");
                        //     await DisplayAlert("Step:", st, "Next");
                    }
                    await CrossTextToSpeech.Current.Speak("You have Reached your Destination", null, 10, null, 100);
                }
            }
            else
            {
                await CrossTextToSpeech.Current.Speak("Position not Defined", null, 10, null, 100);
            }
        }

        #region ListeningForChanges
        //Listening for changes
        public async Task<double> StartListening(Plugin.Geolocator.Abstractions.Position destination)
        {
            var dist = 0d;
            if (!CrossGeolocator.Current.IsListening)
                await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true);

            CrossGeolocator.Current.PositionChanged += (obj, e) =>
            {
                dist = destination.CalculateDistance(e.Position, GeolocatorUtils.DistanceUnits.Kilometers) * 1000; //convert to meters
            };
            CrossGeolocator.Current.PositionError += (obj, a) =>
            {
                DisplayAlert("Position Error:", a.Error.ToString(), "OK");
            };
            return dist;
        }

        private void PositionChanged(object sender, PositionEventArgs e)
        {

            //If updating the UI, ensure you invoke on main thread
            var position = e.Position;
            var output = "Full: Lat: " + position.Latitude + " Long: " + position.Longitude;
            output += "\n" + $"Time: {position.Timestamp}";
            output += "\n" + $"Heading: {position.Heading}";
            output += "\n" + $"Speed: {position.Speed}";
            output += "\n" + $"Accuracy: {position.Accuracy}";
            output += "\n" + $"Altitude: {position.Altitude}";
            output += "\n" + $"Altitude Accuracy: {position.AltitudeAccuracy}";

            //   Debug.WriteLine(output);
        }

        private void PositionError(object sender, PositionErrorEventArgs e)
        {
            //  Debug.WriteLine(e.Error);
            //Handle event here for errors
        }

        public async Task StopListening()
        {
            if (!CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StopListeningAsync();

            CrossGeolocator.Current.PositionChanged -= (obj, a) =>
            {
                DisplayAlert("Position stop event:", a.Position.ToString(), "OK");
            };
            CrossGeolocator.Current.PositionError -= (obj, a) =>
            {
                DisplayAlert("Position Error stop event:", a.Error.ToString(), "OK");
            };
        }
        #endregion

        private async void helpBtn_Clicked(object sender, EventArgs e)
        {
            //recite this
            await CrossTextToSpeech.Current.Speak(@"To get started, Connect stick to application by clicking on the top left corner of your screen.

To get your location, Click on the top right corner of your screen.

To get directions, Click on the bottom left corner of your screen,
Say your destination to the app and press stop(which is at the bottom
right corner of your screen) to end recording.

The application will process your speech and convert to a destination.
It will then guide you through speech.");

            await DisplayAlert("Help", @"To get started, Connect stick to 
application by clicking on the top left corner of your screen.

To get your location, Click on the top right corner of your screen.

To get directions, Click on the bottom left corner of your screen,
Say your destination to the app and press stop (which is at the bottom
right corner of your screen) to end recording.

The application will process your speech and convert to a destination.
It will then guide you through speech", "OK");
        }
    }
}