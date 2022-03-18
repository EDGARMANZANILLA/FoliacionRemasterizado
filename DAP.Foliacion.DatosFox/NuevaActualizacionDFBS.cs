
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAP.Foliacion.DatosFox
{

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////     ACTUALIZA UN REGISTRO EN UN DBF       //////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public class NuevaActualizacionDFBS
    {
       

        /// <summary>
        /// Metodo para actualizar una tabla en especifico pasando la ruta y el nombre del archivo, 
        /// </summary>
        /// <param name="RutaPath"></param>
        /// <param name="NombreArchivo"></param>
        /// <returns></returns>
        public static string ActualizarDBF_Sagitari_A_N(string RutaPath, string NombreArchivo, List<ResumenPersonalAFoliarDTO> ResumenPersonalFoliar)
         {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";

            int ElementosModificados = 0;

            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    con.Open();

                    foreach (ResumenPersonalAFoliarDTO nuevoRegistro in ResumenPersonalFoliar)
                    {
                       

                        string ExecutarQuery = "UPDATE ["+NombreArchivo+"] SET Num_che = '"+nuevoRegistro.NumChe+"', Banco_x = '"+nuevoRegistro.BancoX+"', Cuenta_x = '"+nuevoRegistro.CuentaX+"' , Observa = '"+nuevoRegistro.Observa+"' WHERE NUM = '"+nuevoRegistro.CadenaNumEmpleado+"' and RFC = '"+nuevoRegistro.RFC+"' and LIQUIDO = "+nuevoRegistro.Liquido+" and DELEG = '"+nuevoRegistro.Delegacion+"' ";
                    
                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);

                        int modificado = cmd.ExecuteNonQuery();

                        if (modificado > 0)
                        {
                            ElementosModificados += modificado;
                        }


                    }
                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

                Debug.WriteLine("Ocurrio un error :" + E.Message.ToString());
            }
            return Convert.ToString(ElementosModificados);
        }




        public static string ActualizarDBF_Pagomatico_Prueba_NO_UTILIZABLE(string RutaPath, string NombreArchivo, List<ResumenPersonalAFoliarDTO> ResumenPersonalFoliar, bool EsPena)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


           // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + RutaPath +";Extended Properties=dBASE 5.0;";
           
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";
            int ElementosModificados = 0;

            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    con.Open();

                    foreach (ResumenPersonalAFoliarDTO nuevoRegistro in ResumenPersonalFoliar)
                    {
                        //if (nuevoRegistro.CadenaNumEmpleado == "22125") 
                        //{
                        //    int a  = 0;
                        //}

                        string ExecutarQuery = "";
                        if (EsPena)
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "'  and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' and Benef = '" + nuevoRegistro.NumBeneficiario + "' ";

                        }
                        else
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' ";

                        }



                        //string ExecutarQuery = "UPDATE ["+NombreArchivo+"] SET [Num_che = '"+nuevoRegistro.NumChe+"', Banco_x = '"+nuevoRegistro.BancoX+"', Cuenta_x = '"+nuevoRegistro.CuentaX+"' , Observa = '"+nuevoRegistro.Observa+"' WHERE NUM = '"+nuevoRegistro.CadenaNumEmpleado+"' and RFC = '"+nuevoRegistro.RFC+"' and LIQUIDO = "+nuevoRegistro.Liquido+" and DELEG = '"+nuevoRegistro.Delegacion+"' ";

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        //var a2 = cmd.Parameters.AddWithValue("@RFC", nuevoRegistro.RFC);
                        int modificado = cmd.ExecuteNonQuery();

                        if (modificado > 0)
                        {
                            ElementosModificados += modificado;
                        }


                    }
                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

                Debug.WriteLine("Ocurrio un error :" + E.Message.ToString());
            }
            return Convert.ToString(ElementosModificados);
        }










        public static async Task<string> ActualizarDBF_Pagomaticos(string RutaPath, string NombreArchivo, List<ResumenPersonalAFoliarDTO> ResumenPersonalFoliar, bool EsPena)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";
            string constr = "Provider=VFPOLEDB.1; Data Source=" + RutaPath + "";
            List<Task> tareas = new List<Task>();
            int ElementosModificados = 0;

            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {



                    await con.OpenAsync(); 
                    foreach (ResumenPersonalAFoliarDTO nuevoRegistro in ResumenPersonalFoliar)
                    {
                        

                        string ExecutarQuery = "";
                        if (EsPena)
                        {
                            ExecutarQuery = "UPDATE ["+NombreArchivo+"] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and RFC = '" + nuevoRegistro.RFC + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' and Benef = '" + nuevoRegistro.NumBeneficiario + "' ";

                        }
                        else
                        {
                            ExecutarQuery = "UPDATE ["+NombreArchivo+"] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and RFC = '" + nuevoRegistro.RFC + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' ";

                        }

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        //var a2 = cmd.Parameters.AddWithValue("@RFC", nuevoRegistro.RFC);
                        int modificado = await cmd.ExecuteNonQueryAsync();

                        if (modificado == 1)
                        {
                            ElementosModificados += modificado;
                        }

                        //tareas.Add(new OleDbCommand(ExecutarQuery, con).ExecuteNonQueryAsync());

                    }




                      //await Task.WhenAll(tareas);

                    con.Close();

                  
                    //return Convert.ToString(tareas.Count());
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
               return E.Message.ToString();

                //Debug.WriteLine("Ocurrio un error :" + E.Message.ToString());
            }
           return Convert.ToString(ElementosModificados);
        }


        public static async Task<string> ActualizarDBF_Cheques(string RutaPath, string NombreArchivo, List<ResumenPersonalAFoliarDTO> ResumenPersonalFoliar, bool EsPena)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";
            List<Task> tareas = new List<Task>();
            

            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {



                    await con.OpenAsync();
                    foreach (ResumenPersonalAFoliarDTO nuevoRegistro in ResumenPersonalFoliar)
                    {
                        string ExecutarQuery = "";
                        if (EsPena)
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' and Benef = '" + nuevoRegistro.NumBeneficiario + "' ";

                        }
                        else
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '" + nuevoRegistro.NumChe + "', Banco_x = '" + nuevoRegistro.BancoX + "', Cuenta_x = '" + nuevoRegistro.CuentaX + "' , Observa = '" + nuevoRegistro.Observa + "' WHERE NUM = '" + nuevoRegistro.CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.Liquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' ";

                        }

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        //var a2 = cmd.Parameters.AddWithValue("@RFC", nuevoRegistro.RFC);
                        int modificado = await cmd.ExecuteNonQueryAsync();

                        if (modificado == 1)
                        {
                            totalActualizado += modificado;
                        }

                       // tareas.Add(new OleDbCommand(ExecutarQuery, con).ExecuteNonQueryAsync());

                    }

                   // await Task.WhenAll(tareas);

                     con.Close();


                   // return Convert.ToString(tareas.Count());
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

                //Debug.WriteLine("Ocurrio un error :" + E.Message.ToString());
            }
            return Convert.ToString( totalActualizado);
        }


        public static string SuspenderPagomatico(string RutaPath, string NombreArchivo, Tbl_Pagos nuevoRegistro, bool EsPena, string CadenaNumEmpleado)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";
            List<Task> tareas = new List<Task>();


            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    con.Open();
                   
                        string ExecutarQuery = "";
                        if (EsPena)
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '11111111',  Observa = 'TALON POR CHEQUE' , TALONXCH = 1 WHERE NUM = '" + CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.ImporteLiquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' and Benef = '" + nuevoRegistro.NumBeneficiario + "' ";
                        }
                        else
                        {
                            ExecutarQuery = "UPDATE [" + NombreArchivo + "] SET Num_che = '11111111',  Observa = 'TALON POR CHEQUE' , TALONXCH = 1  WHERE NUM = '" + CadenaNumEmpleado + "' and LIQUIDO = " + nuevoRegistro.ImporteLiquido + " and DELEG = '" + nuevoRegistro.Delegacion + "' ";
                        }

                        OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                        int modificado = cmd.ExecuteNonQuery();

                        if (modificado == 1)
                        {
                            totalActualizado += modificado;
                        }

                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

            }
            return Convert.ToString(totalActualizado);
        }

        public static string ReponerFormaPago(string RutaPath, string ExecutarQuery)
        {
            //EjemploDePath:  string pathPruebaSERVER208 = @"F:\SAGITARI\GENERAL\ARCHIVOS\";


            // string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+RutaPath+";Extended Properties=dBASE 5.0;";
            string constr = "Provider=VFPOLEDB.1 ;Data Source=" + RutaPath + "";
            List<Task> tareas = new List<Task>();


            int totalActualizado = 0;
            try
            {
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    con.Open();

                    OleDbCommand cmd = new OleDbCommand(ExecutarQuery, con);
                    int modificado = cmd.ExecuteNonQuery();

                    if (modificado == 1)
                    {
                        totalActualizado += modificado;
                    }

                    con.Close();
                }
            }
            catch (System.Data.OleDb.OleDbException E)
            {
                return E.Message.ToString();

            }
            return Convert.ToString(totalActualizado);
        }

    }
}
