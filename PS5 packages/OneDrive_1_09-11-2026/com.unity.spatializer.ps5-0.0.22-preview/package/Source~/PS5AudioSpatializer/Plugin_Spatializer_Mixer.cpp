
 #include "AudioPluginUtil.h"

//#include <cstring>
namespace Spatializer
{
    extern "C" UNITY_AUDIODSP_EXPORT_API int SpatializerEndframe();
}

extern float g_globalVolume;


namespace SpatializerMixer
{

    static bool enablecopy = true;

    enum Param
    {
        P_GLOBALVOLUME,
        P_NUM
    };

    struct EffectData
    {
        int test;
        struct Data
        {
            float p[P_NUM];
        };
        union
        {
            Data data;
            unsigned char pad[(sizeof(Data) + 15) & ~15]; // This entire structure must be a multiple of 16 bytes (and and instance 16 byte aligned) for PS3 SPU DMA requirements
        };
    };

    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
        int numparams = P_NUM;
        definition.paramdefs = new UnityAudioParameterDefinition[numparams];
        RegisterParameter(definition, "Global Voume", "", 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, P_GLOBALVOLUME, "Global volume of spatialized objects");

        return 0;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData *effectdata = new EffectData;
        memset(effectdata, 0, sizeof(EffectData));
        InitParametersFromDefinitions(InternalRegisterEffectDefinition, effectdata->data.p);
        state->effectdata = effectdata;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ReleaseCallback(UnityAudioEffectState* state)
    {
        // Cleanup the effect-local data that was created
        EffectData *data = state->GetEffectData<EffectData>();
        if (data)
        {
            delete data;
            state->effectdata = nullptr;
        }
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK SetFloatParameterCallback(UnityAudioEffectState* state, int index, float value)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        data->p[index] = value;
        if (index == P_GLOBALVOLUME)
            g_globalVolume = value;

        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK GetFloatParameterCallback(UnityAudioEffectState* state, int index, float* value, char* valuestr)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        if (value != NULL)
            *value = data->p[index];
        if (valuestr != NULL)
            valuestr[0] = 0;
        return UNITY_AUDIODSP_OK;

    }

    int UNITY_AUDIODSP_CALLBACK GetFloatBufferCallback(UnityAudioEffectState*, const char*, float*, int)
    {
        return UNITY_AUDIODSP_OK;
    }

    // after everything else has been processed with get a mixer callback which needs to advance to the next frame
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(
        UnityAudioEffectState* state, float* inBuffer, float* outBuffer, unsigned int length, int inChannels,
        int outChannels)
    {
#if __PROSPERO__
        Spatializer::SpatializerEndframe();
#endif
        if (enablecopy)
        {
            memcpy(outBuffer, inBuffer, length * outChannels * sizeof(float));      // copy in to out (i.e. do nothing)

        }
        else
        {
            memset(outBuffer, 0, length * outChannels * sizeof(float));      // zero the output buffer

        }

        return UNITY_AUDIODSP_OK;
    }
} // namespace SpatialMixer
