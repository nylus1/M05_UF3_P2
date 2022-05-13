using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace M05_UF3_P2_Template.App_Code.Model
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string IconBackground { get; set; }
        public string CIF { get; set; }
        public string Address { get; set; }
        public string Web { get; set; }
        public string Email { get; set; }


        public Company()
        {
        }
        public Company(DataRow row)
        {
            try
            {
                Id = (int)row[0];
            }
            catch
            {
                Id = 0;
            }
            Name = row[1].ToString();
            Icon = row[2].ToString();
            IconBackground = row[3].ToString();
            CIF = row[4].ToString();
            Address = row[5].ToString();
            Web = row[6].ToString();
            Email = row[7].ToString();
        }
        public Company(int Id) : this(DatabaseManager.Select("Company", null, "Id = " + Id + " ").Rows[0]) { }

        public bool Update()
        {
            DatabaseManager.DB_Field[] fields = new DatabaseManager.DB_Field[]
            {
                new DatabaseManager.DB_Field("Name", Name),
                new DatabaseManager.DB_Field("Icon", Icon),
                new DatabaseManager.DB_Field("IconBackground", IconBackground),
                new DatabaseManager.DB_Field("CIF", CIF),
                new DatabaseManager.DB_Field("Address", Address),
                new DatabaseManager.DB_Field("Web", Web),
                new DatabaseManager.DB_Field("Email", Email)
            };
            return DatabaseManager.Update("Company", fields, "Id = " + Id + " ") > 0 ? true : false;
        }
        public bool Add()
        {
            DatabaseManager.DB_Field[] fields = new DatabaseManager.DB_Field[]
            {
                new DatabaseManager.DB_Field("Name", Name),
                new DatabaseManager.DB_Field("Icon", Icon),
                new DatabaseManager.DB_Field("IconBackground", IconBackground),
                new DatabaseManager.DB_Field("CIF", CIF),
                new DatabaseManager.DB_Field("Address", Address),
                new DatabaseManager.DB_Field("Web", Web),
                new DatabaseManager.DB_Field("Email", Email)
            };
            return DatabaseManager.Insert("Company", fields) > 0 ? true : false;
        }
        public bool Remove()
        {
            return Remove(Id);
        }
        public static bool Remove(int id)
        {
            return DatabaseManager.Delete("Company", id) > 0 ? true : false;
        }
    }
}
