﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OTISCZ.ScmDemand.UI.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://intranetcz.cz.otis.com/ConcordeWebService/internalrequest.asmx")]
        public string OTISCZ_ScmDemand_UI_WsConcordeSupplier_InternalRequest {
            get {
                return ((string)(this["OTISCZ_ScmDemand_UI_WsConcordeSupplier_InternalRequest"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://intranetcz.cz.eu.otis.utc.com/WsOtMail/OtWsMail.asmx")]
        public string OTISCZ_ScmDemand_UI_WsMail_OtWsMail {
            get {
                return ((string)(this["OTISCZ_ScmDemand_UI_WsMail_OtWsMail"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost:55039/ScmDemand.svc")]
        public string OTISCZ_ScmDemand_UI_WsScmDemandDebug_ScmDemand {
            get {
                return ((string)(this["OTISCZ_ScmDemand_UI_WsScmDemandDebug_ScmDemand"]));
            }
        }
    }
}