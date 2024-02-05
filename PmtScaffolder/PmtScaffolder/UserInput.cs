using System.Threading;

namespace PmtScaffolder;

public class UserInput
{
  private static readonly UserInput _instance = new();

  public string ProjPath { get; set; } = "C:\\dev\\ps_scaffolding\\autogen\\autogen";
  public string ProjName { get; set; } = "AutoGen";

  public List<string> Controllers = ["Agile", "MyProjects"];
  public List<List<string>> FileNames = [["one", "two", "three"], ["four", "five", "six"]];

  public List<string> Models = [];
  public List<List<string>> DataTypes = [];
  public List<List<string>> Properties = [];

  private UserInput() { }

  public static UserInput GetUserInput()
  {
    return _instance;
  }
}
