using System;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using MoveLine;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
using KevinAenmey.MoveLine;

namespace KevinAenmey.MoveLinePackage
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidMoveLinePkgString)]
    public sealed class MoveLinePackagePackage : Package
    {
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
                var textView = GetActiveTextView();
                MoveLineCommands.MoveLineUp(textView);
            }
        }

        private void MoveLineDownCallback(object sender, EventArgs e)
        {
            if (this.IsActiveDocumentText(sender))
            {
                var textView = GetActiveTextView();
                MoveLineCommands.MoveLineDown(textView);
            }
        }

        private bool IsActiveDocumentText(object sender)
        {
            var command = sender as MenuCommand;
            EnvDTE.DTE app = (EnvDTE.DTE)GetService(typeof(SDTE));

            return app.ActiveDocument != null && app.ActiveDocument.Type == "Text";
        }

        private IWpfTextView GetActiveTextView()
        {
            IWpfTextView view = null;
            IVsTextView vTextView = null;

            IVsTextManager txtMgr = (IVsTextManager)GetService(typeof(SVsTextManager));

            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);

            IVsUserData userData = vTextView as IVsUserData;
            if (null != userData)
            {
                IWpfTextViewHost viewHost;
                object holder;
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out holder);
                viewHost = (IWpfTextViewHost)holder;
                view = viewHost.TextView;
            }

            return view;
        }
    }
}
