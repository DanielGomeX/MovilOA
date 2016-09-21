using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Zebra.Printing;
using Zebra.Communication;

namespace OA_Movil
{
    public partial class frmConteos : Form
    {

        Inventario metodos = new Inventario();
        DataSet dsPreFactura = new DataSet();
        DataSet dsDiferencias = new DataSet();
        DataSet dsRestantes = new DataSet();
        DataTable dtRestantes = new DataTable();
        DataTable dtDiferencias = new DataTable();
        List<String> ProductosEliminar = new List<String>();

        public string Cliente { get; set; }
        public string NombreCliente { get; set; }
        public string Visita { get; set; }
        public string Producto { get; set; }        
        public float Cantidad { get; set; }
        public string StatusVisita { get; set; }
        public string Recibo { get; set; }

        private string puertoCOM { get; set; }

        //private string puertoCOM = "";
        private string ultimoProducto = "";
        private int contador = 0;
        private int IdProducto = 0;
        private int prefacturaImpresa=0; //Sirve para contabilizar las veces que se ha impreso la PreFactura
        private string importeRecibo;
        private string importeSaldo;
        private string nombreDispositivo = "";

        private decimal subtotalIVA16 = 0;
        private decimal subtotalIEPS3 = 0;
        private decimal IEPS3 = 0;
        private decimal IVA16 = 0;

        //Produccion
        ServicioOA.ServicioOA wsoa = new ServicioOA.ServicioOA();

        //Pruebas
        //ServicioPrueba.ServicioOA wsoa = new ServicioPrueba.ServicioOA();
      
        //
        public frmConteos()
        {
            InitializeComponent();
        }

        #region Metodos
        
        //
        private void IniciarConteo()
        {
            if (txtCliente.Text == "")
            {
                MessageBox.Show("Introduzca el número de cliente.");
                txtCliente.Focus();
            }
            else
            {
                if (metodos.IsConnectionAvailable())
                {
                    
                    this.Cliente = txtCliente.Text.Trim();

                    DataSet dsVisita = new DataSet();
                    DataRow drVisita;
                    Cursor.Current = Cursors.WaitCursor;
                    dsVisita = wsoa.IniciarVisita(this.Cliente);
                    

                    if (dsVisita != null)
                    {
                        if (dsVisita.Tables[0].Rows.Count == 1)
                        {
                            drVisita = dsVisita.Tables[0].Rows[0];
                            this.Visita = drVisita["IdVisita"].ToString();
                            this.Cliente = drVisita["CustId"].ToString();
                            this.NombreCliente = drVisita["Name"].ToString();
                            this.StatusVisita = drVisita["ShipperId"].ToString();
                            this.importeSaldo = drVisita["Saldo"].ToString();
                            txtVisita.Text = this.Visita;

                            if (this.StatusVisita == "Pendiente")
                            {
                                //Verificamos si previamente ya existen Codigos de Barras copiados localmente, para
                                //no volver a copiar datos del servidor
                                int codigosBarrasLocales = metodos.ExistenCodigosBarrasLocales(this.Visita);

                                MessageBox.Show("Codigos existentes localmente: " + codigosBarrasLocales.ToString());

                                if (codigosBarrasLocales <= 0)
                                {
                                    //
                                    int codigos = ObtenerCodigosBarras();
                                    MessageBox.Show("Codigos copiados localmente: " + codigos.ToString());
                                }
                                //else
                                //{
                                //    MessageBox.Show("Codigos localmente existentes: " + codigosBarrasLocales.ToString());
                                //}

                                this.Text = this.Visita;
                                txtSaldo.Text = this.importeSaldo;
                                tcConteos.SelectedIndex = 1;
                                txtCliente.Text = "";
                                pnlConteos.Enabled = true;
                                pnlEliminar.Enabled = true;
                                pnlLogin.Enabled = false;
                                txtSupermercado.Focus();
                            }
                            else
                            {
                                txtVisita.Text = this.Visita;
                                ObtenerPreFactura();
                                pnlLogin.Enabled = false;
                                pnlConteos.Enabled = false;
                                pnlRestantes.Enabled = false;
                                pnlPreFactura.Enabled = true;
                                btnFinalizar.Text = "RE-IMPRESION";
                                MessageBox.Show("La visita ya está finalizada, Solo puede reimprimir el balance. ");
                                tcConteos.SelectedIndex = 5;
                                txtSaldo.Text = this.importeSaldo;
                            }
                           
                        }
                    }
                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    MessageBox.Show("No se ha podido establecer una conexión con el Servicio Web de la aplicación, favor de verificarlo...");
                    txtCliente.Focus();
                }
            }
        }

        private void ObtenerParametrosDispositivo()
        {
            DataSet dsParametros = new DataSet();
            DataRow drParametros;


            dsParametros = wsoa.ObtenerParametros(txtNombreEquipo.Text);

            if (dsParametros.Tables[0].Rows.Count != 0)
            {
                drParametros = dsParametros.Tables[0].Rows[0];
                puertoCOM = drParametros["PuertoImpresion"].ToString().Trim();
            }
        }

        private int Contador(string pProducto)
        {
            if (ultimoProducto==pProducto)
            {
                contador++;
                return contador;
            }
            else
            {                
                ultimoProducto = pProducto;
                contador = 1;
                return contador;
            }
        }

        //
        private void ObtenerPreFactura()
        {
            //DataRow drImporte;
            dgPreFactura.DataSource = null;
            if (metodos.IsConnectionAvailable())
            {
                dsPreFactura = wsoa.ObtenerPreFactura(Visita);
                if (dsPreFactura != null)
                {
                    if (dsPreFactura.Tables[0].Rows.Count != 0)
                    {
                        dgPreFactura.DataSource = dsPreFactura.Tables[0].DefaultView;
                        //drImporte = dsPreFactura.Tables[1].Rows[0];
                        //subtotalIVA16 = drImporte[0].ToString();
                        //subttalIEPS3 = drImporte[1].ToString();
                        //IEPS3 = drImporte[2].ToString();
                        //IVA16 = drImporte[3].ToString();
                        //txtCantDifConteos.Text = "$" + drImporte[0].ToString();
                    }
                }
                btnFinalizar.Enabled = true;
            }
            else
            {
                MessageBox.Show("No se ha podido establecer una conexión con el Servicio Web de la aplicación, verifique si pude navegar en internet, en caso de que si, favor de hablar a TI.");                
            }
        }

        //
        private void ObtenerDiferencias()
        {
            dgDiferencias.DataSource = null;
            if (metodos.IsConnectionAvailable())
            {
                dsDiferencias = wsoa.ObtenerDiferenciasConteo(this.Visita);
                if (dsDiferencias != null)
                {
                    if (dsDiferencias.Tables[0].Rows.Count != 0)
                    {
                        dtDiferencias = dsDiferencias.Tables[0].Copy();
                        dgDiferencias.DataSource = dtDiferencias;
                    }
                }
            }
            else
            {
                MessageBox.Show("No se ha podido establecer una conexión con el Servicio Web de la aplicación, verifique si pude navegar en internet, en caso de que si, favor de hablar a TI.");
            }
        }


        private void ObtenerBalance()
        {
            DataSet dsBalance = new DataSet();
            if (metodos.IsConnectionAvailable())
            {
                dsBalance = wsoa.ObtenerBalance(this.Visita);
            }
            else
            {
                MessageBox.Show("No se ha podido establecer una conexión con el Servicio Web de la aplicación, verifique si pude navegar en internet, en caso de que si, favor de hablar a TI.");
            }
        }

