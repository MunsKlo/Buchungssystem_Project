using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buchungsystem_WindowsAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buchungsystem_WindowsAuth.Pages.User
{
    public class EditModel : PageModel
    {
        enum AdminStatus
        {
            True,
            False,
            Error
        }

        readonly private MainMethods MainMethods = new MainMethods();
        readonly private InterfaceDatabase DbConn = new InterfaceDatabase();

        public string StatusMessage { get; set; } = string.Empty;
        public new UserModel User { get; set; }

        [BindProperty]
        public string FormId { get; set; }
        public void OnGet()
        {
            var isAdmin = MainMethods.IsUserAdmin(base.User.Identity.Name) == "J" ? AdminStatus.True :
                MainMethods.IsUserAdmin(base.User.Identity.Name) == "N" ? AdminStatus.False : AdminStatus.Error ;

            StatusMessage = Request.Query["message"].Count > 0 ? Request.Query["message"][0] : string.Empty;
            if (isAdmin == AdminStatus.True)
            {
                var parameter = Request.Query["id"].Count > 0 ? Request.Query["id"][0] : string.Empty;

                if(uint.TryParse(parameter, out uint id))
                {
                    User = MainMethods.GetUser(id);

                    if(User.Name == null)
                    {
                        Response.Redirect("/Index?status=6");
                    }
                }
            }
            else if (isAdmin == AdminStatus.False)
            {
                Response.Redirect("/Index?status=2");
            }
            else
            {
                Response.Redirect("/Index?status=3");
            }
        }

        public void OnPost()
        {
            var id = Convert.ToUInt32(Request.Query["id"].Count > 0 ? Request.Query["id"][0] : string.Empty);

            User = MainMethods.GetUser(id);

            if (uint.TryParse(FormId, out uint numbId) && User.Id == numbId)
            {
                var res = DbConn.ChangeUserCreator(User.Id, !User.IsCreator);

                if (res == "Success")
                {
                    Response.Redirect("/User/Index?status=1");
                }
                else
                {
                    Response.Redirect("/Index?status=3");
                }
            }
            else
            {
                StatusMessage = "Falsche Id!";
                Response.Redirect($"/User/Edit?id={User.Id}&message={StatusMessage}");
            }
        }
    }
}
