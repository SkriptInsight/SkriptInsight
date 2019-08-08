using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

// ReSharper disable IdentifierTypo

namespace SkriptInsight.Host.Lsp
{
    class GoogleAnalyticsApi
    {
        /*
        *   Author: Tyler Hughes
        *   Credit for the Track function and the Enum HitType goes to 0liver (https://gist.github.com/0liver/11229128)
        *   Credit goes to spyriadis (http://www.spyriadis.net/2014/07/google-analytics-measurement-protocol-track-events-c/)
        *   for the idea of putting the values for each tracking method in its own function
        *
        *   Documentation of the Google Analytics Measurement Protocol can be found at:
        *   https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide
        */

        private const string Endpoint = "http://www.google-analytics.com/collect";
        private const string GoogleVersion = "1";
        private readonly string _googleTrackingId = "UA-145177245-1";
        private readonly string _googleClientId = "DEV-555";

        public bool DisableTracking { get; set; }

        public string? UserAgent { get; set; } = null;
        
        public CancellationTokenSource CancellationToken { get; }
        
        public GoogleAnalyticsApi(string trackingId, string clientId) : this()
        {
            _googleTrackingId = trackingId;
            _googleClientId = clientId;
        }

        public GoogleAnalyticsApi()
        {
            CancellationToken = new CancellationTokenSource();
            Task.Run(SendHeartbeatEvent);
        }

        private async Task SendHeartbeatEvent()
        {
            while (true)
            {
                if (CancellationToken.IsCancellationRequested) return;

                TrackEvent("heartbeat", "Session Heartbeat");
                await Task.Delay((int) TimeSpan.FromMinutes(10).TotalMilliseconds, CancellationToken.Token);
            }
        }

        public void TrackEvent(string action, string label, int? value = null, object? extraValues = null)
        {
            TrackEvent("SkriptInsight-LSP", action, label, value, extraValues);
        }

        public void TrackPageView(string action, string label, int? value = null, object? extraValues = null)
        {
            TrackPageView("SkriptInsight-LSP", action, label, value, extraValues);
        }

        public void TrackEvent(string category, string action, string label, int? value = null,
            object? extraValues = null)
        {
            if (string.IsNullOrEmpty(category)) throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrEmpty(action)) throw new ArgumentNullException(nameof(action));

            var values = DefaultValues;

            values.Add("t", HitType.@event.ToString()); // Event hit type
            values.Add("ec", category); // Event Category. Required.
            values.Add("ea", action); // Event Action. Required.
            if (label != null) values.Add("el", label); // Event label.
            if (value != null) values.Add("ev", value.ToString()); // Event value.
            if (extraValues != null)
            {
                TypeDescriptor.GetProperties(extraValues).Cast<PropertyDescriptor>().All(v =>
                {
                    var val = v.GetValue(extraValues)?.ToString();
                    if (val != null)
                        values.Add(v.Name, val);
                    return true;
                });
            }

            Track(values);
        }

        public void TrackPageView(string category, string action, string label, int? value = null,
            object? extraValues = null)
        {
            if (string.IsNullOrEmpty(category)) throw new ArgumentNullException(nameof(category));
            if (string.IsNullOrEmpty(action)) throw new ArgumentNullException(nameof(action));

            var values = DefaultValues;

            values.Add("t", HitType.@pageview.ToString()); // Event hit type
            values.Add("ec", category); // Event Category. Required.
            values.Add("ea", action); // Event Action. Required.
            if (label != null) values.Add("el", label); // Event label.
            if (value != null) values.Add("ev", value.ToString()); // Event value.
            if (extraValues != null)
            {
                TypeDescriptor.GetProperties(extraValues).Cast<PropertyDescriptor>().All(v =>
                {
                    var val = v.GetValue(extraValues)?.ToString();
                    if (val != null)
                        values.Add(v.Name, val);
                    return true;
                });
            }

            Track(values);
        }

        private void Track(Dictionary<string, string> values)
        {
            if (DisableTracking) return;
         
            if (UserAgent != null)
                values.Add("ua", UserAgent);
            
            var request = (HttpWebRequest) WebRequest.Create(Endpoint);
            request.Method = "POST";
            request.KeepAlive = false;

            var postDataString = values
                .Aggregate("", (data, next) => $"{data}&{next.Key}={HttpUtility.UrlEncode(next.Value)}")
                .TrimEnd('&');

            // set the Content-Length header to the correct value
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataString);

            // write the request body to the request
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postDataString);
            }

            try
            {
                // Send the response to the server
                var webResponse = (HttpWebResponse) request.GetResponse();

                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    DisableTracking = true;
                }

                webResponse.Close();
            }
            catch (Exception)
            {
                DisableTracking = true;
            }
        }

        private enum HitType
        {
            // ReSharper disable InconsistentNaming
            @event,
            @pageview,
            // ReSharper restore InconsistentNaming
        }

        private Dictionary<string, string> DefaultValues
        {
            get
            {
                var data = new Dictionary<string, string>
                {
                    {"v", GoogleVersion}, {"tid", _googleTrackingId}, {"cid", _googleClientId}
                };
                // The protocol version. The value should be 1.
                // Tracking ID / Web property / Property ID.
                // Anonymous Client ID (must be unique).
                return data;
            }
        }
    }
}