        //
        private bool ImprimirPreFactura()
        {

            StringBuilder cad = new StringBuilder();
            char rellenarcon = ' ';
            bool ImpresionCorrecta = false;
            
            //decimal importe = 0;
            //decimal iva = 0;
            decimal total = 0;
            //decimal pctIva = 0.16M;

            if (metodos.IsConnectionAvailable())
            {
                try
                {

                    Cursor.Current = Cursors.WaitCursor;
                    cad.AppendLine(" ");
                    cad.AppendLine(" ");
                    cad.AppendLine("      HERRAMIENTAS HECORT S.A. DE C.V.          ");
                    cad.AppendLine("             PRE FACTURA                        ");
                    cad.AppendLine("    Fecha: " + DateTime.Now.ToLongDateString());
                    cad.AppendLine("     Hora: " + DateTime.Now.ToShortTimeString());
                    cad.AppendLine("   Visita: " + this.Visita);
                    cad.AppendLine("  Cliente: " + this.Cliente);
                    cad.AppendLine("   Nombre: " + this.NombreCliente);
                    cad.AppendLine("================================================");
                    //12345678901234567890123456789012345678901234567890
                    cad.AppendLine("Producto      Uds  Precio  Pct   Precio         ");
                    cad.AppendLine("Clave         Vend  Lista  Desc   Venta  Importe");
                    cad.AppendLine("================================================");
                    foreach (DataRow dr in dsPreFactura.Tables[0].Rows)
                    {
                        cad.AppendLine(dr["Descripcion"].ToString().Trim());
                        cad.Append(dr["Clave"].ToString().Trim().PadRight(15, rellenarcon));
                        cad.Append(dr["Desplazado"].ToString().Trim().PadRight(4, rellenarcon));
                        cad.Append("$" + Math.Round(Convert.ToDecimal(dr["PrecioLista"].ToString().Trim()), 2).ToString().Trim().PadRight(7, rellenarcon));
                        cad.Append(dr["Descuento"].ToString().Trim() + "%".PadRight(4, rellenarcon));
                        cad.Append("$" + Math.Round(Convert.ToDecimal(dr["PrecioVenta"].ToString().Trim()), 2).ToString().Trim().PadRight(7, rellenarcon));
                        cad.Append("$" + Math.Round(Convert.ToDecimal(dr["Importe"].ToString().Trim()), 2).ToString().Trim().PadLeft(7, rellenarcon));
                        cad.Append("------------------------------------------------\r");
                    }
                    //Calculamos los totales
                    DataRow drSubtotales;
                        

                    //if (dsPreFactura.Tables[1].Rows.Count > 0)
                    //{ 
                        //Obtenemos los subtotales de la PreFactura (2do query del SP)
                        drSubtotales = dsPreFactura.Tables[1].Rows[0];



                        //Si el importe no es nulo, entonces realizamos el cálculo de los impuestos
                        if (!drSubtotales.IsNull(0))
                        {
                            subtotalIVA16 = Convert.ToDecimal(drSubtotales[0].ToString());
                            subtotalIEPS3 = Convert.ToDecimal(drSubtotales[1].ToString());
                            IEPS3 = Convert.ToDecimal(drSubtotales[2].ToString());
                            IVA16 = Convert.ToDecimal(drSubtotales[3].ToString());

                            //importe = Convert.ToDecimal(drImporte[0].ToString());
                            //iva = (pctIva * importe);
                            total = subtotalIVA16 + subtotalIEPS3 + IEPS3 + IVA16;

                        }
                    //}

                    this.importeSaldo = total.ToString();

                    cad.AppendLine("  ");
                    cad.AppendLine("                      Subtotal IVA 16: $" + Math.Round(subtotalIVA16, 2).ToString().PadLeft(7, rellenarcon));
                    cad.AppendLine("                      Subtotal IEPS 3: $" + Math.Round(subtotalIEPS3, 2).ToString().PadLeft(7, rellenarcon));
                    cad.AppendLine("                            IEPS (3%): $" + Math.Round(IEPS3, 2).ToString().PadLeft(7, rellenarcon));
                    cad.AppendLine("                            IVA (16%): $" + Math.Round(IVA16, 2).ToString().PadLeft(7, rellenarcon));
                    cad.AppendLine("                                Total: $" + Math.Round(total, 2).ToString().PadLeft(7, rellenarcon));
                    cad.AppendLine("  ");
                    cad.AppendLine("  ");
                    cad.AppendLine("  ");
                    cad.AppendLine("    ____________________________________       ");
                    cad.AppendLine("        NOMBRE Y FIRMA DE ACEPTACION           ");
                    cad.AppendLine("  ");

                    if (metodos.EnviarDatosImpresora(cad, puertoCOM))
                    {
                        ImpresionCorrecta = true;
                    }
                    else
                    {
                        ImpresionCorrecta = false;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error desconodido al tartar de generar el reporte de impresión de la PreFactura. ");
                    ImpresionCorrecta = false;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }

            }
            else
            {
                MessageBox.Show("No hay conexión a Internet, intente nuevamente. ");
                ImpresionCorrecta = false;
            }
            return ImpresionCorrecta;
        }

        private void ImprimirTicketPreFactura()
        {
            CpclLabel Ticket = new CpclLabel();

            int PosX = 0;
            int PosY = 0;

            DataSet dsBalance = new DataSet();
            char rellenarcon = ' ';
            decimal total = 0;

            Ticket.FormFeed = false;

            try
            {
                ArrayList encabezado = new ArrayList();
                encabezado.Add("************************************************");
                encabezado.Add("      HERRAMIENTAS HECORT S.A. DE C.V.          ");
                encabezado.Add("                 PRE FACTURA                    ");
                encabezado.Add("    Fecha: " + DateTime.Now.ToLongDateString());
                encabezado.Add("     Hora: " + DateTime.Now.ToShortTimeString());
                encabezado.Add("   Visita: " + this.Visita);
                encabezado.Add("  Cliente: " + this.Cliente);
                encabezado.Add("   Nombre: " + this.NombreCliente);
                encabezado.Add("                                                ");
                encabezado.Add("================================================");
                encabezado.Add("Producto      Uds  Precio  Pct   Precio         ");
                encabezado.Add("Clave         Vend  Lista  Desc   Venta  Importe");
                encabezado.Add("================================================");

                CpclTextItem Encabezado;
                foreach (Object linea in encabezado)
                {
                    PosY = (PosY + 25);
                    Encabezado = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, 0, PosY, linea.ToString());
                    Ticket.Add(Encabezado);
                }

                /*
                string descripcion = "";
                string clave = "";
                string desplazado = "";
                string precioLista = "";
                string descuento = "";
                string precioVenta = "";
                string importe = "";
                string partida = "";
                 */ 

                ArrayList partidas = new ArrayList();
                foreach (DataRow dr in dsPreFactura.Tables[0].Rows)
                {
                    string descripcion = dr["Descripcion"].ToString().Trim();

                    if (descripcion.Length > 45)
                    {
                        descripcion = descripcion.Substring(1, 45);
                    }
                    partidas.Add(descripcion);

                    string clave = dr["Clave"].ToString().Trim().PadRight(15, rellenarcon);
                    string desplazado = dr["Desplazado"].ToString().Trim().PadRight(4, rellenarcon);
                    string precioLista = "$" + Math.Round(Convert.ToDecimal(dr["PrecioLista"].ToString().Trim()), 2).ToString().Trim().PadRight(7, rellenarcon);
                    string descuento = dr["Descuento"].ToString().Trim() + "%".PadRight(4, rellenarcon);
                    string precioVenta = "$" + Math.Round(Convert.ToDecimal(dr["PrecioVenta"].ToString().Trim()), 2).ToString().Trim().PadRight(7, rellenarcon);
                    string importe = "$" + Math.Round(Convert.ToDecimal(dr["Importe"].ToString().Trim()), 2).ToString().Trim().PadLeft(7, rellenarcon);

                    string partida = clave + desplazado + precioLista + descuento + precioVenta + importe;
                    partidas.Add(partida);
                    partidas.Add("");
                }

                CpclTextItem Partida;
                foreach (Object linea in partidas)
                {
                    PosY = (PosY + 25);
                    Partida = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, PosX, PosY, linea.ToString());
                    Ticket.Add(Partida);
                }

                /***************************************************************************************/
                //Calculamos los totales
                //Obtenemos los subtotales de la PreFactura (2do query del SP)
                DataRow drSubtotales = dsPreFactura.Tables[1].Rows[0];

                //Si el importe no es nulo, entonces realizamos el cálculo de los impuestos
                if (!drSubtotales.IsNull(0))
                {
                    subtotalIVA16 = Convert.ToDecimal(drSubtotales[0].ToString());
                    subtotalIEPS3 = Convert.ToDecimal(drSubtotales[1].ToString());
                    IEPS3 = Convert.ToDecimal(drSubtotales[2].ToString());
                    IVA16 = Convert.ToDecimal(drSubtotales[3].ToString());
                    total = subtotalIVA16 + subtotalIEPS3 + IEPS3 + IVA16;
                }
                this.importeSaldo = total.ToString();

                ArrayList piePagina = new ArrayList();
                piePagina.Add("  ");
                piePagina.Add("                      Subtotal IVA 16: $" + Math.Round(subtotalIVA16, 2).ToString().PadLeft(7, rellenarcon));
                piePagina.Add("                      Subtotal IEPS 3: $" + Math.Round(subtotalIEPS3, 2).ToString().PadLeft(7, rellenarcon));
                piePagina.Add("                            IEPS (3%): $" + Math.Round(IEPS3, 2).ToString().PadLeft(7, rellenarcon));
                piePagina.Add("                            IVA (16%): $" + Math.Round(IVA16, 2).ToString().PadLeft(7, rellenarcon));
                piePagina.Add("                                Total: $" + Math.Round(total, 2).ToString().PadLeft(7, rellenarcon));
                piePagina.Add("  ");
                piePagina.Add("  ");
                piePagina.Add("  ");
                piePagina.Add("    ____________________________________       ");
                piePagina.Add("        NOMBRE Y FIRMA DE ACEPTACION           ");
                piePagina.Add("  ");

                CpclTextItem PiePagina;
                foreach (Object linea in piePagina)
                {
                    PosY = (PosY + 25);
                    PiePagina = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, PosX, PosY, linea.ToString());
                    Ticket.Add(PiePagina);
                }

