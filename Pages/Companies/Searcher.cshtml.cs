using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using M05_UF3_P2_Template.App_Code.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace M05_UF3_P2_Template.Pages.Companies
{
    public class SearcherModel : PageModel
    {
        public List<Company> companies = new List<Company>();
        public void OnGet()
        {
            DataTable dt = DatabaseManager.Select("Company", new string[] { "*" }, "");
            foreach (DataRow dataRow in dt.Rows)
            {
                companies.Add(new Company(dataRow));
            }
        }
        public void OnPostDelete(int id)
        {
            Company.Remove(id);
            OnGet();
        }
    }
}
