namespace ByteDBServer
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ByteDBServerProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ByteDBServerInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ByteDBServerProcessInstaller
            // 
            this.ByteDBServerProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ByteDBServerProcessInstaller.Password = null;
            this.ByteDBServerProcessInstaller.Username = null;
            // 
            // ByteDBServerInstaller
            // 
            this.ByteDBServerInstaller.Description = "Listens for incoming queries";
            this.ByteDBServerInstaller.DisplayName = "ByteDBServer Service";
            this.ByteDBServerInstaller.ServiceName = "ByteDBServerService";
            this.ByteDBServerInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ByteDBServerProcessInstaller,
            this.ByteDBServerInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ByteDBServerProcessInstaller;
        private System.ServiceProcess.ServiceInstaller ByteDBServerInstaller;
    }
}