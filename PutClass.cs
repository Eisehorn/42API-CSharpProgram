namespace APIcalltest;

public class PutClass : PostClass
{
	public async static Task PutFunction(TokenClass token, string url)
	{
		using (HttpClient client = new HttpClient())
		{
			client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token.access_token}");
			while (true)
			{
				Console.WriteLine("Insert PUT request endpoint:");
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
			Program.CheckUrlAndRespond(response.StatusCode, response.IsSuccessStatusCode, "PUT");
		}
	}
}