using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebRole1.Models;

namespace WebRole1
{
    public partial class AdminUser : System.Web.UI.Page
    {
        private string role;
        protected void Page_Load(object sender, EventArgs e)
        {

            CheckCredentials();


            var userStore = new UserStore<ApplicationUser>();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            IQueryable<ApplicationUser> usersQuery = userManager.Users;
            List<ApplicationUser> users = usersQuery.ToList();

            foreach (ApplicationUser user in users)
            {
                TableRow row = new TableRow();
                TableCell email = new TableCell();
                email.Text = user.Email;
                row.Cells.Add(email);
                TableCell tableCell = new TableCell();


                if (user.accountEnabled)
                {
                    Button disableButton = new Button();
                    disableButton.Text = "Disable User";
                    disableButton.Click += disableUser;
                    disableButton.CommandArgument = user.Email;
                    tableCell.Controls.Add(disableButton);
                    row.Cells.Add(tableCell);


                }
                else if (!user.accountEnabled)
                {
                    Button enableButton = new Button();
                    enableButton.Text = "Enable User";
                    enableButton.Click += enableUser;
                    enableButton.CommandArgument = user.Email;
                    tableCell.Controls.Add(enableButton);
                    row.Cells.Add(tableCell);

                }

                table.Rows.Add(row);



            }

        }
        private void CheckCredentials()
        {
            if (User.Identity.GetUserId() == null)
            {                                              
                // if user is not logged in they will be redirected to log in page.
                Response.Redirect("/Account/Login");
            }
            else
            {

                role = getUserRole();

                if (role == "Customer")
                {
                    Response.Redirect("/Account/Login");  
                    // if a customer enters in the admin page they will be redirected to login page.
                }
            }
        }

        protected string getUserRole()
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

        protected void disableUser(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string email = button.CommandArgument;  // this string stores the email of the user

            // 'helper' object to get the user
            var user = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            // get the user
            ApplicationUser applicationUser = user.FindByName(email);

            applicationUser.accountEnabled = false;
            user.Update(applicationUser);
        }

        protected void enableUser(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string email = button.CommandArgument;  // this string stores the email of the user

            // 'helper' object to get the user
            var user = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            // get the user
            ApplicationUser applicationUser = user.FindByName(email);

            applicationUser.accountEnabled = true;
            user.Update(applicationUser);
        }
    }
}
