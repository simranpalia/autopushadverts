using SellerEngine.Models;
using SellerEngine.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SellerEngine.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public ActionResult RootUsers()
        {
            return View(DAL.GetRootUsers());
        }

        [HttpPost]
        public ActionResult NewRoot()
        {
            string userName = Request.Form["txtName"];
            DAL.AddRootUser(userName);
            return RedirectToAction("RootUsers");
        }

        [HttpGet]
        public ActionResult Users(long id)
        {
            return View(DAL.GetUsers(id));
        }

        [HttpGet]
        public ActionResult NewUser(long id = 0, long rootId = 0)
        {
            if (id > 0)
                return View(DAL.FindEntityCredential(id));
            else
                return View(new EntityCredential() { CreatedAt = DateTime.UtcNow, RootId = rootId });
        }

        [HttpPost]
        public ActionResult StoreFBResponse(string authToken, string userId, long rootId, string userName)
        {
            try
            {
                DAL.AddUpdateFBAuth(authToken, userId, rootId, userName);
                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult NewUser(EntityCredential vm)
        {
            try
            {
                DAL.AddUpdateEntityCredentials(vm);

            }
            catch (Exception ex)
            {

            }
            return Redirect("Users?id=" + vm.RootId);
        }

        [HttpGet]
        public ActionResult AllAdds(long rootId)
        {
            return View("ListAdds", DAL.GetAdvertisementsForRootUser(rootId));
        }

        [HttpGet]
        public ActionResult ListAdds(long credId = 0, long rootId = 0)
        {
            return View(DAL.GetAdvertisements(credId));
        }

        [HttpGet]
        public ActionResult New(long credId = 0, long rootId = 0)
        {
            return View(new AdvertModuleViewModel(credId, rootId));
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostAdvert(AdvertModuleViewModel vm)
        {
            AdvertModule newObj = new AdvertModule()
            {
                AdvertId = vm.AdvertId,
                AirCond = vm.AirCond,
                CookingAllowed = vm.CookingAllowed,
                CreatedAt = vm.CreatedAt,
                CredId = vm.CredId,
                EntityName = vm.EntityName,
                EntityUrl = vm.EntityUrl,
                Id = vm.Id,
                Internet = vm.Internet,
                Location = vm.Location,
                NearKTM = vm.NearKTM,
                PicUrl = vm.PicUrl,
                Preference = vm.Preference,
                Price = vm.Price,
                Status = vm.Status,
                Summary = vm.Summary,
                Title = vm.Title
            };

            DAL.AddUpdateAdvert(newObj, string.Empty, false, string.Empty, false);

            return Content(newObj.Id.ToString());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult New(AdvertModuleViewModel vm)
        {
            string localPath = string.Empty;

            var dbObj = DAL.FindEntityCredential(vm.CredId.GetValueOrDefault());
            string resp = string.Empty;

            var dbFile = DAL.FindCloudDoc(vm.CloudId);
            if (vm.CloudId > 0)
                localPath = dbFile.Url;

            var dbUser = DAL.FindEntityCredential(vm.CredId.GetValueOrDefault());

            vm.EntityName = dbUser.EntityName;

            if (dbObj != null)
            {
                vm.PicUrl = localPath;

                AdvertModule newObj = new AdvertModule()
                {
                    AdvertId = vm.AdvertId,
                    AirCond = vm.AirCond,
                    CookingAllowed = vm.CookingAllowed,
                    CreatedAt = vm.CreatedAt,
                    CredId = vm.CredId,
                    EntityName = vm.EntityName,
                    EntityUrl = vm.EntityUrl,
                    Id = vm.Id,
                    Internet = vm.Internet,
                    Location = vm.Location,
                    NearKTM = vm.NearKTM,
                    PicUrl = vm.PicUrl,
                    Preference = vm.Preference,
                    Price = vm.Price,
                    Status = vm.Status,
                    Summary = vm.Summary,
                    Title = vm.Title
                };

                string tempResp = string.Empty;

                if (vm.EntityName.Equals("iBilik"))
                    resp = SeleniumUtils.LoginAndPublishIBilik(dbObj.UserName, dbObj.Password, newObj, localPath, !string.IsNullOrWhiteSpace(localPath) ? dbFile.PublicId : string.Empty);
                else if (vm.EntityName.Equals("Mudah"))
                    SeleniumUtils.LoginAndPublishMudah(dbObj.UserName, dbObj.Password, localPath, "", newObj, dbObj.NickName, "0182222450");

                //fetch resp from ModuleWise
                var dbResponse = DAL.FindModuleWise(newObj.Id, newObj.CredId.GetValueOrDefault());

                if (dbResponse.ResponseId.Equals("Published"))
                {
                    tempResp = "Success^" + newObj.Id + "^" + newObj.EntityUrl;
                }
                else
                {
                    tempResp = dbResponse.ResponseId + "^" + newObj.Id + "^";
                }
                resp = tempResp;
            }

            if (Request.IsAjaxRequest())
            {
                return Content(resp);
            }
            else
            {
                TempData["Message"] = resp;
                return RedirectToAction("ListAdds", new { credId = vm.CredId });
            }
        }

        [HttpPost]
        public ActionResult SaveAddToDB(AdvertModuleViewModel vm)
        {
            string localPath = string.Empty;

            var dbObj = DAL.FindEntityCredential(vm.CredId.GetValueOrDefault());
            string resp = string.Empty;

            var dbFile = DAL.FindCloudDoc(vm.CloudId);
            if (vm.CloudId > 0)
                localPath = dbFile.Url;

            var dbUser = DAL.FindEntityCredential(vm.CredId.GetValueOrDefault());

            vm.EntityName = dbUser.EntityName;

            if (dbObj != null)
            {
                vm.PicUrl = localPath;

                AdvertModule newObj = new AdvertModule()
                {
                    AdvertId = vm.AdvertId,
                    AirCond = vm.AirCond,
                    CookingAllowed = vm.CookingAllowed,
                    CreatedAt = vm.CreatedAt,
                    CredId = vm.CredId,
                    EntityName = vm.EntityName,
                    EntityUrl = vm.EntityUrl,
                    Id = vm.Id,
                    Internet = vm.Internet,
                    Location = vm.Location,
                    NearKTM = vm.NearKTM,
                    PicUrl = vm.PicUrl,
                    Preference = vm.Preference,
                    Price = vm.Price,
                    Status = vm.Status,
                    Summary = vm.Summary,
                    Title = vm.Title
                };

                //save to DB
                DAL.AddUpdateAdvert(newObj, string.Empty, false, "FB", saveToModuleWise: false);

                var dbResponse = DAL.FindModuleWise(newObj.Id, newObj.CredId.GetValueOrDefault());

                string tempResp = string.Empty;

                if (dbResponse.ResponseId.Equals("Published"))
                {
                    tempResp = "Success^" + newObj.Id + "^" + newObj.EntityUrl;
                }
                else
                {
                    tempResp = "Fail^" + newObj.Id + "^";
                }
                resp = tempResp;
            }

            return Content(resp);

        }

        [HttpGet]
        public ActionResult Index()
        {
            var r = SeleniumUtils.LoginAndPublishIBilik("simran.palia@gmail.com", "simran1990");
            return Content("Response: " + r);
        }

        [HttpGet]
        public ActionResult EditAdd(long addId)
        {
            var dbObj = DAL.FindAdvert(addId);

            return View(dbObj);
        }

        [HttpGet]
        public ActionResult DeleteAdd(long addId)
        {

            var dbObj = DAL.FindAdvert(addId);
            if (!string.IsNullOrWhiteSpace(dbObj.AdvertId))
                TempData["Message"] = SeleniumUtils.LoginAndDeleteIBilik("simran.palia@gmail.com", "simran1990", dbObj.AdvertId);
            else
                DAL.DeleteAdvertModuleById(addId);

            return RedirectToAction("ListAdds", new { credId = dbObj.CredId });
        }

        [HttpPost]
        public ActionResult EditAdd(AdvertModule vm)
        {
            var file = Request.Files[0];

            string localPath = string.Empty;
            if (file != null)
            {
                if (!string.IsNullOrWhiteSpace(file.FileName))
                {
                    string dir = System.Web.HttpContext.Current.Server.MapPath("/App_Data");

                    if (Directory.Exists(dir))
                    {
                        var fName = Path.Combine(dir, DateTime.Now.ToString("ddMMyyyy_hhmmss_") + file.FileName);

                        file.SaveAs(fName);

                        localPath = fName;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(vm.AdvertId))
                TempData["Message"] = SeleniumUtils.LoginAndEditIBilik("simran.palia@gmail.com", "simran1990", vm, localPath);
            else
                TempData["Message"] = SeleniumUtils.LoginAndPublishIBilik("simran.palia@gmail.com", "simran1990", vm, localPath, string.Empty);


            return RedirectToAction("ListAdds", new { credId = vm.CredId });
        }

        [HttpPost]
        public ActionResult UploadFile(bool appendUrl = false)
        {
            var file = Request.Files[0];

            string localPath = string.Empty;

            string resp = string.Empty;

            if (file != null)
            {
                if (!string.IsNullOrWhiteSpace(file.FileName))
                {
                    string dir = System.Web.HttpContext.Current.Server.MapPath("/App_Data");

                    if (Directory.Exists(dir))
                    {
                        var fName = Path.Combine(dir, DateTime.Now.ToString("ddMMyyyy_hhmmss_") + file.FileName);

                        file.SaveAs(fName);

                        localPath = fName;

                        //save it to cloudinary
                        resp = CloudinaryUtils.UploadToCloudinary(fName).ToString();

                        System.IO.File.Delete(fName);
                    }
                }
            }

            if (appendUrl)
            {
                var dbCloudObj = DAL.FindCloudDoc(Convert.ToInt64(resp));
                if (dbCloudObj != null)
                    resp = resp + "~" + dbCloudObj.Url;
            }

            return Content(resp);
        }

        [HttpPost]
        public ActionResult DeleteFile(long cloudId)
        {
            CloudinaryUtils.DeleteFromCloudinary(cloudId);

            return Content("OK");

        }

        [HttpGet]
        public ActionResult FBResponse()
        {
            return Redirect("https://graph.facebook.com/oauth/access_token?code=...&client_id=1484207085025748&redirect_uri=...");
        }

        [HttpPost]
        public ActionResult UpdateFBGroups(string groupId, string gName, long credId = 0, long fbUserId = 0)
        {
            credId = DAL.FindEntityCredentialByUserId(fbUserId);
            DAL.AddUpdateGroup(groupId, gName, credId);
            return Content("OK");
        }

        [HttpPost]
        public ActionResult PullToken(long credId)
        {
            try
            {
                return Content(DAL.FindEntityCredential(credId).AuthToken);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SaveToModule(long advertId, string postId, long entityId, string entityName, string entityUrl, long credId)
        {
            DAL.AddToModule(advertId, postId, entityId, entityName, entityUrl, credId);
            return Content("OK");
        }

        [HttpPost]
        public ActionResult DeleteModuleWise(long credId, string entityName, string entityId, long moduleId, long advertId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(entityName))
                {
                    if (entityName == "iBilik")
                    {
                        if (!string.IsNullOrWhiteSpace(entityId))
                        {
                            //remove from server
                            var dbUser = DAL.FindEntityCredential(credId);

                            if (dbUser != null)
                            {
                                SeleniumUtils.LoginAndDeleteIBilik(dbUser.UserName, dbUser.Password, entityId);
                            }
                        }
                    }

                    //remove from DB
                    DAL.DeleteModuleWise(advertId, moduleId, credId, entityId, entityName);
                }

                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
