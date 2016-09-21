using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.IO.Compression;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Zebra.Communication;
using Zebra.Printing;

namespace OA_Movil
{
    public class Inventario
    {
        /// <summary>
        /// Agregamos cada uno de los codigos de barras asociados al inventario del cliente de manera local
        /// </summary>
        /// <param name="pVisita"></param>
        /// <param name="pProducto"></param>
        /// <param name="pDescripcion"></param>
        /// <param name="pCodigoBarras"></param>
        public void AgregarCodigoBarrasLocal(string pVisita, string pProducto, string pDescripcion, string pCodigoBarras)
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "INSERT INTO CodigosBarras (Visita, Producto, Descripcion, CodigoBarras) VALUES (@Visita, @Producto, @Descripcion, @CodigoBarras)";
                cmd.Parameters.AddWithValue("@Visita", pVisita);
                cmd.Parameters.AddWithValue("@Producto", pProducto);
                cmd.Parameters.AddWithValue("@Descripcion", pDescripcion);
                cmd.Parameters.AddWithValue("@CodigoBarras", pCodigoBarras);
                myConn.Open();
                cmd.ExecuteNonQuery();
            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
            } // catch
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally
        }

        /// <summary>
        /// Eliminamos todos los registros de codigos de barras locales existentes
        /// </summary>
        /// <returns></returns>
        public int EliminarCodigoBarrasLocal()
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            int resultado = -1;
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "DELETE FROM CodigosBarras";
                myConn.Open();
                resultado = cmd.ExecuteNonQuery();
            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    //string msj = err.Message.ToString();
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
                return resultado = -1;
            } // catch
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally
            return resultado;
        }

        /// <summary>
        /// Obtenemos la descripción del producto de manera local en función del codigo de barras
        /// </summary>
        /// <param name="pCodigoBarras"></param>
        /// <returns></returns>
        public string ObterDescripcionPorCodigo(string pCodigoBarras)
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            string resultado = "";
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "SELECT Producto +' / '+Descripcion FROM CodigosBarras WHERE CodigoBarras=@CodigoBarras ";
                cmd.Parameters.AddWithValue("@CodigoBarras", pCodigoBarras);
                myConn.Open();
                resultado = cmd.ExecuteScalar().ToString();

            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    //string msj = err.Message.ToString();
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
                return resultado = "Error SqlCE";
            } // catch
            catch (NullReferenceException)
            {
                return resultado = "NO ENCONTRADO";   
            }
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally
            return resultado;
        }

        /// <summary>
        /// Obtenemos la descripción del producto de manera local en función de la clave del producto
        /// </summary>
        /// <param name="pClave"></param>
        /// <returns></returns>
        public string ObterDescripcionPorClave(string pClave)
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            string resultado = "";
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "SELECT DISTINCT (Producto) +' / '+Descripcion FROM CodigosBarras WHERE Producto=@Clave ";
                cmd.Parameters.AddWithValue("@Clave", pClave);
                myConn.Open();
                resultado = cmd.ExecuteScalar().ToString();

            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    //string msj = err.Message.ToString();
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
                return resultado = "Error SqlCE";
            } // catch
            catch (NullReferenceException)
            {
                return resultado = "NO ENCONTRADO";
            }
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally
            return resultado;
        }

        /// <summary>
        /// Agrega a la base de datos local un producto registrado por codigo de barras y cantidad
        /// </summary>
        /// <param name="pVisita"></param>
        /// <param name="pCodigoBarras"></param>
        /// <param name="pCantidad"></param>
        /// <param name="pModoCaptura"></param>
        public void AgregarProductoPorCodigo(string pVisita, string pCodigoBarras, float pCantidad, string pModoCaptura)
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "INSERT INTO Conteos (Visita, Producto, Cantidad, ModoCaptura) VALUES (@Visita, @Producto, @Cantidad, @ModoCaptura)";
                cmd.Parameters.AddWithValue("@Visita", pVisita);
                cmd.Parameters.AddWithValue("@Producto", pCodigoBarras);
                cmd.Parameters.AddWithValue("@Cantidad", pCantidad);
                cmd.Parameters.AddWithValue("@ModoCaptura", pModoCaptura);
                myConn.Open();
                cmd.ExecuteNonQuery();
            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    //string msj = err.Message.ToString();
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
            } // catch
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally
        }

        /// <summary>
        /// Agrega a la base de datos local un producto registrado por clave de artpiculo
        /// </summary>
        /// <param name="pVisita"></param>
        /// <param name="pClave"></param>
        /// <param name="pCantidad"></param>
        /// <param name="pModoCaptura"></param>
        public void AgregarProductoPorClave(string pVisita, string pClave, float pCantidad, string pModoCaptura)
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "INSERT INTO Conteos (Visita, Producto, Cantidad, ModoCaptura) VALUES (@Visita, @Producto, @Cantidad, @ModoCaptura)";
                cmd.Parameters.AddWithValue("@Visita", pVisita);
                cmd.Parameters.AddWithValue("@Producto", pClave);
                cmd.Parameters.AddWithValue("@Cantidad", pCantidad);
                cmd.Parameters.AddWithValue("@ModoCaptura", pModoCaptura);
                myConn.Open();
                cmd.ExecuteNonQuery();

            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    //string msj = err.Message.ToString();
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
            } // catch
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally

        }

        /// <summary>
        /// Elimina de la base de datos local el producto registrado previamente
        /// </summary>
        /// <param name="pProducto"></param>
        public void EliminarProducto(int pIdProducto)
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "DELETE FROM Conteos WHERE ID=@IdProducto";
                cmd.Parameters.AddWithValue("@IdProducto", pIdProducto);
                myConn.Open();
                cmd.ExecuteNonQuery();

            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    //string msj = err.Message.ToString();
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
            } // catch
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally

        }

        /// <summary>
        /// Permite eliminar todos aquellos productos de la misma clave o codigo
        /// </summary>
        /// <param name="pProducto"></param>
        public void EliminarProductos(string pProducto)
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "DELETE FROM Conteos WHERE Producto=@Codigo";
                cmd.Parameters.AddWithValue("@Codigo", pProducto);
                myConn.Open();
                cmd.ExecuteNonQuery();

            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    //string msj = err.Message.ToString();
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
            } // catch
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally

        }

        /// <summary>
        /// Permite obtener los productos registrados en la base de datos local
        /// </summary>
        /// <returns></returns>
        public DataSet ObtenerConteosLocales()
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            DataSet ds = new DataSet();
            SqlCeDataAdapter da = new SqlCeDataAdapter();

            try
            {
    
                //Declaramos el comando para ejecutar el query            
                SqlCeCommand cmd = new SqlCeCommand("SELECT Visita, Producto, Cantidad, ModoCaptura FROM Conteos", conn);
                da.SelectCommand = cmd;
                //Abrimos conexin a BD
                cmd.Connection.Open();
                //Llenanos un DataSet con el resultado de la(s) consulta(s)
                da.Fill(ds);
                //Regresamos los datos encontrados
                return ds;
            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    return null;
                }
            } // catch
            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    //Cerramos conexin a BD
                    conn.Close();
                }
            }//finally
            return null;
        }

        /// <summary>
        /// Permite obtener aquellos conteos registrados localmente, por si se requiere eliminar alguno 
        /// de ellos antyes de ser enviados al servidor,
        /// </summary>
        /// <returns></returns>
        public DataSet ObtenerConteosLocalesEliminar()
        {
            SqlCeConnection conn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            DataSet ds = new DataSet();
            SqlCeDataAdapter da = new SqlCeDataAdapter();

            try
            {                
                SqlCeCommand cmd = new SqlCeCommand("SELECT ID, Producto, Cantidad FROM Conteos", conn);
                //SqlCeCommand cmd = new SqlCeCommand("SELECT a.ID, DISTINCT(a.Producto), b.Descripcion, a.Cantidad FROM Conteos a INNER JOIN CodigosBarras b ON a.Producto=b.Producto", conn);
                da.SelectCommand = cmd;
                //Abrimos conexin a BD
                cmd.Connection.Open();
                //Llenanos un DataSet con el resultado de la(s) consulta(s)
                da.Fill(ds);
                //Regresamos los datos encontrados
                return ds;
            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    return null;
                }
            } // catch
            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                {
                    //Cerramos conexin a BD
                    conn.Close();
                }
            }//finally
            return null;
        }

        /// <summary>
        /// Permite eliminar los productos registrados en la base de datos local
        /// </summary>
        /// <param name="pVisita"></param>
        /// <returns></returns>
        public int EliminarConteosVisita(string pVisita)
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            int resultado = -1;
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "DELETE FROM Conteos";
                myConn.Open();
                resultado = cmd.ExecuteNonQuery();

            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    //string msj = err.Message.ToString();
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
                return resultado = -1;
            } // catch
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally
            return resultado;
        }

        /// <summary>
        /// Regresa un Verdadero si la conexión (página) esta accesible en linea
        /// </summary>
        /// <returns></returns>
        public bool IsConnectionAvailable()
        {
            //Replace www.google.com with a site that is guaranteed to be online 
            //System.Uri objUrl = new Uri("http://www.google.com/");
            System.Uri objUrl = new Uri("http://200.56.117.88/ServicioOA/");
            //System.Uri objUrl = new Uri("http://200.56.117.82/ServicioOA/");
            //Setup WebRequest 
            System.Net.WebRequest objWebReq;
            objWebReq = System.Net.WebRequest.Create(objUrl);
            //System.Net.WebRequest.Create(objUrl);
            System.Net.WebResponse objResp;

            //Attempt to get response and return True 
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                objResp = objWebReq.GetResponse();
                objResp.Close();
                objWebReq = null;
                return true;
            }
            catch (Exception) //Error, exit and return False 
            {
                //MessageBox.Show(ex.Message);
                objWebReq = null;
                return false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }
       
        /// <summary>
        /// Funcion para validar si una cantidad es numerica
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns></returns>
        public bool IsNumeric(string strVal)
        {
            try
            {
                double val = double.Parse(strVal);
                if (val < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Permite enviar la cadena de strings directamente a la impresora
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        public bool EnviarDatosImpresora(StringBuilder pDatos, string pPuerto)
        {
            bool impresionCorrecta = false;
            SerialPort serialPort = new SerialPort(pPuerto);
            try
            {
                serialPort.Open();
                System.Threading.Thread.Sleep(9000);
                serialPort.WriteLine(pDatos.ToString());
                //System.Threading.Thread.Sleep(9000);
                serialPort.DiscardInBuffer();
                impresionCorrecta = true;
            }
            catch (Exception)
            {
                impresionCorrecta = false;
            }
            finally
            {
                serialPort.Close();
                serialPort.Dispose();
            } 
            return impresionCorrecta;
        }


        /// <summary>
        /// Permite enviar a la impresora los dostos del ticket, este método usa la
        /// libreria dll de zebra
        /// </summary>
        /// <param name="pTicket">Información que será impresa</param>
        /// <param name="pPuerto">Número de Puerto COM al que se enviará la impresión</param>
        public void EnviarDatosImpresora(CpclLabel pTicket, string pPuerto)
        {

            CommConfig cCommConfig = new CommConfig();
            cCommConfig.CommunicationMode = 2; //Bluetooth
            cCommConfig.BtPort = pPuerto;
            Comm cComm = new Comm(cCommConfig);
            
            
            bool conectado = cComm.IsConnected;
            try
            {
                Printer impresora = new Printer(cCommConfig);
                impresora.Print(pTicket);                
                cComm.Close();               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar datos a la mpresora: "+ex.Message);
            }
        }


        /// <summary>
        /// Permite contar los productos con codigo de barras registrados en la base de datos local
        /// </summary>
        /// <param name="pVisita"></param>
        /// <returns></returns>
        public int ExistenCodigosBarrasLocales(string pVisita)
        {
            SqlCeConnection myConn = new SqlCeConnection("Data Source =" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\AppDatabase.sdf;"));
            int resultado = -1;
            try
            {
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = myConn;
                cmd.CommandText = "SELECT COUNT(*) FROM CodigosBarras WHERE Visita=@Visita";
                cmd.Parameters.AddWithValue("@Visita", pVisita);
                myConn.Open();
                resultado = (Int32)cmd.ExecuteScalar();

            }//try
            catch (SqlCeException myexception)
            {
                foreach (SqlCeError err in myexception.Errors)
                {
                    //string msj = err.Message.ToString();
                    System.Windows.Forms.MessageBox.Show(err.Message);
                }
                return resultado = -1;
            } // catch
            finally
            {
                if (myConn != null && myConn.State != ConnectionState.Closed)
                {
                    //Cerramos conexión a BD
                    myConn.Close();
                }
            }//finally
            return resultado;
        }


        public List<string> GetAllPorts()
        {
            List<String> allPorts = new List<String>();            
            foreach (String portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                SerialPort serialPort = new SerialPort(portName);
                try
                {                    
                    serialPort.Open();
                    allPorts.Add(portName);
                }
                catch (Exception)
                {
                    //
                 }
                finally
                {
                    serialPort.Close();
                    serialPort.Dispose();
                } 
            }
            return allPorts;
        }

        #region Obsoletos

        public bool ValidaCantidad(string Cantidad)
        {
            //
            Regex ER = new Regex("([0-9]{1,3})");
            return (ER.IsMatch(Cantidad));
        }

        public bool InternetGetConnectedState()
        {
            try
            {
                string host = Dns.GetHostName();

                IPHostEntry entry = Dns.GetHostEntry(host);
                string hostIP = entry.AddressList[0].ToString();

                return (hostIP != IPAddress.Parse("127.0.0.1").ToString());
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DataSet DecompressData(Byte[] data)
        {
            //funcion para descomprimir el dataset que envia el webservice
            try
            {
                MemoryStream memStream = new MemoryStream(data);
                GZipStream unzipStream = new GZipStream(memStream, CompressionMode.Decompress);
                DataSet ds = new DataSet();
                ds.ReadXml(unzipStream, XmlReadMode.ReadSchema);
                unzipStream.Close();
                memStream.Close();
                return ds;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion


    }
}
