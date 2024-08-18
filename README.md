# HUB for Joystick Testing

## Overview

**HUB** is a Windows Forms application designed to provide a user-friendly interface for testing Xbox Series S controllers. It mimics the Xbox home page on a computer, allowing users to launch applications and web URLs using an Xbox controller.

## Purpose

The primary goal of HUB is to give users full control of their PC using an Xbox Series S controller. This aims to eliminate the need for a keyboard, making it easier to control your PC while playing games or relaxing in bed.

## Features

- **Open Applications**: Launch applications like Xbox Game Bar, Steam, and others directly from the HUB.
- **Web URLs**: Open web URLs in default browsers or specific applications.
- **Controller Navigation**: Navigate through the HUB using the Xbox controller's D-pad and select items with the 'A' button.
- **Highlight Selection**: The currently selected button is highlighted and enlarged for better visibility.
- **Customizable**: Easily update paths to executables and URLs.

## Controller Support

- **Xbox Series S**: The application is designed to work with the Xbox Series S controller via Bluetooth.
- **JoyToKey**: Due to challenges in recognizing the Xbox controller's center home button, JoyToKey is used to map the controller buttons for launching the HUB and other functions.

## Setup Instructions

### Running from Visual Studio

1. **Install JoyToKey**: [Download JoyToKey](https://joytokey.net/en/) and set it up to map the Xbox controller buttons to launch the HUB application.
2. **Add Paths**: Update the `exePaths` and `appArguments` arrays in the `Form1` class to specify the paths to your applications and web URLs.
3. **.NET Version**: Ensure you have .NET 6.0 installed as the HUB application is built using .NET 6.0 Windows Forms.
4. **Build and Run**: Build the application using Visual Studio and run it.

### Running the Executable

1. **Download the Executable**: The latest release including the HUB application executable (.exe) is available on the [releases page](URL_TO_RELEASE_PAGE).
2. **Install JoyToKey**: [Download JoyToKey](https://joytokey.net/en/) and configure it to map the Xbox controller buttons to launch the HUB application.
3. **Run the Application**: Simply double-click the downloaded executable to start HUB. You do not need to run it from Visual Studio.

## Usage

- **Navigate**: Use the Xbox controller's D-pad to move left and right through the application buttons.
- **Select**: Press the 'A' button to select the highlighted application or URL.
- **Power Button**: Use the top-left button to turn off the Xbox controller.

## Development Status

The HUB application is currently under construction. Features and functionality are being added incrementally.

## Future Updates

- ** Error handling for opening apps you don´t have or controller not connected.
- **Somehow turning off your controller from it.
- **Steam Big Picture Mode**: Planned enhancements to fully support launching Steam in Big Picture Mode.
- **Web URL Controller Mapping**: Ongoing development to map the controller for better web navigation.
- **Enhanced Controller Support**: Improving recognition and control features for the Xbox controller.
- **More Applications**: Adding support for additional applications and custom URLs.
- **User Interface Enhancements**: Further refinements to the UI for a more polished experience.

## Contact

For issues, suggestions, or contributions, please leave your suggetins at the issues tab.

## License

This project is licensed under the [MIT License](LICENSE).

