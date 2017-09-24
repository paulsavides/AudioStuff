using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace WavReader
{
  /// <summary>
  /// A class to deal with reading a Wav file.
  /// 
  /// Thank you to this page http://soundfile.sapp.org/doc/WaveFormat/ for the information
  /// </summary>
  public class WavFile : IDisposable
  {
    private readonly FileStream fstream;

    public WavFile(string filePath)
    {
      fstream = File.Open(filePath, FileMode.Open, FileAccess.Read);
      ReadFileData();
    }

    public void Dispose()
    {
      fstream.Dispose();
    }

    /// <summary>
    /// Should be called immediately in the constructor
    /// Will read all data from the file stream into our own properties
    /// </summary>
    private void ReadFileData()
    {
      ChunkId = ReadString();
      ChunkSize = ReadInt32();
      Format = ReadString();
      Subchunk1Id = ReadString();
      Subchunk1Size = ReadInt32();
      AudioFormat = ReadInt16();
      NumChannels = ReadInt16();
      SampleRate = ReadInt32();
      ByteRate = ReadInt32();
      BlockAlign = ReadInt16();
      BitsPerSample = ReadInt16();
      Subchunk2Id = ReadString();
      Subchunk2Size = ReadInt32();
      NumSamples = Subchunk2Size / NumChannels / (BitsPerSample / 8);
      ReadSamples();
    }

    public override string ToString()
    {
      var res =
        "Chunk Id      : " + ChunkId + Environment.NewLine
       + "Chunk Size    : " + ChunkSize + Environment.NewLine
       + "Format        : " + Format + Environment.NewLine
       + "Subchunk1ID   : " + Subchunk1Id + Environment.NewLine
       + "Subchunk1Size : " + Subchunk1Size + Environment.NewLine
       + "AudioFormat   : " + AudioFormat + Environment.NewLine
       + "NumChannels   : " + NumChannels + Environment.NewLine
       + "SampleRate    : " + SampleRate + Environment.NewLine
       + "ByteRate      : " + ByteRate + Environment.NewLine
       + "BlockAlign    : " + BlockAlign + Environment.NewLine
       + "BitsPerSample : " + BitsPerSample + Environment.NewLine
       + "Subchunk2Id   : " + Subchunk2Id + Environment.NewLine
       + "Subchunk2Size : " + Subchunk2Size + Environment.NewLine
       + "NumSamples    : " + NumSamples;// + Environment.NewLine;

      return res;
    }

    public string ChunkId { get; private set; }
    public Int32 ChunkSize { get; private set; }
    public string Format { get; private set; }
    public string Subchunk1Id { get; private set; }
    public Int32 Subchunk1Size { get; private set; }
    public Int16 AudioFormat { get; private set; }
    public Int16 NumChannels { get; private set; }
    public Int32 SampleRate { get; private set; }
    public Int32 ByteRate { get; private set; }
    public Int16 BlockAlign { get; private set; }
    public Int16 BitsPerSample { get; private set; }
    public string Subchunk2Id { get; private set; }
    public Int32 Subchunk2Size { get; private set; }
    public Int32 NumSamples { get; private set; }

    // Data is a 3D array like...
    // [Channel 1]
    //     [SampleNum]
    //         [SampleByte]
    //         [SampleByte]
    //     [SampleNum]
    //         [SampleByte]
    //         etc...
    // [Channel 2]
    //     [SampleNum]
    //        [SampleByte]
    //        etc...
    /// <summary>
    /// The raw audio data, stored as a 3D array of bytes,
    /// Data[ChannelNum, SampleNum, SampleByte]
    /// </summary>
    private byte[,,] Data { get; set; }

    private void ReadSamples()
    {
      Data = new byte[NumChannels, NumSamples, BitsPerSample / 8];

      // Wav file stores samples in sequence
      // imagine 16 bit samples with two channels
      //        10010100 01001000 00000010 00010000 10100100 00100100 00100000 11000100
      //        [ chan 1 sample ] [ chan 2 sample ] [ chan 1        ] [ chan 2        ]
      // So we want to read in four bytes for each sample we're processing
      int bytesPerSample = (BitsPerSample / 8) * NumChannels;
      byte[] buffer = new byte[bytesPerSample];

      for (int i = 0; i < NumSamples; i++)
      {
        ReadBytes(ref buffer, bytesPerSample);
        for (int c = 0; c < NumChannels; c++)
        {

          for (int j = 0; j < BitsPerSample / 8; j++)
          {
            // i = current sample
            // c = current channel
            // j = bit per sample
            Data[c, i, j] = buffer[(c * (BitsPerSample / 8)) + j];
          }
        }
      }
    }

    /*****************************
     * FSTREAM READING UTILITIES *
     *****************************/
    private string ReadString(int bytes = 4)
    {
      return Encoding.UTF8.GetString(ReadBytes(4));
    }

    private Int16 ReadInt16()
    {
      var buffer = ReadBytes(2);
      return BitConverter.ToInt16(buffer, 0);
    }

    private Int32 ReadInt32()
    {
      var buffer = ReadBytes(4);
      return BitConverter.ToInt32(buffer, 0);
    }

    private byte[] ReadBytes(int count)
    {
      byte[] buffer = new byte[count];
      fstream.Read(buffer, 0, count);
      return buffer;
    }

    private void ReadBytes(ref byte[] buffer, int count)
    {
      fstream.Read(buffer, 0, count);
    }
  }
}
