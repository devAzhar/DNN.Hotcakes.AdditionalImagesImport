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

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Instrumentation;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.ProcessAdditionalImagesModule.Components;

namespace Hotcakes.Modules.ProcessAdditionalImagesModule.Services.Data
{
    public class ImageProcessingController
    {
        #region Private Members
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ImageProcessingController));
        private HccRequestContext _context;

        private string _localResourceFile = string.Empty;
        private string LocalResourceFile
        {
            get
            {
                if (string.IsNullOrEmpty(_localResourceFile)) 
                {
                    _localResourceFile = Path.Combine(Constants.LOCALIZATION_FILE_PATH);
                }

                return _localResourceFile;
            }
        }
        #endregion

        public ImageProcessingController()
        {
            try
            {
                // get an instance of the store application
                _context = new HccRequestContext();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        public void ImportProductImages()
        {
            try
            {
                Requires.NotNull<HccRequestContext>(_context);
                Logger.Debug("Verified the Hotcakes request context is valid.");

                var accountServices = Factory.CreateService<AccountService>(_context);
                var stores = accountServices.Stores.FindAllPaged(1, int.MaxValue);

                foreach (var store in stores)
                {
                    _context.CurrentStore = store;

                    UpdateProductsForStore(_context);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        private void UpdateProductsForStore(HccRequestContext context)
        {
            Requires.NotNull<HccRequestContext>(context);

            Logger.Debug($"Begin importing product images for Store ID {context.CurrentStore.Id}");

            try
            {
                var products = GetProducts(context);

                Logger.Debug(GetMessage("ProcessTitle"));

                if (products != null && products.Count > 0)
                {
                    // begin processing images for each image
                    var importPath = string.Concat($"~/Portals/{context.CurrentStore.Id}/Hotcakes/Data/", "import/");

                    // display where the module is looking for images
                    Logger.Debug($"{GetMessage("ImportPathLabel")}:  {importPath}");

                    // iterate through each product in the store to find images
                    foreach (var product in products)
                    {
                        // display the product being processed
                        Logger.Debug($"Importing ({product.Sku}) {product.ProductName}");

                        // put together the expected path of the image
                        var filePath = string.Concat(importPath, product.ImageFileMedium);

                        // display the image path to the user
                        Logger.Debug($"{GetMessage("FilePathLabel")}:  {filePath}");

                        // determine if the import file/path exists
                        if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(filePath)))
                        {
                            // let the user know the image import is beginning
                            Logger.Debug(GetMessage("FileImportBegin"));

                            // import the image from the import location
                            DiskStorage.CopyProductImage(context.CurrentStore.Id, product.Bvin, filePath, product.ImageFileMedium);

                            // let the user know the import was completed
                            Logger.Debug(GetMessage("FileImported"));
                        }
                        else
                        {
                            // the image was not found in the import location
                            Logger.Debug(GetMessage("FileNotFound"));
                        }
                    }
                }
                else
                {
                    // no products were found
                    Logger.Debug(GetMessage("NoProducts"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }

            Logger.Debug($"Done importing product images for Store ID {context.CurrentStore.Id}");
        }

        #region Private Helper Methods
        private List<Product> GetProducts(HccRequestContext context)
        {
            try
            {
                Logger.Debug("Instantiating the Hotcakes request context.");
                // create a repo to get the catalog data
                var repoProduct = new ProductRepository(context);

                Logger.Debug("Getting a collection of the products in the store.");
                // get a collection of the products in the store
                var products = repoProduct.FindAllPagedWithCache(1, int.MaxValue);

                return products;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        private string GetMessage(string key)
        {
            try
            {
                return Localization.GetString(key, LocalResourceFile);
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return "[{{ERROR: Localization Key/Value Pair Missing}}]";
            }
        }
        #endregion
    }
}