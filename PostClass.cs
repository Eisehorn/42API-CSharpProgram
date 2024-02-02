using System.Text;
using Newtonsoft.Json;

namespace APIcalltest;

public class PostClass
{
	protected static void NumberInput(string str, Dictionary<string, object?> dictionary, string? valuekey)
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
	protected static void StringInput(string str, Dictionary<string, object?> dictionary, string? valuekey)
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
				break;
			}
		}
	}

	protected static void DataInput(string str, Dictionary<string, object?> dictionary, string? valuekey, PostData data)
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
	
	protected static string ElementToCreate()
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
	protected static void InnerDictionary(string str, Dictionary<string, object?> dictionary, string? valuekey)
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
	
	protected static StringContent CreateContent()
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
	
	public static async Task PostFunction(TokenClass token, string url)
	{
		using (HttpClient client = new HttpClient())
		{
			client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token.access_token}");
			while (true)
			{
				Console.WriteLine("Insert POST request endpoint:");
				string? endpoint = Console.ReadLine();
				if (endpoint == null)
				{
					continue;
				}
				url += endpoint;
				break;
			}
			var content = CreateContent();
			HttpResponseMessage response = await client.PostAsync(url, content);
			Console.WriteLine("\n\n" + GetClass.ParseJson(await content.ReadAsStringAsync()));
			Program.CheckUrlAndRespond(response.IsSuccessStatusCode, "POST");
		}
	}
}