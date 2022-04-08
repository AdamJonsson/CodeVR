# CodeVR

CodeVR is an visual programming language implementation inside VR that is based on [Blockly](https://developers.google.com/blockly). It allows you to program javascript inside VR by connecting block together.

<img src="https://raw.githubusercontent.com/AdamJonsson/CodeVR/issue/74-new-readme/Readme/Banner.png">


---

## Setup for Users
* Setup the config to the correct IP adress. See the development setup.
* Build the Unity project to an Oculus Quest headset.
* Start the website, both the backend and frontend.

---

## Setup for Developers

### Requirements
* Enable "Developer Mode" for the headset. Tutorial here: [Setup Developer Mode](https://developer.oculus.com/documentation/native/android/mobile-device-setup/#enable-developer-mode).
* Download Oculus Developer Hub (ODH). [ODH for Windows](https://developer.oculus.com/downloads/package/oculus-developer-hub-win/) or [ODH for Mac](https://developer.oculus.com/downloads/package/oculus-developer-hub-mac/). This allows you to maintain the device, as well as performance profiling and logging.
* Install Unity Hub.
* Install Unity version 2021.2.x. **OBS**, Android build support must be added when asked during the installation!
* Close this project and open the *CodeVR* > *Unity* > *CodeVR* folder with UnityHub.

### Website
#### Setup
* Located the `_config.json` file located in *Website* > *Frontend* > *src* > *_config.json*
* Duplicate the file and rename it to `config.json`.
* Open the file and enter the IP adress of your computer.
* Go to the *Backend* folder and run `npm install`
* Go to the *Frontend* folder and run `npm install`

#### First time Start
* Go to the *Backend* folder and run `npm start`
* Go to the *Frontend* folder and run `npm start`


### Unity
#### Setup
* Go the the `_Config.cs` file located in *Unity* > *CodeVR* > *Assets* > *Settings* > *_Config.cs*.
* Duplicated the file and rename the new file to `Config.cs`.
* Open the file and remove the underscore in the class name.
* Write the IP adress of where the blockly website is hosted. 

#### To make an APK build
* Go to *File* > *Build Settings*.
* Select Android under the *Platform* list.
* Click *Switch Platform*
* Under *Run Device*, select your Oculus Quest. If the device do not appear, check Error Checklist 2).
* Click *Build* or *Build and Run*. If the installation failed, see Error Checklist 1).
* Done

---

### Error Checklist

**1) Failing to install APK to headset**
* Delete other installed apps that you have developed. It will fail if there is another project using the same developer signature. You can do this easly in the headset or using the ODH.
* Check that the ADB in the ODH is the same as the one used with Unity. Otherwise it wont install.

**2) Can not see device in Unity Build Settings**
* Check that the ADB in the ODH is the same as the one used with Unity.
