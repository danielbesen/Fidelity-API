﻿using FidelityLibrary.DataContext;
using FidelityLibrary.Entity;
using FidelityLibrary.Persistance.Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FidelityLibrary.Persistance.EmployeeDAO
{
    public class EmployeeDAO : GenericDAO<Employee, int>
    {
        public static Employee FindByUserId(int UserId)
        {
            try
            {
                var oEmployee = new Employee();
                using (var context = new ApplicationDbContext())
                {
                    oEmployee = context.DbSetEmployee.FirstOrDefault(x => x.UserId == UserId);
                };

                return oEmployee;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao buscar funcionário pelo Id de usuário: " + e.Message);
            }
        }
    }
}
