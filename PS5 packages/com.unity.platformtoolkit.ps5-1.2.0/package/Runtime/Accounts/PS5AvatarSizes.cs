using System.Linq;
using static Unity.PSN.PS5.Users.UserSystem;

namespace Unity.PlatformToolkit.PS5
{
    static internal class PS5AvatarSizes
    {
        struct Size
        {
            internal int Width;
            internal int Height;
            public ProfilePicture.Sizes ProfilePictureSize { get; internal set; }
            public int PixelSize => Width * Height;
        }

        // https://learn.playstation.net/bundle/content-pipeline/page/Product_Avatar.html
        // Master Avatar 6 = S, Master Avatar 4 = M, Master Avatar 3 = L, Master Avatar 2 = XL.
        private static readonly Size Small = new Size { Width = 50, Height = 50, ProfilePictureSize = ProfilePicture.Sizes.Small };
        private static readonly Size Medium = new Size { Width = 160, Height = 160, ProfilePictureSize = ProfilePicture.Sizes.Medium };
        private static readonly Size Large = new Size { Width = 240, Height = 240, ProfilePictureSize = ProfilePicture.Sizes.Large };
        private static readonly Size ExtraLarge = new Size { Width = 440, Height = 440, ProfilePictureSize = ProfilePicture.Sizes.ExtraLarge };

        private static readonly Size[] Sizes = { Small, Medium, Large, ExtraLarge };

        private static ProfilePicture.Sizes GetSize(float targetPixelSize)
        {
            for (int i = Sizes.Length - 1; i >= 0; --i)
            {
                float pixelSize = Sizes[i].PixelSize;
                if (pixelSize <= targetPixelSize)
                {
                    return Sizes[i].ProfilePictureSize;
                }
            }
            return ProfilePicture.Sizes.Small;
        }

        static internal ProfilePicture GetProfilePicture(float targetPixelSize, UserProfile userProfile)
        {
            var requestSize = GetSize(targetPixelSize);
            return userProfile.Avatars.FirstOrDefault(x => x.Size == requestSize);
        }
    }

}
