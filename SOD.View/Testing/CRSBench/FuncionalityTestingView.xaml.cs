using SOD.ViewModels.Testing.CRSBench;
using SciChart.Charting;
using SciChart.Drawing.VisualXcceleratorRasterizer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SOD.View.Testing.CRSBench
{
    /// <summary>
    /// Логика взаимодействия для FuncionalityTestingView.xaml
    /// </summary>
    public partial class FuncionalityTestingView : UserControl
    {
        public FuncionalityTestingView()
        {
            InitializeComponent();
        }

        private void sciChartSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var hitTestPoint = e.GetPosition(sciChartSurface.GridLinesPanel as UIElement);
            var vm = DataContext as FuncionalityTestViewModel;
            // Show info for series, which HitTest operation was successful for only
            foreach (var renderableSeries in sciChartSurface.RenderableSeries)
            {
                // Get hit-test the RenderableSeries using interpolation
                var hitTestInfo = renderableSeries.HitTestProvider.HitTest(hitTestPoint, true);

                if (hitTestInfo.IsHit)
                {
                    // Convert the result of hit-test operation to SeriesInfo.
                    var seriesInfo = renderableSeries.GetSeriesInfo(hitTestInfo);

                    vm.SelectedSeriesInfo = seriesInfo;
                }
            }
        }

        private void sciChartSurface_TouchDown(object sender, TouchEventArgs e)
        {
            var hitTestPoint = e.GetTouchPoint(sciChartSurface.GridLinesPanel as UIElement);
            var vm = DataContext as FuncionalityTestViewModel;
            // Show info for series, which HitTest operation was successful for only
            foreach (var renderableSeries in sciChartSurface.RenderableSeries)
            {
                // Get hit-test the RenderableSeries using interpolation
                var hitTestInfo = renderableSeries.HitTestProvider.HitTest(hitTestPoint.Position, true);

                if (hitTestInfo.IsHit)
                {
                    // Convert the result of hit-test operation to SeriesInfo.
                    var seriesInfo = renderableSeries.GetSeriesInfo(hitTestInfo);

                    vm.SelectedSeriesInfo = seriesInfo;
                }
            }
        }
    }
}
