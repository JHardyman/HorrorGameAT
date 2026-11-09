
/*
    list of effects are pulled into unity player using UnityGetAudioEffectDefinitions() ... called from :
        GetAudioEffectDefinitions	C++
        GetAudioSpatializerDefinitions	C++
        AudioManager::GetCurrentSpatializerDefinition	C++
    	AudioSource::CreateFMODGroups	C++
        AudioSource::Play	C++

  see Project Settings->Audio->Spatialize Plugin for name of plugin to load.
   having a file called "Assets/Editor/plugin/x86_64/PS5AudioSpatializer.dll" in the project enabled the option in the project settings
   ... which in turn looks for a PS5 native plugin that has a function called "PS5AudioSpatializer" (see AudioManager::GetCurrentSpatializerDefinition() )
    "PS5AudioSpatializer" nneeds to be the name used in the native DECLARE_EFFECT("PS5AudioSpatializer", Spatializer) (see NativeAudioPlugins\NativeCode\PluginList.h)

code in this plugin is called thus:

>	Spatializer::ProcessCallback	C++
 	UnityAudioEffect_InternalReadCallback	C++
 	FMOD::DSPFilter::read(float**,int*,unsigned int*,FMOD_SPEAKERMODE,int,unsigned int)	C++ [mDescription.name="PS5AudioSpatializer"]
 	FMOD::DSPFilter::read(float**,int*,unsigned int*,FMOD_SPEAKERMODE,int,unsigned int)	C++ [mDescription.name="ChannelGroup:ASrcDryGroup"]
 	FMOD::DSPFilter::read(float**,int*,unsigned int*,FMOD_SPEAKERMODE,int,unsigned int)	C++ [mDescription.name="ChannelGroup:ASrcWetGroup"]
 	FMOD::DSPFilter::read(float**,int*,unsigned int*,FMOD_SPEAKERMODE,int,unsigned int)	C++ [mDescription.name="ChannelGroup:objectaudio[port]"]
 	FMOD::DSPFilter::read(float**,int*,unsigned int*,FMOD_SPEAKERMODE,int,unsigned int)	C++ [mDescription.name="Platform Object DSP Output"]
 	FMOD::DSPFilter::read(float**,int*,unsigned int*,FMOD_SPEAKERMODE,int,unsigned int)	C++ [mDescription.name="ChannelGroup:objectaudio[port]"]
 	FMOD::DSPFilter::read(float**,int*,unsigned int*,FMOD_SPEAKERMODE,int,unsigned int)	C++ [mDescription.name="FMOD ChannelGroup Target Unit"]
 	FMOD::DSPFilter::read(float**,int*,unsigned int*,FMOD_SPEAKERMODE,int,unsigned int)	C++ [mDescription.name="FMOD SourceCard Unity"]
 	FMOD::DSPSoundCard::read(void*,unsigned int*,FMOD_SPEAKERMODE,int,unsigned int)	C++ [mDescription.name="FMOD SourceCard Unity"]
 	FMOD::Output::mix(void*,unsigned int)	C++
 	FMOD::OutputAudio2::updateMixer()	C++
 	FMOD::Thread::callback(void*)	C++
 	0x00000008015AF5AF	C++


   "PS5AudioSpatializer"
*/
// Please note that this will only work on Unity 5.2 or higher.

 #include "AudioPluginUtil.h"
#if __PROSPERO__
#include <scebase_common.h>
#include <audio_out2.h>
#define USE_AUDIO_RENDER_THREAD (1)     // when enabled we create a thread to do the blocking sceAudioOut2ContextPush() as per sdk basic_multiplayer audio sample.
#endif
#include <queue>

extern "C" UNITY_AUDIODSP_EXPORT_API int SpatializerEndframe();

static const int32_t kMaxObjectChannels = 64;

#if __PROSPERO__
static SceAudioOut2ContextHandle gSpatializerContextHandle = SCE_AUDIO_OUT2_CONTEXT_HANDLE_INVALID;
SceAudioOut2UserHandle gUserHandle = SCE_AUDIO_OUT2_USER_HANDLE_INVALID;
std::queue<SceAudioOut2PortHandle> gFreePorts;
 #if USE_AUDIO_RENDER_THREAD
static SceKernelEventFlag gAudioDataAvailable;
static ScePthread gAudioOutThreadHandle = SCE_PTHREAD_NEEDS_INIT;
static bool bAudioRunning = true;
static bool gContextPushInProgress = false; // could be used to detect if we are currently waiting for a progress to be pushed, not likely needed if we have contextParam.queueDepth>1
 #endif
