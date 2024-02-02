using DotNetEnv;
using Newtonsoft.Json;

namespace APIcalltest;

public class TokenClass
{
	public string? access_token { get; set; }
	public string? token_type { get; set; }
	public int? expires_in { get; set; }
	public string? scope { get; set; }
	public int? created_at { get; set; }
	public int? secret_valid_until { get; set; }
	public static async Task<TokenClass?> GetToken()
	{
		Env.Load();
		using (HttpClient client = new HttpClient())
		{
			string url = "https://api.intra.42.fr/oauth/token";
			var requestBody = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("grant_type", "client_credentials"),
				new KeyValuePair<string, string>("client_id", Environment.GetEnvironmentVariable("UID")!),
				new KeyValuePair<string, string>("client_secret", Environment.GetEnvironmentVariable("SECRET")!),
				new KeyValuePair<string, string>("scope", "public profile projects elearning tig forum"),
			});
			try
			{
				HttpResponseMessage response = await client.PostAsync(url, requestBody);
				string result = await response.Content.ReadAsStringAsync();
				if (response.IsSuccessStatusCode)
				{
					TokenClass? token = JsonConvert.DeserializeObject<TokenClass>(result);
					return token;
				}
				else
				{
					Console.WriteLine($"Error: {response.StatusCode}");
				}
				return null;
			}
			catch (Exception error)
			{
				Console.Error.WriteLine(error.Message);
				return null;
			}
		}
	}
}