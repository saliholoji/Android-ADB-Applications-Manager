# Mobile Application Management Program 📱

Welcome to the Mobile Application Management Program! This tool is written in C# using the .NET Framework and allows you to manage mobile applications on an Android device via a USB connection using ADB Shell.

## Features ✨

- **Application Information**: Retrieve detailed information about installed applications.
- **Battery Information**: Access the current battery status and details.
- **Installed Applications List**: Get a comprehensive list of all installed applications.
- **Application File Paths and Package Information**: View the file paths and package information of each installed application.
- **APK Installation**: Directly install APK files onto the mobile device.
- **Application Uninstallation**: Remove applications either individually or in bulk.

## Getting Started 🚀

To get started with the Mobile Application Management Program, follow these steps:

1. **Clone the Repository**:
    ```bash
    git clone https://github.com/saliholoji/Android-ADB-Applications-Manager.git
    ```

2. **Open the Solution**:
    - Open the project solution file in Visual Studio.

3. **Build the Project**:
    - Build the project to restore the necessary packages and dependencies.

4. **Connect Your Device**:
    - Connect your Android device via USB and ensure USB debugging is enabled.

5. **Run the Program**:
    - Run the application from Visual Studio. The interface will guide you through managing your mobile applications.

## Prerequisites 📋

- **.NET Framework**: Ensure you have the .NET Framework installed on your machine.
- **ADB (Android Debug Bridge)**: ADB should be installed and accessible via your system's PATH.

## USB Debugging is required
In your android device:

Settings >> Developer Options >> USB Debugging

if 'Deveoper Options' not exsists:

Settings >> About Device >> Click on 'Build number' 7 times then 'developer options' will appear!

## Usage 🛠️

- **Viewing Application Information**:
  - Launch the program and connect your device.
  - Navigate to the 'Application Information' tab to see details of all installed apps.

- **Installing an APK**:
  - Go to the 'APK Installation' tab.
  - Select the APK file from your system and click 'Install'.

- **Uninstalling Applications**:
  - Move to the 'Uninstallation' tab.
  - Select the applications you want to remove and click 'Uninstall'.

## Contributing 🤝

Contributions are welcome! Please fork the repository and create a pull request with your changes. Ensure that your code adheres to the project's coding standards and includes appropriate tests.

## Contact 📧

For any inquiries or support, please contact me at [info@salihuysal.com](mailto:info@salihuysal.com).

## License 📄

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

---

Thank you for using the Mobile Application Management Program! If you find this tool useful, please consider giving it a star ⭐ on GitHub.

