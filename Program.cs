using System.Dynamic;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using APIcalltest;
using DotNetEnv;
using Newtonsoft.Json;
using Sprache;

public class PostData()
{
	public List<int> IntList { get; set; } = new List<int>();
	public List<string> StringList { get; set; } = new List<string>();
}

class Program
{
	public static void CheckUrlAndRespond(HttpStatusCode code, bool success, string call)
	{
		if (success)
		{
			Console.WriteLine($"{call} call ended successfully");
		}
		else
		{
			Console.WriteLine($"There was a problem with your {call} call, error code: {code}");
		}
	}
	private static void ExitFunction()
	{
		Environment.Exit(0);
	}
	private static string? AnotherCall()
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
	static async Task Main()
	{
		TokenClass? token = await TokenClass.GetToken();
		string? userInput = null;
		while (userInput != "EXIT")
		{
			if (token == null)
			{
				Console.WriteLine("Insert your .env file in the same folder as APIcalltest and configure it to be like the .envexample!");
				break;
			}
			Console.WriteLine("Enter your desired call: GET, POST, PATCH or DELETE. Enter EXIT to exit the program.");
			userInput = Console.ReadLine()?.ToUpperInvariant();
			string url = "https://api.intra.42.fr";
			if (token != null)
			{
				switch (userInput)
				{
					case "GET":
						await GetClass.GetFunction(token, url);
						string? anotherCall = null;
						while (anotherCall != "YES" && anotherCall != "NO")
						{
							anotherCall = AnotherCall();
						}
						break;

					case "POST":
					    await PostClass.PostFunction(token, url);
						anotherCall = null;
					    while (anotherCall != "YES" && anotherCall != "NO")
					    {
						    anotherCall = AnotherCall();
					    }
						break;

					case "PUT" :
						await PutClass.PutFunction(token, url);
						anotherCall = null;
						while (anotherCall != "YES" && anotherCall != "NO")
						{
							anotherCall = AnotherCall();
						}
						break;

					case "PATCH":
						await PatchClass.PatchFunction(token, url);
						anotherCall = null;
						while (anotherCall != "YES" && anotherCall != "NO")
						{
							anotherCall = AnotherCall();
						}
						break;

					case "DELETE":
					    await DeleteClass.DeleteFunction(token, url);
						anotherCall = null;
						while (anotherCall != "YES" && anotherCall != "NO")
						{
							anotherCall = AnotherCall();
						}
						break;

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