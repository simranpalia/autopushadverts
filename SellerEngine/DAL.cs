using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SellerEngine
{
    public class DAL
    {
        public static AutoSellerDataContext GetContext()
        {
            return new AutoSellerDataContext();
        }

        internal static List<AdvertModule> GetAdvertisements(long credId)
        {
            using (var db = GetContext())
            {
                return db.AdvertModules.Where(x => x.CredId == credId).ToList();
            }
        }

        internal static void AddRootUser(string userName)
        {
            using (var db = GetContext())
            {
                db.RootUsers.InsertOnSubmit(new RootUser() { CreatedAt = DateTime.UtcNow, NickName = userName });
                db.SubmitChanges();
            }
        }

        internal static void AddCloudDocument(CloudDoc cloudDoc)
        {
            using (var db = GetContext())
            {
                db.CloudDocs.InsertOnSubmit(cloudDoc);

                db.SubmitChanges();
            }
        }

        internal static List<RootUser> GetRootUsers()
        {
            using (var db = GetContext())
            {
                if (!db.DatabaseExists())
                    db.CreateDatabase();
                return db.RootUsers.ToList();
            }
        }

        public static EntityCredential FindEntityCredential(long id)
        {
            using (var db = GetContext())
            {
                return db.EntityCredentials.Where(x => x.Id == id).FirstOrDefault();
            }
        }

        internal static List<EntityCredential> GetUsers(long rootUserId)
        {
            using (var db = GetContext())
            {
                return db.EntityCredentials.Where(x => x.RootId.GetValueOrDefault() == rootUserId).ToList();
            }
        }

        internal static string FindPublicId(long cloudDocId)
        {
            using (var db = GetContext())
            {
                return db.CloudDocs.Where(x => x.Id == cloudDocId).Select(x => x.PublicId).FirstOrDefault();
            }
        }

        internal static void DeleteCloudObject(long cloudDocId)
        {
            using (var db = GetContext())
            {
                var dbObj = db.CloudDocs.Where(x => x.Id == cloudDocId).FirstOrDefault();

                if (dbObj != null)
                {
                    db.CloudDocs.DeleteOnSubmit(dbObj);

                    db.SubmitChanges();
                }
            }
        }

        internal static void AddUpdateFBAuth(string authToken, string userId, long rootId, string fbName)
        {
            using (var db = GetContext())
            {
                var dbObj = db.EntityCredentials.Where(x => x.EntityUserId.Equals(userId) && x.EntityName.Equals("FB") && x.RootId == rootId).FirstOrDefault();
                if (dbObj != null)
                {
                    //update
                    dbObj.AuthToken = authToken;
                    dbObj.UserName = fbName;
                }
                else
                {
                    var dbNewObj = new EntityCredential()
                    {
                        AuthToken = authToken,
                        CreatedAt = DateTime.UtcNow,
                        EntityName = "FB",
                        EntityUserId = userId,
                        NickName = "FB User",
                        RootId = rootId,
                        UserName = fbName
                    };

                    db.EntityCredentials.InsertOnSubmit(dbNewObj);
                }

                db.SubmitChanges();
            }
        }

        internal static CloudDoc FindCloudDoc(long cloudId)
        {
            using (var db = GetContext())
            {
                return db.CloudDocs.Where(x => x.Id == cloudId).FirstOrDefault();
            }
        }

        internal static List<AdvertModule> GetAdvertisementsForRootUser(long rootId)
        {
            using (var db = GetContext())
            {
                var dbUsers = db.EntityCredentials.Where(x => x.RootId == rootId).ToList();

                var moduleWise = db.ModuleWises.Where(x => dbUsers.Select(p => p.Id).ToList().Contains(x.CredId)).Select(x => x.AdvertId).ToList();

                return db.AdvertModules.Where(x => moduleWise.Contains(x.Id)).OrderByDescending(x => x.Id).ToList();
            }
        }

        internal static void AddUpdateEntityCredentials(EntityCredential vm)
        {
            using (var db = GetContext())
            {
                if (vm.Id > 0)
                {
                    var dbObj = db.EntityCredentials.Where(x => x.Id == vm.Id).FirstOrDefault();
                    if (dbObj != null)
                    {
                        dbObj.UserName = vm.UserName;
                        dbObj.Password = vm.Password;
                        dbObj.NickName = vm.NickName;
                        dbObj.RootId = vm.RootId;
                        dbObj.EntityName = vm.EntityName;

                        db.SubmitChanges();

                    }
                }
                else
                {
                    db.EntityCredentials.InsertOnSubmit(vm);
                    db.SubmitChanges();
                }
            }
        }

        internal static void AddUpdateAdvert(AdvertModule vm, string addId, bool hasError, string entityName, bool saveToModuleWise = true)
        {
            using (var db = GetContext())
            {
                if (vm.Id > 0)
                {

                    //save it to ModuleWise
                    if (!db.ModuleWises.Any(x => x.AdvertId == vm.Id && x.CredId == vm.CredId))
                    {
                        // Save it to ModuleWise
                        db.ModuleWises.InsertOnSubmit(new ModuleWise()
                        {
                            AdvertId = vm.Id,
                            ModuleName = entityName,
                            ModuleId = addId,
                            ModuleUrl = vm.EntityUrl,
                            ResponseId = (hasError ? "Pending" : "Published"),
                            CredId = vm.CredId.GetValueOrDefault()
                        });
                        db.SubmitChanges();
                    }
                    else
                    {
                        var dbObj = db.AdvertModules.Where(x => x.Id == vm.Id).FirstOrDefault();
                        if (dbObj != null)
                        {
                            dbObj.Title = vm.Title;
                            dbObj.Summary = vm.Summary;
                            dbObj.Preference = vm.Preference;
                            dbObj.Price = vm.Price;
                            dbObj.AirCond = vm.AirCond;
                            dbObj.NearKTM = vm.NearKTM;
                            dbObj.CookingAllowed = vm.CookingAllowed;
                            dbObj.Internet = vm.Internet;
                            dbObj.PicUrl = vm.PicUrl;
                            //dbObj.AdvertId = (!string.IsNullOrWhiteSpace(vm.AdvertId) ? vm.AdvertId : addId);
                            dbObj.Status = (hasError ? addId : "Published");

                            db.SubmitChanges();
                        }
                    }
                }
                else
                {
                    vm.Status = (hasError ? "Pending" : "Published");

                    //vm.AdvertId = (hasError ? string.Empty : addId);
                    vm.CreatedAt = DateTime.UtcNow;

                    db.AdvertModules.InsertOnSubmit(vm);

                    db.SubmitChanges();

                    if (saveToModuleWise)
                    {
                        // Save it to ModuleWise
                        db.ModuleWises.InsertOnSubmit(new ModuleWise()
                        {
                            AdvertId = vm.Id,
                            ModuleName = entityName,
                            ModuleId = addId,
                            ModuleUrl = vm.EntityUrl,
                            ResponseId = (hasError ? "Pending" : "Published"),
                            CredId = vm.CredId.GetValueOrDefault()
                        });
                        db.SubmitChanges();
                    }
                }
            }
        }

        internal static AdvertModule FindAdvert(long addId)
        {
            using (var db = GetContext())
            {
                return db.AdvertModules.Where(x => x.Id == addId).FirstOrDefault();
            }
        }

        internal static void DeleteAdvert(string addId)
        {
            using (var db = GetContext())
            {
                var dbObj = db.AdvertModules.Where(x => x.AdvertId == addId).FirstOrDefault();

                if (dbObj != null)
                {
                    db.AdvertModules.DeleteOnSubmit(dbObj);

                    db.SubmitChanges();
                }
            }
        }

        internal static void DeleteAdvertModuleById(long id)
        {
            using (var db = GetContext())
            {
                var dbObj = db.AdvertModules.Where(x => x.Id == id).FirstOrDefault();

                if (dbObj != null)
                {
                    db.AdvertModules.DeleteOnSubmit(dbObj);

                    db.SubmitChanges();
                }
            }
        }

        internal static void AddUpdateGroup(string groupId, string gName, long credId)
        {
            using (var db = GetContext())
            {
                if (db.FBGorups.Any(x => x.CredId == credId && x.GroupId == groupId))
                {
                    //update
                    var dbObj = db.FBGorups.Where(x => x.CredId == credId && x.GroupId == groupId).FirstOrDefault();
                    if (dbObj != null)
                    {
                        dbObj.GroupTitle = gName;
                    }
                }
                else
                {
                    //add
                    db.FBGorups.InsertOnSubmit(new FBGorup()
                    {
                        CredId = credId,
                        GroupId = groupId,
                        GroupTitle = gName
                    });

                }
                db.SubmitChanges();
            }
        }

        internal static void AddToModule(long advertId, string postId, long entityId, string entityName, string entityUrl, long credId)
        {
            using (var db = GetContext())
            {
                db.ModuleWises.InsertOnSubmit(new ModuleWise()
                {
                    AdvertId = advertId,
                    ModuleName = entityName,
                    ModuleId = entityId.ToString(),
                    ResponseId = postId.ToString(),
                    CredId = credId
                });

                db.SubmitChanges();
            }
        }

        internal static long FindEntityCredentialByUserId(long fbUserId)
        {
            using (var db = GetContext())
            {
                return db.EntityCredentials.Where(x => x.EntityUserId == fbUserId.ToString()).Select(x => x.Id).FirstOrDefault();
            }
        }

        public static List<FBGorup> GetUserGroups(long credId)
        {
            using (var db = GetContext())
            {
                return db.FBGorups.Where(x => x.CredId == credId).ToList();
            }
        }

        public static IEnumerable<ModuleWise> GetModuleWise(long addId)
        {
            using (var db = GetContext())
            {
                return db.ModuleWises.Where(x => x.AdvertId == addId).ToList();
            }
        }

        internal static ModuleWise FindModuleWise(long advertId, long credId)
        {
            using (var db = GetContext())
            {
                return db.ModuleWises.Where(x => x.CredId == credId && x.AdvertId == advertId).FirstOrDefault();
            }
        }

        internal static void DeleteModuleWise(long advertId, long moduleId, long credId, string entityId, string entityName)
        {
            using (var db = GetContext())
            {
                var dbModuleWiseObj = db.ModuleWises.Where(x => x.Id == moduleId).FirstOrDefault();

                if (dbModuleWiseObj != null)
                {
                    db.ModuleWises.DeleteOnSubmit(dbModuleWiseObj);
                    db.SubmitChanges();

                    //check if no more entities then delete from advert
                    if (!db.ModuleWises.Any(x => x.AdvertId == advertId))
                    {
                        var dbAdvertObj = db.AdvertModules.Where(x => x.Id == advertId).FirstOrDefault();

                        if (dbAdvertObj != null)
                        {
                            db.AdvertModules.DeleteOnSubmit(dbAdvertObj);
                            db.SubmitChanges();
                        }
                    }
                }
            }
        }

        public static FBGorup FindFBGroup(string groupId)
        {
            using (var db = GetContext())
            {
                return db.FBGorups.Where(x => x.GroupId == groupId).FirstOrDefault();
            }
        }
    }
}