using System;
using Rhino;
using Rhino.Commands;
using Rhino.UI;

namespace QuaderGenerator.Commands
{
    /// <summary>
    /// Command to show/hide the Quader Generator panel
    /// </summary>
    public class QuaderGenCommand : Command
    {
        public QuaderGenCommand()
        {
            Instance = this;
        }

        public static QuaderGenCommand Instance { get; private set; }

        public override string EnglishName => "QuaderGen";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var panelId = UI.QuaderPanel.PanelId;
            var visible = Panels.IsPanelVisible(panelId);

            if (!visible)
            {
                Panels.OpenPanel(panelId);
                RhinoApp.WriteLine("QuaderGenerator: Panel opened successfully.");
                RhinoApp.WriteLine("Use QuaderGen command again to toggle panel visibility.");
            }
            else
            {
                Panels.ClosePanel(panelId);
                RhinoApp.WriteLine("QuaderGenerator: Panel closed. Use QuaderGen command to reopen.");
            }

            return Result.Success;
        }
    }
}

