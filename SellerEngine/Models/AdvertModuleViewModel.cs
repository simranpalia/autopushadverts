using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SellerEngine.Models
{
    public class AdvertModuleViewModel : AdvertModule
    {
        public long RootId { get; set; }

        //public long CredId { get; set; }

        public long CloudId { get; set; }

        public List<EntityCredentialViewModel> Users { get; set; }

        public AdvertModuleViewModel()
        {

        }

        public AdvertModuleViewModel(long credId, long rootId)
        {
            this.CredId = credId;
            this.RootId = rootId;

            this.Users = new List<EntityCredentialViewModel>();

            foreach (var item in DAL.GetUsers(this.RootId))
            {
                this.Users.Add(new EntityCredentialViewModel(item));
            }
        }

    }
}