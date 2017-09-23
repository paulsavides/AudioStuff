#ifndef SDL_AUDIO_DEVICE_INCLUDED_HPP
#define SDL_AUDIO_DEVICE_INCLUDED_HPP

#include <string>
#include "../iaudiodevice.hpp"

class SDLAudioDevice : public IAudioDevice
{
public:
  virtual IAudioData* CreateAudioFromFile(const std::string& filePath);
  virtual void ReleaseAudio(IAudioData* audioData);
private:
};

#endif