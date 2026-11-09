using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OneShotInstructionsOverlay : MonoBehaviour
{
    public ThemeStyleSheet myTss;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
#if UNITY_2021_1_OR_NEWER
        UIDocument uiDoc = new GameObject("myUI").AddComponent<UIDocument>();
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        panelSettings.themeStyleSheet = myTss;

        uiDoc.panelSettings = panelSettings;
        var root = uiDoc.rootVisualElement;

        root.Add(new Label(){ text = $"<b>{Application.productName} using Unity {Application.unityVersion}</b>"});

        // for the implementation of the gamepad and keyboard shortcuts see SoundObjectPlayer.cs  SoundObjectPlayer.Start()
        root.Add(new TextElement()
        {
            text =
            @"<color=white><size=20>Example of Using a Unity spatializer plugin to interface with the PS5 object audio sdk to provide 3D Audio rendering.
            Controls:
            <b>R1/key1:</b> Add new 3D audio object to world
            <b>L1/key2:</b> Remove oldest 3D audio object from world
            <b>Triangle:</b> Toggle using 3D system (AudioSource.spatialize)
            <b>Square/key3:</b> Toggle AudioListener.volume
            <b>Circle/key4:</b> Toggle Mixer SfxReverb (requires spatialBlend<1.0f)
            <b>dpad left/right:</b> Adjust spatialBlend
            "
        });
#endif

    }
}
