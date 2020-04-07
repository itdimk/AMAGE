using AMAGE.Common.Imaging;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AMAGE.Imaging.Adapters
{
    internal class ImageAdapter : IImage
    {
        internal WriteableBitmap Target;
        private BitmapContext? pixelWorking;

        public int AnimationDelay { get; set; }

        public int Height => Target.PixelHeight;
        public int Width => Target.PixelWidth;
        public int PixelCount => Width * Height;

        internal ImageAdapter()
        {
            Target = BitmapFactory.New(1, 1);
        }

        public void FromFile(string fileName)
        {
            Stream input = File.OpenRead(fileName);
            FromStream(input);
        }

        public void ToFile(string fileName, string format, int qualityPercent = 100)
        {
            using (Stream output = File.OpenWrite(fileName))
                ToStream(output, format);
        }

        public void FromStream(Stream input)
        {
            BitmapDecoder decoder = DecoderSelector.GetDecoder(input, BitmapCreateOptions.None,
                BitmapCacheOption.None);

            FromBitmapSource(decoder.Frames[0]);
        }

        public void ToStream(Stream output, string format, int qualityPercent = 100)
        {
            BitmapEncoder encoder = EncoderSelector.GetEncoder(format);
            JpegBitmapEncoder jpegEncoder = encoder as JpegBitmapEncoder;

            if (jpegEncoder != null)
                jpegEncoder.QualityLevel = qualityPercent;

            else if(qualityPercent != 100)
                throw new ArgumentException("Can't change quality for this format");

            encoder.Frames.Add(BitmapFrame.Create(Target));
            encoder.Save(output);
        }

        public void FromBitmapSource(BitmapSource image)
        {
            if (image.Format != PixelFormats.Bgra32)
                Target = BitmapFactory.ConvertToPbgra32Format(image);
            else
                Target = new WriteableBitmap(image);
        }

        public BitmapSource ToBitmapSource()
        {
            return Target;
        }

        public unsafe int* BeginPixelWorking(bool writable)
        {
            EndPixelWorking();
            pixelWorking = Target.GetBitmapContext(ReadWriteMode.ReadWrite);
            return pixelWorking.Value.Pixels;
        }

        public void EndPixelWorking()
        {
            if (pixelWorking != null)
            {
                pixelWorking?.Dispose();
                pixelWorking = null;
            }
        }

        public IImage Clone()
        {
            if (pixelWorking == null)
            {
                ImageAdapter adapter = new ImageAdapter();
                adapter.FromBitmapSource(Target.Clone());
                adapter.AnimationDelay = this.AnimationDelay;
                return adapter;
            }
            else
                throw new InvalidOperationException("Pixel working is active. Can't clone");
        }

        public unsafe void CloneTo(IImage output)
        {
            if (pixelWorking == null)
            {
                if (output.Width == Width && output.Height == Height)
                {
                    int* inputPtr = BeginPixelWorking(false);
                    int* outputPtr = output.BeginPixelWorking(true);
                    int pixCount = PixelCount;

                    for (int i = 0; i < pixCount; ++i)
                        outputPtr[i] = inputPtr[i];

                    EndPixelWorking();
                    output.EndPixelWorking();

                    output.AnimationDelay = AnimationDelay;
                }
                else
                {
                    output.FromBitmapSource(Target.Clone());
                    output.AnimationDelay = this.AnimationDelay;
                }
            }
            else
                throw new InvalidOperationException("Pixel working is active. Can't clone");
        }
    }
}
