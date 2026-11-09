# Changelog
All notable changes to the package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

Due to package verification, the latest version below is the unpublished version and the date is meaningless.
however, it has to be formatted properly to pass verification tests.

## [0.0.22-preview] - 2026-07-20

### Changed
- Updated the Documentation button in Package Manager to point to the local index.md file in the Documentation~ folder

## [0.0.21-preview] - 2026-04-28

### Changed
- Add support for SDK 13.0

## [0.0.20-preview] - 2026-01-15

### Fixed
- [Sample] Fixed errors when importing the sample in to a project that has Active Input Handiling set to Input System (New)
- [Sample] Fixed warnings when importing the sample related to empty folders with .meta files

## [0.0.19-preview] - 2025-10-22

### Changed
- Signature files have been updated
- Add support for SDK 12.0

## [0.0.18-preview] - 2025-04-09
### Fixed
Add support for SDK 11.0

## [0.0.17-preview] - 2025-01-29
### Fixed
Fixed a `The namespace '<global namespace>' already contains a definition for 'ReadmeEditor'` error that prevented compilation when combining the Spatializer Sample with other Unity samples (most notably URP)

## [0.0.16-preview] - 2024-12-27
### Fixed
Add support for SDK 10.0

## [0.0.15-preview] - 2024-06-26
### Fixed
Fixed the Plugin being incorrectly included for PS4 on 2021.3

## [0.0.14-preview] - 2024-05-08
### Changed
Added support for SDK 9.0

## [0.0.13-preview] - 2023-11-06
### Changed
Added support for SDK 8.0

## [0.0.12-preview] - 2023-07-11
### Changed
Removed unused .meta files

## [0.0.11-preview] - 2023-05-11
### Added
Added support for SDK 7.0

## [0.0.10-preview] - 2022-12-13
### Changed
Fixes for 2020.3

## [0.0.9-preview] - 2022-12-12
### Changed
Fixed SpatialBlend value being correctly used in sdk audio. Documentation improvements.

## [0.0.8-preview] - 2022-12-06
### Changed
Documentation improvements

## [0.0.7-preview] - 2022-11-27
### Changed
Added global volume parameter to PS5SpatializerMixer. This allows the volume of the 3d object audio to be globally adjusted.

## [0.0.6-preview] - 2022-11-22
### Changed
Improved threading and increased queueDepth to 3

## [0.0.5-preview] - 2022-11-18
### Changed
Added stereo panning to editor playmode. Added local build script at Source~\PS5AudioSpatializer\buildSpatializerPS5NativePlugin.bat


## [0.0.4-preview] - 2022-11-15

### Changed
Added reverb example to sample. Improved audio in editor playmode. Misc fixes.

## [0.0.3-preview] - 2022-10-24

### Changed
Changed context push to use async code. Removes potential hangs in mixer thread.

## [0.0.2-preview] - 2022-10-04

### Added


### Changed

Fixed metadata GUIDs that were conflicting with the PSN package.

## [0.0.1-preview] - 2022-07-29

### Added


### Changed
