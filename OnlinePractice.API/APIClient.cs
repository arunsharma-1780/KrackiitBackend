namespace OnlinePractice.API
{
    public class APIClient
    {
        private readonly HttpClient httpClient;

        public APIClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:1234"); // Replace with your API's base URL
        }

        public async Task<string> GetDataFromApi()
        {
            var response = await httpClient.GetAsync("/api/data"); // Replace with your API endpoint
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
