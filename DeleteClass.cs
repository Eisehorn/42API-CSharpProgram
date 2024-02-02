namespace APIcalltest;

public class DeleteClass
{
	public static async Task DeleteFunction(TokenClass token, string url)
	{
		using (HttpClient client = new HttpClient())
		{
			client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token.access_token}");
			Console.WriteLine("This is a very important call. Are you sure you want to continue?");
			string? answer = Console.ReadLine()!.ToUpperInvariant();
			if (answer != "YES")
				return;
			while (true)
			{
				Console.WriteLine("Insert DELETE request endpoint:");
				string? endpoint = Console.ReadLine();
				if (endpoint == null)
				{
					continue;
				}
				url += endpoint;
				break;
			}
			HttpResponseMessage response = await client.DeleteAsync(url);
			Program.CheckUrlAndRespond(response.IsSuccessStatusCode, "DELETE");
		}
	}
}