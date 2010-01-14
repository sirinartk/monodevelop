// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Platform.Updater {
    
    
    internal partial class UpdateDialog {
        
        private Gtk.Alignment alignment1;
        
        private Gtk.VBox vbox2;
        
        private Gtk.Notebook notebook1;
        
        private Gtk.Alignment alignment2;
        
        private Gtk.Label messageLabel;
        
        private Gtk.VBox vbox3;
        
        private Gtk.Label infoLabel;
        
        private Gtk.ScrolledWindow scrolledwindow1;
        
        private Gtk.VBox productBox;
        
        private Gtk.CheckButton checkAutomaticallyCheck;
        
        private Gtk.CheckButton checkIncludeUnstable;
        
        private Gtk.Button buttonOk;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget MonoDevelop.Platform.Updater.UpdateDialog
            this.Name = "MonoDevelop.Platform.Updater.UpdateDialog";
            this.Title = Mono.Unix.Catalog.GetString("MonoDevelop Updater");
            this.WindowPosition = ((Gtk.WindowPosition)(4));
            // Internal child MonoDevelop.Platform.Updater.UpdateDialog.VBox
            Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.alignment1 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.alignment1.Name = "alignment1";
            // Container child alignment1.Gtk.Container+ContainerChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 2;
            this.vbox2.BorderWidth = ((uint)(6));
            // Container child vbox2.Gtk.Box+BoxChild
            this.notebook1 = new Gtk.Notebook();
            this.notebook1.CanFocus = true;
            this.notebook1.Name = "notebook1";
            this.notebook1.CurrentPage = 0;
            // Container child notebook1.Gtk.Notebook+NotebookChild
            this.alignment2 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.alignment2.Name = "alignment2";
            // Container child alignment2.Gtk.Container+ContainerChild
            this.messageLabel = new Gtk.Label();
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Wrap = true;
            this.alignment2.Add(this.messageLabel);
            this.notebook1.Add(this.alignment2);
            // Container child notebook1.Gtk.Notebook+NotebookChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            // Container child vbox3.Gtk.Box+BoxChild
            this.infoLabel = new Gtk.Label();
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Xpad = 6;
            this.infoLabel.Ypad = 6;
            this.infoLabel.Xalign = 0F;
            this.infoLabel.LabelProp = Mono.Unix.Catalog.GetString("The following updates are available. After downloading,\nplease close MonoDevelop before installing them.");
            this.infoLabel.Wrap = true;
            this.vbox3.Add(this.infoLabel);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.vbox3[this.infoLabel]));
            w4.Position = 0;
            w4.Expand = false;
            w4.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.scrolledwindow1 = new Gtk.ScrolledWindow();
            this.scrolledwindow1.CanFocus = true;
            this.scrolledwindow1.Name = "scrolledwindow1";
            this.scrolledwindow1.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow1.Gtk.Container+ContainerChild
            Gtk.Viewport w5 = new Gtk.Viewport();
            w5.ShadowType = ((Gtk.ShadowType)(0));
            // Container child GtkViewport.Gtk.Container+ContainerChild
            this.productBox = new Gtk.VBox();
            this.productBox.Name = "productBox";
            this.productBox.Spacing = 12;
            w5.Add(this.productBox);
            this.scrolledwindow1.Add(w5);
            this.vbox3.Add(this.scrolledwindow1);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.vbox3[this.scrolledwindow1]));
            w8.Position = 1;
            this.notebook1.Add(this.vbox3);
            Gtk.Notebook.NotebookChild w9 = ((Gtk.Notebook.NotebookChild)(this.notebook1[this.vbox3]));
            w9.Position = 1;
            this.vbox2.Add(this.notebook1);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(this.vbox2[this.notebook1]));
            w10.Position = 0;
            // Container child vbox2.Gtk.Box+BoxChild
            this.checkAutomaticallyCheck = new Gtk.CheckButton();
            this.checkAutomaticallyCheck.CanFocus = true;
            this.checkAutomaticallyCheck.Name = "checkAutomaticallyCheck";
            this.checkAutomaticallyCheck.Label = Mono.Unix.Catalog.GetString("Check for updates automatically");
            this.checkAutomaticallyCheck.DrawIndicator = true;
            this.checkAutomaticallyCheck.UseUnderline = true;
            this.vbox2.Add(this.checkAutomaticallyCheck);
            Gtk.Box.BoxChild w11 = ((Gtk.Box.BoxChild)(this.vbox2[this.checkAutomaticallyCheck]));
            w11.Position = 1;
            w11.Expand = false;
            w11.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.checkIncludeUnstable = new Gtk.CheckButton();
            this.checkIncludeUnstable.CanFocus = true;
            this.checkIncludeUnstable.Name = "checkIncludeUnstable";
            this.checkIncludeUnstable.Label = Mono.Unix.Catalog.GetString("Include unstable/beta versions");
            this.checkIncludeUnstable.DrawIndicator = true;
            this.checkIncludeUnstable.UseUnderline = true;
            this.vbox2.Add(this.checkIncludeUnstable);
            Gtk.Box.BoxChild w12 = ((Gtk.Box.BoxChild)(this.vbox2[this.checkIncludeUnstable]));
            w12.Position = 2;
            w12.Expand = false;
            w12.Fill = false;
            this.alignment1.Add(this.vbox2);
            w1.Add(this.alignment1);
            Gtk.Box.BoxChild w14 = ((Gtk.Box.BoxChild)(w1[this.alignment1]));
            w14.Position = 0;
            // Internal child MonoDevelop.Platform.Updater.UpdateDialog.ActionArea
            Gtk.HButtonBox w15 = this.ActionArea;
            w15.Name = "dialog1_ActionArea";
            w15.Spacing = 10;
            w15.BorderWidth = ((uint)(5));
            w15.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonOk = new Gtk.Button();
            this.buttonOk.CanDefault = true;
            this.buttonOk.CanFocus = true;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseStock = true;
            this.buttonOk.UseUnderline = true;
            this.buttonOk.Label = "gtk-close";
            this.AddActionWidget(this.buttonOk, -7);
            Gtk.ButtonBox.ButtonBoxChild w16 = ((Gtk.ButtonBox.ButtonBoxChild)(w15[this.buttonOk]));
            w16.Expand = false;
            w16.Fill = false;
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 450;
            this.DefaultHeight = 335;
            this.Show();
        }
    }
}
