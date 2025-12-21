using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql; 

namespace YurtVeriTabani
{
    class SqlBaglantisi
    {
        public NpgsqlConnection baglanti()
        {
            NpgsqlConnection baglan = new NpgsqlConnection("Server=localhost;Port=5432;Database=YurtOtomasyon;User Id=postgres;Password=admin;");
            baglan.Open();
            return baglan;
        }
    }
}