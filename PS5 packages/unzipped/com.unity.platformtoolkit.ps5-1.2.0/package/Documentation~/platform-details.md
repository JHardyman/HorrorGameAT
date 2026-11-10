# Platform-specific reference

Details behavior and API usage unique to the Platform Toolkit for PlayStation®5 (PS5) package.

> [!Note]
> Refer to [Get started with Platform Toolkit for PS5](get-started-with-pt-for-ps5.md) for detailed setup instructions.

## PS5 account behavior

- As the `PS5Account` class can retrieve information that requires an account set up with PSN, and sometimes an active connection, it might throw an error when retrieving account attributes.
- A `PSNException` will be thrown if the user isn't currently online.

## InitialUserAlwaysLoggedIn

To access a primary account, you must set **InitialUserAlwaysLoggedIn** to true in the `param.json` file. If this setting is false, access to `IPrimaryAccountSystem` and its functionality isn't supported. If you attempt to access `IPrimaryAccountSystem`, a `NotSupportedException` is thrown. This occurs as the concept of a primary account doesn't exist on PS5 when **InitialUserAlwaysLoggedIn** is set to false.

## Achievements

The following considerations apply when adding achievements on Platform Toolkit with PlayStation®5 (PS5):

* If the achievement type is **single** in the Platform Toolkit Achievement Editor, it must be set as **Binary** on PS5.
* If the achievement type is **progressive** in the Platform Toolkit Achievement Editor, it must be set as **progressive** on PS5.
* For progressive achievements, the progress target must match between Platform Toolkit and PS5.

## PS5 save data committing

If a save data commit on PS5 throws an exception, the system reverts the save data. This process might leave a recently created save empty. Platform Toolkit considers empty saves invalid, and loading an empty save throws a `CorruptedSaveException`.

## Additional resources

* [Get started with Platform Toolkit for PS5](get-started-with-pt-for-ps5.md)