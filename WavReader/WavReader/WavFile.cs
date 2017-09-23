using System;
using System.IO;
using System.Text;

namespace WavReader
{
  public class WavFile : IDisposable
  {
    private readonly FileStream fstream;

    public WavFile(string filePath)
    {
      fstream = File.Open(filePath, FileMode.Open);
      ReadFileData();
    }

    public void Dispose()
    {
      fstream.Close();
    }

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
      Data = ReadBytes(NumSamples);
      //Other = ReadTheRest();
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


      //Other.ForEach(val => res += ", " + val);

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
    public byte[] Data { get; private set; }
    //public List<string> Other { get; private set; }


    //private List<string> ReadTheRest()
    //{
    //  var res = new List<string>();

    //  byte[] buffer = new byte[4];
    //  while (fstream.Read(buffer, 0, 4) != 0)
    //  {
    //    res.Add(Encoding.UTF8.GetString(buffer));
    //  }

    //  return res;
    //}

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


  }
}
