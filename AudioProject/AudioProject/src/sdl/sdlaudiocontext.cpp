#include "sdlaudiocontext.hpp"

#define FREQUENCY 44100
#define SAMPLE_RATE 2048
#define CHANNELS 2

static void SDLAudioContext_AudioCallback(void* userData, Uint8* streamIn, int length)
{
  SDLAudioContext* context = (SDLAudioContext*)userData;
  context->GenerateSamples(streamIn, length);
}

SDLAudioContext::SDLAudioContext()
{
  SDL_AudioSpec spec;

  //TODO: don't hardcode these values!
  SDL_zero(spec);
  spec.freq = FREQUENCY;
  spec.format = AUDIO_S16SYS;
  spec.channels = CHANNELS;
  spec.samples = SAMPLE_RATE;
  spec.callback = SDLAudioContext_AudioCallback;
  spec.userdata = this;

  // TODO: Handle different specs
  m_device = SDL_OpenAudioDevice(NULL, 0, &spec, NULL, SDL_AUDIO_ALLOW_ANY_CHANGE);

  if (m_device == 0)
  {
    throw SDL_GetError();
  }

  SDL_PauseAudioDevice(m_device, 0);
}

SDLAudioContext::~SDLAudioContext()
{
  SDL_CloseAudioDevice(m_device);
}

void SDLAudioContext::PlayAudio(AudioObject& ao)
{
  SDL_LockAudioDevice(m_device);

  // This prevents duplicates
  RemoveAudio(ao);
  m_playingAudio.push_back(&ao);

  SDL_UnlockAudioDevice(m_device);
}

void SDLAudioContext::PauseAudio(AudioObject& ao)
{
  SDL_LockAudioDevice(m_device);

  RemoveAudio(ao);

  SDL_UnlockAudioDevice(m_device);
}

void SDLAudioContext::StopAudio(AudioObject& ao)
{
  SDL_LockAudioDevice(m_device);

  if (RemoveAudio(ao))
  {
    ao.SetPos(0.0);
  }

  SDL_UnlockAudioDevice(m_device);
}

void SDLAudioContext::GenerateSamples(Uint8* streamIn, int streamInLen)
{
  size_t streamLen = (size_t)(streamInLen / 2); // we'll be writing samples as
                                                // int16 so can think of stream as
                                                // half the given length

  m_stream.reserve(streamLen);
  float* floatStream = m_stream.data();  // retrieve raw float array pointer
                                         // TODO: make sure to thank the vector
                                         // for allocating the memory

  for (size_t i = 0; i < streamLen; i++)
  {
    floatStream[i] = 0.0f; // zero out the stream
  }

  std::vector<AudioObject*>::iterator it  = m_playingAudio.begin();
  std::vector<AudioObject*>::iterator end = m_playingAudio.end();

  for (; it != end; ++it)
  {
    if (!(*it)->GenerateSamples(floatStream, streamLen))
    {
      RemoveAudio(*(*it));
    }
  }

  Sint16* stream = (Sint16*)streamIn;
  for (size_t i = 0; i < streamLen; i++)
  {
    float val = floatStream[i];
    val = LimitSample(val);

    stream[i] = (Sint16)(val * 32767);
  }

}

bool SDLAudioContext::RemoveAudio(AudioObject& ao)
{
  std::vector<AudioObject*>::iterator it  = m_playingAudio.begin();
  std::vector<AudioObject*>::iterator end = m_playingAudio.end();

  for (; it != end; ++it)
  {
    if (*it == &ao)
    {
      m_playingAudio.erase(it);
      return true;
    }
  }

  return false;
}

float SDLAudioContext::LimitSample(float val)
{
  if (val > 1.0f)
  {
    val = 1.0f;
  }
  else if (val < -1.0f)
  {
    val = -1.0f;
  }

  return val;
}