#endif

const size_t kPCMCircularBufferSize = 16384*kMaxObjectChannels;
void* gPCMOutputBuffer = NULL;
void* gPCMOutputBufferWritePointer = NULL;

bool gPendingdata = false;
float g_globalVolume = 1.0f;

namespace Spatializer
{
    bool enableProcessCopy = true;
    bool enableSDKOutput = true;


     enum
     {
         P_AUDIOSRCATTN,
         P_FIXEDVOLUME,
         P_CUSTOMFALLOFF,
         P_NUM
     };


    struct EffectData
    {
#if !__PROSPERO__
        EffectData() :
            lastLeftVolume(0.0f),
            lastRightVolume(0.0f)
        { }
#endif


        float p[P_NUM];

#if __PROSPERO__
        SceAudioOut2PortHandle portHandle;
#else
        float lastLeftVolume;
        float lastRightVolume;
#endif
        bool portResetRequired;
    };

    inline bool IsHostCompatible(UnityAudioEffectState* state)
    {
        // Somewhat convoluted error checking here because hostapiversion is only supported from SDK version 1.03 (i.e. Unity 5.2) and onwards.
        // Since we are only checking for version 0x010300 here, we can't use newer fields in the UnityAudioSpatializerData struct, such as minDistance and maxDistance.
        return
            state->structsize >= sizeof(UnityAudioEffectState) &&
            state->hostapiversion >= 0x010300;
    }

    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
        int numparams = P_NUM;
        definition.paramdefs = new UnityAudioParameterDefinition[numparams];
        RegisterParameter(definition, "AudioSrc Attn", "", 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, P_AUDIOSRCATTN, "AudioSource distance attenuation");
        RegisterParameter(definition, "Fixed Volume", "", 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, P_FIXEDVOLUME, "Fixed volume amount");
        RegisterParameter(definition, "Custom Falloff", "", 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, P_CUSTOMFALLOFF, "Custom volume falloff amount (logarithmic)");
        definition.flags |= UnityAudioEffectDefinitionFlags_IsSpatializer;
        return numparams;
    }

    static UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK DistanceAttenuationCallback(UnityAudioEffectState* state, float distanceIn, float attenuationIn, float* attenuationOut)
    {
        EffectData* data = state->GetEffectData<EffectData>();
        *attenuationOut =
            data->p[P_AUDIOSRCATTN] * attenuationIn +
            data->p[P_FIXEDVOLUME] +
            data->p[P_CUSTOMFALLOFF] * (1.0f / FastMax(1.0f, distanceIn));
        return UNITY_AUDIODSP_OK;
    }

#if USE_AUDIO_RENDER_THREAD
    void *audioOutThread(void *pData)
    {
        (void)pData;

        while (bAudioRunning)
        {
            int res = sceKernelWaitEventFlag(gAudioDataAvailable, 1, SCE_KERNEL_EVF_WAITMODE_AND, NULL, NULL);
            if (res != SCE_OK)
                printf("sceKernelWaitEventFlag() returned with 0x%x\n", res);

            // Pushes all audio and attributes to the rendering device.
            res = sceAudioOut2ContextPush(gSpatializerContextHandle, SCE_AUDIO_OUT2_BLOCKING_SYNC);
            gContextPushInProgress = false;
            if (res != SCE_OK)
                    printf("sceAudioOut2ContextPush() returned with 0x%x\n", res);

            res = sceKernelClearEventFlag(gAudioDataAvailable, 0);
            if (res != SCE_OK)
                    printf("sceKernelClearEventFlag() retured with 0x%x\n", res);
        }
        return NULL;
    }
#endif


    // called each time an audio effect dsp is create, e.g. from AudioSource::CreateFMODGroups()
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData* effectdata = new EffectData;
        memset(effectdata, 0, sizeof(EffectData));
        state->effectdata = effectdata;
        if (IsHostCompatible(state))
            state->spatializerdata->distanceattenuationcallback = DistanceAttenuationCallback;
        InitParametersFromDefinitions(InternalRegisterEffectDefinition, effectdata->p);
