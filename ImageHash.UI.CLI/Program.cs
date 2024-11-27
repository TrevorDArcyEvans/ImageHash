namespace ImageHash.UI.CLI;

using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using CommandLine;

internal static class Program
{
  public static async Task Main(string[] args)
  {
    var result = await Parser.Default.ParseArguments<Options>(args)
                             .WithParsedAsync(Run);
    await result.WithNotParsedAsync(HandleParseError);
  }

  private static async Task Run(Options opt)
  {
    await using var img1 = File.OpenRead(opt.ImageFile1Path);
    await using var img2 = File.OpenRead(opt.ImageFile2Path);

    img1.Seek(0, SeekOrigin.Begin);
    img2.Seek(0, SeekOrigin.Begin);
    var avgHash = new AverageHash();
    var avg1 = await avgHash.HashAsync(img1, CancellationToken.None);
    var avg2 = await avgHash.HashAsync(img2, CancellationToken.None);
    var avgSim = CompareHash.Similarity(avg1, avg2);

    img1.Seek(0, SeekOrigin.Begin);
    img2.Seek(0, SeekOrigin.Begin);
    var diffHash = new DifferenceHash();
    var diff1 = await diffHash.HashAsync(img1, CancellationToken.None);
    var diff2 = await diffHash.HashAsync(img2, CancellationToken.None);
    var diffSim = CompareHash.Similarity(diff1, diff2);

    img1.Seek(0, SeekOrigin.Begin);
    img2.Seek(0, SeekOrigin.Begin);
    var perHash = new PerceptualHash();
    var per1 = await perHash.HashAsync(img1, CancellationToken.None);
    var per2 = await perHash.HashAsync(img2, CancellationToken.None);
    var perSim = CompareHash.Similarity(per1, per2);

    Console.WriteLine($"ImageFile1 = {opt.ImageFile1Path}");
    Console.WriteLine($"ImageFile2 = {opt.ImageFile2Path}");
    Console.WriteLine($"  Average:");
    Console.WriteLine($"    hash1   = {avg1}");
    Console.WriteLine($"    hash2   = {avg2}");
    Console.WriteLine($"      match = {avgSim}%");
    Console.WriteLine($"  Difference:");
    Console.WriteLine($"    hash1   = {diff1}");
    Console.WriteLine($"    hash2   = {diff2}");
    Console.WriteLine($"      match = {diffSim}%");
    Console.WriteLine($"  Perceptual:");
    Console.WriteLine($"    hash1   = {per1}");
    Console.WriteLine($"    hash2   = {per2}");
    Console.WriteLine($"      match = {perSim}%");
  }

  private static Task HandleParseError(IEnumerable<Error> errs)
  {
    if (errs.IsVersion())
    {
      Console.WriteLine("Version Request");
      return Task.CompletedTask;
    }

    if (errs.IsHelp())
    {
      Console.WriteLine("Help Request");
      return Task.CompletedTask;
      ;
    }

    Console.WriteLine("Parser Fail");
    return Task.CompletedTask;
  }
}