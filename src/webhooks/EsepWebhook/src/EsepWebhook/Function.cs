using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a GitHub webhook payload, extracts the issue URL, and sends it to Slack
        /// </summary>
        /// <param name="input">GitHub webhook payload</param>
        /// <param name="context">Lambda execution context</param>
        /// <returns>The response from the Slack webhook</returns>
        public string FunctionHandler(object input, ILambdaContext context)
        {
            // Deserialize the input to a dynamic object
            dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());

            // Construct the payload to send to Slack
            string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";

            // Create an HttpClient to send the payload to Slack
            using var client = new HttpClient();
            var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            // Send the payload to Slack and return the response
            var response = client.Send(webRequest);
            using var reader = new StreamReader(response.Content.ReadAsStream());

            return reader.ReadToEnd();
        }
    }
}
