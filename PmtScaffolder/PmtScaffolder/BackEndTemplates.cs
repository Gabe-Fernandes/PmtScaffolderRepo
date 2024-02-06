namespace PmtScaffolder;

public static class BackEndTemplates
{
  private static readonly string br = Environment.NewLine;
  private static readonly UserInput _userInput = UserInput.GetUserInput();

  public static string[] ModelClassHeader(string fileName)
  {
    return [$"Write-Output 'using System.ComponentModel.DataAnnotations;", br,

            $"{br}namespace {_userInput.ProjName}.Data.Models;", br,

            $"{br}public class {fileName}",
            $"{br}{{"];
  }

  public static string[] RepoInterface(string fileName)
  {
    return [$"Write-Output 'using {_userInput.ProjName}.Data.Models;", br,

            $"{br}namespace {_userInput.ProjName}.Data.RepoInterfaces;", br,

            $"{br}public interface I{fileName}Repo",
            $"{br}{{",
              $"{br}\tTask<{fileName}> GetByIdAsync(int id);",
              $"{br}\tbool Add({fileName} {fileName});",
              $"{br}\tbool Update({fileName} {fileName});",
              $"{br}\tbool Delete({fileName} {fileName});",
              $"{br}\tbool Save();",
            $"{br}}}'",
            $"> I{fileName}Repo.cs"];
  }

  public static string[] Repository(string fileName)
  {
    return [$"Write-Output 'using Microsoft.EntityFrameworkCore;",
            $"{br}using {_userInput.ProjName}.Data.RepoInterfaces;", br,

            $"{br}namespace {_userInput.ProjName}.Data.Models;", br,

            $"{br}public class {fileName}Repo(AppDbContext db) : I{fileName}Repo",
            $"{br}{{",
              $"{br}\tprivate readonly AppDbContext _db = db;", br,

              $"{br}\tpublic bool Add({fileName} {fileName})",
              $"{br}\t{{",
                $"{br}\t\t_db.{fileName}s.Add({fileName});",
                $"{br}\t\treturn Save();",
              $"{br}\t}}", br,

              $"{br}\tpublic bool Update({fileName} {fileName})",
              $"{br}\t{{",
                $"{br}\t\t_db.{fileName}s.Update({fileName});",
                $"{br}\t\treturn Save();",
              $"{br}\t}}", br,

              $"{br}\tpublic bool Delete({fileName} {fileName})",
              $"{br}\t{{",
                $"{br}\t\t_db.{fileName}s.Remove({fileName});",
                $"{br}\t\treturn Save();",
              $"{br}\t}}", br,

              $"{br}\tpublic bool Save()",
              $"{br}\t{{",
                $"{br}\t\tint numSaved = _db.SaveChanges(); // returns the number of entries written to the database",
                $"{br}\t\treturn numSaved > 0;",
              $"{br}\t}}", br,

              $"{br}\tTask<{fileName}> GetByIdAsync(int id)",
              $"{br}\t{{",
                $"{br}\t\treturn await _db.{fileName}s.FindAsync(id);",
              $"{br}\t}}",
            $"{br}}}'",
            $"> {fileName}Repo.cs"];
  }

  public static string[] UnitTest(string fileName, string[] mockData, string[] dbCtxMockData)
  {
    string[] unitTestPart0 = [$"Write-Output 'using Microsoft.EntityFrameworkCore;",
            $"{br}using {_userInput.ProjName}.Data.Models;",
            $"{br}using {_userInput.ProjName}.Data;", br,

            $"{br}namespace {_userInput.TestProjName}.Data.Repos;", br,

            $"{br}public class {fileName}RepoTests",
            $"{br}{{",
              $"{br}\tprivate readonly AppDbContext _dbContext;",
              $"{br}\tprivate readonly {fileName}Repo _{fileName}Repo;", br,

              $"{br}\tpublic {fileName}RepoTests()",
              $"{br}\t{{",
                $"{br}\t\t// Dependencies",
                $"{br}\t\t_dbContext = GetDbContext();",
                $"{br}\t\t// SUT",
                $"{br}\t\t_{fileName}Repo = new {fileName}Repo(_dbContext);",
              $"{br}\t}}", br,

              $"{br}\tprivate AppDbContext GetDbContext()",
              $"{br}\t{{",
                $"{br}\t\tvar options = new DbContextOptionsBuilder<AppDbContext>()",
                $"{br}\t\t\t.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;",
                $"{br}\t\tvar dbContext = new AppDbContext(options);",
                $"{br}\t\tdbContext.Database.EnsureCreated();",
                $"{br}\t\tif (dbContext.{fileName}s.Count() < 0)",
                $"{br}\t\t{{",
                  $"{br}\t\t\tfor (int i = 0; i < 10; i++)",
                  $"{br}\t\t\t{{",
                    $"{br}\t\t\t\tdbContext.{fileName}s.Add(new {fileName}()",
                    $"{br}\t\t\t\t{{",
                      $"{br}\t\t\t\t\tId = (i + 1),"];
    string[] unitTestPart1 = [
                    $"{br}\t\t\t\t}});",
                    $"{br}\t\t\t\tdbContext.SaveChangesAsync();",
                  $"{br}\t\t\t}}",
                $"{br}\t\t}}",
                $"{br}\t\treturn dbContext;",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic void Add_ReturnsTrue()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange",
                $"{br}\t\tvar {fileName} = new {fileName}()",
                $"{br}\t\t{{"];
    string[] unitTestPart2 = [
                $"{br}\t\t}};",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = _{fileName}Repo.Add({fileName});",
                $"{br}\t\t// Assert",
                $"{br}\t\tAssert.True(result);",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic void Delete_ReturnsTrue()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange",
                $"{br}\t\tvar {fileName} = new {fileName}()",
                $"{br}\t\t{{"];
    string[] unitTestPart3 = [
                $"{br}\t\t}};",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = _{fileName}Repo.Delete({fileName});",
                $"{br}\t\t// Assert",
                $"{br}\t\tAssert.True(result);",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic void Update_ReturnsTrue()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange",
                $"{br}\t\tvar {fileName} = new {fileName}()",
                $"{br}\t\t{{"];
    string[] unitTestPart4 = [
                $"{br}\t\t}};",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = _{fileName}Repo.Update({fileName});",
                $"{br}\t\t// Assert",
                $"{br}\t\tAssert.True(result);",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic void Save_ReturnsBool()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange (empty)",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = _{fileName}Repo.Save();",
                $"{br}\t\t// Assert",
                $"{br}\t\tAssert.IsType<bool>(result);",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic async void GetByIdAsync_Returns{fileName}Task()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange",
                $"{br}\t\tvar id = 2;",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = await _{fileName}Repo.GetByIdAsync(id);",
                $"{br}\t\t// Assert",
                $"{br}\t\tawait Assert.IsType<Task<{fileName}>>(result);",
              $"{br}\t}}",
            $"{br}}}'",
            $"> {fileName}RepoTests.cs"];

    return unitTestPart0.Concat(dbCtxMockData)
      .Concat(unitTestPart1).Concat(mockData).
      Concat(unitTestPart2).Concat(mockData).
      Concat(unitTestPart3).Concat(mockData).
      Concat(unitTestPart4).ToArray();
  }
}
