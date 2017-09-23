using System;

namespace WavReader
{
  class Program
  {
    static void Main(string[] args)
    {
      string filePath = @"..\Wavs\sample.wav";

      using (var wav = new WavFile(filePath))
      {
        Console.WriteLine(wav);
      }


      Console.ReadKey();
    }
  }
}
