namespace CodeAnalysis.BusinessLogic
{
    using System.CodeDom.Compiler;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using CodeAnalysis.Properties;

    /// <summary>
    /// This class manage user's settings
    /// </summary>
    [CompilerGenerated()]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed class UserSettings : ApplicationSettingsBase
    {
        private static readonly Settings DefaultInstance = ((Settings)(ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default
        {
            get
            {
                return DefaultInstance;
            }
        }

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        public string RepositoryURL
        {
            get
            {
                return ((string)(this["RepositoryURL"]));
            }
            set
            {
                this["RepositoryURL"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [DebuggerNonUserCodeAttribute()]
        public string TrunkName
        {
            get
            {
                return ((string)(this["TrunkName"]));
            }
            set
            {
                this["TrunkName"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [DebuggerNonUserCodeAttribute()]
        public string BrancheName
        {
            get
            {
                return ((string)(this["BrancheName"]));
            }
            set
            {
                this["BrancheName"] = value;
            }
        }
    }
}