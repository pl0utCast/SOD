using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace SOD.View.Controls
{
    public class CustomScrollViewer : ScrollViewer
    {
        public CustomScrollViewer()
        {
            PanningMode = PanningMode.Both;
        }
        protected override void OnManipulationBoundaryFeedback(ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
            base.OnManipulationBoundaryFeedback(e);
        }
    }
}
