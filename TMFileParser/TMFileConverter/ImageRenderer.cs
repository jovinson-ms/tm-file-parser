using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TMFileParser.Models.output;

namespace TMFileConverter
{
    internal class ImageRenderer
    {
        private const float ScaleConstant = 1.5F;
        private const int ImageWidth = 2000;
        private const int ImageHeight = 2200;

        public static void RenderImage(object model, string baseFilePath)
        {
            
            var data = model as TM7All;

            foreach (var diagram in data.Diagrams)
            {
                // todo: get the dimensions from the scan
                var image = new Bitmap(Scale(ImageWidth), Scale(ImageHeight));
                image.SetResolution(Scale(96), Scale(96));

                using (var graphics = Graphics.FromImage(image))
                {
                    graphics.Clear(Color.White);
                    graphics.DrawString("asdf", new Font("Arial", 20), new SolidBrush(Color.Black), new PointF(Scale(0), Scale(0)));
                    RenderDiagram(diagram, graphics);
                }

                image.Save(baseFilePath + "-" + diagram.Diagram + ".png");
            }
        }

        private static int Scale(int input)
        {
            return (int)(input * ScaleConstant);
        }

        private static int Scale(decimal input)
        {
            return (int)((float)input * ScaleConstant);
        }

        private static void RenderDiagram(TM7Diagram diagram, Graphics graphics) 
        {
            var assetPen = new Pen(Color.Black, Scale(1));
            foreach(TM7Asset asset in diagram.Assets)
            {
                var rect = new Rectangle(Scale(asset.Left), Scale(asset.Top), Scale(asset.Width), Scale(asset.Height));
                
                switch (asset.Type)
                {
                    case TM7AssetType.None:
                        continue;
                    case TM7AssetType.StencilEllipse:
                        graphics.DrawEllipse(assetPen, rect);
                        break;
                    default:
                        graphics.DrawRectangle(assetPen, rect);
                        break;
                }
                

                var stringFormat = StringFormat.GenericTypographic;
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                graphics.DrawString(asset.DisplayName, new Font("Arial", 10), new SolidBrush(Color.Black), rect, stringFormat);
            }

            var boundaryPen = new Pen(Color.Red, Scale(1))
            {
                DashStyle = DashStyle.Dash
            };
            foreach (TM7Boundary boundary in diagram.Boundaries)
            {
                var boundaryRect = new Rectangle(Scale(boundary.Left), Scale(boundary.Top), Scale(boundary.Width), Scale(boundary.Height));
                graphics.DrawRectangle(boundaryPen, boundaryRect);

                var boundaryFormat = StringFormat.GenericTypographic;
                boundaryFormat.Alignment = StringAlignment.Far;
                boundaryFormat.LineAlignment = StringAlignment.Near;

                graphics.DrawString(boundary.DisplayName, new Font("Arial", 10), new SolidBrush(Color.Red), boundaryRect, boundaryFormat);
            }

            var connectorPen = new Pen(Color.Black, Scale(1))
            {
                CustomEndCap = new AdjustableArrowCap(Scale(5), Scale(5))
            };
            foreach (TM7Connector connector in diagram.Connectors)
            {
                var scaledConnector = new TM7Connector
                {
                    Id = connector.Id,
                    SourceId = connector.SourceId,
                    TargetId = connector.TargetId,
                    Name = connector.Name,
                    DisplayName = connector.DisplayName,
                    HandleX = Scale(connector.HandleX),
                    HandleY = Scale(connector.HandleY),
                    SourceX = Scale(connector.SourceX),
                    SourceY = Scale(connector.SourceY),
                    TargetX = Scale(connector.TargetX),
                    TargetY = Scale(connector.TargetY),
                };
                
                var points = CalculateBezierPoints(scaledConnector); 
                graphics.DrawCurve(connectorPen, points);

                var font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
                var height = font.Height + Scale(10);
                var width = (int)graphics.MeasureString(scaledConnector.DisplayName, font).Width + Scale(10);
                
                var boundingRect = new Rectangle(scaledConnector.HandleX - width / 2, scaledConnector.HandleY, width, height);
                graphics.FillRectangle(new SolidBrush(Color.Honeydew), boundingRect);
                graphics.DrawRectangle(assetPen, boundingRect);
                
                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                };

                // TODO: make sure this works with multi-line connector names
                graphics.DrawString(scaledConnector.DisplayName, font, new SolidBrush(Color.Black), boundingRect, stringFormat);
            }
        }

        private static Point[] CalculateBezierPoints(TM7Connector connector)
        {
            Point[] result = new Point[32];
            Point control = CalculateBezierControlPoint(connector);
            int ax = connector.SourceX - 2 * control.X + connector.TargetX;
            int bx = 2 * control.X - 2 * connector.SourceX;
            int cx = connector.SourceX;

            int ay = connector.SourceY - 2 * control.Y + connector.TargetY;
            int by = 2 * control.Y - 2 * connector.SourceY;
            int cy = connector.SourceY;

            result[0] = new Point(cx, cy);

            for (int i = 1; i < 32; i++)
            {
                double t = i / (double)32;

                int bx1 = Convert.ToInt32(ax * t * t + bx * t + cx);
                int by1 = Convert.ToInt32(ay * t * t + by * t + cy);

                result[i] = new Point(bx1, by1);
            }

            return result;
        }

        private static Point CalculateBezierControlPoint(TM7Connector connector)
        {
            double distance = Math.Sqrt(Math.Pow(connector.TargetX - connector.SourceX, 2) + Math.Pow(connector.TargetY - connector.SourceY, 2));
            if (distance < 2)
            {
                return new Point(connector.SourceX, connector.SourceY);
            }
            else
            {
                return new Point(
                    connector.SourceX + (int)(2 * (double)connector.HandleX - 1.5 * connector.SourceX - 0.5 * connector.TargetX),
                    connector.SourceY + (int)(2 * (double)connector.HandleY - 1.5 * connector.SourceY - 0.5 * connector.TargetY));
            }
        }
    }
}
