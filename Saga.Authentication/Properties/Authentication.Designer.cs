﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Saga.Authentication.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Authentication : global::System.Configuration.ApplicationSettingsBase {
        
        private static Authentication defaultInstance = ((Authentication)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Authentication())));
        
        public static Authentication Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("A928CDC9DBE8751B3BC99EB65AE07E0C849CE739")]
        public string CRCKEY {
            get {
                return ((string)(this["CRCKEY"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("969170651B6090625BE559B81239C6BA6F1A116C")]
        public string GUID {
            get {
                return ((string)(this["GUID"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public string RequiredAge {
            get {
                return ((string)(this["RequiredAge"]));
            }
        }
    }
}
