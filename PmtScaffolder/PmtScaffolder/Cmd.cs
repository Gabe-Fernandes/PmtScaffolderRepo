using System.Text.RegularExpressions;

namespace PmtScaffolder;

public static class Cmd
{
  private static UserInput _userInput = UserInput.GetUserInput();
  private static readonly Regex regex = new(@"^[a-zA-Z0-9_,@]+$");
  private static readonly Regex pathRegex = new(@"^[a-zA-Z0-9_,\-/"" .\\:]+$");
  private static readonly Regex projNameRegex = new(@"^[a-zA-Z0-9_,\-/""()!@#$%^&*. ]+$");

  public static async Task<bool> RunValidInput(string input)
  {
    string normalizedInput = input.ToLower();

    switch (normalizedInput)
    {
      case "exit": Environment.Exit(0); return true;
      case "pmt exit": Environment.Exit(0); return true;
      case "cls": Console.Clear(); return true;
      case "pmt cls": Console.Clear(); return true;
      case "pmt get proj-path": Console.WriteLine(_userInput.ProjPath); return true;
      case "pmt get proj-name": Console.WriteLine(_userInput.ProjName); return true;
      case "pmt get models": PrintCollection(_userInput.Models); return true;
      case "pmt get file-names": Print2DCollection(_userInput.FileNames); return true;
      case "pmt get data-types": Print2DCollection(_userInput.DataTypes); return true;
      case "pmt get properties": Print2DCollection(_userInput.Properties); return true;
      case "pmt get controllers": PrintCollection(_userInput.Controllers); return true;
      case "pmt args": PrintArgs(); return true;
      case "pmt front": await FrontEnd.ScaffoldFrontEndCode(); return true;
      case "pmt back": await BackEnd.ScaffoldBackEndCode(); return true;
    }

    if (normalizedInput.IndexOf("pmt set ") == 0)
    {
      return ValidSetCmd(normalizedInput, input);
    }

    return false;
  }



  private static bool ValidSetCmd(string normalizedInput, string input)
  {
    if (normalizedInput.IndexOf("proj-path ") == 8 &&
      pathRegex.IsMatch(input[18..]))
    {
      _userInput.ProjPath = input[18..];
      return true;
    }
    else if (normalizedInput.IndexOf("proj-name ") == 8 &&
      projNameRegex.IsMatch(input[18..]))
    {
      _userInput.ProjName = input[18..];
      return true;
    }
    else if (normalizedInput.IndexOf("controllers ") == 8 &&
      regex.IsMatch(input[20..]))
    {
      _userInput.Controllers = ParseInput(input[20..]);
      return true;
    }
    else if (normalizedInput.IndexOf("file-names ") == 8 &&
      regex.IsMatch(input[19..]))
    {
      _userInput.FileNames = Parse2DInput(input[19..]);
      return true;
    }
    else if (normalizedInput.IndexOf("models ") == 8 &&
      regex.IsMatch(input[15..]))
    {
      _userInput.Models = ParseInput(input[15..]);
      return true;
    }
    else if (normalizedInput.IndexOf("data-types ") == 8 &&
      regex.IsMatch(input[19..]))
    {
      _userInput.DataTypes = Parse2DInput(input[19..]);
      return true;
    }
    else if (normalizedInput.IndexOf("properties ") == 8 &&
      regex.IsMatch(input[19..]))
    {
      _userInput.Properties = Parse2DInput(input[19..]);
      return true;
    }

    return false;
  }

  private static List<string> ParseInput(string input)
  {
    List<string> collection = [];
    string tempString = string.Empty;
    for (int i = 0; i < input.Length; i++)
    {
      if (input[i] == ',')
      {
        collection.Add(tempString);
        tempString = string.Empty;
        continue;
      }
      tempString += input[i];
    }
    collection.Add(tempString);
    return collection;
  }

  private static List<List<string>> Parse2DInput(string input)
  {
    List<List<string>> collection = [];
    List<string> tempList = [];
    string tempString = string.Empty;

    for (int i = 0; i < input.Length; i++)
    {
      // this char separates elements
      if (input[i] == ',')
      {
        tempList.Add(tempString);
        tempString = string.Empty;
        continue;
      }
      // this char separates lists
      if (input[i] == '@')
      {
        tempList.Add(tempString);
        collection.Add(tempList);

        tempList = [];
        tempString = string.Empty;
        continue;
      }

      tempString += input[i];
    }

    tempList.Add(tempString);
    collection.Add(tempList);
    return collection;
  }

  private static void PrintCollection(List<string> collection)
  {
    foreach (string item in collection)
    {
      Console.WriteLine(item);
    }
  }

  private static void Print2DCollection(List<List<string>> collection)
  {
    foreach (List<string> list in collection)
    {
      foreach (string item in list)
      {
        Console.WriteLine(item);
      }
      Console.WriteLine();
    }
  }

  private static void PrintArgs()
  {
    Console.WriteLine("\n===== Scaffold Arguments =====");
    Console.Write("\nProject Name: ");
    Console.WriteLine(_userInput.ProjName);
    Console.Write("\nProject Path: ");
    Console.WriteLine(_userInput.ProjPath);
    Console.Write("\nTest Project Name: ");
    Console.WriteLine(_userInput.TestProjName);
    Console.Write("\nTest Project Path: ");
    Console.WriteLine(_userInput.TestProjPath);
    Console.WriteLine("\nControllers:");
    PrintCollection(_userInput.Controllers);
    Console.WriteLine("\nFile Names:");
    Print2DCollection(_userInput.FileNames);
    Console.WriteLine("Models:");
    PrintCollection(_userInput.Models);
    Console.WriteLine("\nData Types:");
    Print2DCollection(_userInput.DataTypes);
    Console.WriteLine("Properties:");
    Print2DCollection(_userInput.Properties);
    Console.WriteLine("===== ___ =====\n");
  }
}
