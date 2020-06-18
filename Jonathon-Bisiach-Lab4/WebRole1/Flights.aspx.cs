using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebRole1.Models;

namespace WebRole1
{
    public partial class Flights : System.Web.UI.Page
    {
        private string role;
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckCredentials();

        }

        private void CheckCredentials()
        {
            if (User.Identity.GetUserId() != null)
            {
                BtnGet.Visible = true;       
                // If customer or admin has logged in the 'Offer' button will display
                role = getUserRole();
            }

        }

        private string getUserRole()
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            if (User.Identity.GetUserId() == null) return null;
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);


            var myroles = roleManager.Roles.ToList();
            foreach (IdentityRole arole in myroles)
            {
                string roleName = arole.Name;
                if (User.IsInRole(roleName))
                    return arole.Name;
            }
            return null;
        }
    }
}