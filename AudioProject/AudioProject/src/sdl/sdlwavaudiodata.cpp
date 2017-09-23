#include "sdlwavaudiodata.hpp"
#include <iostream>

typedef int32_t int32;

SDLWAVAudioData::SDLWAVAudioData(const std::string& fileName, bool streamFromFile)
{
  // TODO: Handle stream from file
  SDL_AudioSpec wavSpec;
  Uint8* wavStart;
  Uint32 wavLength;

  if (SDL_LoadWAV(fileName.c_str(), &wavSpec, &wavStart, &wavLength) == NULL)
  {
    throw fileName;
  }

  m_pos    = wavStart; // pos will eventually be moved to different location
  m_start  = wavStart;
  m_end    = m_start + wavLength;
}

SDLWAVAudioData::~SDLWAVAudioData()
{
  SDL_FreeWAV(m_start);
}


size_t SDLWAVAudioData::GenerateSamples(float* stream, size_t streamLength, size_t pos, const SampleInfo& info)
{
  m_pos = m_start + pos;

  Uint32 length_left = (Uint32) (m_end - m_pos);
  if (length_left == 0 || m_end < m_pos)
  {
    return (size_t)-1;
  }

  Uint32 length = (Uint32)streamLength;
  length = (length) > length_left ? length_left : length;

  // TODO: Make this more general
  Sint16* samples = (Sint16*)m_pos;

  float factor = (float) info.volume * (1.0f/32768.0f);
  for (Uint32 i = 0; i < length; i++)
  {
    stream[i] = (*samples) * factor;
    samples++;
  }

  m_pos = (Uint8*) samples;

  std::cout << (int) m_pos << "," << (int) m_start << "," << (int) m_end << std::endl;

  return (size_t)(m_pos - m_start);
}

size_t SDLWAVAudioData::GetAudioLength()
{
  return (size_t) (m_end - m_start);
}