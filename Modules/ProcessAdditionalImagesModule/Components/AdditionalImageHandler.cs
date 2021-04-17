namespace Hotcakes.Modules.ProcessAdditionalImagesModule.Components
{
    using Hotcakes.Commerce;
    using Hotcakes.Commerce.Catalog;
    using Hotcakes.Commerce.Storage;
    using Hotcakes.Commerce.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    public static class AdditionalImageHandler
    {
        public static bool MoveToProcessedFolder(FileInfo file)
        {
            try
            {
                var processed = Path.Combine(file.Directory.FullName, "processed");

                if (!Directory.Exists(processed))
                {
                    Directory.CreateDirectory(processed);
                }

                var newPath = Path.Combine(processed, file.Name);

                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }

                File.Move(file.FullName, newPath);

                return File.Exists(newPath);
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        /// <summary>
        /// Write file to path
        /// </summary>
        /// <param name="saveLocation"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool WriteFileToPath(string saveLocation, FileStream stream)
        {
            var result = true;

            try
            {
                if (stream != null)
                {
                    bool createDirectory = !Directory.Exists(Path.GetDirectoryName(saveLocation));

                    if (createDirectory)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));
                    }

                    using (var fileStream = File.Create(saveLocation, (int)stream.Length))
                    {
                        var array = new byte[stream.Length];
                        stream.Read(array, 0, array.Length);
                        fileStream.Write(array, 0, array.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Upload additional product image
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="productId"></param>
        /// <param name="imageId"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool UploadAdditionalProductImage(long storeId, string productId, string imageId, string filePath, string fileName, FileStream imageFile = null)
        {
            var isValidFile = DiskStorage.ValidateImageType(Path.GetExtension(fileName));
            var result = false;

            if (isValidFile)
            {
                var subpath = string.Format("products/{0}/additional/{1}/{2}", productId, imageId, CleanFileName(Path.GetFileName(fileName)));

                var storeDataPhysicalPath = DiskStorage.GetStoreDataPhysicalPath(storeId, subpath);
                var fileAlreadyExists = File.Exists(storeDataPhysicalPath);

                if (fileAlreadyExists)
                {
                    File.SetAttributes(storeDataPhysicalPath, FileAttributes.Normal);
                    File.Delete(storeDataPhysicalPath);
                }

                bool fileCreated = false;

                if (imageFile == null)
                {
                    using (imageFile = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        fileCreated = WriteFileToPath(storeDataPhysicalPath, imageFile);
                    }
                }
                else
                {
                    fileCreated = WriteFileToPath(storeDataPhysicalPath, imageFile);
                }

                if (fileCreated)
                {
                    fileCreated = ImageProcessing.ShrinkToTiny(storeDataPhysicalPath);

                    if (fileCreated)
                    {
                        fileCreated = ImageProcessing.ShrinkToSmall(storeDataPhysicalPath);

                        if (fileCreated)
                        {
                            fileCreated = ImageProcessing.ShrinkToMedium(storeDataPhysicalPath);
                        }
                    }
                }

                result = fileCreated;
            }
            return result;
        }

        /// <summary>
        /// Clean file name
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanFileName(string input)
        {
            string text = input.Replace(" ", "-");
            text = text.Replace("\"", string.Empty);
            text = text.Replace("&", "and");
            text = text.Replace("?", string.Empty);
            text = text.Replace("=", string.Empty);
            text = text.Replace("/", string.Empty);
            text = text.Replace("\\", string.Empty);
            text = text.Replace("%", string.Empty);
            text = text.Replace("#", string.Empty);
            text = text.Replace("*", string.Empty);
            text = text.Replace("!", string.Empty);
            text = text.Replace("$", string.Empty);
            text = text.Replace("+", "-plus-");
            text = text.Replace(",", "-");
            text = text.Replace("@", "-at-");
            text = text.Replace(":", "-");
            text = text.Replace(";", "-");
            text = text.Replace(">", string.Empty);
            text = text.Replace("<", string.Empty);
            text = text.Replace("{", string.Empty);
            text = text.Replace("}", string.Empty);
            text = text.Replace("~", string.Empty);
            text = text.Replace("|", "-");
            text = text.Replace("^", string.Empty);
            text = text.Replace("[", string.Empty);
            text = text.Replace("]", string.Empty);
            text = text.Replace("`", string.Empty);
            text = text.Replace("'", string.Empty);
            text = text.Replace("©", string.Empty);
            text = text.Replace("™", string.Empty);
            return text.Replace("®", string.Empty);
        }

        /// <summary>
        /// Initialize product file
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ProductFile InitializeProductFile(string productId, string fileName)
        {
            var file = new ProductFile
            {
                FileName = Path.GetFileName(fileName),
                ProductId = productId,
                MaxDownloads = 0
            };

            file.ShortDescription = file.FileName;

            return file;
        }
    }
}