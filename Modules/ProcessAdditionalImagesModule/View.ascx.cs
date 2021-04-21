/*
' Copyright (c) 2020 Upendo Ventures, LLC
'  All rights reserved.
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy 
' of this software and associated documentation files (the "Software"), to deal 
' in the Software without restriction, including without limitation the rights 
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
' copies of the Software, and to permit persons to whom the Software is 
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all 
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
' SOFTWARE.
*/

namespace Hotcakes.Modules.ProcessAdditionalImagesModule
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using Components;
    using DotNetNuke.Instrumentation;
    using DotNetNuke.Services.Exceptions;
    using Hotcakes.Commerce;
    using Hotcakes.Commerce.Catalog;
    using Hotcakes.Commerce.Extensions;
    using Hotcakes.Commerce.Storage;
    using Hotcakes.Commerce.Utilities;

    public partial class View : ProcessAdditionalImagesModuleBase
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(View));

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack) BindData();
            }
            catch (Exception exc)
            {
                // Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void btnProcessAdditionalImages_OnClick(object sender, EventArgs e)
        {
            ProcessImages(true);
        }

        protected void lnkProcessImages_OnClick(object sender, EventArgs e)
        {
            ProcessImages();
        }
        #endregion

        #region Helper Methods
        private void BindData()
        {
            LocalizeModule();
        }

        private void LocalizeModule()
        {
            btnProcessImages.Text = GetLocalizedString("btnProcessImages");
            btnProcessAdditionalImages.Text = GetLocalizedString("btnProcessAdditionalImages");
        }

        private void ProcessImages(bool isAdditional = false)
        {
            Logger.Debug("Running the Hotcakes Commerce product additional image import module.");
            var prefix = isAdditional ? "Additional" : string.Empty;
            var context = HccRequestContext.Current;
            var app = HotcakesApplication.Current;
            var sb = new StringBuilder();

            var folderPath = Convert.ToString(this.Settings[Constants.SETTINGS_DOWNLOADS_PATH]);
            folderPath = string.IsNullOrEmpty(folderPath) ? string.Concat(DiskStorage.GetStoreDataVirtualPath(context.CurrentStore.Id), Constants.IMPORT_DOWNLOAD_FOLDER_PATH) : folderPath;
            var downloadsFolderPath = folderPath;

            if (isAdditional)
            {
                folderPath = Convert.ToString(this.Settings[Constants.SETTINGS_ADDITIONAL_PATH]);
                folderPath = string.IsNullOrEmpty(folderPath) ? string.Concat(DiskStorage.GetStoreDataVirtualPath(context.CurrentStore.Id), Constants.IMPORT_ADDITIIONAL_FOLDER_PATH) : folderPath;
                downloadsFolderPath = folderPath;
            }

            // create a repo to get the catalog data
            var repoProduct = new ProductRepository(context);

            // get a collection of the products in the store
            var products = repoProduct.FindAllPagedWithCache(1, int.MaxValue);

            try
            {
                // a title simply to give context of the text to follow
                sb.Append("<blockquote style=\"background-color: #f1f1f1; padding: 1.5em;\">");
                sb.Append($"<h2>{GetLocalizedString($"Process{prefix}Title")}</h2>");

                if (!Directory.Exists(Server.MapPath(downloadsFolderPath)))
                {
                    // NEW Resource
                    sb.Append($"<p>{GetLocalizedString("ImportFolderNotFound")}</p>");
                }
                else if (products != null && products.Count > 0)
                {
                    var files = Directory.GetFiles(Server.MapPath(downloadsFolderPath));

                    // display where the module is looking for images
                    sb.Append($"<p><strong>{GetLocalizedString("ImportPathLabel")}:</strong> {downloadsFolderPath}</p>");
                    sb.Append($"<p><strong>Files Found:</strong> {files.Length}</p>");

                    foreach (var file in files)
                    {
                        var path = file;
                        var fileInfo = new FileInfo(file);
                        var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        var productName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        var extension = Path.GetExtension(fileInfo.Name);
                        var isValidExtension = DiskStorage.ValidateImageType(extension);
                        var product = products.Where(x => x.ProductName == productName).FirstOrDefault();

                        if (product == null)
                        {
                            product = products.Where(x => productName.StartsWith(x.ProductName)).FirstOrDefault();
                        }

                        sb.Append($"<hr/><h3>Importing {prefix}({fileInfo.Name})</h3>");

                        if (!isValidExtension)
                        {
                            sb.Append($"<p style=\"color: #ff0000\">{fileInfo.Name} is not a valid file.</p>");
                        }
                        else if (product == null)
                        {
                            sb.Append($"<p style=\"color: #ff0000\">Could not find matching product for ({fileInfo.Name})");
                        }
                        else
                        {
                            sb.Append($"<p>... {prefix} File ({fileInfo.Name} - Product {productName} {product.Bvin} )</p>");
                            var fileSaved = false;
                            var created = false;

                            using (var imageFile = File.Open(path, FileMode.Open, FileAccess.Read))
                            {
                                try
                                {
                                    if (isAdditional)
                                    {
                                        fileName = AdditionalImageHandler.CleanFileName(fileName);
                                        var productImage = new ProductImage
                                        {
                                            Bvin = Guid.NewGuid().ToString()
                                        };

                                        created = AdditionalImageHandler.UploadAdditionalProductImage(app.CurrentStore.Id, product.Bvin, productImage.Bvin, path, fileInfo.Name, imageFile);

                                        if (created)
                                        {
                                            productImage.FileName = productImage.AlternateText = fileName + extension;
                                            productImage.Caption = string.Empty;
                                            productImage.ProductId = product.Bvin;
                                            fileSaved = app.CatalogServices.ProductImageCreate(productImage);
                                        }
                                    }
                                    else
                                    {
                                        var newFile = AdditionalImageHandler.InitializeProductFile(product.Bvin, path);
                                        // Check if the image already exists or not?
                                        // var existingImages = app.CatalogServices.ProductFiles.FindByProductId(prod.Bvin);

                                        created = app.CatalogServices.ProductFiles.Create(newFile);

                                        if (created)
                                        {
                                            fileSaved = ProductFile.SaveFile(context.CurrentStore.Id, newFile.Bvin, newFile.FileName, imageFile);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    sb.Append($"<p style=\"color: #ff0000\">{ex.Message}</p>");
                                    Exceptions.LogException(ex);
                                    Logger.Error(ex.Message, ex);
                                }

                                if (fileSaved)
                                {
                                    sb.Append($"<p style=\"color: #00ff00\"> {prefix} File successfully uploaded.</p>");
                                }
                                else
                                {
                                    sb.Append($"<p style=\"color: #ff0000\"> {prefix} Failed - Created: {created} Saved: {fileSaved}</p>");
                                }
                            }

                            if (fileSaved)
                            {
                                AdditionalImageHandler.MoveToProcessedFolder(fileInfo);
                            }
                        }
                    }

                    if (false)
                    {
                        // begin processing images for each image
                        var importPath = string.Concat(DiskStorage.GetStoreDataVirtualPath(context.CurrentStore.Id), "import/");

                        // iterate through each product in the store to find images
                        foreach (var product in products)
                        {
                            // display the product being processed
                            sb.Append($"<h3>Importing ({product.Sku}) {product.ProductName}</h3>");

                            // put together the expected path of the image
                            var filePath = importPath + product.ImageFileMedium;

                            // display the image path to the user
                            sb.Append($"<p><strong>{GetLocalizedString("FilePathLabel")}</strong>: <a href=\"{filePath.Replace("~", string.Empty)}\" target=\"_blank\">{filePath}</a></p>");

                            // determine if the import file/path exists
                            if (File.Exists(Server.MapPath(filePath)))
                            {
                                // let the user know the image import is beginning
                                sb.Append($"<p>{GetLocalizedString("FileImportBegin")}</p>");

                                // import the image from the import location
                                DiskStorage.CopyProductImage(context.CurrentStore.Id, product.Bvin, filePath, product.ImageFileMedium);

                                // let the user know the import was completed
                                sb.Append($"<p>{GetLocalizedString("FileImported")}</p>");
                            }
                            else
                            {
                                // the image was not found in the import location
                                sb.Append($"<p style=\"color: #ff0000\">{GetLocalizedString("FileNotFound")}</p>");
                            }
                        }
                    }
                }
                else
                {
                    sb.Append($"<p>{GetLocalizedString("NoProducts")}</p>");
                }

                sb.Append("</blockquote>");

                // render the output to the user
                phImportSummary.Controls.Add(new LiteralControl(sb.ToString()));
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                Logger.Error(ex.Message, ex);
                throw ex;
            }

            Logger.Debug("Hotcakes Commerce product image import COMPLETE.");
        }

        #endregion
    }
}