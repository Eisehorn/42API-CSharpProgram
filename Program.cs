using System.Dynamic;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using DotNetEnv;
using Newtonsoft.Json;
using Sprache;

public class Token()
{
	public string? access_token { get; set; }
	public string? token_type { get; set; }
	public int? expires_in { get; set; }
	public string? scope { get; set; }
	public int? created_at { get; set; }
	public int? secret_valid_until { get; set; }
}

public class ApiResponse
{
	public dynamic? Data { get; set; }
}

public class PostData()
{
	public List<int> IntList { get; set; } = new List<int>();
	public List<string> StringList { get; set; } = new List<string>();
}

class Program
{
	//TOKEN CREATION
	static async Task<Token?> GetToken()
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
					Token? token = JsonConvert.DeserializeObject<Token>(result);
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
	// GET FUNCTIONS
	static void PrintResult(List<string> allResponses)
	{
		ApiResponse apiResponse = new ApiResponse();
		int page = 1;
		foreach (var response in allResponses)
		{
			Console.WriteLine($"Result of page number {page}:");
			page++;
			apiResponse.Data = ParseJson(response);
			Console.WriteLine(apiResponse.Data);
		}
	}
	static void PrintResultFilename(List<string> allResponses)
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
	
	static string CheckNextLink(string nextlink)
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
	
	static string? InsertUrl()
	{
		Console.WriteLine("Enter GET request endpoint:");
		string? endpoint = Console.ReadLine();
		return endpoint;
	}

	static string InsertFilter()
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

	static async Task<string> CheckIfCorrectUrl(HttpClient client)
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
	static dynamic ParseJson(string json)
	{
		dynamic parsedData = JsonConvert.DeserializeObject<dynamic>(json)!;
		return parsedData;
	}

