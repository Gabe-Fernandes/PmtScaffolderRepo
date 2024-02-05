using PmtScaffolder;

while (true)
{
  Console.Write("PMT Scaffolder> ");
  string input = Console.ReadLine();

  if (Cmd.RunValidInput(input) == false)
  {
    Console.WriteLine($"Unrecognized command: {input}");
  }
}
