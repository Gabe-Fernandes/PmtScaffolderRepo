namespace PmtScaffolder;

public static class FrontEndTemplates
{
  private static readonly string br = Environment.NewLine;
  private static readonly UserInput _userInput = UserInput.GetUserInput();

  public static string[] SassFile(string fileName)
  {
    return [$@"Write-Output '@import ""../_library.scss"";", br,

            $"{br}#{fileName}Content {{",
              $"{br}\t",
            $"{br}}}'",

            $"> {fileName}.scss"];
  }

  public static string[] JsFile(string fileName)
  {
    return [$@"Write-Output '$(function () {{",
              $"{br}\t",
            $"{br}}});'",

            $"> {fileName}.js"];
  }

  public static string[] CsHtmlFile(string fileName, string controllerName)
  {
    return [$@"Write-Output '@{{",
            $@"{br}  ViewData[""Title""] = ""{fileName}"";",
            $"{br}}}", br,

            $@"{br}<script type=""text/javascript"" src=""~/js/{controllerName}/{fileName}.js""></script>", br,

            $@"{br}<section id=""{fileName}Content"">",
              $"{br}\t",
            $"{br}</section>'",

            $"> {fileName}.cshtml"];
  }

  public static string[] ControllerFile(string controllerName)
  {
    return [$@"Write-Output 'using Microsoft.AspNetCore.Authorization;",
      $"{br}using Microsoft.AspNetCore.Mvc;",
      $"{br}using {_userInput.ProjName}.Data.Models;",
      $"{br}using {_userInput.ProjName}.Data.RepoInterfaces;", br,

      $"{br}namespace {_userInput.ProjName}.Controllers;", br,

      $"{br}[Authorize]",
      $"{br}public class {controllerName}Controller : Controller",
      $"{br}{{",
        $"{br}\t",
      $"{br}}}'",

      $"> {controllerName}Controller.cs"];
  }
}
