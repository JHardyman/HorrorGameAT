# Get started with Platform Toolkit for PS5

Use the following steps to set up your Unity project for PlayStation®5 (PS5) with the Platform Toolkit package.

> [!NOTE]
> At the time of release, Platform Toolkit was tested for compatibility with SDK 11.000.

## 1. Install the required packages

Install the following Platform Toolkit packages. For information on installing packages with Unity, refer to [Package management with the Package Manager window](https://docs.unity3d.com/Manual/managing-packages-window.html).

- com.unity.platformtoolkit
- com.unity.platformtoolkit.ps5

The following Unity PS5 packages are also required:

- com.unity.commondialog.ps5
- com.unity.psn.ps5
- com.unity.savedata.ps5 (version 1.4)
- com.unity.inputsystem.ps5

> [!NOTE]
> Version 2.0+ of the com.unity.savedata.ps5 package isn't compatible with Platform Toolkit. Ensure you have version 1.4 installed to avoid compatibility issues.

You can download these PS5 packages from the [PlayStation 5 Package Downloads](https://discussions.unity.com/t/playstation-5-package-downloads/1568720) Unity Discussions page.

## 2. Configure trophies

Use the following steps to configure trophies for your PS5 title.

> [!NOTE]
> When handling trophies on PS5, ensure you follow the [R5013 Trophy](https://game.develop.playstation.net/resources/documents/TRC/latest/TRC/R5013.html) TRC requirements and the provided [test case](https://game.develop.playstation.net/resources/documents/TRC/latest/TRC/R5013-testcase.html).

1. Set up trophies for your title with the [UDS Management Tool](https://game.develop.playstation.net/resources/documents/SDK/latest/Universal_Data_System-Guide/using-the-uds-management-tool.html). Access the UDS Management Tool from the PlayStation Partner Center under **UDS Management** > **Features** > **Trophies** or from your Products DevNet page.

> [!NOTE]
> Platform Toolkit only supports title managed achievements.

2. Open the Achievement Editor. In the Unity Editor, navigate to **Window** > **Platform Toolkit** > **Achievement Editor**.

3. In the PlayStation 5 column of the Achievement Editor, add achievement IDs that match the Trophy IDs in the PlayStation Partner Center. You can find IDs in the UDS Management Tool. Access the UDS Management Tool from the PlayStation Partner Center under **UDS Management** > **Features** > **Trophies** or from your Products DevNet page.

## 3. Download PlayStation services configuration files for your title

1. Download the `npconfig.zip` file from the **Package/Disc Management Tool (GEMS)** and select **Download npconfig.zip**. Access the **Package/Disc Management Tool (GEMS)** from the [PlayStation Partners Hub](https://partners.playstation.net/hub) or via your products PS5 DevNet Page.
2. In the Unity Editor, navigate to **Edit** > **Project Settings** > **Player**
3. Select **Package metadata file (npconfig.zip)** and choose your recently downloaded npconfig.zip file.

> [!NOTE]
> After every change in the PlayStation Partner Center for Trophies, you must repeat these steps to update the Trophy data set in Unity.

## 4. Set an allocated save size value

To set an allocated save size value in the Unity Editor to use the saving system API, use the following steps:

1. In the Unity Editor, navigate to **Edit** > **Project Settings** > **Platform Toolkit** > **PlayStation 5**.
2. Set the **Allocated Save Size** value. The default value is 3072.

> [!NOTE]
> When configuring save information, you must adhere to the following TRC requirements:
> - [R5021 Save Date - Online Storage Support](https://game.develop.playstation.net/resources/documents/TRC/latest/TRC/R5021.html)
> - [R5100 Save Data - Maximum Total Size per User](https://game.develop.playstation.net/resources/documents/TRC/latest/TRC/R5100.html)
> - [R5096 Error Handling in Saving/Loading Save Data, Images, and Videos](https://game.develop.playstation.net/resources/documents/TRC/latest/TRC/R5096.html)

For more information on PS5 save data and limits, refer to [SaveData Library Overview](https://game.develop.playstation.net/resources/documents/SDK/latest/SaveData-Overview/library-overview.html)

## Additional resources

* [Platform-specific reference](platform-details.md)