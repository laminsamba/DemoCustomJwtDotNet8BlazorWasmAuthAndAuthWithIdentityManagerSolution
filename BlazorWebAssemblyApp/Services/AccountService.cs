using Blazored.LocalStorage;
using SharedClassLibrary.Contracts;
using SharedClassLibrary.DTOs;
using SharedClassLibrary.GenericModels;
using static SharedClassLibrary.DTOs.ServiceResponses;

namespace BlazorWebAssemblyApp.Services
{
    public class AccountService : IUserAccount, IWeather
    {
        public AccountService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            this.httpClient = httpClient;
            this.localStorageService = localStorageService;
        }
        private const string BaseUrl = "api/Account";
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;

        public async Task<GeneralResponse> CreateAccount(UserDTO userDTO)
        {
            var response = await httpClient
                 .PostAsync($"{BaseUrl}/register",
                 Generics.GenerateStringContent(
                 Generics.SerializeObj(userDTO)));

            //Read Response
            if (!response.IsSuccessStatusCode)
                return new GeneralResponse(false, "Error occured. Try again later...");

            var apiResponse = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonString<GeneralResponse>(apiResponse);
        }

        

        public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
        {
            var response = await httpClient
               .PostAsync($"{BaseUrl}/login",
               Generics.GenerateStringContent(
               Generics.SerializeObj(loginDTO)));

            //Read Response
            if (!response.IsSuccessStatusCode)
                return new LoginResponse(false, null!, "Error occured. Try again later...");

            var apiResponse = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonString<LoginResponse>(apiResponse); 

        }

        public async Task<WeatherForecast[]> GetWeatherForecast()
        {
            string token = await localStorageService.GetItemAsStringAsync("token");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.GetAsync("api/WeatherForecast/user");


            //Read Response
            if (!response.IsSuccessStatusCode)
                return null!;

            var result = await response.Content.ReadAsStringAsync();
            return [.. Generics.DeserializeJsonStringList<WeatherForecast>(result)];
        }


    }
}
