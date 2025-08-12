# Changelog

## [0.3.1] - 2025-08-12

### Fixed

- Version number and changelog

## [0.3.0] - 2025-08-12

### Added

- New event-based core behaviours
- Custom behaviour disconnects on bugle drop
- Acceptable ranges for numeric config entries
- `HarmonicSmoothMult` and `PitchSmoothMult` config entries

### Removed

- `UILoadInterval` config entry

### Changed

- Reorganized and restructured core implementation
- UI now loads without direct polling 
- Default key for `ToggleUIKey` config entry is now `U`

### Fixed

- Compatibility with PEAK version .1.21.a 4587d11a2
- AudioSource properly stops on pause in offline mode
- AudioSource properly stops when not tooting

## [0.2.2] - 2025-08-05

### Fixed

- Initial bend position now properly resets only when your own bugle starts tooting

## [0.2.1] - 2025-08-05

### Changed

- Reverted vertical angle smoothing to previous value

### Removed

- Disabled pitch smoothing

## [0.2.0] - 2025-08-05

### Added

- Slight pitch smoothing

### Changed

- Volume no longer immediately resets to zero at the start of a toot
- Connects sync on first toot instead of via polling
- Slightly increased vertical angle smoothing for selecting partials

### Removed

- ConnectInterval config entry (no longer used)

## [0.1.1] - 2025-08-04

### Changed

- Updated README

## [0.1.0] - 2025-08-02

### Added

- Total bugle overhaul

<!---
https://keepachangelog.com/en/1.1.0/
--->
