using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace LanguageCenter.Areas.Identity.Pages.Account
{
    public class SelectRoleModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public SelectRoleModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            Roles = new List<string>();
        }

        public List<string> Roles { get; set; } = new List<string>();


        public void OnGet()
        {
            Roles = _roleManager.Roles
                .Select(r => r.Name)
                .Where(name => name != null && name != "Admin")
                .Select(name => name!)
                .ToList();
        }
    }
}
