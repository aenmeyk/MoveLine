using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using MoveLine;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
using KevinAenmey.MoveLine;
using System.ComponentModel.Composition.Hosting;

namespace KevinAenmey.MoveLinePackage
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidMoveLinePkgString)]
    [ContentType("text")]
    public sealed class MoveLinePackage : Package
    {
        private LineMover lineMover;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                this.lineMover = new LineMover();
                CommandID upCommandID = new CommandID(GuidList.guidMoveLineCmdSet, (int)PkgCmdIDList.cmdidMoveLineUp);
                MenuCommand upCommand = new MenuCommand(MoveLineUpCallback, upCommandID);
                mcs.AddCommand(upCommand);

                CommandID downCommandID = new CommandID(GuidList.guidMoveLineCmdSet, (int)PkgCmdIDList.cmdidMoveLineDown);
                MenuCommand downCommand = new MenuCommand(MoveLineDownCallback, downCommandID);
                mcs.AddCommand(downCommand);
            }
        }

        private void MoveLineUpCallback(object sender, EventArgs e)
        {
            if (this.IsActiveDocumentText(sender))
            {
                var activeTextView = this.GetActiveTextView();
                this.lineMover.MoveLineUp(activeTextView);
            }
        }

        private void MoveLineDownCallback(object sender, EventArgs e)
        {
            if (this.IsActiveDocumentText(sender))
            {
                this.lineMover.MoveLineDown(this.GetActiveTextView());
            }
        }

        private bool IsActiveDocumentText(object sender)
        {
            var app = (EnvDTE.DTE)GetService(typeof(SDTE));
            return app.ActiveDocument != null && app.ActiveDocument.Type == "Text";
        }

        private IWpfTextView GetActiveTextView()
        {
            IWpfTextView wpfTextView = null;

            var textManager = (IVsTextManager)GetService(typeof(SVsTextManager));
            IVsTextView vsTextView = null;
            textManager.GetActiveView(fMustHaveFocus: 1, pBuffer: null, ppView: out vsTextView);
            var userData = vsTextView as IVsUserData;

            if (userData != null)
            {
                object holder;
                var guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out holder);
                var viewHost = (IWpfTextViewHost)holder;
                wpfTextView = viewHost.TextView;
            }

            return wpfTextView;
        }
    }
}