	static dynamic StringToJson(string str)
	{
		dynamic parsedData = JsonConvert.SerializeObject(str);
		return parsedData;
	}
	static async Task GetFunction(Token token)
	{
		using (HttpClient client = new HttpClient())
		{
			List<string> allResponses = new List<string>();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.access_token}");
			string url = $"https://api.intra.42.fr";
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
	
	//POST FUNCTIONS
	static void NumberInput(string str, Dictionary<string, object?> dictionary, string? valuekey)
	{
		while (true)
		{
			Console.WriteLine($"Enter your {str}");
			valuekey = Console.ReadLine();
			if (!string.IsNullOrEmpty(valuekey))
			{
				try
				{
					int inputNumber = int.Parse(valuekey);
					dictionary.Add(str, inputNumber);
					break;
				}
				catch (FormatException)
				{
					Console.WriteLine("Invalid input for an integer. Try again.");
				}
			}
			else
			{
				Console.WriteLine("Empty string is not a valid input for an integer. Try again.");
			}
		}
	}
	static void StringInput(string str, Dictionary<string, object?> dictionary, string? valuekey)
	{
		Console.WriteLine($"Enter your {str}");
		valuekey = Console.ReadLine();
		while (true)
		{
			Console.WriteLine("Can your string be empty?");
			string answer = Console.ReadLine()!.ToUpperInvariant();
			if (string.IsNullOrEmpty(answer))
			{
				Console.WriteLine("Invalid input. Please enter yes or no.");
			}
			else if (answer == "YES")
			{
				dictionary.Add(str, valuekey);
				break;
			}
			else if (answer == "NO")
			{
				while (true)
				{
					if (string.IsNullOrEmpty(valuekey))
					{
						Console.WriteLine("Empty input is invalid. Please enter a valid input.");
						Console.WriteLine($"Enter your {str}");
						valuekey = Console.ReadLine();
					}
					else
					{
						dictionary.Add(str, valuekey);
						break;
					}
				}
			}
		}
	}

	static string ElementToCreate()
	{
		string? elementToCreate;
		while (true)
		{
			Console.WriteLine("Enter the element to create:");
			elementToCreate = Console.ReadLine();
			if (string.IsNullOrEmpty(elementToCreate))
			{
				Console.WriteLine("Input cannot be empty.");
			}
			else
			{
				break;
			}
		}
		return elementToCreate;
	}

	static void DataInput(string str, Dictionary<string, object?> dictionary, string? valuekey, PostData data)
	{
		while (true)
		{
			Console.WriteLine("Do you want to add a list of numbers or strings?");
			string? answer = Console.ReadLine()!.ToUpperInvariant();
			if (answer == "STRINGS" || answer == "NUMBERS")
			{
				Console.WriteLine($"Enter all attributes you want to assign to {str} separated by a comma.");
				var fields = Console.ReadLine()!;
				string[] attribute = fields.Split(',');
				attribute = attribute.Select(parameter => parameter.Trim()).ToArray();
				foreach (string field in attribute)
				{
					if (answer == "NUMBERS")
					{
						try
						{
							int inputNumber = int.Parse(field);
							data.IntList.Add(inputNumber);
						}
						catch (FormatException)
						{
							Console.WriteLine("Invalid input for an integer. Try again.");
						}
					}
					else
					{
						data.StringList.Add(field);
					}
				}
				if (answer == "NUMBERS")
				{
					string jsonNumber = JsonConvert.SerializeObject(data.IntList);
					dictionary.Add(str, jsonNumber);
				}
				else
				{
					string jsonString = JsonConvert.SerializeObject(data.StringList);
					dictionary.Add(str, jsonString);
				}
				break;
			}
			Console.WriteLine("Input error. Please answer numbers or strings.");
		}
	}
	static void InnerDictionary(string str, Dictionary<string, object?> dictionary, string? valuekey)
	{
		while (true)
		{
			Console.WriteLine($"Do you need a string, number, dictionary or data as input for: {str}?");
			string? input = Console.ReadLine()!.ToUpperInvariant();
			if (input == "STRING")
			{
				StringInput(str, dictionary, valuekey);
				break;
			}
			else if (input == "NUMBER")
			{
				NumberInput(str, dictionary, valuekey);
				break;
			}
			else if (input == "DICTIONARY")
			{
				Console.WriteLine("Enter all the required inner fields here separated by a comma.\nUsage example: campus, first_name, last_name (Do not put an empty string)");
				var fields = Console.ReadLine()!;
				string[] fieldkey = fields.Split(',');
				fieldkey = fieldkey.Select(parameter => parameter.Trim()).ToArray();
				string? innerValueKey = null;
				var innerDictionary = new Dictionary<string, object?>();
				foreach (string field in fieldkey)
				{
					InnerDictionary(field, innerDictionary, innerValueKey);
				}
				dictionary.Add(str, innerDictionary);
				break;
			}
			else if (input == "DATA")
			{
				PostData data = new PostData();
				DataInput(str, dictionary, valuekey, data);
				break;
			}
			Console.WriteLine("Invalid Input. Please enter string or number as input.");
		}
	}
	
	static StringContent CreatePostContent()
	{
		Console.WriteLine("Enter all the required fields here separated by a comma.\nUsage example: campus, first_name, last_name (Do not put an empty string)");
		var fields = Console.ReadLine()!;
		string[] fieldkey = fields.Split(',');
		fieldkey = fieldkey.Select(parameter => parameter.Trim()).ToArray();
		string? valuekey = null;
		Dictionary<string, object?> dictionary = new Dictionary<string, object?>();
		foreach (string str in fieldkey)
		{
			InnerDictionary(str, dictionary, valuekey);
		}
		while (true)
		{
			Console.WriteLine("Do you need to add an element with his data?\nUsage example Yes: user: { {key, value}}\nUsage example No: {key, value}");
			string? answer = Console.ReadLine()?.ToUpperInvariant();
			if (answer == "YES")
			{
				string? elementToCreate = ElementToCreate();
				var postContent = new Dictionary<string, object>()
				{
					{ elementToCreate, dictionary }
				};
				string json = JsonConvert.SerializeObject(postContent);
				var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
				return jsonContent;
			}
			else if (answer == "NO")
			{
				string json = JsonConvert.SerializeObject(dictionary);
				var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
				return jsonContent;
			}
			else
			{
				Console.WriteLine("Invalid input.");
			}
		}
	}

	static void CheckUrlAndRespond(bool success, string call)
	{
		if (success)
		{
			Console.WriteLine($"{call} call ended successfully");
		}
		else
		{
			Console.WriteLine($"There was a problem with your {call} call");
		}
	}
	static async Task PostFunction(Token token)
	{
		using (HttpClient client = new HttpClient())
		{
			client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token.access_token}");
			string url = "https://api.intra.42.fr";
			Console.WriteLine("Insert POST request endpoint:");
			url += Console.ReadLine();
			var content = CreatePostContent();
			HttpResponseMessage response = await client.PostAsync(url, content);
			Console.WriteLine("\n\n" + ParseJson(await content.ReadAsStringAsync()));
			CheckUrlAndRespond(response.IsSuccessStatusCode, "POST");
		}
	}
	
	//EXIT FUNCTION
	static void ExitFunction()
	{
		Environment.Exit(0);
	}

	//CHECK FOR ANOTHER CALL
	static string? AnotherCall()
	{
		Console.WriteLine('\n' + "Do you want to perform another call?");
		string? anotherCall = Console.ReadLine()?.ToUpperInvariant();
		if (anotherCall == "NO")
			ExitFunction();
		else if (anotherCall != "YES")
		{
			Console.WriteLine("Invalid input. Please enter Yes or No");
		}
		return anotherCall;
	}
	
	//MAIN
	static async Task Main()
	{
		Token? token = await GetToken();
		string? userInput = null;
		while (userInput != "EXIT")
		{
			Console.WriteLine("Enter your desired call: GET, POST, DELETE. Enter EXIT to exit the program.");
			userInput = Console.ReadLine()?.ToUpperInvariant();
			if (token != null)
			{
				switch (userInput)
				{
					case "GET":
						await GetFunction(token);
						string? anotherCall = null;
						while (anotherCall != "YES" && anotherCall != "NO")
						{
							anotherCall = AnotherCall();
						}
						break;

					case "POST":
					 	await PostFunction(token);
						anotherCall = null;
					    while (anotherCall != "YES" && anotherCall != "NO")
					    {
						    anotherCall = AnotherCall();
					    }
						break;
					//
					// case "DELETE":
					// 	await deleteFunction(token);
					// 	break;

					case "EXIT":
						ExitFunction();
						break;
					
					default:
						Console.WriteLine("Invalid input. Please enter GET, POST, DELETE or EXIT.");
						break;
				}
			}
		}
	}
}