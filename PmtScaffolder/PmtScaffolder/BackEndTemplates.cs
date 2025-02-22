namespace PmtScaffolder;

public static class BackEndTemplates
{
  private static readonly string br = Environment.NewLine;
  private static readonly UserInput _userInput = UserInput.GetUserInput();

  public static string[] ModelClassHeader(string fileName)
  {
    fileName = Util.Capital(fileName, true);

    if (fileName.ToLower() == "appuser")
    {
      return [$"Write-Output 'using Microsoft.AspNetCore.Identity;",
              $"{br}using System.ComponentModel.DataAnnotations;", br,

              $"{br}namespace {_userInput.ProjName}.Data.Models;", br,

              $"{br}public class {fileName} : IdentityUser",
              $"{br}{{"];
    }
    return [$"Write-Output 'using System.ComponentModel.DataAnnotations;", br,

            $"{br}namespace {_userInput.ProjName}.Data.Models;", br,

            $"{br}public class {fileName}",
            $"{br}{{",
            $"{br}\t[Key]"];
  }

  public static string[] RepoInterface(string fileName)
  {
    fileName = Util.Capital(fileName, true);
    string lowerCaseFileName = Util.Capital(fileName, false);
    string idDataType = (fileName.ToLower() == "appuser") ? "string" : "int";

    return [$"Write-Output 'using {_userInput.ProjName}.Data.Models;", br,

            $"{br}namespace {_userInput.ProjName}.Data.RepoInterfaces;", br,

            $"{br}public interface I{fileName}Repo",
            $"{br}{{",
              $"{br}\tTask<{fileName}> GetByIdAsync({idDataType} id);",
              $"{br}\tbool Add({fileName} {lowerCaseFileName});",
              $"{br}\tbool Update({fileName} {lowerCaseFileName});",
              $"{br}\tbool Delete({fileName} {lowerCaseFileName});",
              $"{br}\tbool Save();",
            $"{br}}}",

            $"'> I{fileName}Repo.cs"];
  }

  public static string[] Repository(string fileName, string pluralName)
  {
    fileName = Util.Capital(fileName, true);
    pluralName = Util.Capital(pluralName, true);
    string lowerCaseFileName = Util.Capital(fileName, false);
    string idDataType = "int";

    if (fileName.ToLower() == "appuser")
    {
      idDataType = "string";
      pluralName = "Users";
    }

    return [$"Write-Output 'using Microsoft.EntityFrameworkCore;",
            $"{br}using {_userInput.ProjName}.Data.Models;",
            $"{br}using {_userInput.ProjName}.Data.RepoInterfaces;", br,

            $"{br}namespace {_userInput.ProjName}.Data.Repos;", br,

            $"{br}public class {fileName}Repo(AppDbContext db) : I{fileName}Repo",
            $"{br}{{",
              $"{br}\tprivate readonly AppDbContext _db = db;", br,

              $"{br}\tpublic bool Add({fileName} {lowerCaseFileName})",
              $"{br}\t{{",
                $"{br}\t\t_db.{pluralName}.Add({lowerCaseFileName});",
                $"{br}\t\treturn Save();",
              $"{br}\t}}", br,

              $"{br}\tpublic bool Update({fileName} {lowerCaseFileName})",
              $"{br}\t{{",
                $"{br}\t\t_db.{pluralName}.Update({lowerCaseFileName});",
                $"{br}\t\treturn Save();",
              $"{br}\t}}", br,

              $"{br}\tpublic bool Delete({fileName} {lowerCaseFileName})",
              $"{br}\t{{",
                $"{br}\t\t_db.{pluralName}.Remove({lowerCaseFileName});",
                $"{br}\t\treturn Save();",
              $"{br}\t}}", br,

              $"{br}\tpublic bool Save()",
              $"{br}\t{{",
                $"{br}\t\tint numSaved = _db.SaveChanges(); // returns the number of entries written to the database",
                $"{br}\t\treturn numSaved > 0;",
              $"{br}\t}}", br,

              $"{br}\tpublic async Task<{fileName}> GetByIdAsync({idDataType} id)",
              $"{br}\t{{",
                $"{br}\t\treturn await _db.{pluralName}.FindAsync(id);",
              $"{br}\t}}",
            $"{br}}}",

            $"'> {fileName}Repo.cs"];
  }

  public static string[] AppDbCtx()
  {
    return [$"Write-Output 'using Microsoft.AspNetCore.Identity.EntityFrameworkCore;",
            $"{br}using Microsoft.EntityFrameworkCore;",
            $"{br}using {_userInput.ProjName}.Data.Models;", br,

            $"{br}namespace {_userInput.ProjName}.Data;", br,

            $"{br}public class AppDbContext : IdentityDbContext<AppUser>",
            $"{br}{{",
              $"{br}\tpublic AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {{ }}", br,

              $"{br}\t// PMT Landmark", br,

              $"{br}\tprotected override void OnModelCreating(ModelBuilder builder)",
              $"{br}\t{{",
              $"{br}\t\tbase.OnModelCreating(builder);", br,

              $"{br}\t\tbuilder.Entity<AppUser>()",
                $"{br}\t\t.Ignore(x => x.TwoFactorEnabled)",
                $"{br}\t\t.Ignore(x => x.AccessFailedCount)",
                $"{br}\t\t.Ignore(x => x.LockoutEnabled)",
                $"{br}\t\t.Ignore(x => x.LockoutEnd)",
                $"{br}\t\t.Ignore(x => x.PhoneNumberConfirmed);",
              $"{br}\t}}",
            $"{br}}}",

            $"'> AppDbContext.cs"];
  }

