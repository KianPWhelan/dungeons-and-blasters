using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Net.Http.Headers;

public class Authenticator : MonoBehaviour
{
    HttpClient httpClient = new HttpClient();

    public void Awake()
    {
        // SignUp("test@email.com", "12345678");
        StartSession("test@email.com", "12345678");
    }

    public async void SignUp(string email, string password)
    {
        using (httpClient)
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.lootlocker.io/white-label-login/sign-up"))
            {
                request.Headers.TryAddWithoutValidation("domain-key", "9lqmswlv");
                string content = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}";
                Debug.Log(content);
                request.Content = new StringContent(content);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                Debug.Log(request);

                var response = await httpClient.SendAsync(request);
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }
    }

    public async void StartSession(string email, string password)
    {
        using (httpClient)
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.lootlocker.io/game/v2/session/white-label"))
            {
                string content = "{\"game_key\": \"1aad681d4545ff5c6f798923f332c01543058300\", \"email\": \"" + email + "\", \"password\": \"" + password + "\", \"game_version\": \"0.1.0.0\", \"development_mode\": \"true\"}";
                Debug.Log(content);
                request.Content = new StringContent(content);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                Debug.Log(request);

                var response = await httpClient.SendAsync(request);
                Debug.Log(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
