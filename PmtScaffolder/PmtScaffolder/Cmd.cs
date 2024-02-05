using System.Text.RegularExpressions;

namespace PmtScaffolder;

public static class Cmd
{
  private static UserInput _userInput = UserInput.GetUserInput();
  private static readonly Regex regex = new(@"^[a-zA-Z0-9_,]+$");
  private static readonly Regex pathRegex = new(@"^[a-zA-Z0-9_,\-/"" .]+$");
  private static readonly Regex projNameRegex = new(@"^[a-zA-Z0-9_,\-/""()!@#$%^&*. ]+$");

  public static bool RunValidInput(string input)
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
      case "pmt get models": Console.WriteLine(_userInput.Models); return true;
      case "pmt get file-names": Console.WriteLine(_userInput.FileNames); return true;
      case "pmt get data-types": Console.WriteLine(_userInput.DataTypes); return true;
      case "pmt get properties": Console.WriteLine(_userInput.Properties); return true;
      case "pmt get controllers": Console.WriteLine(_userInput.Controllers); return true;
      case "pmt args": PrintArgs(); return true;
      case "pmt front": return true;
      case "pmt back": return true;
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
      _userInput.Controllers = input[20..];
      return true;
    }
    else if (normalizedInput.IndexOf("file-names ") == 8 &&
      regex.IsMatch(input[19..]))
    {
      _userInput.FileNames = input[19..];
      return true;
    }
    else if (normalizedInput.IndexOf("models ") == 8 &&
      regex.IsMatch(input[15..]))
    {
      _userInput.Models = input[15..];
      return true;
    }
    else if (normalizedInput.IndexOf("data-types ") == 8 &&
      regex.IsMatch(input[19..]))
    {
      _userInput.DataTypes = input[19..];
      return true;
    }
    else if (normalizedInput.IndexOf("properties ") == 8 &&
      regex.IsMatch(input[19..]))
    {
      _userInput.Properties = input[19..];
      return true;
    }

    return false;
  }

  private static void PrintArgs()
  {
    Console.WriteLine("\n===== Scaffold Arguments =====");
    Console.WriteLine("\nProject Name:");
    Console.WriteLine(_userInput.ProjName);
    Console.WriteLine("\nProject Path:");
    Console.WriteLine(_userInput.ProjPath);
    Console.WriteLine("\nControllers:");
    Console.WriteLine(_userInput.Controllers);
    Console.WriteLine("\nFile Names:");
    Console.WriteLine(_userInput.FileNames);
    Console.WriteLine("\nModels:");
    Console.WriteLine(_userInput.Models);
    Console.WriteLine("\nData Types:");
    Console.WriteLine(_userInput.DataTypes);
    Console.WriteLine("\nProperties:");
    Console.WriteLine(_userInput.Properties);
    Console.WriteLine("\n===== ___ =====\n");
  }
}
