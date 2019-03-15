﻿using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ARXToKDRGold.Communication
{
    internal static class HttpRequest
    {
        private static String Base_URL = Data.Base_URL;
        private static HttpResponseMessage Response { get; set; }
        public static String Jsonresponse { get; set; }

        public static async Task PostAsync(Object user, String _Command)
        {
            var client = new HttpClient();
            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.Unicode, "application/json");
            var request = new HttpRequestMessage();
            string tmp = Base_URL + _Command;
            Response = await client.PostAsync(tmp, content);
        }

        public static async Task<Boolean> PostMasterKey(Object user, String _Command)
        {
            var client = new HttpClient();
            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.Unicode, "application/json");
            var request = new HttpRequestMessage();
            Response = await client.PostAsync(Base_URL + _Command, content).ConfigureAwait(false);
            Jsonresponse = await Response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return Jsonresponse.Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }

        public static async Task<String> Get(String _Command)
        {
            Jsonresponse = "";
            var request = new HttpRequestMessage();

            var client = new HttpClient();

            Response = await client.GetAsync(Base_URL + _Command).ConfigureAwait(false);

            if (Response.IsSuccessStatusCode)
            {
                Jsonresponse = await Response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return await Response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}