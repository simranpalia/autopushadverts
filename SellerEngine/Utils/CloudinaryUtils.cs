using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace SellerEngine.Utils
{
    public class CloudinaryUtils
    {
        public static long UploadToCloudinary(string filePath)
        {
            long cloudDocId = 0;
            try
            {
                Account account = new Account(
 "dfxcoimwt",
 "641878399272384",
 "hIUfmeaPoy-iiaeO8UIxUc31pBM");

                Cloudinary cloudinary = new Cloudinary(account);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath)
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                var dbObj = new CloudDoc()
                {
                    CreatedAt = DateTime.UtcNow,
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Uri.AbsoluteUri
                };

                DAL.AddCloudDocument(dbObj);

                cloudDocId = dbObj.Id;
            }
            catch (Exception ex)
            {

            }

            return cloudDocId;
        }

        public static void DeleteFromCloudinary(long cloudDocId)
        {

            try
            {
                Account account = new Account(
 "dfxcoimwt",
 "641878399272384",
 "hIUfmeaPoy-iiaeO8UIxUc31pBM");

                Cloudinary cloudinary = new Cloudinary(account);

                var publicId = DAL.FindPublicId(cloudDocId);

                var destroyParam = new DeletionParams(publicId);

                var uploadResult = cloudinary.Destroy(destroyParam);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //delete from DB
                    DAL.DeleteCloudObject(cloudDocId);
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}