  public static string[] UnitTest(string fileName, string pluralName, string[] mockData, string[] dbCtxMockData)
  {
    fileName = Util.Capital(fileName, true);
    pluralName = Util.Capital(pluralName, true);
    string lowerCaseFileName = Util.Capital(fileName, false);
    string idInstantiation = "Id = (i + 1),";
    string arrangedId = "2";

    if (fileName.ToLower() == "appuser")
    {
      idInstantiation = "Id = (i + 1).ToString(),";
      pluralName = "Users";
      arrangedId = "\"2\"";
    }

    string[] unitTestPart0 = [$"Write-Output 'using Microsoft.EntityFrameworkCore;",
            $"{br}using {_userInput.ProjName}.Data.Models;",
            $"{br}using {_userInput.ProjName}.Data.Repos;",
            $"{br}using {_userInput.ProjName}.Data;", br,

            $"{br}namespace {_userInput.TestProjName}.Data.Repos;", br,

            $"{br}public class {fileName}RepoTests",
            $"{br}{{",
              $"{br}\tprivate static async Task<AppDbContext> GetDbContext()",
              $"{br}\t{{",
                $"{br}\t\tvar options = new DbContextOptionsBuilder<AppDbContext>()",
                $"{br}\t\t\t.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;",
                $"{br}\t\tvar dbContext = new AppDbContext(options);",
                $"{br}\t\tdbContext.Database.EnsureCreated();",
                $"{br}\t\tif (!await dbContext.{pluralName}.AnyAsync())",
                $"{br}\t\t{{",
                  $"{br}\t\t\tfor (int i = 0; i < 10; i++)",
                  $"{br}\t\t\t{{",
                    $"{br}\t\t\t\tdbContext.{pluralName}.Add(new {fileName}()",
                    $"{br}\t\t\t\t{{",
                      $"{br}\t\t\t\t\t{idInstantiation}"];
    string[] unitTestPart1 = [
                    $"{br}\t\t\t\t}});",
                    $"{br}\t\t\t\tawait dbContext.SaveChangesAsync();",
                  $"{br}\t\t\t}}",
                $"{br}\t\t}}",
                $"{br}\t\treturn dbContext;",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic async Task {fileName}Repo_Add_ReturnsTrue()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange",
                $"{br}\t\tvar dbCtxStub = await GetDbContext();",
                $"{br}\t\t{fileName}Repo sut = new(dbCtxStub);",
                $"{br}\t\t{fileName} {lowerCaseFileName} = new()",
                $"{br}\t\t{{"];
    string[] unitTestPart2 = [
                $"{br}\t\t}};",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = sut.Add({lowerCaseFileName});",
                $"{br}\t\t// Assert",
                $"{br}\t\tAssert.True(result);",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic async Task {fileName}Repo_Delete_ReturnsTrue()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange",
                $"{br}\t\tvar dbCtxStub = await GetDbContext();",
                $"{br}\t\t{fileName} {lowerCaseFileName} = await dbCtxStub.{pluralName}.FirstOrDefaultAsync();",
                $"{br}\t\t{fileName}Repo sut = new(dbCtxStub);",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = sut.Delete({lowerCaseFileName});",
                $"{br}\t\t// Assert",
                $"{br}\t\tAssert.True(result);",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic async Task {fileName}Repo_Update_ReturnsTrue()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange",
                $"{br}\t\tvar dbCtxStub = await GetDbContext();",
                $"{br}\t\t{fileName} {lowerCaseFileName} = await dbCtxStub.{pluralName}.FirstOrDefaultAsync();",
                $"{br}\t\t{fileName}Repo sut = new(dbCtxStub);",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = sut.Update({lowerCaseFileName});",
                $"{br}\t\t// Assert",
                $"{br}\t\tAssert.True(result);",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic async Task {fileName}Repo_Save_ReturnsBool()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange",
                $"{br}\t\tvar dbCtxStub = await GetDbContext();",
                $"{br}\t\t{fileName}Repo sut = new(dbCtxStub);",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = sut.Save();",
                $"{br}\t\t// Assert",
                $"{br}\t\tAssert.IsType<bool>(result);",
              $"{br}\t}}", br,

              $"{br}\t[Fact]",
              $"{br}\tpublic async Task {fileName}Repo_GetByIdAsync_Returns{fileName}()",
              $"{br}\t{{",
                $"{br}\t\t// Arrange",
                $"{br}\t\tvar dbCtxStub = await GetDbContext();",
                $"{br}\t\t{fileName}Repo sut = new(dbCtxStub);",
                $"{br}\t\tvar id = {arrangedId};",
                $"{br}\t\t// Act",
                $"{br}\t\tvar result = await sut.GetByIdAsync(id);",
                $"{br}\t\t// Assert",
                $"{br}\t\tAssert.IsType<{fileName}>(result);",
              $"{br}\t}}",
            $"{br}}}",

            $"'> {fileName}RepoTests.cs"];

    return unitTestPart0.Concat(dbCtxMockData)
      .Concat(unitTestPart1).Concat(mockData).
      Concat(unitTestPart2).ToArray();
  }

  public static string DiRepoService(string modelName)
  {
    modelName = Util.Capital(modelName, true);
    return $"{br}builder.Services.AddTransient<I{modelName}Repo, {modelName}Repo>();";
  }

  public static string DbSet(string singularModelName, string pluralModelName)
  {
    singularModelName = Util.Capital(singularModelName, true);
    pluralModelName = Util.Capital(pluralModelName, true);
    return $"{br}\tpublic DbSet<{singularModelName}> {pluralModelName} {{ get; set; }}";
  }
}