                Ticket.Height = PosY + 100;
                //Mandamos a imprimir el ticket
                metodos.EnviarDatosImpresora(Ticket, puertoCOM);

            }
            catch (Exception)
            {
                MessageBox.Show("Error desconodido al tartar de generar el ticket de impresión. ");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }

        //
        private bool ImprimirBalance()
        {

            StringBuilder cad = new StringBuilder();
            DataSet dsBalance = new DataSet();
            char rellenarcon = ' ';
            float invAnterior = 0;
            float invInicial = 0;
            float entregas = 0;
            bool ImpresionCorrecta = false;


            if (metodos.IsConnectionAvailable())
            {
                dsBalance = wsoa.ObtenerBalance(this.Visita);

                if (dsBalance != null)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        cad.AppendLine("************************************************");
                        cad.AppendLine("      HERRAMIENTAS HECORT S.A. DE C.V.          ");
                        cad.AppendLine("                 BALANCE                        ");
                        cad.AppendLine("    Fecha: " + DateTime.Now.ToLongDateString());
                        cad.AppendLine("     Hora: " + DateTime.Now.ToShortTimeString());
                        cad.AppendLine("   Visita: " + this.Visita);
                        cad.AppendLine("  Cliente: " + this.Cliente);
                        cad.AppendLine("   Nombre: " + this.NombreCliente);
                        cad.AppendLine("================================================");
                                     //12345678901234567890123456789012345678901234567890
                        //cad.AppendLine("Desc. Producto");
                        cad.AppendLine("Clave        I.Ant. Entreg. I.Ini. Desp  I.Fin  ");
                        cad.AppendLine("================================================");
                        foreach (DataRow dr in dsBalance.Tables[0].Rows)
                        {
                            //cad.AppendLine(dr["Descripcion"].ToString().Trim());
                            cad.Append(dr["InvtId"].ToString().Trim().PadRight(14, rellenarcon));
                            //Calculamos las unidades Entregadas que es igual a la resta del Inventario
                            //actual menos el inventario anterior
                            invAnterior = Convert.ToSingle(dr["InventarioAnterior"]);
                            invInicial = Convert.ToSingle(dr["InventarioInicial"]);
                            entregas = invInicial - invAnterior;

                            //Inv. Anterior
                            cad.Append(invAnterior.ToString().Trim().PadRight(7, rellenarcon));
                            //Entregas
                            cad.Append((entregas.ToString().Trim().PadRight(7, rellenarcon)));
                            //Inv. Inicial
                            cad.Append(invInicial.ToString().Trim().PadRight(7, rellenarcon));

                            cad.Append((dr["Desplazado"].ToString().Trim().PadRight(7, rellenarcon)));
                            cad.Append((dr["InventarioFinal"].ToString().Trim().PadRight(6, rellenarcon)));                            
                            cad.Append("\r");                            
                        }
                        cad.AppendLine("==============================================");
                        cad.Append("\r");
                        cad.AppendLine("  ");
                        cad.AppendLine("    ____________________________________       ");
                        cad.AppendLine("        NOMBRE Y FIRMA DE ACEPTACION           ");
                        cad.AppendLine("  ");
                        cad.AppendLine("  ");

                        if (metodos.EnviarDatosImpresora(cad, puertoCOM))
                        {
                            ImpresionCorrecta = true;
                        }
                        else
                        {
                            ImpresionCorrecta = false;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error desconodido al tartar de generar el reporte de impresión. ");
                        ImpresionCorrecta = false;
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
                else
                {
                    MessageBox.Show("Error al obtener datos para imprimir");
                }
            }
            else
            {
                MessageBox.Show("No hay conexión a Internet, intente nuevamente. ");
                ImpresionCorrecta = false;
            }
            return ImpresionCorrecta;
        }

        private void ImprimirTicketBalance()
        {
            CpclLabel Ticket = new CpclLabel();
            DataSet dsBalance = new DataSet();

            int PosX = 0;
            int PosY = 0;            
            char rellenarcon = ' ';
            float invAnterior = 0;
            float invInicial = 0;
            float entregas = 0;

            

            Ticket.FormFeed = false;

            ArrayList encabezado = new ArrayList();
            encabezado.Add("************************************************");
            encabezado.Add("      HERRAMIENTAS HECORT S.A. DE C.V.          ");
            encabezado.Add("                 BALANCE                        ");
            encabezado.Add("    Fecha: " + DateTime.Now.ToLongDateString());
            encabezado.Add("     Hora: " + DateTime.Now.ToShortTimeString());
            encabezado.Add("   Visita: " + this.Visita);
            encabezado.Add("  Cliente: " + this.Cliente);
            encabezado.Add("   Nombre: " + this.NombreCliente);
            encabezado.Add("                                                ");
            encabezado.Add("================================================");
            encabezado.Add("Clave        I.Ant. Entreg. I.Ini. Desp  I.Fin  ");
            encabezado.Add("================================================");

            CpclTextItem Encabezado;
            foreach (Object linea in encabezado)
            {
                PosY = (PosY + 25);
                Encabezado = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, 0, PosY, linea.ToString());
                Ticket.Add(Encabezado);
            }

            ArrayList partidas = new ArrayList();

            string producto = "";
            string iAnterior = "";
            string iInicial = "";
            string entrega = "";
            string desplazado = "";
            string iFinal = "";
            string renglon = "";

            dsBalance = wsoa.ObtenerBalance(this.Visita);
            Cursor.Current = Cursors.WaitCursor;
            foreach (DataRow dr in dsBalance.Tables[0].Rows)
            {

                producto=(dr["InvtId"].ToString().Trim().PadRight(14, rellenarcon));

                //Calculamos las unidades Entregadas que es igual a la resta del Inventario actual menos el inventario anterior
                invAnterior = Convert.ToSingle(dr["InventarioAnterior"]);
                invInicial = Convert.ToSingle(dr["InventarioInicial"]);
                entregas = invInicial - invAnterior;

                iAnterior = invAnterior.ToString().Trim().PadRight(7, rellenarcon);               
                entrega = entregas.ToString().Trim().PadRight(7, rellenarcon);
                iInicial = invInicial.ToString().Trim().PadRight(7, rellenarcon);
                desplazado = dr["Desplazado"].ToString().Trim().PadRight(7, rellenarcon);
                iFinal = dr["InventarioFinal"].ToString().Trim().PadRight(6, rellenarcon);

                renglon = producto + iAnterior + entrega + iInicial + desplazado + iFinal;
                partidas.Add(renglon);
            }

            CpclTextItem Partida;
            foreach (Object linea in partidas)
            {
                PosY = (PosY + 25);
                Partida = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, PosX, PosY, linea.ToString());
                Ticket.Add(Partida);
            }


