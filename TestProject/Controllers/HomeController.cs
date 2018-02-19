using DapperUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;

namespace TestProject.Controllers
{
    public class HomeController : Controller
    {
        public JsonResult insert()
        {
            try
            {
                Teste teste = new Teste()
                {
                    teste1 = 2,
                    teste2 = null
                };

                using (DBUtils<Teste> insert = new DBUtils<Teste>())
                {
                    return Json(insert.Insert(teste), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult updateError()
        {
            try
            {
                Teste teste = new Teste()
                {
                    codigo = 2,
                    teste1 = 3,
                    teste2 = "update realizado com sucesso"
                };

                using(DBUtils<Teste> update = new DBUtils<Teste>())
                {
                    return Json(update.Update(teste), JsonRequestBehavior.AllowGet);
                }
            }catch(Exception e)
            {
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult updateSuccess()
        {
            try
            {
                Teste teste = new Teste()
                {
                    codigo = 23,
                    teste1 = 3,
                    teste2 = "update realizado com sucesso"
                };

                using (DBUtils<Teste> update = new DBUtils<Teste>())
                {
                    return Json(update.Update(teste), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(e.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}