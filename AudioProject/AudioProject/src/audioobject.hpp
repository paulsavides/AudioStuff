#ifndef AUDIO_OBJECT_INCLUDED_HPP
#define AUDIO_OBJECT_INCLUDED_HPP

#include "iaudiodata.hpp"
#include "sampleinfo.hpp"

class AudioObject
{
public:
  AudioObject(const SampleInfo& info, IAudioData* audioData);
  bool GenerateSamples(float* stream, size_t streamLength);
  void SetPos(double pos);
private:
  size_t      m_audioPos;
  size_t      m_audioLength;
  SampleInfo  m_sampleInfo;
  IAudioData* m_audioData;

  size_t PosToAbsolutePos(double pos);
};

#endif