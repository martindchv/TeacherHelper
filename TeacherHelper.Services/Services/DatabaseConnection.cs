using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherHelper.Models.Models;

namespace TeacherHelper.Services.Services
{
    public class DatabaseConnection
    {
        public static bool Try()
        {
            try
            {
                using (var db = new TeacherHelperContext())
                {
                    return db.Database.CanConnect();
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
