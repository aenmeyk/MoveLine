using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.ExtensionManager;

namespace KevinAenmey.MoveLine
{
    [TextViewRole("INTERACTIVE")]
    [ContentType("text")]
    [Export(typeof(IVsTextViewCreationListener))]
    internal sealed class VsEditorCreationListener : IVsTextViewCreationListener
    {
        [Import]
        private IVsEditorAdaptersFactoryService AdaptersFactory;
        [Import]
        private SVsServiceProvider ServiceProvider;
        [Import]
        private IEditorOperationsFactoryService EditorOperationsFactory;
        [Import]
        private SVsServiceProvider serviceProvider;
        [Import]
        private IOutliningManagerService OutliningManagerService;
        private static bool KeyBindingsDone;

        private bool IsStandaloneExtensionInstalled
        {
            get
            {
                return VsEditorCreationListener.CheckIsStandaloneExtensionInstalled(this.serviceProvider);
            }
        }

        static VsEditorCreationListener()
        {
        }

        internal static bool CheckIsStandaloneExtensionInstalled(SVsServiceProvider serviceProvider)
        {
            IVsExtensionManager extensionManager = serviceProvider.GetService(typeof(SVsExtensionManager)) as IVsExtensionManager;
            if (extensionManager == null)
                return false;
            IInstalledExtension result;
            extensionManager.TryGetInstalledExtension("8f767aee-d7db-489e-b6b4-de14c5a391a3", out result);
            return result != null;
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            if (this.IsStandaloneExtensionInstalled)
                return;
            this.EnsureKeyBindings();
            IWpfTextView wpfTextView = this.AdaptersFactory.GetWpfTextView(textViewAdapter);
            if (wpfTextView == null)
                return;
            CommandFilter commandFilter = new CommandFilter(wpfTextView, this.EditorOperationsFactory.GetEditorOperations((ITextView)wpfTextView), this.OutliningManagerService.GetOutliningManager((ITextView)wpfTextView));
            IOleCommandTarget ppNextCmdTarg;
            if (!ErrorHandler.Succeeded(textViewAdapter.AddCommandFilter((IOleCommandTarget)commandFilter, out ppNextCmdTarg)))
                return;
            commandFilter.Next = ppNextCmdTarg;
        }

        private void EnsureKeyBindings()
        {
            if (VsEditorCreationListener.KeyBindingsDone)
                return;
            VsEditorCreationListener.KeyBindingsDone = true;
            DTE dte = this.ServiceProvider.GetService(typeof(SDTE)) as DTE;
            Command command1 = dte.Commands.Item((object)"Edit.MoveLineUp", -1);
            Command command2 = dte.Commands.Item((object)"Edit.MoveLineDown", -1);
            object[] objArray1 = command1.Bindings as object[];
            object[] objArray2 = command2.Bindings as object[];
            if (command1.Bindings != null && objArray1 == null)
                throw new Exception();
            if (command2.Bindings != null && objArray2 == null)
                throw new Exception();
            if (command1.Bindings == null || objArray1.Length == 1 && string.CompareOrdinal("Global::Ctrl+Shift+Alt+Up Arrow, Ctrl+Shift+Alt+Up Arrow", objArray1[0] as string) == 0)
                command1.Bindings = (object)"Text Editor::Alt+Up Arrow";
            if (command2.Bindings != null && (objArray2.Length != 1 || string.CompareOrdinal("Global::Ctrl+Shift+Alt+Down Arrow, Ctrl+Shift+Alt+Down Arrow", objArray2[0] as string) != 0))
                return;
            command2.Bindings = (object)"Text Editor::Alt+Down Arrow";
        }
    }
}
