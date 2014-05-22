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

namespace App.Core.Utility
{
    public class S3Helper
    {
        private AmazonS3 client;
        private string accessKey { get; set; }
        private string secretKey { get; set; }
        public string cloudfront_cdn { get; set; }
        public string S3_bucket_url { get; set; }
        private string cloudfront_distributionId { get; set; }
        private WebClient webClient = new WebClient();

        public string bucket { get; set; }
        public string env { get; set; }

        public S3Helper()
        {
            var appConfig = ConfigurationManager.AppSettings;
            accessKey = appConfig["S3_accessKey"];
            secretKey = appConfig["S3_secretKey"];
            bucket = appConfig["S3_bucket"];
            env = appConfig["S3_env"];
            cloudfront_cdn = appConfig["cdn_url"];
            cloudfront_distributionId = appConfig["cloudfront_distributionId"];
            S3_bucket_url = appConfig["origin_url"];
            AmazonS3Config config = new AmazonS3Config();
            config.WithServiceURL("s3-ap-southeast-1.amazonaws.com");
            client = Amazon.AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey, config);
        }

        public string Hash(string image, int id = 0)
        {
            //byte[] inputSha256Byte = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            //return BitConverter.ToString(inputSha256Byte).ToLower().Replace("-", String.Empty);
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

        public Image CropImage(Image image, Rectangle rectangle)
        {
            Bitmap bitmap = new Bitmap(image);
            Bitmap croppedBitmap = bitmap.Clone(rectangle, bitmap.PixelFormat);
            bitmap.Dispose();

            return (Image)(croppedBitmap);
        }

        public bool CropImage(string absoluteUri, string destinationS3path, int x1, int x2, int y1, int y2, int width = 680, int height = 400)
        {
            WebImage webImage = new WebImage(webClient.DownloadData(absoluteUri));

            int top = y1;
            int bottom = webImage.Height - y2;
            int left = x1;
            int right = webImage.Width - x2;

            WebImage croppedWebImage = webImage.Crop(top, left, bottom, right);
            croppedWebImage.Resize(width, height);

            //  Obtain byte array of image data
            string[] separators = { "/" };
            string[] filenames = destinationS3path.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            string filename = filenames[filenames.Length - 1];

            byte[] thumbnail = CreateImage(croppedWebImage.GetBytes(webImage.ImageFormat), filename, width, height);
            bool result = UploadToS3(thumbnail, destinationS3path);
            //List<string> photo = new List<string>();
            //photo.Add(destinationS3path);
            //  InvalidateContent(cloudfront_distributionId, photo);
            return result;
        }

        public bool DeletePhoto(string filename)
        {
            bool result = false;
            DeleteObjectRequest request = new DeleteObjectRequest()
            {
                BucketName = bucket,
                Key = env + '/' + filename
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

        public bool UploadToS3(byte[] buffer, string s3Path)
        {
            try
            {
                if (buffer.Length > 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        PutObjectRequest request = new PutObjectRequest();
                        request.InputStream = memoryStream;
                        request.BucketName = bucket;
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

        public bool UploadToS3(string localPath, string s3Path)
        {
            PutObjectRequest request = new PutObjectRequest();

            Stream fileStream = (new FileStream((localPath), FileMode.Open));

            //eg test/ + 'MY/propertyshowcase/1010/test.jpg'
            s3Path = env + "/" + s3Path;

            if (fileStream.Length > 0)
            {
                var fileStreamSize = fileStream.Length;
                request.WithAutoCloseStream(true);
                request.WithInputStream(fileStream);

                request.WithBucketName(bucket);
                request.WithKey(s3Path);
                request.AddHeaders(Amazon.S3.Util.AmazonS3Util.CreateHeaderEntry("Cache-Control", "max-age=31536000"));
                request.AddHeaders(Amazon.S3.Util.AmazonS3Util.CreateHeaderEntry("Expires", "Tue, 01 Jan 2030 03:54:42 GMT"));
                client.PutObject(request);

                SetACLRequest aclRequest = new SetACLRequest();
                aclRequest.Key = s3Path;
                aclRequest.BucketName = bucket;
                aclRequest.CannedACL = S3CannedACL.PublicRead;
                SetACLResponse aclResponse = client.SetACL(aclRequest);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InvalidateContent(string distributionId, List<string> files)
        {
            AmazonCloudFrontClient cloudclient = new AmazonCloudFrontClient(accessKey, secretKey);

            PostInvalidationRequest request = new PostInvalidationRequest();
            InvalidationBatch batch = new InvalidationBatch();

            batch.Paths = files;
            batch.CallerReference = System.DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss ", System.Globalization.CultureInfo.InvariantCulture) + "GMT";
            request.InvalidationBatch = batch;
            request.DistributionId = distributionId;

            PostInvalidationResponse response = cloudclient.PostInvalidation(request);
            if (!String.IsNullOrEmpty(response.RequestId))
            {
                bool bInProgress = true;
                while (bInProgress)
                {
                    GetInvalidationRequest getReq = new GetInvalidationRequest();
                    getReq.DistributionId = distributionId;
                    getReq.InvalidationId = response.Id;

                    GetInvalidationResponse getRes = cloudclient.GetInvalidation(getReq);
                    bInProgress = getRes.Status.Equals("InProgress");

                    if (!bInProgress)
                    {
                        return true;
                        Thread.Sleep(28800);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }

            return true;
        }

        // Create a thumbnail in byte array format from the image encoded in the passed byte array.  
        // (RESIZE an image in a byte[] variable.)  
        public byte[] CreateImage(byte[] PassedImage, string filename, int Width = 640, int Height = 400, int quality = 85)
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
    }
}
