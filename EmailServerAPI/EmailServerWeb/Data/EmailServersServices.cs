using EmailServerAPI.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.Text;

namespace EmailServerWeb.Data
{
    public class EmailServersServices : IEmailServersServices
    {
        private readonly HttpClient _httpClient;
        public EmailServersServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AutoEmail>> DeleteEmails([FromBody] List<AutoEmail> emails_to_delete)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_httpClient.BaseAddress + "api/AutoEmail/GroupDelete"),
                Content = new StringContent(JsonConvert.SerializeObject(emails_to_delete), Encoding.UTF8, "application/json")
            };
            var aresponse = await _httpClient.SendAsync(request);
            var ppp = aresponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert
                    .DeserializeObject<List<AutoEmail>>(aresponse
                                                                .Content
                                                                .ReadAsStringAsync()
                                                                .GetAwaiter()
                                                                .GetResult());
            
        }

        public async Task<AutoEmailServers> DeleteServer([FromBody] AutoEmailServers server)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync("api/EmailServers");


            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_httpClient.BaseAddress + "api/EmailServers"),
                Content = new StringContent(JsonConvert.SerializeObject(server), Encoding.UTF8, "application/json")
            };
            var aresponse = await _httpClient.SendAsync(request);
            return null;
        }

        public async Task<List<AutoEmailServers>> DeleteServer([FromBody] List<AutoEmailServers> servers)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_httpClient.BaseAddress + "api/AutoEmailServers/GroupDelete"),
                Content = new StringContent(JsonConvert.SerializeObject(servers), Encoding.UTF8, "application/json")
            };
            var aresponse = await _httpClient.SendAsync(request);
            var ppp = aresponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert
                    .DeserializeObject<List<AutoEmailServers>>(aresponse
                                                                .Content
                                                                .ReadAsStringAsync()
                                                                .GetAwaiter()
                                                                .GetResult());
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AutoEmailServers>> GetAll()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AutoEmailServers>>("api/AutoEmailServers");
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AutoEmailServers>> GetByUser(string initials)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AutoEmail>> GetEmails()
        {
            return (await _httpClient.GetFromJsonAsync<IEnumerable<AutoEmail>>("api/AutoEmailServers/GetAllEmails")).ToList();
            throw new NotImplementedException();
        }

        public async Task<List<AutoEmail>> GetEmailsofServer(string servername)
        {
            return (await _httpClient.GetFromJsonAsync<IEnumerable<AutoEmail>>("api/AutoEmailServers/GetMail/" + servername)).ToList();
        }
    }
}
