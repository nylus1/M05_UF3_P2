using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using M05_UF3_P2_Template.App_Code.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace M05_UF3_P2_Template.Pages.Companies
{
    public class CompanyModel : PageModel
    {
        [BindProperty(SupportsGet =true)]
        public int Id { get; set; }
        [BindProperty]
        public Company company { get; set; }
        public void OnGet()
        {
            if(Id > 0)
            {
                company = new Company(Id);
            }
        }
        public void OnPost()
        {
            company.Id = Id;
            if (Id > 0)
            {
                company.Update();
            }
            else
            {
                company.Add();
                Id = (int)DatabaseManager.Scalar("Company", DatabaseManager.SCALAR_TYPE.MAX, new string[] { "Id" }, "");
                OnGet();
            }
        }
    }
}
