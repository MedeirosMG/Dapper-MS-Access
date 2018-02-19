using DapperUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Test.Models
{
    public class Teste
    {
        [Key]
        public int codigo { get; set; }

        public int teste1 { get; set; }

        public string teste2 { get; set; }

        [AttributeTable]
        public string tbl_teste { get; set; }

        [AttributeExcluir]
        public Teste2 tes { get; set; }
    }
}