using System;
using System.Threading.Tasks;
using Discord.Commands;
using Rem.Bot;
using Rem.Attributes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Image = SixLabors.ImageSharp.Image;

namespace Rem.Commands
{
    public class HeatingUpModule : ModuleBase
    {
        private readonly BotState _botState;
        private static Image<Rgba32>[] Digits;
        private static Image<Rgba32> HeatingUpImg;
        
        public HeatingUpModule(BotState state)
        {
            _botState = state;
            if (Digits == null)
            {
                Digits = new Image<Rgba32>[10];
                
                for (var i = 0; i < 10; i++)
                {
                    Digits[i] = Image.Load(@"Images/HeatingUp/" + $"{i}.png");
                }
            }

            if (HeatingUpImg == null)
            {
                HeatingUpImg = Image.Load(@"Images/HeatingUp/heatingup.png");
            }
        }

        [RequireRole("regulars")]
        [Command("heatingup"), Summary("Whoa, this thing's finally heating up for the Xth time.")]
        public async Task HeatingUp()
        {
            _botState.BribeCount += 1;
            using (Context.Channel.EnterTypingState())
            {
                using (var img = GenerateImage(_botState.BribeCount))
                {
                    img.Save("output.png");
                    await Context.Channel.SendFileAsync("output.png");
                }
            }
        }

        [RequireRole("regulars")]
        [Command("coolingdown"), Summary("Whoa, this thing's cooling down.")]
        public async Task CoolingDown()
        {
            _botState.BribeCount -= 1;
            using (Context.Channel.EnterTypingState())
            {
                using (var img = GenerateImage(_botState.BribeCount))
                {
                    img.Mutate(i => i.Rotate(RotateMode.Rotate180));
                    img.Save("output.png");
                    await Context.Channel.SendFileAsync("output.png");
                }
            }
        }

        private Image<Rgba32> GenerateImage(int num)
        {
            if (num >= 100)
            {
                num = 99;
            }
            var tens = num / 10;
            var ones = num % 10;

            var maxWidth = 54;
            var maxHeight = 54;

            var tensImg = Digits[tens];
            var onesImg = Digits[ones];

            var combinedWidth = tensImg.Width + onesImg.Width + 2;
            var scaleFactorWidth = maxWidth / (double)combinedWidth;
            var scaleFactorHeight = maxHeight / (double)Math.Max(tensImg.Height, onesImg.Height);

            var scaleFactor = Math.Min(scaleFactorHeight, scaleFactorWidth);
            var finalHeight = (int)(Math.Max(tensImg.Height, onesImg.Height) * scaleFactor);
            var finalWidth = (int)(combinedWidth * scaleFactor);

            var img = HeatingUpImg.Clone();
            var tensScaled = tensImg.Clone();
            tensScaled.Mutate(i => i.Resize((int)(tensScaled.Width * scaleFactor), (int)(tensScaled.Height * scaleFactor)));
            var onesScaled = onesImg.Clone();
            onesScaled.Mutate(i => i.Resize((int)(onesScaled.Width * scaleFactor), (int)(onesScaled.Height * scaleFactor)));

            var startX = 507 + (maxWidth - finalWidth) / 2;
            var startX2 = 507 + maxWidth - ((maxWidth - finalWidth) / 2 + onesScaled.Width);

            img.Mutate(i => i.DrawImage(tensScaled, new Point(startX, 182 + (maxHeight - tensScaled.Height) / 2), 1));
            img.Mutate(i => i.DrawImage(onesScaled, new Point(startX2, 182 + (maxHeight - onesScaled.Height) / 2), 1));

            tensScaled.Dispose();
            onesScaled.Dispose();

            return img;
        }
    }
}
