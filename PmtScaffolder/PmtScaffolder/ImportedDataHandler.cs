namespace PmtScaffolder;

public class ImportedDataHandler
{
  public const string delimiter = "_____&_____";
  public const string newModelDelimiter = "M____&_____";
  private static readonly UserInput _userInput = UserInput.GetUserInput();

  public static void ParseImportedFile(string data, string importType)
  {
    int endOf1dDataIndex = data.IndexOf("###PMT###");
    string raw1dData = data.Substring(0, endOf1dDataIndex);
    string raw2dData = data.Substring(endOf1dDataIndex + 9);

    if (importType == "front")
    {
      _userInput.Controllers = ExtractData(raw1dData);
      _userInput.FileNames = Extract2dData(raw2dData);
    }
    if (importType == "back")
    {
      _userInput.Models = ExtractData(raw1dData);
      ParsePropData(raw2dData);
    }
  }

  private static void ParsePropData(string rawPropData)
  {
    List<List<string>> propData = Extract2dData(rawPropData);

    for (int i = 0; i < propData.Count; i++)
    {
      _userInput.Properties.Add([]);
      _userInput.DataTypes.Add([]);
      for (int j = 0; j < propData[i].Count; j++)
      {
        if (j % 2 == 0)
        {
          _userInput.DataTypes[i].Add(propData[i][j]);
          continue;
        }
        _userInput.Properties[i].Add(propData[i][j]);
      }
    }
  }

  private static List<string> ExtractData(string data)
  {
    if (string.IsNullOrEmpty(data)) { return []; }

    List<string> extractedList = [];
    string extractedElement = "";

    for (int i = 0; i < data.Length; i++)
    {
      if (CheckForDelimiter(data, i, delimiter))
      {
        i += delimiter.Length - 1;
        extractedList.Add(extractedElement);
        extractedElement = "";
        continue;
      }
      extractedElement += data[i];
    }

    return extractedList;
  }

  private static bool CheckForDelimiter(string data, int index, string delimiter)
  {
    for (int i = 0; i < delimiter.Length; i++)
    {
      if (delimiter[i] != data[index]) { return false; }
      index++;
    }
    return true;
  }

  private static List<List<string>> Extract2dData(string data)
  {
    if (string.IsNullOrEmpty(data)) { return []; }

    List<List<string>> extractedList = [];
    int currentModelIndex = -1;
    string extractedElement = "";

    for (int i = 0; i < data.Length; i++)
    {
      if (CheckForDelimiter(data, i, delimiter))
      {
        i += delimiter.Length - 1;
        extractedList[currentModelIndex].Add(extractedElement);
        extractedElement = "";
      }
      else if (CheckForDelimiter(data, i, newModelDelimiter))
      {
        i += delimiter.Length - 1;
        List<string> modelContainer = [];
        currentModelIndex++;
        extractedList.Add(modelContainer);
      }
      else
      {
        extractedElement += data[i];
      }
    }

    return extractedList;
  }
}
