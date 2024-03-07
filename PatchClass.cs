using Newtonsoft.Json;

namespace APIcalltest;

public class PatchClass : PostClass
{
	public async static Task PatchFunction(TokenClass token, string url)
	{
		using (HttpClient client = new HttpClient())
		{
			client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token.access_token}");
			while (true)
			{
				Console.WriteLine("Insert PATCH request endpoint:");
				string? endpoint = Console.ReadLine();
				if (endpoint == null)
				{
					continue;
				}
				url += endpoint;
				break;
			}
			var content = CreateContent();
			HttpResponseMessage response = await client.PatchAsync(url, content);
			Console.WriteLine("\n\n" + GetClass.ParseJson(await content.ReadAsStringAsync()));
			Program.CheckUrlAndRespond(response.StatusCode,response.IsSuccessStatusCode, "PATCH");
		}
	}
}