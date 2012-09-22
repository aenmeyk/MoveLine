using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using KevinAenmey.MoveLine;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace KevinAenmey.MoveLinePackage
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidMoveLinePkgString)]
    [ContentType("text")]
    public sealed class MoveLinePackage : Package
    {
        private readonly Lazy<LineMoveDirector> lazyLineMoveUp = new Lazy<LineMoveDirector>(() => new LineMoveDirector(new LineMoverUp()));
        private readonly Lazy<LineMoveDirector> lazyLineMoveDown = new Lazy<LineMoveDirector>(() => new LineMoveDirector(new LineMoverDown()));

        protected override void Initialize()
        {
            base.Initialize();
            var menuCommandService = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (menuCommandService != null)
            {
                var upCommandId = new CommandID(GuidList.guidMoveLineCmdSet, (int)PkgCmdIDList.cmdidMoveLineUp);
                var upMenuCommand = new MenuCommand(MoveLineUpCallback, upCommandId);
                menuCommandService.AddCommand(upMenuCommand);

                var downCommandId = new CommandID(GuidList.guidMoveLineCmdSet, (int)PkgCmdIDList.cmdidMoveLineDown);
                var downMenuCommand = new MenuCommand(MoveLineDownCallback, downCommandId);
                menuCommandService.AddCommand(downMenuCommand);
            }
        }

        private void MoveLineUpCallback(object sender, EventArgs e)
        {
            if (this.IsActiveDocumentText(sender))
            {
                this.lazyLineMoveUp.Value.MoveLine(this.GetActiveWpfTextView());
            }
        }

        private void MoveLineDownCallback(object sender, EventArgs e)
        {
            if (this.IsActiveDocumentText(sender))
            {
                this.lazyLineMoveDown.Value.MoveLine(this.GetActiveWpfTextView());
            }
        }

        private bool IsActiveDocumentText(object sender)
        {
            var app = (EnvDTE.DTE)this.GetService(typeof(SDTE));
            return app.ActiveDocument != null && app.ActiveDocument.Type == "Text";
        }

        private IWpfTextView GetActiveWpfTextView()
        {
            IWpfTextView wpfTextView = null;

            var textManager = (IVsTextManager)this.GetService(typeof(SVsTextManager));
            IVsTextView vsTextView;
            textManager.GetActiveView(fMustHaveFocus: 1, pBuffer: null, ppView: out vsTextView);
            var userData = vsTextView as IVsUserData;

            if (userData != null)
            {
                object host;
                var guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out host);
                var viewHost = (IWpfTextViewHost)host;
                wpfTextView = viewHost.TextView;
            }

            return wpfTextView;
        }
    }
}
