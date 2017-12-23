using System;
using System.Collections.Generic;
using System.Text;

namespace WavReader
{
  public static class BaseFrequency
  {
    private static double _baseFreq = 440;
    private static double BaseFreq
    {
      get
      {
        return _baseFreq;
      }
      set
      {
        switch (value)
        {
          case 432:
          case 434:
          case 436:
          case 438:
          case 440:
          case 442:
          case 444:
          case 446:
            _baseFreq = value;
            break;
        }
      }
    }
  }
}
