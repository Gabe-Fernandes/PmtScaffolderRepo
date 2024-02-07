using System;

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
    return [$"Write-Output '$(function () {{",
              $"{br}\t",
            $"{br}}});'",

            $"> {fileName}.js"];
  }

  public static string[] CsHtmlFile(string fileName, string controllerName)
  {
    return [$"Write-Output '@{{",
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
    return [$"Write-Output 'using Microsoft.AspNetCore.Authorization;",
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

  public static string[] ViewStartFile(string controllerName)
  {
    return [$"Write-Output '@{{",
            $@"{br}  Layout = ""_{controllerName}_Layout"";",
            $"{br}}}'",

            $"> _ViewStart.cshtml"];
  }

  public static string[] LayoutFile(string controllerName)
  {
    return [$"Write-Output '<!DOCTYPE html>",
            $@"{br}<html lang=""en"">",
            $"{br}<head>",
              $@"{br}  <meta charset=""utf-8"" />",
              $@"{br}  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />",
              $@"{br}  <title>@ViewData[""Title""] - {_userInput.ProjName}</title>",
              $@"{br}  <link rel=""icon"" href="""">",
              $@"{br}  <link rel=""stylesheet"" href=""~/css/site.css"" asp-append-version=""true"" />",
              $@"{br}  // PMT Landmark",
              $@"{br}  <script type=""text/javascript"" src=""~/lib/jquery/dist/jquery.js""></script>",
              $@"{br}  <script defer type=""text/javascript"" src=""~/js/site.js""></script>",
              $@"{br}</head>",
              $@"{br}<body>",
              $@"{br}  <div class=""no-js-warning"">This application relies heavily on javascript - please ensure it is enabled in your browser and refresh the page.</div>",
              $@"{br}  <script>$("".no-js-warning"").remove();</script>", br,

              $@"{br}  <main role=""main"">",
              $"{br}\t\t@RenderBody()",
              $"{br}\t</main>",
              $"{br}</body>",
              $"{br}</html>'",

            $"> _{controllerName}_Layout.cshtml"];
  }

  public static string CssLinkEle(string fileName)
  {
    return $"{br}\t<link rel=\"stylesheet\" href=\"~/css/{fileName}.css\" asp-append-version=\"true\" />";
  }
}
