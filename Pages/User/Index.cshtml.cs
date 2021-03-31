using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buchungsystem_WindowsAuth.Pages.User
{
    public class IndexModel : PageModel
    {
        enum AdminStatus
        {
            True,
            False,
            Error
        }
        public List<UserModel> UserList { get; set; } = new List<UserModel>();
        public string StatusMessage { get; set; } = string.Empty;
        public List<string> StatusUser { get; set; } = new List<string> { "Ja", "Nein" };

        readonly private MainMethods MainMethods = new MainMethods();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();

        [BindProperty]
        public string SearchString { get; set; }
        public void OnGet()
        {
            var isAdmin = MainMethods.IsUserAdmin(User.Identity.Name) == "J" ? AdminStatus.True :
                MainMethods.IsUserAdmin(User.Identity.Name) == "N" ? AdminStatus.False : AdminStatus.Error;
            if (isAdmin == AdminStatus.False)
            {
                Response.Redirect("/Index?status=2");
            }
            else if (isAdmin == AdminStatus.Error)
            {
                Response.Redirect("/Index?status=3");
            }
            else
            {
                SearchString = Request.Query["search"].Count > 0 ? Request.Query["search"][0] : string.Empty;
                var parameter = Convert.ToInt32(Request.Query["status"]);

                StatusMessage = MainMethods.StatusHandler(parameter);

                if (SearchString != string.Empty)
                {
                    var res = DbConn.GetAllSearchedUsers(SearchString);

                    if (res[0].ToString() == "Success")
                    {
                        var paraList = (List<object>)res[1];
                        UserList = MainMethods.FillUserList(paraList);
                    }
                    else
                    {
                        StatusMessage = MainMethods.StatusHandler(3);
                    }
                }
                else
                {
                    var res = DbConn.GetAllUsers();

                    if (res[0].ToString() == "Success")
                    {
                        var paraList = (List<object>)res[1];
                        UserList = MainMethods.FillUserList(paraList);
                    }
                    else
                    {
                        StatusMessage = MainMethods.StatusHandler(3);
                    }
                }
            }
        }

        public void OnPost()
        {
            var resList = DbConn.GetAllSearchedUsers(SearchString);

            if (resList[0].ToString() == "Success")
            {
                Response.Redirect($"/User/Index?search={SearchString}");
            }
            else
            {
                Response.Redirect("/User/Index?status=3");
            }
        }
    }
}
