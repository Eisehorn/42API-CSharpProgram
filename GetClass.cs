using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace APIcalltest;

public class GetClass
{
	private static dynamic? Data { get; set; }

	private static void PrintResult(List<string> allResponses)
	{
		int page = 1;
		foreach (var response in allResponses)
		{
			Console.WriteLine($"Result of page number {page}:");
			page++;
			Data = ParseJson(response);
			Console.WriteLine(Data);
		}
	}
	private static void PrintResultFilename(List<string> allResponses)
	{
		Console.WriteLine("Enter the file name in which you want to save your output\nLeave it empty to print to the terminal");
		string? output = Console.ReadLine();
		if (!string.IsNullOrEmpty(output))
		{
			using (StreamWriter writer = new StreamWriter(output))
			{
				TextWriter originalConsoleOut = Console.Out;
				Console.SetOut(writer);
				PrintResult(allResponses);
				Console.SetOut(originalConsoleOut);
			}
		}
		else
		{
			PrintResult(allResponses);
		}
	}
	
	private static string CheckNextLink(string nextlink)
	{
		string input = nextlink;
		string pattern = @"<([^>]+)>; rel=""next""";
		Match match = Regex.Match(input, pattern);
		if (match.Success)
		{
			string secondLink = match.Groups[1].Value;
			return secondLink;
		}
		else
		{
			Console.WriteLine("No match found.");
		}

		return "Big Error Encountered";
	}
	
	private static string? InsertUrl()
	{
		Console.WriteLine("Enter GET request endpoint:");
		string? endpoint = Console.ReadLine();
		return endpoint;
	}

	private static string InsertFilter()
	{
		string? answer = null;
		while (answer != "YES" && answer != "NO")
		{
			Console.WriteLine("Do you want to add any filter to your endpoint?");
			answer = Console.ReadLine()?.ToUpperInvariant();
			if (answer == "YES")
			{
				Console.WriteLine("Enter you filter now please");
				Console.WriteLine("Page filter already present, usage ex: &filter[id]=a_value,another_value");
				string filter = "?page=1";
				filter += Console.ReadLine();
				return filter;
			}
			else if (answer != "NO")
			{
				Console.WriteLine("Input error. Please insert Yes or No.");
			}
		}
		return "?page=1";
	}

	private static async Task<string> CheckIfCorrectUrl(HttpClient client)
	{
		string url = $"https://api.intra.42.fr";
		url += InsertUrl();
		string filter = InsertFilter();
		url = url + filter;
		HttpResponseMessage response = await client.GetAsync(url);
		if (response.IsSuccessStatusCode == true)
		{
			return url;
		}
		Console.WriteLine($"{url} returned an error, please retry");
		return "Big Error Encountered";
	}
	public static dynamic ParseJson(string json)
	{
		dynamic parsedData = JsonConvert.DeserializeObject<dynamic>(json)!;
		return parsedData;
	}
	public static async Task GetFunction(TokenClass token, string url)
	{
		using (HttpClient client = new HttpClient())
		{
			List<string> allResponses = new List<string>();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.access_token}");
			while (true)
			{
				url = await CheckIfCorrectUrl(client);
				if (url != "Big Error Encountered")
				{
					break;
				}
			}
			while (true)
			{
				HttpResponseMessage response = await client.GetAsync(url);
				HttpContent responseContent = response.Content;
				Console.WriteLine("Calls are being made, please wait...");
				if (response.IsSuccessStatusCode)
				{
					allResponses.Add(await responseContent.ReadAsStringAsync());
					var nextlink = response.Headers.GetValues
						("Link")?.FirstOrDefault(link => link.Contains("rel=\"next\""));
					if (nextlink != null)
					{
						url = CheckNextLink(nextlink);
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
				Thread.Sleep(300);
			}
			PrintResultFilename(allResponses);
		}
	}
}