#if __PROSPERO__
        if (gSpatializerContextHandle == SCE_AUDIO_OUT2_CONTEXT_HANDLE_INVALID)
        {
            #define GRAIN 256

            SceAudioOut2ContextParam contextParam;

            size_t memorySize;
            int32_t result;

            sceAudioOut2ContextResetParam(&contextParam);
            contextParam.maxObjectPorts = kMaxObjectChannels;
            contextParam.guaranteeObjectPorts = 0; // Set to 0 if maxObjectPorts != 0.
            contextParam.maxPorts = 8;
            contextParam.numGrains = GRAIN;
            contextParam.queueDepth = 3;    // by increasing this number (from 1) we can overlap queues which can be important when we are pushing contexts while building the next ones, 3 is required for high (>60hz) frame rates.
            contextParam.flags = SCE_AUDIO_OUT2_CONTEXT_PARAM_FLAG_SUB; // 4 subports can be

            result = sceAudioOut2ContextQueryMemory(&contextParam, &memorySize);
            if (result < SCE_OK)
            {
                printf("sceAudioOut2ContextQueryMemory() failed error:0x%x\n", result);
                // Error handling
                return UNITY_AUDIODSP_ERR_UNSUPPORTED;
            }

            void *contextMemory = malloc(memorySize);

            result = sceAudioOut2ContextCreate(&contextParam, contextMemory, memorySize,
                &gSpatializerContextHandle);
            if (result < SCE_OK)
            {
                printf("sceAudioOut2ContextCreate() failed error:0x%x\n", result);
                free(contextMemory);
                // Error handling
                return UNITY_AUDIODSP_ERR_UNSUPPORTED;
            }

 //           printf("sceAudioOut2ContextCreate() created 0x%lx\n", gSpatializerContextHandle);

            result = sceAudioOut2UserCreate(SCE_USER_SERVICE_USER_ID_SYSTEM, &gUserHandle);
            if (result < SCE_OK)
            {
                printf("sceAudioOut2UserCreate() failed error:0x%x\n", result);
                return UNITY_AUDIODSP_ERR_UNSUPPORTED;
            }

            if (gPCMOutputBuffer == nullptr)
            {
                gPCMOutputBuffer = malloc(kPCMCircularBufferSize);
                gPCMOutputBufferWritePointer = gPCMOutputBuffer;
            }

#if USE_AUDIO_RENDER_THREAD
            result = sceKernelCreateEventFlag(&gAudioDataAvailable, "audioDataAvailable\0", SCE_KERNEL_EVF_ATTR_TH_FIFO, 1, NULL);
            if (result < SCE_OK)
                printf("sceKernelCreateEventFlag() failed error:0x%x\n", result);

            result = scePthreadCreate(&gAudioOutThreadHandle, NULL, audioOutThread, NULL, "audioOutThread");
            if (result < SCE_OK)
                printf("scePthreadCreate() failed error:0x%x\n", result);
#endif

        }


        SceAudioOut2PortHandle newport = SCE_AUDIO_OUT2_PORT_HANDLE_INVALID;
        if (!gFreePorts.empty())
        {
            newport = gFreePorts.front();
            gFreePorts.pop();
            effectdata->portResetRequired = true;   // if we are re-using a port we need to reset it correctly before using it
        }
        else
        {
            int32_t result;
            SceAudioOut2PortParam portParam;

            portParam.dataFormat = SCE_AUDIO_OUT2_DATA_FORMAT_FLOAT_MONO;
            portParam.portType = SCE_AUDIO_OUT2_PORT_TYPE_OBJECT_MAIN;
            portParam.samplingFreq = 48000;
            portParam.userHandle = gUserHandle;
            portParam.flags = 0;
            portParam.pad = 0;
            memset(portParam.reserved, 0, sizeof(portParam.reserved));

            result = sceAudioOut2PortCreate(gSpatializerContextHandle, &portParam, &newport);
            if (result < SCE_OK)
            {
                if (result == SCE_AUDIO_OUT2_ERROR_OUT_OF_RESOURCE)
                {
                    printf("unable to allocate object audio port resource\n");
                }
                else
                    printf("sceAudioOut2PortCreate() failed error:0x%x\n", result);

                delete effectdata;
                return UNITY_AUDIODSP_ERR_UNSUPPORTED;
            }
            effectdata->portResetRequired = false;  // new ports do not require resets
        }
        effectdata->portHandle = newport;
