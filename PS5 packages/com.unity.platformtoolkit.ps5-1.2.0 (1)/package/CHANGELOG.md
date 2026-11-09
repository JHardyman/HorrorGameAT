# Changelog

## [1.2.0] - 2026-09-17

### Fixed
- The saving system no longer needs to buffer contents in memory before they are committed.
- Fixed an issue where the wrong result was checked for initialisation failures.
- Fixed a bug where initializing Platform Toolkit very early could cause a duplicate internal add of a user.

## [1.1.0] - 2026-05-25

### Added
- Support for Build Profiles has been added for 6000.4+. This is provided via a new Build Profile component which allows an implementation to be configured to override the Platform Toolkit settings in Project Settings. These settings still function as the default for when this component is non in-use.
- IPS5Settings now exposes an AttributeSettings property, consistent with other platform settings interfaces.

### Fixed
- Fixed IPS5Settings not being returned by PlatformToolkitEditor.TryGetSettings method.
- Fixed an issue where disabling InitialUserAlwaysLoggedIn caused the wrong exception to be thrown.
- Fixed PlatformToolkit.Capabilities.PrimaryAccount always reporting true on PS5 regardless of the InitialUserAlwaysLoggedIn param file setting. It now correctly reports false when that setting is disabled.

## [1.0.1] - 2026-02-16

### Changed
- Update for internal changes to the core package.
- Bumped package dependency versions for latest SDK support.

### Fixed
- Fixed an issue where the input pairing system's events did not align to similar ones found in the Input System package. When the Input System package is in-use, Platform Toolkit now hooks up to these events to synchronize them.


## [1.0.0] - 2025-11-03
- Initial release
