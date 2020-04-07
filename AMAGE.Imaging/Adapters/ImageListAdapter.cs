using AMAGE.Common.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System;
using System.Drawing;
using Gif.Components;
using AMAGE.Imaging.Properties;

namespace AMAGE.Imaging.Adapters
{
    public class ImageListAdapter : List<IImage>, IImageList
    {
        public void FromFile(string fileName)
        {
            Stream input = File.OpenRead(fileName);
            FromStream(input);
        }

        public void ToFile(string fileName, string format)
        {
            using (Stream output = File.OpenWrite(fileName))
                ToStream(output, format);
        }

        public void FromStream(Stream input)
        {
            BitmapDecoder decoder = DecoderSelector.GetDecoder(input, BitmapCreateOptions.None,
                BitmapCacheOption.None);

            foreach (BitmapFrame frame in decoder.Frames)
            {
                IImage image = Image.Create();
                image.FromBitmapSource(frame);


                BitmapMetadata metadata = frame.Metadata as BitmapMetadata;

                if (metadata?.ContainsQuery("/grctlext/Delay") == true)
                {
                    object delay = metadata.GetQuery("/grctlext/Delay");
                    image.AnimationDelay = (int)(Convert.ToDouble(delay) * 10.0);
                }

                Add(image);
            }

            Coalesce();
        }

        public unsafe void ToStream(Stream output, string format)
        {
            format = format.ToLower().Replace(".", "");

            if (format != "gif") // TODO: Can't set delay to BitmapEncoder
            {
                BitmapEncoder encoder = EncoderSelector.GetEncoder(format);

                foreach (IImage image in this)
                {
                    BitmapSource bitmapSource = image.ToBitmapSource();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                }

                encoder.Save(output);
            }
            else
            {
                AnimatedGifEncoder encoder = new AnimatedGifEncoder();
                encoder.SetQuality(100);
                encoder.SetRepeat(0);

                encoder.Start(output);

                foreach (IImage image in this)
                {
                    IntPtr ptr = (IntPtr)image.BeginPixelWorking(false);

                    Bitmap frame = new Bitmap(image.Width, image.Height, image.Width * 4,
                        System.Drawing.Imaging.PixelFormat.Format32bppPArgb, ptr);

                    image.EndPixelWorking();

                    encoder.SetDelay(image.AnimationDelay);
                    encoder.AddFrame(frame);

                    frame.Dispose();
                }

                encoder.Finish();
            }
        }

        public void FromBitmapSources(BitmapSource[] images)
        {
            while (Count < images.Length)
                RemoveAt(0);

            while (Count < images.Length)
                Add(Image.Create());

            for (int i = 0; i < images.Length; ++i)
                this[i].FromBitmapSource(images[i]);
        }
        public BitmapSource[] ToBitmapSources()
        {
            return this.Select(i => i.ToBitmapSource())
                .ToArray();
        }

        public IImageList Clone()
        {
            IImageList imageList = ImageList.Create();

            foreach (IImage image in this)
                imageList.Add(image.Clone());

            return imageList;
        }

        public void CloneTo(IImageList output)
        {
            while (output.Count < Count)
                output.Add(Image.Create());

            while (output.Count > Count)
                output.RemoveAt(0);

            for (int i = 0; i < Count; ++i)
                this[i].CloneTo(output[i]);
        }

        private unsafe void Coalesce()
        {
            if (Count > 1)
            {

                for (int i = 1; i < Count; ++i)
                {
                    IImage first = this[i - 1];

                    int* input = first.BeginPixelWorking(false);
                    int* output = this[i].BeginPixelWorking(true);

                    int startX = (first.Width - this[i].Width) / 2;
                    int startY = (first.Height - this[i].Height) / 2;

                    int width = Math.Min(this[i].Width, first.Width);
                    int height = Math.Min(this[i].Height, first.Height);

                    if (startX < 0 || startY < 0)
                        throw new NotSupportedException(Resources.NotSupportedGif);

                    for (int y = startY; y < height; ++y)
                        for (int x = startX; x < width; ++x)
                        {
                            int p = y * width + x;

                            if ((output[p] >> 24) == 0)
                                output[p] = input[p];
                        }

                    this[i].EndPixelWorking();
                    first.EndPixelWorking();
                }

            }
        }
    }
}

