namespace APIcalltest;

public class DeleteClass : PostClass
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
			url = EndpointContent(url);
			Console.WriteLine(url);
			HttpResponseMessage response = await client.DeleteAsync(url);
			Program.CheckUrlAndRespond(response.StatusCode, response.IsSuccessStatusCode, "DELETE");
		}
	}

	private static string EndpointContent(string url)
	{
		while (true)
		{
			Console.WriteLine("Do you need to add any parameter?");
			string answer = Console.ReadLine()!.ToUpperInvariant();
			if (answer == "YES")
			{
				Console.WriteLine("Please enter you parameters separated by a comma. Usage example: duration,reason,amount");
				var fields = Console.ReadLine()!;
				string[] fieldKey = fields.Split(',');
				fieldKey = fieldKey.Select(parameter => parameter.Trim()).ToArray();
				url += "?";
				for (int i = 0; i < fieldKey.Length; i++)
				{
					if (i != 0)
					{
						url += "&";
					}
					Console.WriteLine($"Insert value of {fieldKey[i]}");
					string value = Console.ReadLine()!;
					url += $"{fieldKey[i]}={value}";
				}
				return url;
			}
			if (answer == "NO")
			{
				return url;
			}
			Console.WriteLine("Wrong input. Please enter yes or no.");
		}
	}
}