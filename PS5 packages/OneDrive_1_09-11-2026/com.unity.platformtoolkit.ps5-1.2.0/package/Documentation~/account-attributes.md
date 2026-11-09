# PS5 account attributes reference

Understand the attributes available on PlayStation®5 (PS5) accounts when using the Platform Toolkit package.

## Usage
You can use `HasAttribute` and `GetAttribute` on any IAccount object to make sure the attribute exists for the platform.

## Attributes available on PS5

| **Attribute name**   | **Attribute ID** | **Type** | **Exceptions thrown** | **Description** |
| ---------------- | ---- | ---- | ----------- |---|
| **UserId**           | `UserId` | int  | TemporarilyUnavailableException |The SceUserServiceUserId. For more information, refer to the PlayStation®5 DevNet [SceUserServiceUserId](https://game.develop.playstation.net/resources/documents/SDK/latest/UserService-Reference/sce-user-service-user-id.html) documentation. |
| **OnlineId**       | `OnlineId` | string |TemporarilyUnavailableException |The SceNpOnlineId. For more information, refer to the PlayStation®5 DevNet [SceNpOnlineId](https://game.develop.playstation.net/resources/documents/SDK/latest/Np-Reference/sce-np-online-id.html) documentation. |
| **First Name**        | `FirstName` | string |TemporarilyUnavailableException |The user's first name as a UTF-8 string. For more information, refer to the PlayStation®5 DevNet [GetPublicProfiles](https://game.develop.playstation.net/resources/documents/WebAPI/1/User_Profile_WebAPI-Reference/0007.html) documentation. |
| **Middle Name**       | `MiddleName` | string | TemporarilyUnavailableException |The user's middle name as a UTF-8 string. For more information, refer to the PlayStation®5 DevNet [GetPublicProfiles](https://game.develop.playstation.net/resources/documents/WebAPI/1/User_Profile_WebAPI-Reference/0007.html) documentation. <br /> <br /> **Note**: **MiddleName** might return null if the user doesn't have a middle name set at the system level. |
| **Last Name**         | `LastName` | string | TemporarilyUnavailableException|The user's last name as a UTF-8 string. For more information, refer to the PlayStation®5 DevNet [GetPublicProfiles](https://game.develop.playstation.net/resources/documents/WebAPI/1/User_Profile_WebAPI-Reference/0007.html) documentation. |
| **Small Avatar**      | `SmallAvatar` | Texture2D |TemporarilyUnavailableException |The small dimension avatar image of the player, referred to as Master Avatar 6. For more information, refer to the PlayStation®5 DevNet [PlayStation Learn Avatar](https://learn.playstation.net/bundle/content-pipeline/page/Product_Avatar.html) documentation. |
| **Medium Avatar**     | `MediumAvatar` | Texture2D |TemporarilyUnavailableException | The medium dimension avatar image of the player, referred to as Master Avatar 4. For more information, refer to the PlayStation®5 DevNet [PlayStation Learn Avatar](https://learn.playstation.net/bundle/content-pipeline/page/Product_Avatar.html) documentation. |
| **Large Avatar**      | `LargeAvatar` | Texture2D |TemporarilyUnavailableException |The large dimension avatar image of the player, referred to as Master Avatar 3. For more information, refer to the PlayStation®5 DevNet [PlayStation Learn Avatar](https://learn.playstation.net/bundle/content-pipeline/page/Product_Avatar.html) documentation. |
| **Extra Large Avatar** | `ExtraLargeAvatar` | Texture2D |TemporarilyUnavailableException |The extra large dimension avatar image of the player, referred to as Master Avatar 2. For more information, refer to the PlayStation®5 DevNet [PlayStation Learn Avatar](https://learn.playstation.net/bundle/content-pipeline/page/Product_Avatar.html) documentation. |

> [!Note]
> For documentation on how to download different Avatar sizes, refer to [GetPublicProfiles](https://game.develop.playstation.net/resources/documents/WebAPI/1/User_Profile_WebAPI-Reference/0007.html).
