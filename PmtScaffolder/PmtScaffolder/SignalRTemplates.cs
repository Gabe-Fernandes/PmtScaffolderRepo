namespace PmtScaffolder;

public static class SignalRTemplates
{
  private static readonly string br = Environment.NewLine;
  private static readonly UserInput _userInput = UserInput.GetUserInput();

  public static string[] ClientSideHub(string fileName)
  {
    return [$"Write-Output '$(function () {{",
              $"{br}\t// ============================================================================= init page =============================================================================",
              $"{br}\t",
              $"{br}\t// Create and start signalR connection",
              $"{br}\tvar hubConnection = new signalR.HubConnectionBuilder().withUrl(\"/{fileName}\").build();",
              $"{br}\tfunction hubConnectionSuccess() {{ console.log(\"hubConnection success\"); pageLoad() }}",
              $"{br}\tfunction failure() {{ console.log(\"failure\") }}",
              $"{br}\thubConnection.start().then(hubConnectionSuccess, failure);",
              $"{br}\thubConnection.onclose(async () => await hubConnection.start());",
              $"{br}\t",
              $"{br}\tfunction callServer(methodName, clientObj) {{",
              $"{br}\t\tif (hubConnection.state !== \"Connected\") {{",
              $"{br}\t\t\talert(\"Connection lost - please refresh the page.\");",
              $"{br}\t\t\treturn;",
              $"{br}\t\t}}",
              $"{br}\t\thubConnection.send(methodName, clientObj);",
              $"{br}\t}}",
              $"{br}\t",
              $"{br}\tfunction pageLoad() {{",
              $"{br}\t\t",
              $"{br}\t}}",
              $"{br}\t",
              $"{br}\t// ============================================================================= events =============================================================================",
              $"{br}\t",
              $"{br}\t",
              $"{br}\t",
              $"{br}\t// ============================================================================= callbacks =============================================================================",
              $"{br}\t",
              $"{br}\t",
              $"{br}\t",
            $"{br}}});",

            $"'> {fileName}.js"];
  }

  public static string[] ServerSideHub(string controllerName, string fileName)
  {
    return [$"Write-Output 'using Microsoft.AspNetCore.SignalR;",
                $"{br}using Newtonsoft.Json;",
                $"{br}using System.Security.Claims;{br}",

                $"{br}namespace {_userInput.ProjName}.SignalRHubs.{controllerName}Hubs.{fileName}Files;", br,

                $"{br}public class {fileName}Hub(IHttpContextAccessor contextAccessor) : Hub<I{fileName}Hub>",
                $"{br}{{",
                  $"{br}\tprivate readonly IHttpContextAccessor _contextAccessor = contextAccessor;{br}{br}{br}",



                  $"{br}\tpublic override async Task OnConnectedAsync()",
                  $"{br}\t{{",
                    $"{br}\t\tstring appUserId = _contextAccessor.HttpContext.User.FindFirstValue(\"Id\");",
                    $"{br}\t\tawait Groups.AddToGroupAsync(Context.ConnectionId, appUserId);",
                  $"{br}\t}}",
                  $"{br}\tpublic async Task OnDisconnectedAsync()",
                  $"{br}\t{{",
                    $"{br}\t\tstring appUserId = _contextAccessor.HttpContext.User.FindFirstValue(\"Id\");",
                    $"{br}\t\tawait Groups.RemoveFromGroupAsync(Context.ConnectionId, appUserId);",
                  $"{br}\t}}{br}{br}{br}",



                  $"{br}\tpublic async Task MyHubMethod(object clientData)",
                  $"{br}\t{{",
                    $"{br}\t\tstring clientDataAsString = clientData.ToString();",
                    $"{br}\t\tPlaceholder ph = JsonConvert.DeserializeObject<Placeholder>(clientDataAsString);",
                  $"{br}\t}}",
                $"{br}}}",

                $"'> {fileName}Hub.cs"];
  }

  public static string[] HubDTO(string controllerName, string fileName)
  {
    return [$"Write-Output 'namespace {_userInput.ProjName}.SignalRHubs.{controllerName}Hubs.{fileName}Files;", br,

                $"{br}public class Placeholder",
                $"{br}{{",
                  $"{br}\tpublic string MyProp {{ get; set; }}",
                $"{br}}}",

                $"'> {fileName}DTOs.cs"];
  }

  public static string[] HubInterface(string controllerName, string fileName)
  {
    return [$"Write-Output 'namespace {_userInput.ProjName}.SignalRHubs.{controllerName}Hubs.{fileName}Files;", br,

                $"{br}public interface I{fileName}Hub",
                $"{br}{{",
                  $"{br}\t\t",
                $"{br}}}",

                $"'> I{fileName}Hub.cs"];
  }

  public static string AddSignalRService()
  {
    return $"{br}builder.Services.AddSignalR();";
  }

  public static string MapHubMiddleWare(string fileName)
  {
    return $"{br}app.MapHub<{fileName}Hub>(\"{Util.Capital(fileName, false)}\");";
  }
}
