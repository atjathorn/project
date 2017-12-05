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
using System.IO;
using System.Runtime.Serialization.Json;

namespace App7
{
    class Jsonclass
    {

        public static string JSONSerialize<T>(T obj) // serialize json for sending to php
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        //      public static T JSONDeserialise<T>(string json) // deserialize json for getting data
        //      {
        //          T obj = Activator.CreateInstance<T>();
        //          using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
        //          {
        //              DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        //              obj = (T)serializer.ReadObject(ms);
        //              return obj;
        //          }
        //      }

    }
}