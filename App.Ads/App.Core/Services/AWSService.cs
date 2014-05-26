using Amazon.S3;
using Amazon.S3.Model;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.Util;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Web.Helpers;
using ImageResizer;
using App.Core.Models;
using App.Core.Services;
using App.Core.Utility;

namespace App.Core.Services
{
    public interface IAWSService
    {
        bool DeletePhoto(string fileName);
        byte[] CreateImage(byte[] PassedImage, string filename, int Width = 640, int Height = 400, int quality = 85);
        bool UploadToS3(byte[] buffer, string s3Path);
        string Hash(string image, int id = 0);
    }

    public class AWSService : IAWSService
    {
        private AmazonS3 client;
        private WebClient webClient = new WebClient();

        private IConfigService configService;
        private S3AssetsModel assets;

        public AWSService(IConfigService configService)
        {
            this.configService = configService;

            assets = new S3AssetsModel
            {
                S3AccessKey = this.configService.GetValue(ConfigName.S3AccessKey),
                S3SecretKey = this.configService.GetValue(ConfigName.S3SecretKey),
                S3Bucket = this.configService.GetValue(ConfigName.S3Bucket),
                S3Env = this.configService.GetValue(ConfigName.S3Env),
                S3Url = this.configService.GetValue(ConfigName.S3Url),
                S3ServiceUrl = this.configService.GetValue(ConfigName.S3ServiceUrl)
            };

            AmazonS3Config config = new AmazonS3Config();
            config.WithServiceURL(assets.S3ServiceUrl);

            client = Amazon.AWSClientFactory.CreateAmazonS3Client(assets.S3AccessKey, assets.S3SecretKey, config);
        }

        string IAWSService.Hash(string image, int id = 0)
        {
            var fileArr = image.Split('.');
            var fileName = fileArr[0].Truncate(15);
            var fileExt = fileArr[fileArr.Length - 1];

            char[] chars = new char[10];
            chars =
            "1234567890".ToCharArray();
            int maxSize = 6;
            byte[] data = new byte[maxSize];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();

            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder randomString = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                randomString.Append(chars[b % (chars.Length)]);
            }

            var newString = randomString.ToString();
            var epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            //var result = id + "_" + newString + "_" + epoch + "." + fileExt;
            //DateTime.Now.ToString("yyyymmdd")
            var result = newString + "-" + fileName + "." + fileExt;

            return result;
        }

        bool IAWSService.DeletePhoto(string fileName)
        {
            bool result = false;
            DeleteObjectRequest request = new DeleteObjectRequest()
            {
                BucketName = assets.S3Bucket,
                Key = assets.S3Env + '/' + fileName
            };

            try
            {
                S3Response response = client.DeleteObject(request);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        byte[] IAWSService.CreateImage(byte[] PassedImage, string filename, int Width = 640, int Height = 400, int quality = 85)
        {
            byte[] ReturnedThumbnail;

            using (MemoryStream StartMemoryStream = new MemoryStream(),
                                NewMemoryStream = new MemoryStream())
            {
                // write the string to the stream  
                StartMemoryStream.Write(PassedImage, 0, PassedImage.Length);

                // create the start Bitmap from the MemoryStream that contains the image  
                Bitmap startBitmap = new Bitmap(StartMemoryStream);

                // set thumbnail height and width proportional to the original image.  
                int newHeight;
                int newWidth;
                double HW_ratio;
                HW_ratio = 1.8823;

                if (startBitmap.Height > Height || (startBitmap.Width < Width && startBitmap.Height < Height))
                {
                    // newHeight = LargestSide;
                    // HW_ratio = (double)((double)LargestSide / (double)startBitmap.Height);
                    // newWidth = (int)(HW_ratio * (double)startBitmap.Width);
                    newHeight = Height;
                    HW_ratio = (double)((double)Height / (double)startBitmap.Height);
                    newWidth = (int)(HW_ratio * (double)startBitmap.Width);
                }
                else
                {
                    //  newWidth = LargestSide;
                    // HW_ratio = (double)((double)LargestSide / (double)startBitmap.Width);
                    //  newHeight = (int)(HW_ratio * (double)startBitmap.Height);
                    newWidth = Width;
                    HW_ratio = (double)((double)Width / (double)startBitmap.Width);
                    newHeight = (int)(HW_ratio * (double)startBitmap.Height);
                }

                ImageFormat imageExtension = GetImageFormat(filename);
                // create a new Bitmap with dimensions for the thumbnail.  
                Bitmap newBitmap = new Bitmap(startBitmap.Width, startBitmap.Height);

                // Copy the image from the START Bitmap into the NEW Bitmap.  
                // This will create a thumnail size of the same image.i
                newBitmap = ResizeImage(startBitmap, newWidth, newHeight);

                //Reduce quality of image to reduce file upload/download size
                newBitmap.SetResolution(quality, quality); // OR 70,70

                // Save this image to the specified stream in the specified format.  
                //newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                newBitmap.Save(NewMemoryStream, imageExtension);

                // Fill the byte[] for the thumbnail from the new MemoryStream.  
                ReturnedThumbnail = NewMemoryStream.ToArray();
            }

            // return the resized image as a string of bytes.  
            return ReturnedThumbnail;
        }

        bool IAWSService.UploadToS3(byte[] buffer, string s3Path)
        {
            try
            {
                if (buffer.Length > 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        PutObjectRequest request = new PutObjectRequest();
                        request.InputStream = memoryStream;
                        request.BucketName = assets.S3Bucket;
                        request.AddHeader("x-amz-acl", "public-read");
                        request.AddHeader("Cache-Control", "max-age=31536000");
                        request.AddHeader("Expires", "Tue, 01 Jan 2030 03:54:42 GMT");

                        request.Key = s3Path;

                        client.PutObject(request).Headers.Add("Content-Length", buffer.Length.ToString());
                        memoryStream.Close();
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex) { throw ex; }
        }

        #region Helper

        private ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));

            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    throw new NotImplementedException();
            }
        }

        // Resize a Bitmap  
        private Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        }

        #endregion

    }

}