#endif

        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ReleaseCallback(UnityAudioEffectState* state)
    {
        EffectData* data = state->GetEffectData<EffectData>();
#if __PROSPERO__
        gFreePorts.push(data->portHandle);  // add it to the list of port so that it can get reused
#endif
        delete data;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK SetFloatParameterCallback(UnityAudioEffectState* state, int index, float value)
    {
        EffectData* data = state->GetEffectData<EffectData>();
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        data->p[index] = value;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK GetFloatParameterCallback(UnityAudioEffectState* state, int index, float* value, char *valuestr)
    {
        EffectData* data = state->GetEffectData<EffectData>();
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        if (value != NULL)
            *value = data->p[index];
        if (valuestr != NULL)
            valuestr[0] = 0;
        return UNITY_AUDIODSP_OK;
    }

    int UNITY_AUDIODSP_CALLBACK GetFloatBufferCallback(UnityAudioEffectState* state, const char* name, float* buffer, int numsamples)
    {
        return UNITY_AUDIODSP_OK;
    }


#if !__PROSPERO__       // i.e. editor only
    UNITY_AUDIODSP_RESULT ProcessCallbackPlaymode(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
        static const float kRad2Deg = 180.0f / kPI;

        EffectData* data = state->GetEffectData<EffectData>();

        // Check that I/O formats are right and that the host API supports this feature
        if (inchannels != 2 || outchannels != 2 || !IsHostCompatible(state) || state->spatializerdata == NULL)
        {
            memcpy(outbuffer, inbuffer, length * outchannels * sizeof(float));
            return UNITY_AUDIODSP_OK;
        }

        // Grab what we need from the spatializer data passed to us.
        float* l = state->spatializerdata->listenermatrix;
        float* s = state->spatializerdata->sourcematrix;
        float stereoPan = state->spatializerdata->stereopan;
        float spatialBlend = state->spatializerdata->spatialblend;
        float spread = state->spatializerdata->spread;

        // Calculate the azimuth based on the audio source's position relative to the listener's position and orientation. In this case,
        // azimuth will be zero when the source is in front of the listener, 90 when to the right, 180 when behind, 270 when to the left.
        float px = s[12];
        float py = s[13];
        float pz = s[14];

        float dir_x = l[0] * px + l[4] * py + l[8] * pz + l[12];
        float dir_y = l[1] * px + l[5] * py + l[9] * pz + l[13];
        float dir_z = l[2] * px + l[6] * py + l[10] * pz + l[14];

        float azimuth = (fabsf(dir_z) < 0.001f) ? 0.0f : atan2f(dir_x, dir_z);
        if (azimuth < 0.0f)
            azimuth += 2.0f * kPI;
        azimuth = FastClip(azimuth * kRad2Deg, 0.0f, 360.0f);

        // Calculate 3d channel volumes. If the source is in front of the listener, then both channels play at 0.7f.
        // If 100% to the left, then the left volume is 1.0f and the right is 0.0f. If 100% to the right, then the right volume is 1.0f and the left is 0.0f. We just
        // linearly interpolate between these hard-left, centered, and hard-right values. This isn't entirely correct, but it is pretty simple.
        bool onRightSide = azimuth <= 180.0f;

        // With these next operations, we'll end up with azimuth between 0 and 90. 0 still means in front of the listener
        // and 90 means either far left or far right.
        if (azimuth > 180.0f)
            azimuth -= 180.0f;
        if (azimuth > 90.0f)
            azimuth = 180.0f - azimuth;

        float maxLeftVolumeChange = onRightSide ? -0.7f : 0.3f;
        float maxRightVolumeChange = onRightSide ? 0.3f : -0.7f;
        float sideFactor = azimuth / 90.0f;
        float leftVolume3d = 0.7f + sideFactor * maxLeftVolumeChange;
        float rightVolume3d = 0.7f + sideFactor * maxRightVolumeChange;

        // Now, take spread into consideration. If spread is zero, we do nothing. If it is 180, left/right volume is equal no matter what the 3d position of the sound source. When it is greater than 180, we start reversing the 3d left and right volumes.
        if (spread > 180.0f)
        {
            float leftVolumeTemp = leftVolume3d;
            leftVolume3d = rightVolume3d;
            rightVolume3d = leftVolumeTemp;
            spread = 360.0f - spread;
        }

        // Spread now ranges from 0 to 180.
        float spreadNormalized = spread / 180.0f;
        leftVolume3d = leftVolume3d * (1.0f - spreadNormalized) + 0.7f * spreadNormalized;
        rightVolume3d = rightVolume3d * (1.0f - spreadNormalized) + 0.7f * spreadNormalized;

        // Calculate the 2d volumes. The stereo pan parameter dictates whether this is left (-1.0f), centered (0.0f), or right (1.0f).
        maxLeftVolumeChange = (stereoPan <= 0.0f) ? 0.3f : 0.7f;
        maxRightVolumeChange = (stereoPan <= 0.0f) ? 0.7f : 0.3f;
        float leftVolume2d = 0.7f - (stereoPan * maxLeftVolumeChange);
        float rightVolume2d = 0.7f + (stereoPan * maxRightVolumeChange);

        // Now, we determine the final volume based on the spatial blend, which tells us whether the sound is 2d (0.0f) or 3d (1.0f) or somewhere in between.
        float leftVolume = leftVolume2d*(1.0f-spatialBlend) + leftVolume3d*spatialBlend;
        float rightVolume = rightVolume2d*(1.0f-spatialBlend) + rightVolume3d*spatialBlend;

        // Rather than just snapping to the new left/right volumes, let's interpolate smoothly from the last volumes. This adds a delay in how we react to parameter changes,
        // but we should pretty much avoid clicks/pops when a sound moves quickly past the listener (and other abrupt parameter changes).
        float lastLeftVolume = data->lastLeftVolume;
        float lastRightVolume = data->lastRightVolume;
        float leftVolumeStep = (leftVolume - lastLeftVolume) / length;
        float rightVolumeStep = (rightVolume - lastRightVolume) / length;
        for (int i = 0; i < (int)length*outchannels; i += outchannels)
        {
            lastLeftVolume += leftVolumeStep;
            lastRightVolume += rightVolumeStep;

            outbuffer[i] = lastLeftVolume * inbuffer[i];
            outbuffer[i+1] = lastRightVolume * inbuffer[i+1];
        }

        data->lastLeftVolume = leftVolume;
        data->lastRightVolume = rightVolume;

        return UNITY_AUDIODSP_OK;
    }

#endif

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
 //       return UNITY_AUDIODSP_OK;   // do nothing
//        printf("Plugin_spatializer called, ProcessCallback() inbuffer:0x%p outbuffer:0x%p length:%d inchannels:%d outchannels:%d spatializer maxdist:%f currdsptick:%d\n", inbuffer, outbuffer, length, inchannels, outchannels, state->spatializerdata->maxDistance, state->currdsptick);

        EffectData* effect = state->GetEffectData<EffectData>();
        static UInt64 lastdataDspTick = 0;

        // if time has moved on since the last callback then advance the data ... this should really be done at the end of the lastframe not the start of the next one
        if (state->currdsptick != lastdataDspTick)
        {
            lastdataDspTick = state->currdsptick;
            SpatializerEndframe();  // if we haven't called SpatializerEndframe() from user scripts then call it now. this means data will be a gameframe delayed.
        }
        float passthrough = 1.0f - state->spatializerdata->spatialblend;

#if !__PROSPERO__
        passthrough = 1.0f;
        // alternatively you just set the audioSource.spatialBlend value to 0.0 ... but that's not what we want for playstation5
        ProcessCallbackPlaymode(state, inbuffer, outbuffer,length,inchannels,outchannels);
        return UNITY_AUDIODSP_OK;
#endif

        if (passthrough >= 1.0f)
        {
            memcpy(outbuffer, inbuffer, length * outchannels * sizeof(float));      // copy everything to the output
            return UNITY_AUDIODSP_OK;       // return here to avoid sending any data to the sdk
        }
        else if (passthrough <= 0.0f)
        {
            memset(outbuffer, 0, length * outchannels * sizeof(float));                 // zero out the outbuffer so nothing get passed back into the tree
        }
        else
        {
            for (unsigned int i = 0; i < length * outchannels; i++)
            {
                outbuffer[i] = inbuffer[i] * passthrough;
            }
        }

        if (enableSDKOutput == false)
        {
            return UNITY_AUDIODSP_OK;       // return here to avoid sending any data to the sdk
        }
#if __PROSPERO__
        float objectAudioPassthrough = fmaxf(fminf(state->spatializerdata->spatialblend, 1.0f), 0.0f);

        void* pcmData = gPCMOutputBuffer;
        if (intptr_t(pcmData) + length * sizeof(float) > intptr_t(gPCMOutputBuffer) + kPCMCircularBufferSize)
        {
            pcmData = gPCMOutputBufferWritePointer;   //
        }
        float* audioData = (float*)pcmData;
        if (audioData)
        {
            //if stereo or surround channel, down - mix the first two channels (matching what we do for pad audio)
            if ((inchannels > 1) )
            {
                for (int sample = 0; sample < length; sample++)
                    audioData[sample] = inbuffer[sample * inchannels + 0] * 0.707 + inbuffer[sample * inchannels + 1] * 0.707;
            }
            else
            {
                memcpy(audioData, inbuffer, length * outchannels * sizeof(float));
            }
        }
        gPCMOutputBufferWritePointer = (void *)(intptr_t(pcmData) + length * sizeof(float));

        const int kMaxAttributes = 16;
        SceAudioOut2Attribute aAttribute[kMaxAttributes];
        uint32_t numAttributes = 0;


        if (effect->portResetRequired)
        {
            effect->portResetRequired = false;
            aAttribute[numAttributes].attributeId = SCE_AUDIO_OUT2_PORT_ATTRIBUTE_ID_RESET_STATE;
            aAttribute[numAttributes].value = nullptr;
            aAttribute[numAttributes].valueSize = 0;
            numAttributes++;
        }


        SceAudioOut2Pcm pcm;
        pcm.data = pcmData;
        aAttribute[numAttributes].attributeId = SCE_AUDIO_OUT2_PORT_ATTRIBUTE_ID_PCM;
        aAttribute[numAttributes].value = &pcm;
        aAttribute[numAttributes].valueSize = sizeof(pcm);
        numAttributes++;

        float m_gain = g_globalVolume * objectAudioPassthrough;
        float m_pos[3];
        float m_spread = state->spatializerdata->spread * ((2.0f*3.14159265f)/360.f);

        aAttribute[numAttributes].attributeId = SCE_AUDIO_OUT2_PORT_ATTRIBUTE_ID_GAIN;
        aAttribute[numAttributes].value = &m_gain;
        aAttribute[numAttributes].valueSize = sizeof(float);
        numAttributes++;

        float* m = state->spatializerdata->listenermatrix;
        float* s = state->spatializerdata->sourcematrix;

        float px = s[12];
        float py = s[13];
        float pz = s[14];

        m_pos[0] = m[0] * px + m[4] * py + m[8] * pz + m[12];
        m_pos[1] = m[1] * px + m[5] * py + m[9] * pz + m[13];
        m_pos[2] = m[2] * px + m[6] * py + m[10] * pz + m[14];



        aAttribute[numAttributes].attributeId = SCE_AUDIO_OUT2_PORT_ATTRIBUTE_ID_POSITION;
        aAttribute[numAttributes].value = &m_pos;
        aAttribute[numAttributes].valueSize = sizeof(m_pos);
        numAttributes++;

        aAttribute[numAttributes].attributeId = SCE_AUDIO_OUT2_PORT_ATTRIBUTE_ID_SPREAD;
        aAttribute[numAttributes].value = &m_spread;
        aAttribute[numAttributes].valueSize = sizeof(m_spread);
        numAttributes++;

        int result = sceAudioOut2PortSetAttributes(effect->portHandle, aAttribute, numAttributes);
        if (result != SCE_OK)
        {
            printf("sceAudioOut2PortSetAttributes returned 0x%x\n", result);
        }

#endif

        gPendingdata = true;



        return UNITY_AUDIODSP_OK;
    }



    extern "C" UNITY_AUDIODSP_EXPORT_API int SpatializerEndframe()
    {
        int res = 0;
        if (gPendingdata)
        {
            gPendingdata = false;
#if __PROSPERO__
            res = sceAudioOut2ContextAdvance(gSpatializerContextHandle);
            if (res != SCE_OK)
            {
                printf("sceAudioOut2ContextAdvance() failed with 0x%x\n", res);
            }
#if USE_AUDIO_RENDER_THREAD
            gContextPushInProgress = true;

            res = sceKernelSetEventFlag(gAudioDataAvailable, 1);
            if (res != SCE_OK)
                printf("sceKernelSetEventFlag() failed with 0x%x\n", res);
#else
 #if USE_AUDIO_BLOCKING_SYNC
            res = sceAudioOut2ContextPush(gSpatializerContextHandle, SCE_AUDIO_OUT2_BLOCKING_SYNC);
 #else
            // don't block ... relies on fmod audioout to trigger the mixing with the correct timing. avoids potential hangs in mixer thread, which in turn feedbacks more mixer work.
            res = sceAudioOut2ContextPush(gSpatializerContextHandle, SCE_AUDIO_OUT2_BLOCKING_ASYNC);
 #endif
            if (res != SCE_OK)
                printf("sceAudioOut2ContextPush() failed with 0x%x\n", res);
#endif
#endif
        }
        return res;

    }


}
