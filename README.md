# Floating Player Controller

A Unity project that implements a floating player controller with advanced movement mechanics, including floating, jumping, dodging, and camera control.

## Features

- **Floating Mechanic**: Keeps the player at a specific height above the ground using spring forces.
- **Smooth Movement**: Allows the player to move in any direction with smooth rotation.
- **Jumping**: Supports jumping with optional animations.
- **Dodging**: Includes a dodge mechanic with cooldown and directional control.
- **Camera Control**: Provides first-person or third-person camera control with mouse input.
- **Slope Handling**: Adjusts movement to handle slopes seamlessly.

## Scripts Overview

### 1. `FloatingPlayerController.cs`
- Manages player movement, floating, jumping, and dodging.
- Uses physics-based forces to maintain a floating height.
- Updates animations for running, jumping, and grounded states.

### 2. `PlayerLook.cs`
- Handles camera rotation based on mouse input.
- Locks the cursor for better control.
- Supports clamping vertical rotation to prevent over-rotation.

## Requirements

- Unity 2021.3 or later.
- `.NET Framework 4.7.1` compatibility.
- Rigidbody component on the player GameObject.

## Installation

1. Clone this repository: ```https://github.com/BROODHONEY/Character-Controller.git```
2. Open the project in Unity
3. Attach the `FloatingPlayerController` script to your player GameObject.
4. Attach the `PlayerLook` script to the camera or a child GameObject of the player.
5. Configure the public fields in the Inspector (e.g., `orientation`, `groundCheck`, `animator`, etc.).

## Usage

- **Movement**: Use `W`, `A`, `S`, `D` to move the player.
- **Jumping**: Press `Space` to jump.
- **Dodging**: Press `Left Shift` to dodge in the direction of movement.
- **Camera Control**: Move the mouse to rotate the camera.

## Customization

- Adjust the floating height, spring strength, and damping in the `FloatingPlayerController` script.
- Modify the camera sensitivity and rotation limits in the `PlayerLook` script.
- Add animations to the `Animator` component for smoother transitions.

## Debugging

- Use the `OnDrawGizmosSelected` method in `FloatingPlayerController` to visualize ground checks and floating height in the Scene view.

## Contributing

Contributions are welcome! Feel free to submit a pull request or open an issue for suggestions or bug reports.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Unity Documentation for Rigidbody and Physics.
- Inspiration from various Unity tutorials on player movement and camera control.
