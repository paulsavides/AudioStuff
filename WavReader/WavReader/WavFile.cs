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
    private readonly FileStream _fstream;

    public WavFile(string filePath)
    {
      _fstream = File.Open(filePath, FileMode.Open, FileAccess.Read);
      ReadFileData();
    }

    /// <summary>
    /// Used if you want to make your own sounds
    /// </summary>
    public WavFile() {}

    public void Dispose()
    {
      _fstream.Dispose();
    }

    public void SaveFile(string filePath)
    {
      var fstream = File.Open(filePath, FileMode.CreateNew, FileAccess.Write);
      fstream.Write(Encoding.Default.GetBytes(ChunkId), 0, 4);
      fstream.Write(BitConverter.GetBytes(ChunkSize), 0, 4);
      fstream.Write(Encoding.Default.GetBytes(Format), 0, 4);
      fstream.Write(Encoding.Default.GetBytes(Subchunk1Id), 0, 4);
      fstream.Write(BitConverter.GetBytes(Subchunk1Size), 0, 4);
      fstream.Write(BitConverter.GetBytes(AudioFormat), 0, 2);
      fstream.Write(BitConverter.GetBytes(NumChannels), 0, 2);
      fstream.Write(BitConverter.GetBytes(SampleRate), 0, 4);
      fstream.Write(BitConverter.GetBytes(ByteRate), 0, 4);
      fstream.Write(BitConverter.GetBytes(BlockAlign), 0, 2);
      fstream.Write(BitConverter.GetBytes(BitsPerSample), 0, 2);
      fstream.Write(Encoding.Default.GetBytes(Subchunk2Id), 0, 4);
      fstream.Write(BitConverter.GetBytes(Subchunk2Size), 0, 4);

      for (int i = 0; i < NumSamples; i++)
      {
        for (int c = 0; c < NumChannels; c++)
        {
          for (int j = 0; j < BitsPerSample / 8; j++)
          {
            fstream.WriteByte(Data[c, i, j]);
          }
        }
      }

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
       + "NumSamples    : " + NumSamples;

      return res;
    }

    public void MakeSquareWave(double volume, int bitsPerSample, int lengthInSeconds, int sampleRate)
    {
      ChunkId = "RIFF";
      //      ChunkSize = sample
      Format = "WAVE";
      Subchunk1Id = "fmt "; // space at the end or everything will crash b/c
                            // "fmt" is just three bytes but
                            // "fmt " is four
      Subchunk1Size = 16;
      AudioFormat = 1; // PCB
      NumChannels = 1;
      SampleRate = sampleRate;
      BitsPerSample = (Int16) bitsPerSample;

      ByteRate = SampleRate * NumChannels * (BitsPerSample / 8);
      BlockAlign = (Int16) (NumChannels * (BitsPerSample / 8));

      Subchunk2Id = "data";
      Subchunk2Size = (lengthInSeconds * sampleRate) * BlockAlign;

      NumSamples = Subchunk2Size / NumChannels / (BitsPerSample / 8);

      ChunkSize = 36 /*HEADER SIZE*/ + Subchunk2Size;

      Data = new byte[NumChannels, NumSamples, BitsPerSample / 8];

      int freq = sampleRate / 60;

      for (int i = 0; i < NumSamples; i++)
      {
        var bytes = BitConverter.GetBytes((Int16)(Math.Sin((i / (double)30.0)) > 0 ? 1 : -1 * (double)(Int16.MaxValue * volume)));
        Data[0, i, 0] = bytes[0];
        Data[0, i, 1] = bytes[1];
      }
    }

    public string ChunkId { get; private set; } // RIFF Header
    public Int32 ChunkSize { get; private set; }
    public string Format { get; private set; }

    public string Subchunk1Id { get; private set; } // WAV Header
    public Int32 Subchunk1Size { get; private set; }
    public Int16 AudioFormat { get; private set; }
    public Int16 NumChannels { get; private set; }
    public Int32 SampleRate { get; private set; }
    public Int32 ByteRate { get; private set; }
    public Int16 BlockAlign { get; private set; }
    public Int16 BitsPerSample { get; private set; }
    public string Subchunk2Id { get; private set; }
    public Int32 Subchunk2Size { get; private set; }

    public Int32 NumSamples { get; private set; } // calculated, not part of header

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
    private byte[,,] Data { get; set; } // Not exactly as stored in file

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
      _fstream.Read(buffer, 0, count);
      return buffer;
    }

    private void ReadBytes(ref byte[] buffer, int count)
    {
      _fstream.Read(buffer, 0, count);
    }
  }
}
