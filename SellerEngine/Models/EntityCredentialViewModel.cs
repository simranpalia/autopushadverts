using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SellerEngine.Models
{
    public class EntityCredentialViewModel : EntityCredential
    {
        public bool IsSelected { get; set; }

        public EntityCredentialViewModel()
        {

        }

        public EntityCredentialViewModel(EntityCredential dbObj)
        {
            this.Id = dbObj.Id;
            this.AuthToken = dbObj.AuthToken;
            this.CreatedAt = dbObj.CreatedAt;
            this.EntityName = dbObj.EntityName;
            this.EntityUserId = dbObj.EntityUserId;
            this.NickName = dbObj.NickName;
            this.Password = dbObj.Password;
            this.RootId = dbObj.RootId;
            this.UserName = dbObj.UserName;
        }
    }
}