            ArrayList piePagina = new ArrayList();
            piePagina.Add("==============================================");
            piePagina.Add("\r");
            piePagina.Add("\r");
            piePagina.Add("    ____________________________________       ");
            piePagina.Add("        NOMBRE Y FIRMA DE ACEPTACION           ");
            piePagina.Add("\r");
            piePagina.Add("\r");
            piePagina.Add("================================================");
            piePagina.Add("Manifiesto que junto con el Proveedor he revisa_");
            piePagina.Add("do todos y cada uno de los productos que han    ");
            piePagina.Add("quedado detallados en este documento, los cuales");
            piePagina.Add("me fueron entregados nuevos y en su empaque y/o ");
            piePagina.Add("presentación comercial comprometiéndome a cubrir ");
            piePagina.Add("el importe de los mismos en los términos y condi_");
            piePagina.Add("ciones que se mencionen en la factura respectiva,");
            piePagina.Add("reconociendo que el presente listado es una rela_");
            piePagina.Add("ción actualizada de los productos que he recibido");
            piePagina.Add("del Proveedor, sustituyendo en consecuencia  a   ");
            piePagina.Add("cualquier otro inventario  que se haya expedido  ");
            piePagina.Add("con anterioridad, firmando al efecto a mi entera ");
            piePagina.Add("satisfacción.                                    ");

