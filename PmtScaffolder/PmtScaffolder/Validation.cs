using System.Text.RegularExpressions;

namespace PmtScaffolder;

public static class Validation
{
  private static UserInput _userInput = UserInput.GetUserInput();
  private static readonly Regex regex = new(@"^[a-zA-Z0-9_,@]+$");
  private static readonly Regex pathRegex = new(@"^[a-zA-Z0-9_,\-/"" .\\:]+$");
  private static readonly Regex projNameRegex = new(@"^[a-zA-Z0-9_,\-/""()!@#$%^&*. ]+$");

  public static bool Models()
  {
    if (_userInput.Models.Count != _userInput.Properties.Count)
    {
      Console.WriteLine("\nInvalid input - every model needs at least one property\n");
      Console.WriteLine($"\nThere are {_userInput.Models.Count} models and {_userInput.Properties.Count} property lists\n");
      return false;
    }

    if (_userInput.Properties.Count != _userInput.DataTypes.Count)
    {
      Console.WriteLine($"\nInvalid input - property list and data type lists don't match\n");
      Console.WriteLine($"\nThere are {_userInput.Properties.Count} property lists and {_userInput.DataTypes.Count} data type lists\n");
      return false;
    }

    for (int i = 0;  i < _userInput.Properties.Count; i++)
    {
      if (_userInput.Properties[i].Count != _userInput.DataTypes[i].Count)
      {
        Console.WriteLine("\nInvalid input - every property needs exactly one data type\n");
        Console.WriteLine($"                     -- {_userInput.Models[i]} --");
        Console.WriteLine("\nProperties:\n");
        Cmd.PrintCollection(_userInput.Properties[i]);
        Console.WriteLine("\nData Types:\n");
        Cmd.PrintCollection(_userInput.DataTypes[i]);
        Console.WriteLine();
        return false;
      }
    }

    return true;
  }

  public static bool Controllers()
  {
    if (_userInput.Controllers.Count != _userInput.FileNames.Count)
    {
      Console.WriteLine("\nInvalid input - every controller needs at least one file name\n");
      Console.WriteLine($"\nThere are {_userInput.Controllers.Count} controllers and {_userInput.FileNames.Count} file name lists\n");
      return false;
    }

    return true;
  }

  public static bool ValidSetCmd(string normalizedInput, string input)
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
    else if (normalizedInput.IndexOf("test-proj-path ") == 8 &&
      pathRegex.IsMatch(input[23..]))
    {
      _userInput.TestProjPath = input[23..];
      return true;
    }
    else if (normalizedInput.IndexOf("test-proj-name ") == 8 &&
      projNameRegex.IsMatch(input[23..]))
    {
      _userInput.TestProjName = input[23..];
      return true;
    }
    else if (normalizedInput.IndexOf("controllers ") == 8 &&
      regex.IsMatch(input[20..]))
    {
      _userInput.Controllers = Cmd.ParseInput(input[20..]);
      return true;
    }
    else if (normalizedInput.IndexOf("file-names ") == 8 &&
      regex.IsMatch(input[19..]))
    {
      _userInput.FileNames = Cmd.Parse2DInput(input[19..]);
      return true;
    }
    else if (normalizedInput.IndexOf("models ") == 8 &&
      regex.IsMatch(input[15..]))
    {
      _userInput.Models = Cmd.ParseInput(input[15..]);
      return true;
    }
    else if (normalizedInput.IndexOf("data-types ") == 8 &&
      regex.IsMatch(input[19..]))
    {
      _userInput.DataTypes = Cmd.Parse2DInput(input[19..]);
      return true;
    }
    else if (normalizedInput.IndexOf("properties ") == 8 &&
      regex.IsMatch(input[19..]))
    {
      _userInput.Properties = Cmd.Parse2DInput(input[19..]);
      return true;
    }

    return false;
  }

  public static void PrintErrorReport()
  {
    if (_userInput.ErrorReport.Count != 0)
    {
      Console.WriteLine("\n\n\n_______________________________________ ERROR REPORT _______________________________________\n\n\n");
    }

    for (int i = 0; i < _userInput.ErrorReport.Count; i++)
    {
      Console.WriteLine(_userInput.ErrorReport[i]);
    }
  }
}
