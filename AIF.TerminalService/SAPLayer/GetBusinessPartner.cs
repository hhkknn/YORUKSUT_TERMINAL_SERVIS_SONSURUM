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
    public class GetBusinessPartner
    {
        public Response getBusinessVendorPartner(string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {

                    var query = string.Format(@"select '' as 'CardCode', '' as 'CardName' UNION ALL select CardCode,  CardName as CardName from OCRD where CardType='S' and ISNULL(validFor,'Y') = 'Y'");
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
                                        dt.TableName = "businessVendorList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "SİSTEMDE SATICI BULUNAMADI." };
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getBusinessCustomerPartner(string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {

                    var query = string.Format(@"select '' as 'CardCode', '' as 'CardName' UNION ALL select CardCode, CardName as CardName from OCRD where CardType='C'");
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
                                        dt.TableName = "businessVendorList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "SİSTEMDE SATICI BULUNAMADI." };
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new Response { _list = dt, Val = 0 };
        }

        public Response getAllBusinessPartner(string dbName, string mKod)
        {
            DataTable dt = new DataTable();
            try
            {
                GetConnectionString n = new GetConnectionString();
                string connstring = n.getConnectionString(dbName,mKod);

                if (connstring != "")
                {

                    var query = "select '' as 'CardCode', '' as 'CardName' UNION ALL select CardCode, CardName as CardName from OCRD";
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
                                        dt.TableName = "businessAllList";

                                        if (dt.Rows.Count == 0)
                                        {
                                            return new Response { _list = null, Val = -111, Desc = "SİSTEMDE MUHATAP BULUNAMADI." };
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new Response { _list = dt, Val = 0 };
        }
    }
}