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
    public class GetBatchCustomTable
    {
        public Response getBatchNextNumber(string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "SELECT TOP 1 T0.[DocEntry], T0.[U_BatchPrefix], T0.[U_StartNumber], T0.[U_NextNumber] FROM [dbo].[@AIF_WMS_BATCH]  T0";

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
                                        dt.TableName = "BatchNextNumber";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -555, Desc = "PARTİ NUMARASI GİRİŞİ YAPILMAMIŞTIR." };
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -9999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -9998, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response updateBatchNextNumber(string dbName, int docentry, int nextnumber, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "UPDATE [@AIF_WMS_BATCH] SET U_NextNumber = " + (nextnumber + 1) + " where DocEntry = " + docentry + "";

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
                                        dt.TableName = "UpdateBatchNextNumber";

                                        //if (dt.Rows.Count == 0)
                                        //{
                                        //    return new Response { _list = null, Val = -555, Desc = "PARTİ NUMARASI GÜNCELLENİRKEN HATA OLUŞTU." };
                                        //}
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -9999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -9998, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getPartiNoSorgula(string dbName, string itemcode, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName, mKod);

                if (connstring != "")
                {
                    var query = "SELECT CONVERT(varchar,(Count(T0.\"DistNumber\"))) as \"Parti\" FROM \"OBTN\" T0 WHERE T0.\"ItemCode\" = '" + itemcode + "' AND T0.\"CreateDate\" = '" + DateTime.Now.ToString("yyyyMMdd") + "' ";

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
                                        dt.TableName = "PartiNumarasiVarmi";

                                        if (dt != null)
                                        {
                                            int count = Convert.ToInt32(dt.Rows[0][0]);

                                            if (count == 0)
                                            {
                                                string partino = DateTime.Now.ToString("yyyyMMdd");

                                                dt.Rows[0][0] = partino;
                                            }
                                            else
                                            {
                                                string partino = DateTime.Now.ToString("yyyyMMdd") + "-" + count;
                                                dt.Rows[0][0] = partino;

                                            }
                                        } 
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Response { _list = null, Val = -9999, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response { _list = null, Val = -9998, Desc = "BİLİNMEYEN HATA OLUŞTU." + ex.Message };
            }
            return new Response { _list = dt, Val = 0 };
        }

    }
}