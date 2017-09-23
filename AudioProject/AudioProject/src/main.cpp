#include <iostream>
#include <SDL2/SDL.h>

#include "sdl/sdlaudiodevice.hpp"
#include "sdl/sdlaudiocontext.hpp"

#define FILE_PATH "D:\\Workspace\\audio\\AudioProject\\res\\audio\\testClip.wav\0"
#undef main // SDL defines its own main and i don't want it



int main(int argc, char** argv)
{
  SDL_Init(SDL_INIT_AUDIO);

  IAudioDevice* device = new SDLAudioDevice();
  IAudioContext* context = new SDLAudioContext();

  IAudioData* data = device->CreateAudioFromFile(FILE_PATH);

  SampleInfo info;
  info.volume = 1.0;

  AudioObject sound(info, data);

  try
  {
    char in = 0;
    while (in != 'q')
    {
      std::cin >> in;
      switch (in)
      {
      case 'a':
        context->PlayAudio(sound);
        break;
      case 's':
        context->PauseAudio(sound);
        break;
      case 'd':
        context->StopAudio(sound);
        break;
      }
    }
  }
  catch (const char* msg)
  {
    std::cerr << msg << std::endl;
    std::string pause;
    std::cin >> pause;
  }

  device->ReleaseAudio(data);
  delete context;
  delete device;

  SDL_Quit();

  return 0;
}