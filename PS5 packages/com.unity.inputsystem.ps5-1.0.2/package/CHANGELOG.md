# Changelog
All notable changes to the input system package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

Due to package verification, the latest version below is the unpublished version and the date is meaningless.
however, it has to be formatted properly to pass verification tests.

## [1.0.2] - 2026-01-13

### Changed

- Updated links in the documentation from Unity DevNet resources to Unity Discussions.
- Updated documentation to explicitly outline the features that are not supported in the Unity Editor.

## [1.0.1] - 2025-10-21

### Changed
- Signature files have been updated

### Fixed
- In the Sample, fixed a InvalidOperationException when Active Input Handiling was set to "Input System (New)"

## [1.0.0] - 2025-01-07

### Added 
Added SetAngularVelocityDeadbandState to set the deadband filter of the angular velocity sensor

Added AdvancedAndCompatible and AdvancedAndCompatible2 to VibrationMode so that Advanced and Compatible(2) vibration can be used without having to switch modes

### Fixed
In the Sample SetTiltCorrectionState is now called when the Gamepad is connected so that the vertial state of the controller is correct rather than offset to the rotation of the controler was when the sample started

### Changed
Improved documentation for Trigger Effects and other gamepad features.

## [0.2.2-preview] - 2024-06-03

### Added
Added delta and touch phase properties to a PS5Touch on the touchpad

### Fixed
Fixed an an issue where DualSenseGamepad.current and DualSenseGamepadPC.current would return a DualShock controller, they now return their own types

### Changed
DualSenseGamepadPC now uses the input report of, and inherits from, DualSenseGamepadHID from the main input system packge. This means that this device now supports all operations of DualSenseGamepadHID, including bluetooth input (not output) support.

DualSenseGamepadPC will now be used when a Dualsense Edge Controller is plugged in the Editor rather than falling back to DualSenseGamepadHID

Changed the display name of DualSenseGamepadPC from "PS5 DualSense (on PC)" to "PS5 DualSense (in Editor)" to better reflect its use

Updated the display names of Acceleration, Orientation, Angular Velocity, PS5TouchControl/ID, PS5TouchControl/Position

Updated Input System Package dependency version to 1.10.0

## [0.2.1-preview] - 2024-04-25

### Fixed
Fixed some GUID conflicts with the PS4 Input System Package
Guarantee that Input Processors are initalised before devices to avoid a InvalidOperationException that could occour in some scenarios

## [0.2.0-preview] - 2024-02-28

### Added
Added in Unity 6000.0.0b12 (Unity 6 Beta) or later, the following properties to DualSenseGamepad on PS5, previously only avalible via PS5Input. functions: touchPadInformation, stickInfo, deviceClass, connectionType. Pressing both thumbsticks together in the Input System Sample will show the infomation listed above.

Added in Unity 6000.0.0b12 (Unity 6 Beta) or later, Added the ability to SetVibrationMode, SetTiltCorrectionState, SetMotionSensorState via DualSenseGamepad on PS5

### Fixed
Fixed the Acceleration, Orientation and Angular Velocity values of a Dualsense Controller on PS5 being reported in the SDK's Right-Handed Coordinates Space rather than Unity's Left-Handed Coordinates Space

## [0.1.13-preview] - 2023-10-30

Fixed the incorrect device layout (DualSenseGamepadHID) being used with Dualsense Controllers in the Editor for Controllers on firmware 0356 or later
Removed a warning when a new device was added in the Editor

Added Sample for PS5 Dualsense Controller Features

## [0.1.12-preview] - 2023-06-30

Set Input System runtime to run in the background. This fixes issues where the InputSystem would not receive input events when the game was not in focus.
Update Input System dependency to version 1.6.1

## [0.1.11-preview] - 2022-09-01

Add support for different slotIndex values in Editor for up to four connected gamepads

## [0.1.10-preview] - 2022-01-19

Change when the device layout is registered from being after a scene is loaded to after the assemblies are loaded. This fixes issues where some systems could create the InputSystem code earlier than we were registering the device layout causing PS5 controllers to not be registered correctly until they were disconnected and reconnected.

## [0.1.9-preview] - 2021-11-23

Add support for new TriggerEffect modes

## [0.1.8-preview] - 2021-10-21

Fixes for TriggerEffectMode.Vibration when in editor

## [0.1.7-preview] - 2021-01-14

Fixes for input debugger

## [0.1.6-preview] - 2020-07-24

Added support for name change of device to "PS5DualSenseGamepad"

## [0.1.5-preview] - 2020-05-26

Added support for using the controller inside Unity editor

## [0.1.4-preview] - 2020-05-08

Fixed missing meta files

## [0.1.3-preview] - 2020-04-10

Fixed incorrect triggering of "select" gamepad events when square button is pressed 

## [0.1.2-preview] - 2019-11-27

Updated package to include platform name

## [0.1.1-preview] - 2019-10-30

Updated package dependencies to use latest `com.unity.inputsystem` package version 1.0.0-preview.1

## [0.1.0-preview] - 2019-10-18

First release.





