using Android.App;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace App7
{
    [Activity(Label = "La-orutis ")]
    public class Activity1 : Activity, ILocationListener
    {
        static readonly string TAG = "X:" + typeof(Activity1).Name;
        TextView _addressText;
        Android.Locations.Location _currentLocation;
        LocationManager _locationManager;
        string _locationProvider;
        TextView _locationText;

        TextView name, status;
        string send_name,id,send_status;
        double haversine;
        public async void OnLocationChanged(Android.Locations.Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _locationText.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                _locationText.Text = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
                Address address = await ReverseGeocodeCurrentLocation();
                // harversine
                haversine = Haversine.Calculate(13.775445, 100.509833, _currentLocation.Latitude, _currentLocation.Longitude);

                DisplayAddress(address);
                Sendlocation();
            }
        }
        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Debug(TAG, "{0}, {1}", provider, status);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.layout2);

            _addressText = FindViewById<TextView>(Resource.Id.address_text);
            _locationText = FindViewById<TextView>(Resource.Id.location_text);
            name = FindViewById<TextView>(Resource.Id.name);
            status = FindViewById<TextView>(Resource.Id.status);

            FindViewById<TextView>(Resource.Id.get_address_button).Click += AddressButton_OnClick;

            string lname = Intent.GetStringExtra("lname") ?? "Data not available";
            string fname = Intent.GetStringExtra("name") ?? "Data not available";
            id = Intent.GetStringExtra("id") ?? "Data not available";
            //sendText1 = result2;
            name.Text = "ชื่อ "+ fname+"  "+lname;
            send_name = name.Text;

            InitializeLocationManager();
        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }

        protected override void OnResume()
        {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
            Log.Debug(TAG, "No longer listening for location updates.");
        }

        async void AddressButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (_currentLocation == null)
            {
                _addressText.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
            }

            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);
            send_status = "online";
            status.Text = "สถานะ : " + send_status;
            Sendstatus();
            // Toast.MakeText(this, haversine.ToString(), ToastLength.Long).Show();
        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                // Remove the last comma from the end of the address.
                _addressText.Text = deviceAddress.ToString();
            }
            else
            {
                _addressText.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }
        //// end location

        public static class Haversine
        {
            public static double Calculate(double lat1, double lon1, double lat2, double lon2)
            {
                var R = 6372.8; // In kilometers
                var dLat = ToRadians(lat2 - lat1);
                var dLon = ToRadians(lon2 - lon1);
                lat1 = ToRadians(lat1);
                lat2 = ToRadians(lat2);

                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
                var c = 2 * Math.Asin(Math.Sqrt(a));
                return R * 2 * Math.Asin(Math.Sqrt(a));
            }

            public static double ToRadians(double angle)
            {
                return Math.PI * angle / 180.0;
            }
        }

        // end haversine

        public class Data
        {
            // Add e.g. strings, int, DateTime,... for each datafield in your database.
            public string Name { get; set; } // These strings should get the same name as your databasefields.
            public double Lati { get; set; }
            public double Longi { get; set; }
            public double Distance { get; set; }
            public string Status { get; set; }

        }

        // data

        private void Sendstatus()
        {
            try
            {
                // Create a new data object
                Data DataObj = new Data()
                {
                    Name = id,
                    Status = send_status
                };


                // Serialize your data object.
                string JSONString = Jsonclass.JSONSerialize<Data>(DataObj);

                // Set your Url for your php-file on your webserver.
                string url = "http://5711011940087.sci.dusit.ac.th/api/update_status.php";

                // Create your WebRequest
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

                myRequest.Method = "POST";

                string postData = JSONString;

                byte[] pdata = Encoding.UTF8.GetBytes(postData);

                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentLength = pdata.Length;

                Stream myStream = myRequest.GetRequestStream();
                myStream.Write(pdata, 0, pdata.Length);


                // Get response from your php file.
                WebResponse myResponse = myRequest.GetResponse();

                Stream responseStream = myResponse.GetResponseStream();

                StreamReader streamReader = new StreamReader(responseStream);

                // Pass the response to a string and display it in a toast message.
                string result = streamReader.ReadToEnd();

                Toast.MakeText(this, result, ToastLength.Long).Show();

                // Close your streams.
                streamReader.Close();
                responseStream.Close();
                myResponse.Close();
                myStream.Close();
            }
            catch (WebException ex)
            {
                string _exception = ex.ToString();
                // Toast.MakeText(this, _exception, ToastLength.Long).Show();
                System.Console.WriteLine("--->" + _exception);
            }
        }

        private void Sendlocation()
        {
            try
            {
                // Create a new data object
                Data DataObj = new Data()
                {
                    Name = id,
                    Distance = haversine,
                    Longi = _currentLocation.Longitude,
                    Lati = _currentLocation.Latitude
                };


                // Serialize your data object.
                string JSONString = Jsonclass.JSONSerialize<Data>(DataObj);

                // Set your Url for your php-file on your webserver.
                string url = "http://5711011940087.sci.dusit.ac.th/api/update_location.php";

                // Create your WebRequest
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

                myRequest.Method = "POST";

                string postData = JSONString;

                byte[] pdata = Encoding.UTF8.GetBytes(postData);

                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentLength = pdata.Length;

                Stream myStream = myRequest.GetRequestStream();
                myStream.Write(pdata, 0, pdata.Length);


                // Get response from your php file.
                WebResponse myResponse = myRequest.GetResponse();

                Stream responseStream = myResponse.GetResponseStream();

                StreamReader streamReader = new StreamReader(responseStream);

                // Pass the response to a string and display it in a toast message.
                string result = streamReader.ReadToEnd();

                Toast.MakeText(this, result, ToastLength.Long).Show();

                // Close your streams.
                streamReader.Close();
                responseStream.Close();
                myResponse.Close();
                myStream.Close();
            }
            catch (WebException ex)
            {
                string _exception = ex.ToString();
                // Toast.MakeText(this, _exception, ToastLength.Long).Show();
                System.Console.WriteLine("--->" + _exception);
            }
        }

        // end sendapi


    }

}