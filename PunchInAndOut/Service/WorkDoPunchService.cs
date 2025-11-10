using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PunchInAndOut.Model;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace PunchInAndOut.Service
{
    public class WorkDoPunchService
    {
        private readonly WorkDoSetting setting;
        private readonly ILogger<WorkDoPunchService> logger;
        private readonly HttpClient httpClient = new HttpClient();
        private const string logonUrl = "https://www.workdo.co/bdddweb/api/dweb/BDD771M/userLogin";
        private const string punchUrl = "https://www.workdo.co/bdddweb/api/dweb/CCN102M/saveFromCreate102M3";
        private const string calenderUrl = "https://www.workdo.co/bdddweb/api/dweb/HRS003W/execute003W3FromMenu";
        private const string punchStatusUrl = "https://www.workdo.co/bdddweb/api/dweb/CCN102M/saveFromCreate102M3";

        public WorkDoPunchService(IConfiguration config, ILogger<WorkDoPunchService> logger)
        {
            setting = config.GetSection("WorkDoSetting").Get<WorkDoSetting>();
            this.logger = logger;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7"));
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla/5.0", "Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.60 Safari/537.36"));
            httpClient.DefaultRequestHeaders.Add("tenant_id", "aa6pd97f");
            httpClient.DefaultRequestHeaders.Add("timezone", "GMT+0800");
        }

        public async Task GetLoginCookiesAsync()
        {
            var loginData = new WorkDoLoginDto()
            {
                ClientType = "Web",
                ClientModel = "Chrome 100.0.4896.127",
                ClientOs = "Windows 10",
                AppVersion = "wd_aweb_6.5.5",
                TimeZone = "GMT+0800",
                LoginEmail = setting.EMail,
                Password = setting.Password,
                LoginId = setting.EMail,
            };
            
            var response = await httpClient.PostAsJsonAsync(logonUrl, loginData);
            if (!response.IsSuccessStatusCode) logger.LogError("Login failed");
        }

        public async Task PunchIn(bool hasCookies = false)
        {
            if (!hasCookies) await GetLoginCookiesAsync();

            await PunchIn();
        }
        private async Task PunchIn()
        {

            var loginData = new WorkDoLoginDto()
            {
                ClientType = "Web",
                ClientModel = "Chrome 100.0.4896.127",
                ClientOs = "Windows 10",
                AppVersion = "wd_aweb_6.5.5",
                TimeZone = "GMT+0800",
                LoginEmail = setting.EMail,
                Password = setting.Password,
                LoginId = setting.EMail,
            };

            var response = await httpClient.PostAsJsonAsync(logonUrl, loginData);
            if (!response.IsSuccessStatusCode) logger.LogError("Login failed");
        }

        public void PunchOut()
        {
            // Punch out
        }
    }
    public class ZealogicsPunchService
    {
        private readonly ZealogicsSetting setting;
        private readonly ILogger<ZealogicsPunchService> logger;
        private readonly HttpClient httpClient = new HttpClient();
        private const string loginUrl = "https://timezone.zealogics.com/sign-in.php";
        private static readonly Uri LogTimeUri = new Uri("https://timezone.zealogics.com/ajax/ajaxpage.php/");
        public ZealogicsPunchService(IConfiguration config, ILogger<ZealogicsPunchService> logger)
        {
            setting = config.GetSection(nameof(ZealogicsSetting)).Get<ZealogicsSetting>() ?? throw new ArgumentNullException();
            this.logger = logger;
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authority", "timezone.zealogics.com");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("method", "POST");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("path", "/ajax/ajaxpage.php/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("scheme", "https");
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.Add("DNT", "1");
            httpClient.DefaultRequestHeaders.Add("Origin", "https://timezone.zealogics.com");
            httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
            httpClient.DefaultRequestHeaders.Add("Priority", "u=0, i");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://timezone.zealogics.com/index.php?page=myTime");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Chromium\";v=\"142\", \"Google Chrome\";v=\"142\", \"Not_A Brand\";v=\"99\"");
            httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
            httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        }


        public async Task GetLoginCookiesAsync()
        {
            var response = await httpClient.GetAsync(loginUrl);
            if (!response.IsSuccessStatusCode) logger.LogError("Open Page failed");
        }

        public async Task StartLoginCookiesAsync()
        {
            var loginData = new ZealogicsDto()
            {
                emailAddy = setting.EMail,
                password = setting.Password,
                submit = "signIn"
            };

            var contentBody = FormUrlEncodedHelper.ToFormUrlEncodedContent(loginData);
            var response = await httpClient.PostAsync(loginUrl, contentBody);

            if (!response.IsSuccessStatusCode) logger.LogError("Login failed");
        }

        public async Task Login()
        {
            await GetLoginCookiesAsync();
            await StartLoginCookiesAsync();
        }

        public async Task PunchIn(DateTime dateTime)
        {
            var dto = new PersonalLogInfo()
            {
                LogDate = dateTime,
                UserName = setting.UserName,
                UserId = setting.UserId,
                ProjectId = setting.ProjectId
            }.To();

            // 轉成表單 URL Encoded 內容,並確保 Content-Type 包含 charset=UTF-8
            var contentBody = FormUrlEncodedHelper.ToFormUrlEncodedContent(dto);
            var response = await httpClient.PostAsync(LogTimeUri, contentBody);

            if (!response.IsSuccessStatusCode) logger.LogError("PunchIn failed");
        }

        public void PunchOut()
        {
            // Punch out
        }
    }
    public static class FormUrlEncodedHelper
    {
        /// <summary>
        /// 將任意物件轉為 application/x-www-form-urlencoded 的 HttpContent
        /// </summary>
        public static StringContent ToFormUrlEncodedContent(object obj)
        {
            var props = obj.GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var p in props)
            {
                // URL-encode key 與 value
                var name = WebUtility.UrlEncode(p.Name);
                var value = WebUtility.UrlEncode(p.GetValue(obj)?.ToString() ?? "");
                sb.Append($"{name}={value}&");
            }
            if (sb.Length > 0) sb.Length--;  // 移除最後一個 '&'

            // 指定 mediaType 及 charset
            return new StringContent(
                sb.ToString(),
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            );
        }
    }
}