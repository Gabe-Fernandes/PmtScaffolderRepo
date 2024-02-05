namespace PmtScaffolder;

public class UserInput
{
  private static readonly UserInput _instance = new();

  public string ProjPath { get; set; }
  public string ProjName { get; set; }
  public string Models { get; set; }
  public string FileNames { get; set; }
  public string DataTypes { get; set; }
  public string Properties { get; set; }
  public string Controllers { get; set; }

  private UserInput()
  {

  }

  public static UserInput GetUserInput()
  {
    return _instance;
  }
}
