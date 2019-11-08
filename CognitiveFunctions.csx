#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;

public static async Task<IActionResult> Run(HttpRequest req, CloudBlockBlob inputBlob, ILogger log)
{
    var client = new HttpClient();
    var queryString = HttpUtility.ParseQueryString(string.Empty);

    // Request headers
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "<YOUR_SUBSCRIPTION_KEY>");
    queryString["returnFaceAttributes"] = "emotion";

    var uri = "https://<YOUR_FACE_API_SERVICE_NAME>.cognitiveservices.azure.com/face/v1.0/detect?" + queryString;
    
    HttpResponseMessage response;

    // Request body
    string body = "{\"url\": " + $"\" {inputBlob.Uri} \"" + "}";

    log.LogInformation($"C# Blob trigger function Processed blob\nURI:{body}");

    byte[] byteData = Encoding.UTF8.GetBytes(body);

    using (var content = new ByteArrayContent(byteData))
    {
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        response = await client.PostAsync(uri, content);
    }

    string contentString = await response.Content.ReadAsStringAsync();
    
    return (ActionResult)new OkObjectResult($"{contentString}");
}
