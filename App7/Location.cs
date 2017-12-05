using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.OS;
using Android.Util;
using Android.Widget;
using Android.App;

using Android.Locations;
using Android.Runtime;
using Android.Content;
using Android.Net;
using System.Net;
using System.IO;
using Org.Json;
using System.Collections.Specialized;


namespace App7
{
    [Activity(Label = "App7")]
    public class Location : Activity, ILocationListener
    {
        Android.Locations.Location currentLocation;
        LocationManager locationManager;
        string locationProvider;
#pragma warning disable 0169
        TextView latitude, longitude;
        string sendText1 ,Name;
        double sendText2, sendText3,sendText4;
        string  sendText5;
        //EditText edittext1;
        TextView text1, text2, text3;

       

        //#pragma warning restore 0169
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
           
            // Create your application here
            SetContentView(Resource.Layout.layout1);
            TextView _locationProvider = FindViewById<TextView>(Resource.Id.Provider);
            TextView text1 = FindViewById<TextView>(Resource.Id.textView1);
            TextView text2 = FindViewById<TextView>(Resource.Id.textView2);
            TextView text3 = FindViewById<TextView>(Resource.Id.textView3);
            latitude = FindViewById<TextView>(Resource.Id.latitude);
            longitude = FindViewById<TextView>(Resource.Id.longitude);
            Button click = FindViewById<Button>(Resource.Id.click);
           
            GetJSONString();
            // test = FindViewById<TextView>(Resource.Id.fullname);
            string result1 = Intent.GetStringExtra("MyData") ?? "Data not available";
            string result2 = Intent.GetStringExtra("MyData1") ?? "Data not available";
            string result3 = Intent.GetStringExtra("MyData2") ?? "Data not available";
            string result4 = Intent.GetStringExtra("MyData3") ?? "Data not available";
            //sendText1 = result2;
            text1.Text = result1;
            text2.Text = result3;
            text3.Text = result4;
            //Toast.MakeText(this, result1, ToastLength.Long).Show();

            if (Checknetwork())
            {
                locationManager = (LocationManager)(this).GetSystemService(Context.LocationService);
                Criteria c = new Criteria();
                locationProvider = locationManager.GetBestProvider(c, true);
                OnLocationChanged(locationManager.GetLastKnownLocation(locationProvider));
                _locationProvider.Text = "Provider : " + locationProvider;
            }
            else
            {
                _locationProvider.Text = "";

            }
            click.Click += Click_Click;
        }

        private void Click_Click(object sender, EventArgs e)
        {

            //double test = Haversine.calculate(13.775445, 100.509833, currentLocation.Latitude, currentLocation.Longitude);
            //sendText4 = test.ToString();
            string re = Intent.GetStringExtra("MyData1") ?? "Data not available";
            sendText1 = re;
            string online1;
            online1 = "online";
            sendText5 = online1;

            Toast.MakeText(this, sendText5, ToastLength.Long).Show();
            SendToPhp();
            //GetJSONString();
        }

        //

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

        //

        private bool Checknetwork()
        {
            ConnectivityManager connmanager = (ConnectivityManager)GetSystemService(ConnectivityService);
            if (connmanager != null)
            {
                NetworkInfo currentnetwork = connmanager.ActiveNetworkInfo;
                bool Isok = (currentnetwork != null) && currentnetwork.IsConnected;
                return Isok;

            }
            return false;

        }

        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);


        }
        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);

        }
        public void OnLocationChanged(Android.Locations.Location location)
        {
            currentLocation = location;
            if (currentLocation != null)
            {
                latitude.Text = "Latitude : " + currentLocation.Latitude.ToString();
                longitude.Text = "Longgitude : " + currentLocation.Longitude.ToString();
                sendText2 = currentLocation.Latitude;
                sendText3 = currentLocation.Longitude;

                double test = Haversine.Calculate(13.775445, 100.509833, currentLocation.Latitude, currentLocation.Longitude);
                sendText4 = test;
                //Toast.MakeText(this, sendText2, ToastLength.Long).Show();
                SendToPhp();
                //Toast.MakeText(this, "sucess", ToastLength.Long).Show();

            }
            else
            {
                latitude.Text = "";
                longitude.Text = "";

            }
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }

        //
        public class Data
        {
            // Add e.g. strings, int, DateTime,... for each datafield in your database.
            public string Name { get; set; } // These strings should get the same name as your databasefields.
            public double Lati { get; set; }
            public double Longi { get; set; }
            public double Distance { get; set; }
            public string Status { get; set; }

        }
        // 
        //Toast.MakeText(this, sendText1, ToastLength.Short).Show();
        private void SendToPhp()
        {
            try
            {
                // Create a new data object
                Data DataObj = new Data()
                {
                    Name = sendText1,
                    Lati = sendText2,
                    Longi = sendText3,
                    Distance = sendText4,
                    Status = sendText5
                };


                // Serialize your data object.
                string JSONString = Jsonclass.JSONSerialize<Data>(DataObj);

                // Set your Url for your php-file on your webserver.
                string url = "http://10.210.63.36/project/api/update.php";

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

        public void GetJSONString()
        {
            string url = "http://10.210.63.36/project/api/select.php";

            //

            WebClient client = new WebClient();
            System.Uri uri = new System.Uri(url);
            NameValueCollection parameters = new NameValueCollection
            {
                { "username", sendText1 }
            };
            var response = client.UploadValues(uri, parameters);
            var responseString = Encoding.Default.GetString(response);
            //JSONObject o = new JSONObject(responseString);

            // Create your WebRequest
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //request.ContentType = "application/json";
            //request.Method = "GET";
            // 
            /*
            var content = "";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Response contained empty body...");
                    }
                    else
                    {
                        Console.Out.WriteLine("Response Body: \r\n {0}", content);
                    }
                    //Assert.NotNull(content);
                }
            } */
            //Toast.MakeText(this, responseString, ToastLength.Short).Show();
            //initializeMap(responseString);

        }
        //
    }
}