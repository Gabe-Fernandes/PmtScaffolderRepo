using Syroot.Windows.IO;

namespace PmtScaffolder;

public static class Cmd
{
  private static UserInput _userInput = UserInput.GetUserInput();

  public static async Task<bool> RunValidInput(string input)
  {
    string normalizedInput = input.ToLower();

    switch (normalizedInput)
    {
      case "exit": Environment.Exit(0); return true;
      case "pmt exit": Environment.Exit(0); return true;
      case "cls": Console.Clear(); return true;
      case "pmt cls": Console.Clear(); return true;
      case "pmt": PrintPmtHelp(); return true;
      case "pmt help": PrintPmtHelp(); return true;
      case "pmt import": ReadImportedFile(); return true;
      case "pmt get proj-path": Console.WriteLine(_userInput.ProjPath); return true;
      case "pmt get proj-name": Console.WriteLine(_userInput.ProjName); return true;
      case "pmt get test-proj-path": Console.WriteLine(_userInput.TestProjPath); return true;
      case "pmt get test-proj-name": Console.WriteLine(_userInput.TestProjName); return true;
      case "pmt get models": PrintCollection(_userInput.Models); return true;
      case "pmt get file-names": Print2DCollection(_userInput.FileNames); return true;
      case "pmt get data-types": Print2DCollection(_userInput.DataTypes); return true;
      case "pmt get properties": Print2DCollection(_userInput.Properties); return true;
      case "pmt get controllers": PrintCollection(_userInput.Controllers); return true;
      case "pmt args": PrintArgs(); return true;
      case "pmt front": 
        if (Validation.Controllers())
        {
          await FrontEnd.ScaffoldFrontEndCode();
        }
        return true;
      case "pmt back":
        if (Validation.Models())
        {
          await BackEnd.ScaffoldBackEndCode();
        }
        return true;
    }

    if (normalizedInput.IndexOf("pmt set ") == 0)
    {
      return Validation.ValidSetCmd(normalizedInput, input);
    }

    return false;
  }



  private static void ReadImportedFile()
  {
    string downloadsPath = KnownFolders.Downloads.Path;
    string path = Path.Combine(downloadsPath, "___PMT___DATA___FROM___WEBAPP___.txt");

    if (File.Exists(path))
    {
      using (StreamReader sr = File.OpenText(path))
      {
        string fileText = sr.ReadToEnd();
        Console.WriteLine("\nImporting...\n");
        Console.WriteLine(fileText);
        ImportedDataHandler handler = new();
        ImportedDataHandler.ParseImportedFile(fileText);
      }
      return;
    }

    Console.WriteLine("\nFailed to find PMT data\n");
  }



  public static List<string> ParseInput(string input)
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

  public static List<List<string>> Parse2DInput(string input)
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

  public static void PrintCollection(List<string> collection)
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

  private static void PrintModels()
  {
    int iterations = Math.Max(_userInput.Models.Count, Math.Max(_userInput.DataTypes.Count, _userInput.Properties.Count));

    for (int i = 0; i < iterations; i++)
    {
      string model = "_____";

      if (_userInput.Models.Count > i)
      {
        model = _userInput.Models[i];
      }

      Console.WriteLine($"{model}:");

      int innerPropCount = 0;
      int innerDataTypeCount = 0;

      if (_userInput.Properties.Count > i)
      {
        innerPropCount = _userInput.Properties[i].Count;
      }

      if (_userInput.DataTypes.Count > i)
      {
        innerDataTypeCount = _userInput.DataTypes[i].Count;
      }

      int innerIterations = Math.Max(innerPropCount, innerDataTypeCount);

      for (int j = 0; j < innerIterations; j++)
      {
        string dataType = "_____";
        string property = "_____";

        // these if statements are to ensure these collections contain elements at the given indeces
        if (_userInput.DataTypes.Count > i)
        {
          if (_userInput.DataTypes[i].Count > j)
          {
            dataType = _userInput.DataTypes[i][j];
          }
        }

        if (_userInput.Properties.Count > i)
        {
          if (_userInput.Properties[i].Count > j)
          {
            property = _userInput.Properties[i][j];
          }
        }

        Console.WriteLine($"-- {dataType} {property}");
      }
    }
  }

  private static void PrintArgs()
  {
    Console.WriteLine("\n==================== Scaffold Arguments ====================");
    Console.Write("\nProject Name: ");
    Console.WriteLine(_userInput.ProjName);
    Console.Write("\nProject Path: ");
    Console.WriteLine(_userInput.ProjPath);
    Console.Write("\nTest Project Name: ");
    Console.WriteLine(_userInput.TestProjName);
    Console.Write("\nTest Project Path: ");
    Console.WriteLine(_userInput.TestProjPath);
    Console.WriteLine("\n============================================================\n");
    Console.WriteLine("===== Controllers =====");
    PrintCollection(_userInput.Controllers);
    Console.WriteLine("\n===== File Names =====");
    Print2DCollection(_userInput.FileNames);
    Console.WriteLine("\n========================== Models ==========================\n");
    PrintModels();
    Console.WriteLine("\n============================================================\n");
  }

  private static void PrintPmtHelp()
  {
    Console.WriteLine("\n==================== PMT ====================\n");
    Console.WriteLine("pmt exit");
    Console.WriteLine("exit");
    Console.WriteLine("pmt cls");
    Console.WriteLine("cls");
    Console.WriteLine("pmt help");
    Console.WriteLine("pmt");

    Console.WriteLine("\npmt get proj-path");
    Console.WriteLine("pmt get proj-name");
    Console.WriteLine("pmt get test-proj-path");
    Console.WriteLine("pmt get test-proj-name");

    Console.WriteLine("pmt get controllers");
    Console.WriteLine("pmt get file-names");

    Console.WriteLine("pmt get models");
    Console.WriteLine("pmt get data-types");
    Console.WriteLine("pmt get properties");

    Console.WriteLine("\n\n-Separate lists with ',' (no spaces).");
    Console.WriteLine("Example:");
    Console.WriteLine("pmt set controllers account,inventory,crm\n\n");
    Console.WriteLine("-2D lists like properties for the models list use ',' and '@' similarly");
    Console.WriteLine("Example:");
    Console.WriteLine("pmt set properties id,firstName,lastName,age@id,title,description\n\n");
    Console.WriteLine("-The first char will automatically be made upper/lower case as needed.\n\n");

    Console.WriteLine("pmt set proj-path");
    Console.WriteLine("pmt set proj-name");
    Console.WriteLine("pmt set test-proj-path");
    Console.WriteLine("pmt set test-proj-name");

    Console.WriteLine("pmt set controllers");
    Console.WriteLine("pmt set file-names");

    Console.WriteLine("pmt set models");
    Console.WriteLine("pmt set data-types");
    Console.WriteLine("pmt set properties");

    Console.WriteLine("\npmt import\n");

    Console.WriteLine("\npmt args");
    Console.WriteLine("pmt front");
    Console.WriteLine("pmt back");
    Console.WriteLine("\n=============================================\n");
  }
}
