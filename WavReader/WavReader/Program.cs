using System;

namespace WavReader
{
  class Program
  {
    static void Main(string[] args)
    {
      string filePath = @"..\Wavs\testClip.wav";

      using (var wav = new WavFile(filePath))
      {
        Console.WriteLine(wav);

        wav.PrintChannelSamples(1);
      }

      Console.ReadKey();
    }
  }
}
