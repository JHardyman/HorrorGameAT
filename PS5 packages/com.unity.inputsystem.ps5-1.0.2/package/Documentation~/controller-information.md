# Controller information

Use the following APIs to obtain specific information about the connected controllers. For more information, refer to [Obtaining Information About the Features of a Controller](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Reference/obtaining-controller-feature-information.html) (PlayStation®5 DevNet).

## Connection type

Use [`ConnectionType`](xref:UnityEngine.InputSystem.PS5.ConnectionType) to check if the controller is connected locally or remotely.


|**Connection type**|**Description**|
|---|---|
|**Local**|Indicates that the controller is connected over USB or a Bluetooth connection.|
|**Remote**|Indicates that a compatible controller is connected via remote play. For more information refer to [Handling of Devices Connected with Remote Play (or Share Play)](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/using-remote-play.html) (PlayStation®5 DevNet).|


## Device class

Use [`DeviceClass`](xref:UnityEngine.InputSystem.PS5.DeviceClass) to identify the type of controller connected to the PS5. For more information on the supported controller types, refer to [Using Special Controllers](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/using-special-controllers.html) (PlayStation®5 DevNet).

The available classes are as follows:

* Standard
* Guitar
* Drum
* DjTurntable 
* Dancemat	
* SteeringWheel	
* Stick
* FlightStick	

## Stick information

Use [`StickInformation`](xref:UnityEngine.InputSystem.PS5.StickInformation) to return information about the analog sticks dead zones. For more information, refer to [ScePadStickInformation](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Reference/sce-pad-stick-information.html) (PlayStation®5 DevNet)

## Touchpad information

Use [`TouchPadInformation`](xref:UnityEngine.InputSystem.PS5.TouchPadInformation) to return the resolution and pixel density of the connected controllers touchpad. For more information on using this data, refer to [Touchpad](touchpad.md).



