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
			Console.WriteLine("Enter your desired call: GET, POST, DELETE. Enter EXIT to exit the program.");
			userInput = Console.ReadLine()?.ToUpperInvariant();
			if (token != null)
			{
				switch (userInput)
				{
					case "GET":
						await GetClass.GetFunction(token);
						string? anotherCall = null;
						while (anotherCall != "YES" && anotherCall != "NO")
						{
							anotherCall = AnotherCall();
						}
						break;

					case "POST":
					 	await PostClass.PostFunction(token);
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