            CpclTextItem PiePagina;
            foreach (Object linea in piePagina)
            {
                PosY = (PosY + 25);
                PiePagina = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, PosX, PosY, linea.ToString());
                Ticket.Add(PiePagina);
            }
           

            Ticket.Height = PosY + 100;
            //Mandamos a imprimir el ticket
            metodos.EnviarDatosImpresora(Ticket, puertoCOM);

            Cursor.Current = Cursors.Default;       
        }

        //
        private bool ImprimirDiferencias()
        {

            StringBuilder cad = new StringBuilder();
            char rellenarcon = ' ';
            bool ImpresionCorrecta = false;

                if (dsDiferencias != null)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        cad.AppendLine("================================================");
                        cad.AppendLine("Producto");
                                     //"1234567890123412345678123456781234567812345678
                        cad.AppendLine("Clave         Inic.   Desp.   Excedte     Final ");
                        cad.AppendLine("================================================");
                        foreach (DataRow dr in dsDiferencias.Tables[0].Rows)
                        {
                            cad.AppendLine(dr["Descripcion"].ToString().Trim());
                            cad.Append(dr["InvtId"].ToString().Trim().PadRight(15, rellenarcon));
                            cad.Append(" "+(dr["InventarioInicial"].ToString().Trim().PadRight(8, rellenarcon)));
                            cad.Append((dr["Desplazado"].ToString().Trim().PadRight(8, rellenarcon)));
                            cad.Append((dr["Excedente"].ToString().Trim().PadRight(8, rellenarcon)));
                            cad.Append((dr["Existencia"].ToString().Trim().PadLeft(6, rellenarcon))+"  ");
                            cad.Append("-----------------------------------------------\r");
                        }
                        cad.Append("\r");

                        if (metodos.EnviarDatosImpresora(cad, puertoCOM))
                        {
                            ImpresionCorrecta = true;
                        }
                        else
                        {
                            ImpresionCorrecta = false;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error desconodido al tartar de generar el reporte de impresión. ");
                        ImpresionCorrecta = false;
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
                else
                {
                    MessageBox.Show("Error al obtener datos para imprimir");
                }

            return ImpresionCorrecta;
        }

        private void ImprimirTicketDiferencias()
        {
            CpclLabel Ticket = new CpclLabel();

            int PosX = 0;
            int PosY = 0;
            char rellenarcon = ' ';
            decimal importeTotal = 0;
            string producto = "";
            string productoLinea1 = "";
            string productoLinea2 = "";

            Ticket.FormFeed = false;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ArrayList encabezado = new ArrayList();
                encabezado.Add("");
                encabezado.Add("");
                encabezado.Add("REPORTE DE DIFERENCIAS                          ");
                encabezado.Add("================================================");
                encabezado.Add("PRODUCTO                                        ");
                encabezado.Add("Inic.     Vend.   Excdte.   Final     Importe   ");
                encabezado.Add("================================================");

                CpclTextItem Encabezado;
                foreach (Object linea in encabezado)
                {
                    PosY = (PosY + 25);
                    Encabezado = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, PosX, PosY, linea.ToString());
                    Ticket.Add(Encabezado);
                }

                ArrayList partidas = new ArrayList();
                foreach (DataRow dr in dsDiferencias.Tables[0].Rows)
                {
 
                    string clave = dr["InvtId"].ToString().Trim();
                    string descripcion = dr["Descripcion"].ToString().Trim();
                    //concatenamo el producto con la clave y la descripción delproducto
                    producto = clave + "  " + descripcion;

                    int longitudProducto = producto.Length;

                    //si la descripción del producto es mayor a 45 caracteres, entonces se divide en dos líneas
                    if (longitudProducto > 45)
                    {
                        productoLinea1=producto.Substring(0, 45);
                        partidas.Add(productoLinea1);
                        
                        //si la descripción del producto es mayor a 45 caracteres, entonces se divide en dos líneas
                        if (longitudProducto < 90)
                        {
                            productoLinea2 = producto.Substring(45, (longitudProducto - 45));
                            partidas.Add(productoLinea2);
                        }
                        else
                        {
                             //truncamos a 90 caracteres máximo y solo tomamos los últimos 45
                            productoLinea2 = producto.Substring(46,90);
                            partidas.Add(productoLinea2);
                        }
                    }
                    else
                    {
                        partidas.Add(producto);
                    }

                    string inventarioInicial=" " + (dr["InventarioInicial"].ToString().Trim().PadRight(9, rellenarcon));
                    string vendido = " " + dr["Desplazado"].ToString().Trim().PadRight(7, rellenarcon);
                    string excedente = " " + dr["Excedente"].ToString().Trim().PadRight(9, rellenarcon);
                    string existencia = " " + dr["Existencia"].ToString().Trim().PadRight(9, rellenarcon);
                    string importe = " $" + dr["Importe"].ToString().Trim().PadRight(7, rellenarcon);
                    importeTotal = importeTotal + Convert.ToDecimal(dr["Importe"].ToString());//esto es para sumar el importe de las partidas

                    //concatenamos cada campo en una sola línea
                    string partida = inventarioInicial + vendido + excedente + existencia +importe;
                    partidas.Add(partida);
                    partidas.Add("");
                    //importeTotal = importeTotal + Convert.ToDecimal(importe);
                }
                partidas.Add("Importe total: $" +importeTotal.ToString());
                partidas.Add("");
                partidas.Add("El importe total se calcula sobre el precio de  ");
                partidas.Add("lista, no incluye iva, ni descuentos.           ");
                partidas.Add("");
                partidas.Add("");
                CpclTextItem Partidas;
                foreach (Object linea in partidas)
                {
                    PosY = (PosY + 25);
                    Partidas = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, PosX, PosY, linea.ToString());
                    Ticket.Add(Partidas);
                }
                
                Ticket.Height = PosY + 100;

                //Mandamos a imprimir el ticket
                metodos.EnviarDatosImpresora(Ticket, puertoCOM);
            }
            catch (Exception er)
            {
                MessageBox.Show("Error desconodido al tartar de generar el ticket de impresión. " +er.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        //
        private bool ImprimirRecibo()
        {
 
            StringBuilder cad = new StringBuilder();
            bool ImpresionCorrecta = false;

            string importe = String.Format("{0:C2}", Convert.ToDecimal(this.importeRecibo));

            //string importe = String.Format("{0:C2}", 10);

            NumLetra nl = new NumLetra();
            string importeLetra = nl.Convertir(this.importeRecibo, true);

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                //Realizamos un ciclo para imprimir el recibo 2 veces
                for (int i = 0; i <= 1; i++)
                {
                    //cad.AppendLine("===============================================");
                    cad.AppendLine("  ");
                    cad.AppendLine("      HERRAMIENTAS HECORT S.A. DE C.V.         ");
                    cad.AppendLine("           RECIBO DE COBRANZA                  ");
                    cad.AppendLine("  ");
                    cad.AppendLine("       Fecha: " + DateTime.Now.ToLongDateString());
                    cad.AppendLine("        Hora: " + DateTime.Now.ToShortTimeString());
                    cad.AppendLine("      Visita: " + this.Visita);
                    cad.AppendLine("     Cliente: " + this.Cliente);
                    cad.AppendLine("      Nombre: " + this.NombreCliente);
                    cad.AppendLine("  ");
                    cad.AppendLine("  No. Recibo: " + this.Recibo);
                    cad.AppendLine("  Forma Pago: " + cbTipoPago.Text);
                    cad.AppendLine("     Importe: " + importe);
                    cad.AppendLine("  ");
                    cad.AppendLine("    " + importeLetra);
                    cad.AppendLine("  ");
                    cad.AppendLine("  ");
                    cad.AppendLine("    ____________________________________       ");
                    cad.AppendLine("           NOMBRE Y FIRMA CLIENTE              ");
                    cad.AppendLine("  ");
                    cad.AppendLine("  ");
                    cad.AppendLine("    ____________________________________       ");
                    cad.AppendLine("           NOMBRE Y FIRMA VENDEDOR             ");
                    cad.AppendLine("  ");
                    cad.AppendLine("  ");
                    //cad.AppendLine("===============================================");                    
                }


                if (metodos.EnviarDatosImpresora(cad, puertoCOM))
                {
                    ImpresionCorrecta = true;
                }
                else
                {
                    ImpresionCorrecta = false;
                }


            }
            catch (Exception)
            {
                MessageBox.Show("Error desconodido al tartar de generar el reporte de impresión. ");
                ImpresionCorrecta = false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }                           
            return ImpresionCorrecta;
        }

        private void ImprimirTicketCobranza()
        {
            CpclLabel Ticket = new CpclLabel();
            NumLetra nl = new NumLetra();

            int PosX = 0;
            int PosY = 0;
                        
            string importe = String.Format("{0:C2}", Convert.ToDecimal(this.importeRecibo));
            string importeLetra = nl.Convertir(this.importeRecibo, true);
            
            Ticket.FormFeed = false;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ArrayList encabezado = new ArrayList();

                encabezado.Add("===============================================");
                encabezado.Add("");
                encabezado.Add("      HERRAMIENTAS HECORT S.A. DE C.V.         ");
                encabezado.Add("           RECIBO DE COBRANZA                  ");
                encabezado.Add("");
                encabezado.Add("       Fecha: " + DateTime.Now.ToLongDateString());
                encabezado.Add("        Hora: " + DateTime.Now.ToShortTimeString());
                encabezado.Add("      Visita: " + this.Visita);
                encabezado.Add("     Cliente: " + this.Cliente);
                encabezado.Add("      Nombre: " + this.NombreCliente);
                encabezado.Add("");
                encabezado.Add("  NO. RECIBO: " + this.Recibo);
                encabezado.Add("  Forma Pago: " + cbTipoPago.Text);
                encabezado.Add("     Importe: " + importe);
                encabezado.Add("  ");
                encabezado.Add(importeLetra);
                encabezado.Add("");
                encabezado.Add("");
                encabezado.Add("");
                encabezado.Add("");
                encabezado.Add("_______________________________________________");
                encabezado.Add("           NOMBRE Y FIRMA CLIENTE              ");
                encabezado.Add("");
                encabezado.Add("");
                encabezado.Add("");
                encabezado.Add("");
                encabezado.Add("_______________________________________________ ");
                encabezado.Add("           NOMBRE Y FIRMA VENDEDOR             ");
                encabezado.Add("");
                encabezado.Add("");

                CpclTextItem Encabezado;
                foreach (Object linea in encabezado)
                {
                    PosY = (PosY + 25);
                    Encabezado = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, PosX, PosY, linea.ToString());
                    Ticket.Add(Encabezado);
                }
             Ticket.Height = PosY;

             Ticket.Quantity = 2;
             metodos.EnviarDatosImpresora(Ticket, puertoCOM);

            }
            catch (Exception)
            {
                MessageBox.Show("Error desconodido al tartar de generar el ticket de impresión. ");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        //
        private void EnviarConteos()
        {

            if (metodos.IsConnectionAvailable())
            {

                DataSet dsConteos = new DataSet();
                //Limpiamos el grid de conteos Restantes antes de enviar los conteos

                dgRestantes.DataSource = null;
                Cursor.Current = Cursors.WaitCursor;
                dsConteos = metodos.ObtenerConteosLocales();
               

                //Contamos los productos registrados localmente que se van a enviar
                //int enviados = dsConteos.Tables[0].Rows.Count;

                //Obtenemos los conteos agregados, deben de ser los mismos a los enviados
                int agregados = wsoa.AgregarConteos(dsConteos);

                //Eliminamos los productos registrados localmente, una vez registrados en la Base de Datos de Produccón
                int eliminados = metodos.EliminarConteosVisita(Visita);

                Cursor.Current = Cursors.Default;
                Limpiar();

                DialogResult result = MessageBox.Show("Registros enviados: " + agregados.ToString() + " , ¿Desea consultar los restantes? ", "Aviso!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    //Obtenemos los productos que aun restan por contar
                    if (ObtenerConsteosRestantes() > 0)
                    {
                        dtRestantes=dsRestantes.Tables[0].Copy();
                        dgRestantes.DataSource = dtRestantes;
                        lbProductoSeleccionado.Text = "";
                        pnlRestantes.Enabled = true;
                        tcConteos.SelectedIndex = 3;
                    }
                    else
                    {
                        MessageBox.Show("Conteo completo");
                        //Limpiamos el registro del ultimo producto registrado localmente
                        this.Producto = "";

                        ObtenerDiferencias();
                        ObtenerPreFactura();
                        tcConteos.SelectedIndex = 4;
                        pnlRestantes.Enabled=false;
                        pnlDiferenicas.Enabled = true;
                        pnlPreFactura.Enabled = true;
                    }                                        
                }
                else
                {
                    txtCodigoBarras.Focus();
                }
            }
            else
            {
                MessageBox.Show("No se ha podido establecer una conexión con el Servicio Web de la aplicación, verifique si pude navegar en internet, en caso de que si, favor de hablar a TI.");
                btnTerminarConteo.Focus();
            }
        }

        private int ObtenerConsteosRestantes()
        {
            int restantes = 0;
            
            //Invocamos el método del WebService que obtiene los Productos que restan de contar
            dsRestantes = wsoa.ObtenerRestantesConteo(Visita);
            if (dsRestantes != null)
            {
                if (dsRestantes.Tables[0].Rows.Count != 0)
                {
                    restantes = dsRestantes.Tables[0].Rows.Count;
                }
                else
	                {
                        restantes=0;
	                }
            }
            //Regresamos el total de resgistros de producto que faltan por contar
            return restantes;
        }

        private void EliminarConteos()
        {
            DataSet dsEliminar = new DataSet();

            //Limpiamos el grid de conteos Restantes antes de enviar los conteos
            dgEliminar.DataSource = null;
            dsEliminar = metodos.ObtenerConteosLocalesEliminar();

            if (dsEliminar != null)
            {
                if (dsEliminar.Tables[0].Rows.Count != 0)
                {
                    //Mostramos los datos de la consulta en el grid (1er Query)
                    dgEliminar.DataSource = dsEliminar.Tables[0].DefaultView;
                }
            }
        }

        private int ObtenerCodigosBarras()
        {
            int codigosBarrasInsertados=0;
            DataSet dsCodigosBarras= new DataSet();
            if (metodos.IsConnectionAvailable())
            {
                
                dsCodigosBarras = wsoa.ObtenerCodigosBarras(Visita);
                if (dsCodigosBarras != null)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    foreach (DataRow drProducto in dsCodigosBarras.Tables[0].Rows)
                    {
                        string Producto=drProducto["Producto"].ToString();
                        string Descripcion=drProducto["Descripcion"].ToString();
                        string CodigoBarras=drProducto["CodigoBarras"].ToString();
                        //
                        metodos.AgregarCodigoBarrasLocal(Visita, Producto, Descripcion, CodigoBarras);
                        codigosBarrasInsertados++;                       
                    }
                    Cursor.Current = Cursors.Default;
                }
                else
                {                    
                    return 0;
                }
            }
            return codigosBarrasInsertados;
        }

        private void Limpiar()
        {
            this.Producto = "";
            cbCantidades.SelectedIndex = -1;
            btnContador.Text = "CONTADOR";
            btnEliminarDiferencia.Text = "ELIMINAR";
            lbProductoSeleccionado.Text = "PRODUCTO";
            lbProductoDiferencia.Text = "PRODUCTO";
            lbProductoEliminar.Text = "PRODUCTO";
            btnEliminarClave.Text = "ELIMINAR MISMO CODIGO";
            btnEliminarSel.Text = "ELIMINAR SELECCIONADO";
            btnRegistrarExistencia.Text = "REGISTRAR EXISTENCIA";
            lbDescripcion.Text = "DESCRIPCION";
        }

        private void RemoverProductoGridRestantes()
        {
            try
            {
                foreach (DataRow row in dtRestantes.Rows)
                {
                    if (row[0].ToString() == this.Producto)
                    {
                        row.Delete();
                        dtRestantes.AcceptChanges();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            dgRestantes.DataSource = dtRestantes;
        }

        private void RemoverProductoGridDiferencias()
        {
            try
            {
                foreach (DataRow row in dtDiferencias.Rows)
                {
                    if (row[0].ToString() == this.Producto)
                    {
                        row.Delete();
                        dtDiferencias.AcceptChanges();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            dgDiferencias.DataSource = dtDiferencias;
        }


        #endregion


        #region Eventos
        
        //
        private void txtCodigoHT_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Producto = txtCodigoHT.Text.ToUpper().Trim();
                lbDescripcion.Text = metodos.ObterDescripcionPorClave(this.Producto);
                txtCantidad.Text = "1";
                txtCantidad.SelectAll();
                txtCantidad.Focus();
            }
        }
        
        //
        private void frmConteos_Load(object sender, EventArgs e)
        {
            string direccionWebService = wsoa.Url;

            nombreDispositivo=System.Net.Dns.GetHostName();
            txtNombreEquipo.Text = nombreDispositivo;

            if (direccionWebService == "http://200.56.117.82/ServicioOA/Serviciooa.asmx")
            {
                label3.Visible = true;
                label3.ForeColor = Color.Red;
                label3.Text = "ESTAS CONECTADO A PRUEBAS";
            }
            else
            {
                label3.Visible = false;
                label3.ForeColor = Color.Empty;
                label3.Text = "";
            }

            ObtenerParametrosDispositivo();
            pnlConteos.Enabled = false;
            pnlEliminar.Enabled = false;
            pnlRestantes.Enabled = false;
            pnlDiferenicas.Enabled = false;
            pnlPreFactura.Enabled = false;
            pnlRecibo.Enabled = false;
            btnImprimirBalance.Enabled = false;
            txtCliente.BackColor = Color.Yellow;
            txtCliente.Focus();
        }

                
        //
        private void btnTerminarConteo_Click(object sender, EventArgs e)
        {
            EnviarConteos();
        }

        //        
        private void btnIniciarConteo_Click(object sender, EventArgs e)
        {
            IniciarConteo();
        }
        
        //
        private void txtCliente_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                IniciarConteo();
            }
        }
        
        //
        private void frmConteos_Closing(object sender, CancelEventArgs e)
        {
            Application.Exit();
        }
        
        //
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        //
        private void txtCantidad_KeyUp(object sender, KeyEventArgs e)
        {
            
            if (e.KeyCode == Keys.Enter)
            {
                if (metodos.IsNumeric(txtCantidad.Text))
                {
                    this.Cantidad = Convert.ToSingle(txtCantidad.Text.Trim());
                    metodos.AgregarProductoPorClave(Visita, this.Producto, this.Cantidad, "Clave");
                    txtCodigoHT.Text = "";
                    txtCantidad.Text = "";
                    btnContador.Text = this.Producto;                        
                    contador = Contador(this.Producto);
                    btnContador.Text = this.Producto;
                    txtCodigoHT.Focus();
                }
                else
                {
                    MessageBox.Show("Introduzca la cantidad correctamente");
                    txtCantidad.SelectAll();
                    txtCantidad.Focus();                    
                }
            }
        }
        
        //
        private void txtCodigoBarras_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Producto = txtCodigoBarras.Text.ToUpper().Trim();
                lbDescripcion.Text = metodos.ObterDescripcionPorCodigo(this.Producto);
                txtCantidadCodigoBarras.Text = "1";
                txtCantidadCodigoBarras.SelectAll();
                txtCantidadCodigoBarras.Focus();
            }
        }

        //
        private void dgRestantes_CurrentCellChanged(object sender, EventArgs e)
        {
            int renglon;
            renglon = dgRestantes.CurrentCell.RowNumber;
            this.Producto = dgRestantes[renglon, 0].ToString().Trim();
            lbProductoSeleccionado.Text = this.Producto+" / "+dgRestantes[renglon, 1].ToString().Trim();          
            txtCodigoHT.Text = this.Producto;
        }

        //
        private void txtCantidadCodigoBarras_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (metodos.IsNumeric(txtCantidadCodigoBarras.Text))
                {
                    this.Cantidad = Convert.ToSingle(txtCantidadCodigoBarras.Text.Trim());
                    metodos.AgregarProductoPorCodigo(Visita, this.Producto, this.Cantidad, "CodigoBarras");
                    contador = Contador(this.Producto);
                    btnContador.Text = this.Producto + ", Contados:" + contador;
                    txtCodigoBarras.Text = "";
                    txtCantidadCodigoBarras.Text = "";
                    txtCodigoBarras.Focus();                        
                }
                else
                {
                    MessageBox.Show("Introduzca la cantidad correctamente");
                    txtCantidadCodigoBarras.SelectAll();
                    txtCantidadCodigoBarras.Focus();

                }
            }
        }
        
        //
        private void txtCodigoBarras_GotFocus(object sender, EventArgs e)
        {
            plCodigoBarras.BackColor = Color.Yellow;
            plCodigoHT.BackColor = Color.Empty;
            pnlSupermercado.BackColor = Color.Empty;

            txtSupermercado.Text = "";
            txtCodigoHT.Text = "";
            txtCantidad.Text = "";
        }

        //
        private void txtCodigoHT_GotFocus(object sender, EventArgs e)
        {            
            plCodigoHT.BackColor = Color.Yellow;
            plCodigoBarras.BackColor = Color.Empty;
            pnlSupermercado.BackColor = Color.Empty;

            txtCodigoBarras.Text = "";
            txtCantidadCodigoBarras.Text = "";
            txtSupermercado.Text = "";
        }

        //
        private void txtSupermercado_GotFocus(object sender, EventArgs e)
        {
            pnlSupermercado.BackColor = Color.Yellow;
            plCodigoBarras.BackColor = Color.Empty;
            plCodigoHT.BackColor = Color.Empty;
            txtCodigoBarras.Text = "";
            txtCantidadCodigoBarras.Text = "";
            txtCodigoHT.Text = "";
            txtCantidad.Text = "";
        }

        //
        private void txtSupermercado_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtSupermercado.Text != "")
                {
                    this.Producto = txtSupermercado.Text.ToUpper().Trim();
                    lbDescripcion.Text=metodos.ObterDescripcionPorCodigo(this.Producto);
                    metodos.AgregarProductoPorCodigo(Visita, this.Producto, 1, "CodigoBarras");
                    contador = Contador(this.Producto);
                    btnContador.Text = this.Producto + ", Contados:" + contador;
                    txtSupermercado.Text = "";
                    txtSupermercado.Focus();
                }
                else { MessageBox.Show("El código del artículo no puede ir vacio"); }
            }
        }

        //
        private void dgPreFactura_CurrentCellChanged(object sender, EventArgs e)
        {
            int renglon;
            renglon = dgPreFactura.CurrentCell.RowNumber;
            this.Producto = dgPreFactura[renglon, 0].ToString().Trim();
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            //Si la visita aun tiene status de Pendiente, entonces se procede a imprimir
            //la prefactura y el balance

            //if (this.StatusVisita == "Pendiente")
            //{
                //if (ImprimirPreFactura())
                //{
                    ImprimirTicketPreFactura();
                    prefacturaImpresa++;

                    //Despues de la primera impresión de la prefactura, finalizamos la visita.                  
                    if (prefacturaImpresa == 1)
                    {                        
                        wsoa.FinalizarVisita(this.Visita, Convert.ToDecimal(this.importeRecibo)); //Marcamos la visita como "GENERAR" 

                        //
                        metodos.EliminarCodigoBarrasLocal();

                        btnFinalizar.Text = "RE-IMPRIMIR PREFACTURA";

                        pnlRestantes.Enabled = false;
                        pnlConteos.Enabled = false;
                        pnlDiferenicas.Enabled = false;
                        btnImprimirBalance.Enabled = true;
                        pnlRecibo.Enabled = true;
                    }

                //}
            //}
            //else
            //{
            //    if (ImprimirPreFactura())
            //    {
            //        btnFinalizar.Text = "RE-IMPRIMIR PREFACTURA";
            //        pnlRestantes.Enabled = false;
            //        pnlConteos.Enabled = false;
            //        btnImprimirBalance.Enabled = true;
            //    }
            //}
        }

        private void dgDiferencias_CurrentCellChanged(object sender, EventArgs e)
        {
            int renglon;
            renglon = dgDiferencias.CurrentCell.RowNumber;
            this.Producto = dgDiferencias[renglon, 0].ToString().Trim();
            lbProductoDiferencia.Text = this.Producto + " / " + dgDiferencias[renglon, 1].ToString().Trim();
            
            btnEliminarDiferencia.Text = string.Format("Eliminar: {0} ", this.Producto); ;
            btnEliminarDiferencia.Enabled = true;
        }

        private void btnEliminarDiferencia_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Seguro de eliminar la clave: " + this.Producto, "Aviso!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {

                ProductosEliminar.Add(this.Producto);

                btnFinalizar.Enabled = false;
                RemoverProductoGridDiferencias();
                Limpiar();
                dgDiferencias.Focus();

                DialogResult result2 = MessageBox.Show("Desea eliminar otro producto", "Aviso!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result2 == DialogResult.No)
                {
                    if (metodos.IsConnectionAvailable())
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        //Pasamos a la pantalla de captura para volver a capturar el producto recien eliminado
                        int Eliminados = wsoa.EliminarProductosConteo(ProductosEliminar.ToArray(), this.Visita);
                        tcConteos.SelectedIndex = 1;
                        ProductosEliminar.Clear();
                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        MessageBox.Show("No se ha podido establecer una conexión con el Servicio Web de la aplicación, verifique si pude navegar en internet, en caso de que si, favor de hablar a TI.");
                    }
                }
            }
            else
            {
                Limpiar();
                dgDiferencias.Focus();
            }
        }

        private void btnRegistrarExistencia_Click(object sender, EventArgs e)
        {
            if (this.Producto != "")
            {
                if (cbCantidades.Text != "")
                {
                    int cantSeleccionada = Convert.ToInt16(cbCantidades.Text);
                    metodos.AgregarProductoPorClave(Visita, this.Producto, Convert.ToSingle(cantSeleccionada), "Clave");
                    RemoverProductoGridRestantes();                  
                    Limpiar();
                    dgRestantes.Focus();
                }
                else
                {
                    MessageBox.Show("Debe de seleccionar una cantidad del listado, o bien registrar manualmente el conteo. ", "Aviso!!!");
                    cbCantidades.Focus();
                }
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningún Producto. ", "Aviso!!!");

            }            
        }

        private void dgEliminar_CurrentCellChanged(object sender, EventArgs e)
        {
            int renglon;
            renglon = dgEliminar.CurrentCell.RowNumber;
            this.IdProducto = Convert.ToInt16(dgEliminar[renglon, 0].ToString());
            this.Producto = dgEliminar[renglon, 1].ToString().Trim();

            //Buscamos la descripción del Producto seleccionado, primero buscamos por Codigo de Barras
            lbProductoEliminar.Text = metodos.ObterDescripcionPorCodigo(this.Producto);

            //Si la descripción del producto es igual a NO ENCONTRADO, entonces ahora buscamos el producto por la Clave
            if (lbProductoEliminar.Text == "NO ENCONTRADO")
            {
                lbProductoEliminar.Text = metodos.ObterDescripcionPorClave(this.Producto);
            }

            string msjEliminarSel = string.Format("ELIMINAR SELECCIONADO: {0}", IdProducto);
            string msjEliminarClave = string.Format("ELIMINAR MISMO CODIGO");

            btnEliminarSel.Text = msjEliminarSel;
            btnEliminarClave.Text = msjEliminarClave;
        }

        private void tcConteos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcConteos.SelectedIndex == 2)
            {
                EliminarConteos();
            }

        }

        private void btnContador_Click(object sender, EventArgs e)
        {
            tcConteos.SelectedIndex = 2;
        }

        private void btnImpDiferencias_Click(object sender, EventArgs e)
        {
            //ImprimirDiferencias();
            ImprimirTicketDiferencias();
        }

        private void btnEliminarSel_Click(object sender, EventArgs e)
        {
            if (this.Producto == "")
            {
                MessageBox.Show("No ha seleccionado ningún producto.");
            }
            else
            {                
                metodos.EliminarProducto(this.IdProducto);
                Limpiar();
                EliminarConteos();
            }
        }

        private void btnEliminarClave_Click_1(object sender, EventArgs e)
        {
            if (this.Producto == "")
            {
                MessageBox.Show("No existe producto asignado");
            }
            else
            {
                Cursor.Current = Cursors.WaitCursor;
                metodos.EliminarProductos(this.Producto);
                Limpiar();
                EliminarConteos();
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnImprimirRecibo_Click(object sender, EventArgs e)
        {
  
            //string formaPago = "PARCIAL";
                
            if (metodos.IsNumeric(txtImporte.Text))
            {

                decimal importe = Convert.ToDecimal(txtImporte.Text);
                decimal saldo = Convert.ToDecimal(this.importeSaldo);

                if (importe <= 0 )
                {                  
                    string mensaje=string.Format("El monto del pago no puede ser menor o igual a cero, favor de verificarlo. ");
                    
                    MessageBox.Show(mensaje, "Aviso!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        txtImporte.SelectAll();
                        txtImporte.Focus();                    
                }
                else
                {
                    RegistrarPago(importe);
                }
            }
            else
            {
                MessageBox.Show("Favor de digitar correctamente la cantidad. ","AVISO...",MessageBoxButtons.OK,MessageBoxIcon.Hand,MessageBoxDefaultButton.Button1);
                txtImporte.SelectAll();
                txtImporte.Focus();
            }


            /*
            //Si la visita aun presenta un saldo, entonces, se puede registrar un Recibo (abaono)
            if (saldo > 0)
            {
                if (metodos.IsNumeric(txtImporte.Text))
                {
                    if (importe > saldo)
                    {
                        decimal diferencia = importe - saldo;
                        string mensaje = string.Format("El monto del importe es mayor al saldo, por: {0:C2}, desea continuar?", diferencia);

                        DialogResult result = MessageBox.Show(mensaje, "Aviso!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (result == DialogResult.Yes)
                        {
                            RegistrarPago(importe);
                        }
                        else
                        {
                            txtImporte.SelectAll();
                            txtImporte.Focus();
                        }
                    }
                    else
                    {
                        RegistrarPago(importe);
                    }
                }
                else
                {
                    MessageBox.Show("Favor de digitar correctamente la cantidad. ", "AVISO...", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    txtImporte.SelectAll();
                    txtImporte.Focus();
                }
            }
            else
            {
                MessageBox.Show("El inventarios de esta visita está saldado, ya no puede registrarle más abonos. ");
            }
            */

        }

        private bool RegistrarPago(decimal pImporte)
        {
            bool resultado = false;
            DataSet dsRecibo = new DataSet();
            DataSet dsReciboCan = new DataSet();

            if (metodos.IsConnectionAvailable())
            {                
                dsRecibo = wsoa.GenerarRecibo(this.Visita, this.Cliente, pImporte, cbTipoPago.Text, "", txtReferencia.Text, txtBanco.Text);

                DataRow drRecibo = dsRecibo.Tables[0].Rows[0];
                this.Recibo = drRecibo[0].ToString();
                this.importeSaldo = drRecibo[1].ToString();
                this.importeRecibo = pImporte.ToString();
                lbRecibo.Text = "Recibo No.:" + Recibo;
                txtSaldo.Text = this.importeSaldo;

                ImprimirTicketCobranza();
                pnlPreFactura.Enabled = false;
                //Limpiamos los objetos del recibo
                cbTipoPago.Text = "";
                cbTipoPago.SelectedIndex = -1;
                txtBanco.Text = "";
                txtReferencia.Text = "";
                txtImporte.Text = "";
                resultado = true;
                dsRecibo.Clear();
                dsRecibo.Dispose();


                /*
                if (ImprimirRecibo())
                {
                    pnlPreFactura.Enabled = false;
                    //Limpiamos los objetos del recibo
                    cbTipoPago.Text = "";
                    cbTipoPago.SelectedIndex = -1;
                    txtBanco.Text = "";
                    txtReferencia.Text = "";
                    txtImporte.Text = "";
                    resultado = true;
                    dsRecibo.Clear();
                    dsRecibo.Dispose();
                }
                else
                {
                    //Si no se imprimió el recibo, entonces eliminamos el recibo y actualizamos el saldo
                    dsRecibo = wsoa.EliminarRecibo(this.Recibo);
                    DataRow drReciboCancelado = dsRecibo.Tables[0].Rows[0];
                    this.importeSaldo = drRecibo[0].ToString();
                    txtSaldo.Text = this.importeSaldo;
                }
                */

            }
            else
            {
                MessageBox.Show("No hay conexión a Internet, intente nuevamente. ");
                resultado=false;
            }
            return resultado;
        }

        private void btnImprimirBalance_Click(object sender, EventArgs e)
        {
            ObtenerBalance();
            //ImprimirBalance();
            ImprimirTicketBalance();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visita = txtVisita.Text;
            this.Cliente = "Prueba";
            this.NombreCliente = "Prueba";
            ObtenerDiferencias();
            ImprimirTicketDiferencias(); ;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visita = txtVisita.Text;
            this.Cliente = "Prueba";
            this.NombreCliente = "Prueba";
            ObtenerPreFactura();            
            ImprimirTicketPreFactura();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Visita = txtVisita.Text;
            this.Cliente = "Prueba";
            this.NombreCliente = "Prueba";
            ObtenerBalance();
            ImprimirTicketBalance();
        }

        private void button4_Click(object sender, EventArgs e)
        {            
            //tcConteos.SelectedIndex = 6;
            //pnlRecibo.Enabled = true;
            this.Visita = txtVisita.Text;
            this.Cliente = "Prueba";
            this.NombreCliente = "Prueba";
            this.importeRecibo = "1200";
            ImprimirTicketCobranza();
        }

        private void cbPuertos_SelectedValueChanged(object sender, EventArgs e)
        {
            puertoCOM = cbPuertos.Text.Trim();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DataSet dsVisitaRecibo = new DataSet();

            dsVisitaRecibo = wsoa.ObtenerDatosVisitaRecibo(txtCliente.Text.Trim());

            if (dsVisitaRecibo.Tables[0].Rows.Count>0)
            {

                DataRow drVisitaRecibo = dsVisitaRecibo.Tables[0].Rows[0];

                this.Visita = drVisitaRecibo[0].ToString();
                this.Cliente = drVisitaRecibo[1].ToString();
                this.NombreCliente = drVisitaRecibo[2].ToString().Trim();
                this.importeSaldo = drVisitaRecibo[4].ToString();
                txtSaldo.Text = this.importeSaldo;

                //ObtenerPuertoCOMDefault();
                btnIniciarConteo.Enabled = false;
                pnlRecibo.Enabled = true;
                tcConteos.SelectedIndex = 6;
            }
            else
            {
                MessageBox.Show("NO existen saldos pendientes del cliente, no es posible capturar un recibo");
                txtCliente.Focus();
            }

        }
        
        
        #endregion

        private void btnPruebas_Click(object sender, EventArgs e)
        {
            CpclLabel Ticket = new CpclLabel();

            int PosX = 0;
            int PosY = 0;

            /*
            char rellenarcon = ' ';
            decimal importeTotal = 0;
            string producto = "";
            string productoLinea1 = "";
            string productoLinea2 = "";
            */

            Ticket.FormFeed = false;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ArrayList encabezado = new ArrayList();
                encabezado.Add("");
                encabezado.Add("");
                encabezado.Add("              TEST DE IMPRESION                 ");
                encabezado.Add("================================================");
                encabezado.Add("PUERTO COM CONFIGURADO:   " + puertoCOM);
                encabezado.Add("================================================");

                
                CpclTextItem Encabezado;
                foreach (Object linea in encabezado)
                {
                    PosY = (PosY + 25);
                    Encabezado = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, PosX, PosY, linea.ToString());
                    Ticket.Add(Encabezado);
                }

                /*
                ArrayList partidas = new ArrayList();
                foreach (DataRow dr in dsDiferencias.Tables[0].Rows)
                {

                    string clave = dr["InvtId"].ToString().Trim();
                    string descripcion = dr["Descripcion"].ToString().Trim();
                    //concatenamo el producto con la clave y la descripción delproducto
                    producto = clave + "  " + descripcion;

                    int longitudProducto = producto.Length;

                    //si la descripción del producto es mayor a 45 caracteres, entonces se divide en dos líneas
                    if (longitudProducto > 45)
                    {
                        productoLinea1 = producto.Substring(0, 45);
                        partidas.Add(productoLinea1);

                        //si la descripción del producto es mayor a 45 caracteres, entonces se divide en dos líneas
                        if (longitudProducto < 90)
                        {
                            productoLinea2 = producto.Substring(45, (longitudProducto - 45));
                            partidas.Add(productoLinea2);
                        }
                        else
                        {
                            //truncamos a 90 caracteres máximo y solo tomamos los últimos 45
                            productoLinea2 = producto.Substring(46, 90);
                            partidas.Add(productoLinea2);
                        }
                    }
                    else
                    {
                        partidas.Add(producto);
                    }

                    string inventarioInicial = " " + (dr["InventarioInicial"].ToString().Trim().PadRight(9, rellenarcon));
                    string vendido = " " + dr["Desplazado"].ToString().Trim().PadRight(7, rellenarcon);
                    string excedente = " " + dr["Excedente"].ToString().Trim().PadRight(9, rellenarcon);
                    string existencia = " " + dr["Existencia"].ToString().Trim().PadRight(9, rellenarcon);
                    string importe = " $" + dr["Importe"].ToString().Trim().PadRight(7, rellenarcon);
                    importeTotal = importeTotal + Convert.ToDecimal(dr["Importe"].ToString());//esto es para sumar el importe de las partidas

                    //concatenamos cada campo en una sola línea
                    string partida = inventarioInicial + vendido + excedente + existencia + importe;
                    partidas.Add(partida);
                    partidas.Add("");
                    //importeTotal = importeTotal + Convert.ToDecimal(importe);
                }
                partidas.Add("Importe total: $" + importeTotal.ToString());
                partidas.Add("");
                partidas.Add("El importe total se calcula sobre el precio de  ");
                partidas.Add("lista, no incluye iva, ni descuentos.           ");
                partidas.Add("");
                partidas.Add("");
                CpclTextItem Partidas;
                foreach (Object linea in partidas)
                {
                    PosY = (PosY + 25);
                    Partidas = new CpclTextItem(CpclTextOrientation._0_Degrees, "7", 0, PosX, PosY, linea.ToString());
                    Ticket.Add(Partidas);
                }
                 * 
                */
                Ticket.Height = PosY + 100;

                //Mandamos a imprimir el ticket
                //metodos.EnviarDatosImpresora(Ticket, puertoCOM);

                CommConfig cCommConfig = new CommConfig();
                cCommConfig.CommunicationMode = 2; //Bluetooth
                cCommConfig.BtPort = puertoCOM;

                //cCommConfig.CommunicationMode= 4; //USB
                //cCommConfig.SerialPort = "USB002";

                Comm cComm = new Comm(cCommConfig);


                bool conectado = cComm.IsConnected;
                try
                {
                    Printer impresora = new Printer(cCommConfig);
                    impresora.Print(Ticket);
                    cComm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al enviar datos a la mpresora: " + ex.Message);
                }


            }
            catch (Exception er)
            {
                MessageBox.Show("Error desconodido al tartar de generar el ticket de impresión. " + er.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }

        private void txtVisita_TextChanged(object sender, EventArgs e)
        {

        }





    }

}