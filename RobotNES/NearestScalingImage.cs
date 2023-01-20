using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RobotNES
{
    internal class NearestScalingImage : Image
    {
        public NearestScalingImage(WriteableBitmap source) : base()
        {
            Source = source;
        }

        protected override void OnRender(DrawingContext dc)
        {
            VisualBitmapScalingMode = BitmapScalingMode.NearestNeighbor;
            base.OnRender(dc);
        }
    }
}
