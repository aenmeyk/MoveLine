using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Outlining;
using MoveLine;

namespace KevinAenmey.MoveLine
{
    internal sealed class CommandFilter : IOleCommandTarget
    {
        private readonly IWpfTextView wpfTextView;
        private readonly IEditorOperations operations;
        private readonly IOutliningManager outliningManager;
        internal IOleCommandTarget Next;
        private LineMover lineMover;

        public CommandFilter(IWpfTextView wpfTextView, IEditorOperations operations, IOutliningManager outliningManager)
        {
            this.wpfTextView = wpfTextView;
            this.operations = operations;
            this.outliningManager = outliningManager;
            this.lineMover = new LineMover();
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == GuidList.guidMoveLineCmdSet)
            {
                switch (nCmdID)
                {
                    case 256U:
                        this.lineMover.MoveLineUp(this.wpfTextView);
                        return 1;
                            
//                        return !MoveLineCommands.MoveLineUp(this.wpfTextView, this.operations, this.outliningManager) ? 1 : 0;
                    case 257U:
                        return 1;
//                        return !MoveLineCommands.MoveLineDown(this.wpfTextView, this.operations, this.outliningManager) ? 1 : 0;
                }
            }
            return this.Next.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (!(pguidCmdGroup == GuidList.guidMoveLineCmdSet))
                return this.Next.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            bool flag = true;
            for (int index = 0; (long)index < (long)cCmds; ++index)
            {
                switch (prgCmds[index].cmdID)
                {
                    case 256U:
                    case 257U:
                        prgCmds[index].cmdf = 3U;
                        break;
                    default:
                        OLECMD[] prgCmds1 = new OLECMD[1]
            {
              prgCmds[index]
            };
                        if (ErrorHandler.Succeeded(this.Next.QueryStatus(ref pguidCmdGroup, 1U, prgCmds1, pCmdText)))
                        {
                            prgCmds[index] = prgCmds1[0];
                            break;
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                }
            }
            return !flag ? -2147467259 : 0;
        }
    }
}
