using Microsoft.AspNetCore.Mvc;

namespace LineNotifyApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class LineNotify : ControllerBase
{
    private readonly string? _lineUserToken;

    public LineNotify(IConfiguration config)
    {
        _lineUserToken = config.GetValue<string?>("Line:UserToken");
    }

    [HttpGet]
    public async Task<ActionResult> SendNotify(string message = "")
    {
        if(string.IsNullOrWhiteSpace(_lineUserToken))
            return BadRequest("Line:UserToken not found on Environment");

        var result = await SendLineNotify(_lineUserToken, message);

        return result.IsSuccessStatusCode ? Ok() : BadRequest(result.Content.ReadAsStringAsync());
    }

    private Task<HttpResponseMessage> SendLineNotify(string lineId, string message)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization",
            $"Bearer {lineId}");

        var context = new FormUrlEncodedContent(
            new List<KeyValuePair<string, string>>()
            {
                    //todo 針對發送的訊息進行排版
                    new("message", message)
            });

        return httpClient.PostAsync(
            "https://notify-api.line.me/api/notify", context);
    }
}
