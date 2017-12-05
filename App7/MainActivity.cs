
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Org.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
namespace App7
{
    [Activity(Label = "La-orutis ", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string id_u, name_u, lname_u;
        string name, pass;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // SetContentView(Resource.Layout.Main);
            EditText username = FindViewById<EditText>(Resource.Id.txtUsername);
            EditText password = FindViewById<EditText>(Resource.Id.txtPassword);
            Button btnlogin = FindViewById<Button>(Resource.Id.btnLogin);

            //Jalankan fungsi untuk login cek apakah username dan password benar?
            btnlogin.Click += delegate
            {
                name = username.Text;
                pass = password.Text;
                WebClient client = new WebClient();
                Uri uri = new Uri("http://5711011940087.sci.dusit.ac.th/api/xamarinsignIn.php");
                NameValueCollection parameters = new NameValueCollection
                {
                    { "xusername", name },
                    { "xpassword", pass }
                };
                var response = client.UploadValues(uri, parameters);
                var responseString = Encoding.Default.GetString(response);
                JSONObject o = new JSONObject(responseString);

                if (o.OptString("success").Equals("1"))
                {

                    GetJSONString();
                    
                    var activity2 = new Intent(this, typeof(Activity1));
                    activity2.PutExtra("id", id_u);
                    activity2.PutExtra("name", name_u);
                    activity2.PutExtra("lname", lname_u);
                    StartActivity(activity2);
                    //Toast.MakeText(this, name_u, ToastLength.Short).Show();
                }
                else
                {

                    Toast.MakeText(this, "Username and Password Fail", ToastLength.Short).Show();

                }
            };

        }
        //

        public void GetJSONString()
        {
            string url = "http://5711011940087.sci.dusit.ac.th/api/select.php";

            //

            WebClient client = new WebClient();
            System.Uri uri = new System.Uri(url);
            NameValueCollection parameters = new NameValueCollection
            {
                { "xusername", name },
                { "xpassword", pass }
            };
            var response = client.UploadValues(uri, parameters);
            var responseString = Encoding.Default.GetString(response);
            
            InitializeMap(responseString);
        }
        //
        public void InitializeMap(String responseString)
        {
            try
            {
                JSONArray json = new JSONArray(responseString);
                for (int i = 0; i < json.Length(); i++)
                {
                    JSONObject obj = json.GetJSONObject(i);
                    string id_member = obj.GetString("id_member");
                    string fname_member = obj.GetString("fname_member");
                    string lname_member = obj.GetString("lname_member");

                    id_u = id_member;
                    name_u = fname_member;
                    lname_u = lname_member;

                }
            }
            catch (JSONException e)
            {
                e.StackTrace.ToString();
            }

        }
        //
    }
}

