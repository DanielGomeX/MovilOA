﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión del motor en tiempo de ejecución:2.0.50727.6413
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Microsoft.CompactFramework.Design.Data generó automáticamente este código fuente, versión=2.0.50727.6413.
// 
namespace OA_Movil.ServicioOA {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    using System.Data;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ServicioOASoap", Namespace="http://200.56.117.88/ServicioOA/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(object[]))]
    public partial class ServicioOA : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public ServicioOA() {
            this.Url = "http://200.56.117.88/ServicioOA/ServicioOA.asmx";
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/IniciarVisita", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet IniciarVisita(string pCliente) {
            object[] results = this.Invoke("IniciarVisita", new object[] {
                        pCliente});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginIniciarVisita(string pCliente, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("IniciarVisita", new object[] {
                        pCliente}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndIniciarVisita(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/FinalizarVisita", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool FinalizarVisita(string pVisita, decimal pImporteTotal) {
            object[] results = this.Invoke("FinalizarVisita", new object[] {
                        pVisita,
                        pImporteTotal});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginFinalizarVisita(string pVisita, decimal pImporteTotal, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("FinalizarVisita", new object[] {
                        pVisita,
                        pImporteTotal}, callback, asyncState);
        }
        
        /// <remarks/>
        public bool EndFinalizarVisita(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/EliminarProductosConteo", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int EliminarProductosConteo(object[] pProductos, string pVisita) {
            object[] results = this.Invoke("EliminarProductosConteo", new object[] {
                        pProductos,
                        pVisita});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginEliminarProductosConteo(object[] pProductos, string pVisita, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("EliminarProductosConteo", new object[] {
                        pProductos,
                        pVisita}, callback, asyncState);
        }
        
        /// <remarks/>
        public int EndEliminarProductosConteo(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/ObtenerRestantesConteo", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ObtenerRestantesConteo(string pVisita) {
            object[] results = this.Invoke("ObtenerRestantesConteo", new object[] {
                        pVisita});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginObtenerRestantesConteo(string pVisita, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ObtenerRestantesConteo", new object[] {
                        pVisita}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndObtenerRestantesConteo(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/ObtenerDiferenciasConteo", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ObtenerDiferenciasConteo(string pVisita) {
            object[] results = this.Invoke("ObtenerDiferenciasConteo", new object[] {
                        pVisita});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginObtenerDiferenciasConteo(string pVisita, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ObtenerDiferenciasConteo", new object[] {
                        pVisita}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndObtenerDiferenciasConteo(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/ObtenerPreFactura", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ObtenerPreFactura(string pVisita) {
            object[] results = this.Invoke("ObtenerPreFactura", new object[] {
                        pVisita});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginObtenerPreFactura(string pVisita, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ObtenerPreFactura", new object[] {
                        pVisita}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndObtenerPreFactura(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/ObtenerBalance", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ObtenerBalance(string pVisita) {
            object[] results = this.Invoke("ObtenerBalance", new object[] {
                        pVisita});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginObtenerBalance(string pVisita, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ObtenerBalance", new object[] {
                        pVisita}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndObtenerBalance(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/AgregarConteos", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int AgregarConteos(System.Data.DataSet dsProductos) {
            object[] results = this.Invoke("AgregarConteos", new object[] {
                        dsProductos});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginAgregarConteos(System.Data.DataSet dsProductos, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("AgregarConteos", new object[] {
                        dsProductos}, callback, asyncState);
        }
        
        /// <remarks/>
        public int EndAgregarConteos(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/ObtenerCodigosBarras", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ObtenerCodigosBarras(string pVisita) {
            object[] results = this.Invoke("ObtenerCodigosBarras", new object[] {
                        pVisita});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginObtenerCodigosBarras(string pVisita, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ObtenerCodigosBarras", new object[] {
                        pVisita}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndObtenerCodigosBarras(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/GenerarRecibo", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GenerarRecibo(string pVisita, string pCliente, decimal pImporte, string pFormaPago, string pTipoPago, string pReferencia, string pBanco) {
            object[] results = this.Invoke("GenerarRecibo", new object[] {
                        pVisita,
                        pCliente,
                        pImporte,
                        pFormaPago,
                        pTipoPago,
                        pReferencia,
                        pBanco});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGenerarRecibo(string pVisita, string pCliente, decimal pImporte, string pFormaPago, string pTipoPago, string pReferencia, string pBanco, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GenerarRecibo", new object[] {
                        pVisita,
                        pCliente,
                        pImporte,
                        pFormaPago,
                        pTipoPago,
                        pReferencia,
                        pBanco}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndGenerarRecibo(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/EliminarRecibo", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet EliminarRecibo(string pRecibo) {
            object[] results = this.Invoke("EliminarRecibo", new object[] {
                        pRecibo});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginEliminarRecibo(string pRecibo, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("EliminarRecibo", new object[] {
                        pRecibo}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndEliminarRecibo(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/ObtenerParametros", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ObtenerParametros(string pNombreEquipo) {
            object[] results = this.Invoke("ObtenerParametros", new object[] {
                        pNombreEquipo});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginObtenerParametros(string pNombreEquipo, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ObtenerParametros", new object[] {
                        pNombreEquipo}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndObtenerParametros(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://200.56.117.88/ServicioOA/ObtenerDatosVisitaRecibo", RequestNamespace="http://200.56.117.88/ServicioOA/", ResponseNamespace="http://200.56.117.88/ServicioOA/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ObtenerDatosVisitaRecibo(string pCliente) {
            object[] results = this.Invoke("ObtenerDatosVisitaRecibo", new object[] {
                        pCliente});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginObtenerDatosVisitaRecibo(string pCliente, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ObtenerDatosVisitaRecibo", new object[] {
                        pCliente}, callback, asyncState);
        }
        
        /// <remarks/>
        public System.Data.DataSet EndObtenerDatosVisitaRecibo(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.Data.DataSet)(results[0]));
        }
    }
}
