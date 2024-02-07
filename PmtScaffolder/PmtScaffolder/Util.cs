namespace PmtScaffolder;

public static class Util
{
  private static readonly string br = Environment.NewLine;

  public static int TestPath(string currentPathToTestFrom, string pathToTest)
  {
    var bufferedCmd = PSCmd.RunPowerShell(currentPathToTestFrom, $"Test-Path -Path {pathToTest}");
    if (bufferedCmd.Result == null) { return -1; }
    return (bufferedCmd.Result.StandardOutput == "True\r\n") ? 1 : 0;
  }



  public static async Task InsertCode(string parentPath, string codeToInsert, string fileNameWithExtension, bool htmlLandmark = false)
  {
    string fileText = await ExtractFileText(parentPath, fileNameWithExtension);
    if (string.IsNullOrEmpty(fileText)) { return; }

    // locate PMT Landmark
    string landmark = htmlLandmark ? "<!--PMT Landmark-->" : "// PMT Landmark";
    int landmarkIndex = fileText.IndexOf(landmark);
    if (landmarkIndex == -1) // error handling
    {
      Console.WriteLine($"\n===========================Please ensure the code file at {Path.Combine(parentPath, fileNameWithExtension)} has the comment, \"{landmark}\" above where lines of code are to be inserted.===========================\n");
      return;
    }

    // partition code file, concatenate, and write
    string preLandmark = fileText.Substring(0, landmarkIndex + landmark.Length);
    string postLandmark = fileText.Substring(landmarkIndex + landmark.Length);
    string finalOutput = PartitionCodeFile(preLandmark + codeToInsert + postLandmark);

    await PSCmd.RunPowerShellBatch(parentPath, CreateTemplateFormat(finalOutput, fileNameWithExtension));
  }



  public static async Task<string> ExtractFileText(string parentPath, string fileNameWithExtension)
  {
    var fileContent = await PSCmd.RunPowerShell(parentPath, $"Get-Content -Path {Path.Combine(parentPath, fileNameWithExtension)} -Raw");
    if (fileContent == null) { return string.Empty; }
    return fileContent.StandardOutput;
  }



  public static string PartitionCodeFile(string output)
  {
    output = output.Substring(0, output.Length - 4); // chop the last 2 line breaks off
    // multiple iterations of insertions were adding spaces at the end of files due to how PowerShell writes them which eventually register as tabs - remove this space when it's present
    output = (output[output.Length - 1] == ' ') ? output.Substring(0, output.Length - 1) : output;
    output = output.Replace("  ", "PMT_TAB");
    return output.Replace("\t", "PMT_TAB");
  }



  public static string[] CreateTemplateFormat(string rawOutput, string fileNameWithExtension)
  {
    List<string> output = [];
    string tempString = "Write-Output '";

    for (int i = 0; i < rawOutput.Length; i++)
    {
      if (rawOutput[i..].StartsWith("PMT_TAB"))
      {
        // add \t for every occurence of PMT_TAB
        tempString += "\t";

        for (int j = i + 7; j < rawOutput.Length; j += 7)
        {
          if (rawOutput[j..].StartsWith("PMT_TAB") == false) { i = j - 1; break; }
          tempString += "\t";
        }
        continue;
      }

      if (rawOutput[i..].StartsWith(" \r\n"))
      {
        output.Add(tempString);

        // add line breaks for every occurence of " \r\n" after the first occurence (the first is accounted for by the br at the start of each tempString)
        for (int j = i + 3; j < rawOutput.Length; j += 3)
        {
          if (rawOutput[j..].StartsWith(" \r\n") == false) { i = j - 1; break; }
          output.Add(br);
        }

        tempString = br;
        continue;
      }

      tempString += rawOutput[i];
    }

    output.Add(tempString);
    output.Add($"' > {fileNameWithExtension}");
    return output.ToArray();
  }
}
