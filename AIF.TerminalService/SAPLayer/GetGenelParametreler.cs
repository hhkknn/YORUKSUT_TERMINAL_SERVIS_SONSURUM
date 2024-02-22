using AIF.TerminalService.DatabaseLayer;
using AIF.TerminalService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AIF.TerminalService.SAPLayer
{
    public class GetGenelParametreler
    {
        public Response getGenelParametreler(string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {

                    var query = "Select TOP 1 \"U_DepoYeriCalisir\",\"U_TlpBglOrj\",\"U_TurkceArama\",\"U_CrystalKullan\" as \"U_CrystalKullan\", \"U_CkmFzlMktr\" as \"U_CkmFzlMktr\",\"U_TlpszTslk\" as \"U_TlpszTslk\",\"U_CkmGrpla\",\"U_SayimMikOto\",\"U_SayimBtnOto\",\"U_OndalikMiktar\",\"U_DepoCalismaTipi\",\"U_BarkodKalemOku\",\"U_TarihBazParti\",\"U_YetkiSifre\",\"U_UrnSrguFiyat\",\"U_UrnSrguFytList\",\"U_SubeSecimi\",\"U_PltYapDepSec\" from \"@AIF_WMS_GNLPRM\" ";
                     
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connstring))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    using (dt = new DataTable())
                                    {
                                        sda.Fill(dt);
                                        dt.TableName = "genelParametreList";
                                    }

                                }
                            }
                            con.Close();
                        } 
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -9999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };

                    }
                }
                else
                { 
                }
            }
            catch (Exception ex)
            {

            }
            return new Response { _list = dt, Val = 0 };
        } 
    }
}