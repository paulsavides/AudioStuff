#ifndef SDL_WAV_AUDIO_DATA_INCLUDED_HPP
#define SDL_WAV_AUDIO_DATA_INCLUDED_HPP

#include "../iaudiodata.hpp"
#include <SDL2/SDL.h>
#include <string>

class SDLWAVAudioData : public IAudioData
{
public:
  SDLWAVAudioData(const std::string& fileName, bool streamFromFile);
  virtual ~SDLWAVAudioData();

  virtual size_t GenerateSamples(float* stream, size_t streamLength,
          size_t post, const SampleInfo& info);
  virtual size_t GetAudioLength();
private:
  Uint8* m_pos;
  Uint8* m_start;
  Uint8* m_end;
  
  SDLWAVAudioData(SDLWAVAudioData& other) { (void)other; }
  void operator=(const SDLWAVAudioData& other) { (void)other; }
};

#endif