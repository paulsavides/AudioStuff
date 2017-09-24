using System;

namespace WavReader
{
  class Program
  {
    static void Main(string[] args)
    {
      string filePath = @"..\Wavs\sample.wav";

      //using (var wav = new WavFile(filePath))
      //{
      //  wav.SaveFile();
      //}

      var wav = new WavFile();
      wav.MakeSquareWave(.1, 16, 5, 44000);
      wav.SaveFile(@"..\Wavs\hmmm_" + DateTime.Now.Millisecond + ".wav");

      Console.ReadKey();
    }
  }
}
