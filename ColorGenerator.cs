using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SilverlightCalendar
{
    public class ColorGenerator
    {
        private ObservableCollection<ColorRatio> _usedColors = new ObservableCollection<ColorRatio>();

        public ObservableCollection<ColorRatio> UsedColors
        {
            get { return _usedColors; }
        }

        public int Accuracy { get; set; }

        public ColorGenerator()
        {
            DistanceMin = MaximumDistanceMin;
            _usedColors.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_usedColors_CollectionChanged);
            Accuracy = 200;
        }

        // Only to help a bit keeping good diff. It should re-adjust itself.
        void _usedColors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                DistanceMin = MaximumDistanceMin;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count <= 2)
                {
                    DistanceMin += .1;
                }
                else
                {
                    DistanceMin = MaximumDistanceMin;
                }
            }
        }

        private double _distanceMin;

        private double DistanceMin
        {
            get { return _distanceMin; }
            set
            {
                if (value > MaximumDistanceMin)
                {
                    _distanceMin = MaximumDistanceMin;
                }
                else if (value < MinimumDistanceMin)
                {
                    _distanceMin = MinimumDistanceMin;
                }
                else
                {
                    _distanceMin = value;
                }
            }
        }

        private const double MaximumDistanceMin = 1.0;
        private const double MinimumDistanceMin = .01;
        private int _badTryCount = 0;
        private Random _random = new Random();

        public Color GetNextColor()
        {
            while (true)
            {
                double hue = _random.NextDouble() * 360;
                double saturation;
                double luminance;

                // To go quicker and darker for white background
                // saturation = Math.Sqrt(_random.NextDouble()) ; 
                // luminance = Math.Sqrt(_random.NextDouble());

                // To go quicker and lighter for dark background
                //saturation = Math.Pow(_random.NextDouble(), 2.0); 
                //luminance = Math.Pow(_random.NextDouble(), 2.0);

                // Less performance but higher compatibility
                saturation = _random.NextDouble();
                luminance = _random.NextDouble();

                HSL hsl = new HSL(hue, saturation, luminance);
                Color c = hsl.ToColor();

                if (IsFarEnoughFromExistingColor(c, DistanceMin))
                {
                    UsedColors.Add(new ColorRatio(c));
                    DistanceMin += .02;
                    _badTryCount = 0;
                    return c;
                }

                _badTryCount++;
                if (_badTryCount > Accuracy)
                {
                    _badTryCount = 0;
                    DistanceMin -= .002;
                }
            }
        }

        private bool IsFarEnoughFromExistingColor(Color c, double distanceMin)
        {
            foreach (ColorRatio coloRatio in UsedColors)
            {
                // double distance = ColorSpaceHelper.GetColorDistance(c, coloRatio.Color);

                // This is a lot better differences between color with CIELab calc.
                double distance = ColorSpaceHelper.GetColorDistanceCIELab(c, coloRatio.Color) / 100;

                if (distance / coloRatio.KeepAwayRatio < distanceMin)
                {
                    return false; // Too close
                }
            }

            return true;
        }

        public struct CIELab
        {
            /// <summary>
            /// Gets an empty CIELab structure.
            /// </summary>
            public static readonly CIELab Empty = new CIELab();

            private double l;
            private double a;
            private double b;

            /// <summary>
            /// Gets or sets L component.
            /// </summary>
            public double L
            {
                get
                {
                    return this.l;
                }
                set
                {
                    this.l = value;
                }
            }

            /// <summary>
            /// Gets or sets a component.
            /// </summary>
            public double A
            {
                get
                {
                    return this.a;
                }
                set
                {
                    this.a = value;
                }
            }

            /// <summary>
            /// Gets or sets a component.
            /// </summary>
            public double B
            {
                get
                {
                    return this.b;
                }
                set
                {
                    this.b = value;
                }
            }

            /// <summary>
            /// Returns the color difference (distance) between a sample color CIELap(2) and a reference color CIELap(1)
            /// <para>in accorance with CIE 2000 alogorithm.</para>
            /// </summary>
            /// <param name="lab1">CIELap reference color.</param>
            /// <param name="lab2">CIELap sample color.</param>
            /// <returns>Color difference.</returns>
            public static double GetDistanceBetweenCie2000(CIELab lab1, CIELab lab2)
            {
                double p25 = Math.Pow(25, 7);

                double C1 = Math.Sqrt(lab1.A * lab1.A + lab1.B * lab1.B);
                double C2 = Math.Sqrt(lab2.A * lab2.A + lab2.B * lab2.B);
                double avgC = (C1 + C2) / 2F;

                double powAvgC = Math.Pow(avgC, 7);
                double G = (1 - Math.Sqrt(powAvgC / (powAvgC + p25))) / 2D;

                double a_1 = lab1.A * (1 + G);
                double a_2 = lab2.A * (1 + G);

                double C_1 = Math.Sqrt(a_1 * a_1 + lab1.B * lab1.B);
                double C_2 = Math.Sqrt(a_2 * a_2 + lab2.B * lab2.B);
                double avgC_ = (C_1 + C_2) / 2D;

                double h1 = (Atan(lab1.B, a_1) >= 0 ? Atan(lab1.B, a_1) : Atan(lab1.B, a_1) + 360F);
                double h2 = (Atan(lab2.B, a_2) >= 0 ? Atan(lab2.B, a_2) : Atan(lab2.B, a_2) + 360F);

                double H = (h1 - h2 > 180D ? (h1 + h2 + 360F) / 2D : (h1 + h2) / 2D);

                double T = 1;
                T -= 0.17 * Cos(H - 30);
                T += 0.24 * Cos(2 * H);
                T += 0.32 * Cos(3 * H + 6);
                T -= 0.20 * Cos(4 * H - 63);

                double deltah = 0;
                if (h2 - h1 <= 180)
                    deltah = h2 - h1;
                else if (h2 <= h1)
                    deltah = h2 - h1 + 360;
                else
                    deltah = h2 - h1 - 360;

                double avgL = (lab1.L + lab2.L) / 2F;
                double deltaL_ = lab2.L - lab1.L;
                double deltaC_ = C_2 - C_1;
                double deltaH_ = 2 * Math.Sqrt(C_1 * C_2) * Sin(deltah / 2);

                double SL = 1 + (0.015 * Math.Pow(avgL - 50, 2)) / Math.Sqrt(20 + Math.Pow(avgL - 50, 2));
                double SC = 1 + 0.045 * avgC_;
                double SH = 1 + 0.015 * avgC_ * T;

                double exp = Math.Pow((H - 275) / 25, 2);
                double teta = Math.Pow(30, -exp);

                double RC = 2D * Math.Sqrt(Math.Pow(avgC_, 7) / (Math.Pow(avgC_, 7) + p25));
                double RT = -RC * Sin(2 * teta);

                double deltaE = 0;
                deltaE = Math.Pow(deltaL_ / SL, 2);
                deltaE += Math.Pow(deltaC_ / SC, 2);
                deltaE += Math.Pow(deltaH_ / SH, 2);
                deltaE += RT * (deltaC_ / SC) * (deltaH_ / SH);
                deltaE = Math.Sqrt(deltaE);

                return deltaE;
            }

            /// <summary>
            /// Returns the angle in degree whose tangent is the quotient of the two specified numbers.
            /// </summary>
            /// <param name="y">The y coordinate of a point.</param>
            /// <param name="x">The x coordinate of a point.</param>
            /// <returns>Angle in degree.</returns>
            private static double Atan(double y, double x)
            {
                return Math.Atan2(y, x) * 180D / Math.PI;
            }

            /// <summary>
            /// Returns the cosine of the specified angle in degree.
            /// </summary>
            /// <param name="d">Angle in degree</param>
            /// <returns>Cosine of the specified angle.</returns>
            private static double Cos(double d)
            {
                return Math.Cos(d * Math.PI / 180);
            }

            /// <summary>
            /// Returns the sine of the specified angle in degree.
            /// </summary>
            /// <param name="d">Angle in degree</param>
            /// <returns>Sine of the specified angle.</returns>
            private static double Sin(double d)
            {
                return Math.Sin(d * Math.PI / 180);
            }
        }

        public struct CIEXYZ
        {
            /// <summary>
            /// Gets an empty CIEXYZ structure.
            /// </summary>
            public static readonly CIEXYZ Empty = new CIEXYZ();
            /// <summary>
            /// Gets the CIE D65 (white) structure.
            /// </summary>
            public static readonly CIEXYZ D65 = new CIEXYZ(0.9505, 1.0, 1.0890);

            private double x;
            private double y;
            private double z;

            /// <summary>
            /// Gets or sets X component.
            /// </summary>
            public double X
            {
                get
                {
                    return this.x;
                }
                set
                {
                    this.x = (value > 0.9505) ? 0.9505 : ((value < 0) ? 0 : value);
                }
            }

            /// <summary>
            /// Gets or sets Y component.
            /// </summary>
            public double Y
            {
                get
                {
                    return this.y;
                }
                set
                {
                    this.y = (value > 1.0) ? 1.0 : ((value < 0) ? 0 : value);
                }
            }

            /// <summary>
            /// Gets or sets Z component.
            /// </summary>
            public double Z
            {
                get
                {
                    return this.z;
                }
                set
                {
                    this.z = (value > 1.089) ? 1.089 : ((value < 0) ? 0 : value);
                }
            }

            public CIEXYZ(double x, double y, double z)
            {
                this.x = (x > 0.9505) ? 0.9505 : ((x < 0) ? 0 : x);
                this.y = (y > 1.0) ? 1.0 : ((y < 0) ? 0 : y);
                this.z = (z > 1.089) ? 1.089 : ((z < 0) ? 0 : z);
            }
        }

        public class ColorRatio
        {
            public Color Color { get; set; }
            public double KeepAwayRatio { get; set; } // Usually 1, it could be ~ 4 for backcolor and 32 is extremely high. It define the distance between this color and the new generated one. Higher = greather distance. 

            public ColorRatio(Color color)
            {
                Color = color;
                KeepAwayRatio = 1;
            }

            public ColorRatio(Color color, double keepAwayRatio)
            {
                Color = color;
                KeepAwayRatio = keepAwayRatio;
            }

            public static implicit operator Color(ColorRatio coloRatio)
            {
                return coloRatio.Color;
            }
        }

    public sealed class ColorSpaceHelper
    {
        private ColorSpaceHelper() { }

        /// <summary>
        /// Gets the "distance" between two colors calculated with CIELab algo
        /// </summary>
        /// EO Added 2011-09-08
        /// <param name="color1">First color.</param>
        /// <param name="color2">Second color.</param>
        public static double GetColorDistanceCIELab(Color c1, Color c2)
        {
            return CIELab.GetDistanceBetweenCie2000(RGBtoLab(c1), RGBtoLab(c2));
        }

        /// <summary>
        /// Converts HSL to RGB.
        /// </summary>
        /// <param name="h">Hue, must be in [0, 360].</param>
        /// <param name="s">Saturation, must be in [0, 1].</param>
        /// <param name="l">Luminance, must be in [0, 1].</param>
        public static RGB HSLtoRGB(double h, double s, double l)
        {
            if (s == 0)
            {
                return new RGB(
                    Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", l * 255.0))),
                    Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", l * 255.0))),
                    Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", l * 255.0)))
                    );
            }
            else
            {
                double q = (l < 0.5) ? (l * (1.0 + s)) : (l + s - (l * s));
                double p = (2.0 * l) - q;

                double Hk = h / 360.0;
                double[] T = new double[3];
                T[0] = Hk + (1.0 / 3.0);
                T[1] = Hk;
                T[2] = Hk - (1.0 / 3.0);

                for (int i = 0; i < 3; i++)
                {
                    if (T[i] < 0) T[i] += 1.0;
                    if (T[i] > 1) T[i] -= 1.0;

                    if ((T[i] * 6) < 1)
                    {
                        T[i] = p + ((q - p) * 6.0 * T[i]);
                    }
                    else if ((T[i] * 2.0) < 1)
                    {
                        T[i] = q;
                    }
                    else if ((T[i] * 3.0) < 2)
                    {
                        T[i] = p + (q - p) * ((2.0 / 3.0) - T[i]) * 6.0;
                    }
                    else T[i] = p;
                }

                return new RGB(
                    Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", T[0] * 255.0))),
                    Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", T[1] * 255.0))),
                    Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", T[2] * 255.0)))
                    );
            }
        }

        /// <summary>
        /// Converts HSL to .net Color.
        /// </summary>
        /// <param name="hsl">The HSL structure to convert.</param>
        public static Color HSLtoColor(double h, double s, double l)
        {
            RGB rgb = HSLtoRGB(h, s, l);

            //return Color.FromRgb((byte)rgb.Red, (byte)rgb.Green, (byte)rgb.Blue);
            return Color.FromArgb(255, (byte)rgb.Red, (byte)rgb.Green, (byte)rgb.Blue);
        }

        /// <summary>
        /// Converts RGB to CIE XYZ (CIE 1931 color space)
        /// </summary>
        /// <param name="red">Red must be in [0, 255].</param>
        /// <param name="green">Green must be in [0, 255].</param>
        /// <param name="blue">Blue must be in [0, 255].</param>
        public static CIEXYZ RGBtoXYZ(int red, int green, int blue)
        {
            // normalize red, green, blue values
            double rLinear = (double)red / 255.0;
            double gLinear = (double)green / 255.0;
            double bLinear = (double)blue / 255.0;

            // convert to a sRGB form
            // corrected 2.2 to 2.4 (Rob)
            double r = (rLinear > 0.04045) ? Math.Pow((rLinear + 0.055) / (1 + 0.055), 2.4) : (rLinear / 12.92);
            double g = (gLinear > 0.04045) ? Math.Pow((gLinear + 0.055) / (1 + 0.055), 2.4) : (gLinear / 12.92);
            double b = (bLinear > 0.04045) ? Math.Pow((bLinear + 0.055) / (1 + 0.055), 2.4) : (bLinear / 12.92);

            // converts using sRGB Working Space Matrix and D65 Reference White
            // expanded constants (Rob)
            return new CIEXYZ(
                (r * 0.4124564 + g * 0.3575761 + b * 0.1804375),
                (r * 0.2126729 + g * 0.7151522 + b * 0.0721750),
                (r * 0.0193339 + g * 0.1191920 + b * 0.9503041)
                );
        }

        /// <summary>
        /// Converts RGB to CIELab.
        /// </summary>
        public static CIELab RGBtoLab(Color color)
        {
            return XYZtoLab(RGBtoXYZ(color.R, color.G, color.B));
        }

        /// <summary>
        /// XYZ to L*a*b* transformation function.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static double Fxyz(double t)
        {
            return ((t > 0.008856) ? Math.Pow(t, (1.0 / 3.0)) : (7.787 * t + 16.0 / 116.0));
        }

        /// <summary>
        /// Converts CIEXYZ to CIELab structure.
        /// </summary>
        public static CIELab XYZtoLab(double x, double y, double z)
        {
            CIELab lab = CIELab.Empty;
            lab.L = 116.0 * Fxyz(y / CIEXYZ.D65.Y) - 16;
            lab.A = 500.0 * (Fxyz(x / CIEXYZ.D65.X) - Fxyz(y / CIEXYZ.D65.Y));
            lab.B = 200.0 * (Fxyz(y / CIEXYZ.D65.Y) - Fxyz(z / CIEXYZ.D65.Z));

            return lab;
        }

        /// <summary>
        /// Converts CIEXYZ to CIELab structure.
        /// </summary>
        public static CIELab XYZtoLab(CIEXYZ xyz)
        {
            return XYZtoLab(xyz.X, xyz.Y, xyz.Z);
        }
    }

        public struct HSL
        {
            /// <summary>
            /// Gets an empty HSL structure;
            /// </summary>
            public static readonly HSL Empty = new HSL();

            #region Fields
            private double hue;
            private double saturation;
            private double luminance;
            #endregion

            /// <summary>
            /// Creates an instance of a HSL structure.
            /// </summary>
            /// <param name="h">Hue value.</param>
            /// <param name="s">Saturation value.</param>
            /// <param name="l">Lightness value.</param>
            public HSL(double h, double s, double l)
            {
                hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
                saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
                luminance = (l > 1) ? 1 : ((l < 0) ? 0 : l);
            }

            public Color ToColor()
            {
                return ColorSpaceHelper.HSLtoColor(hue, saturation, luminance);
            }
        }

        public struct RGB
        {
            /// <summary>
            /// Gets an empty RGB structure;
            /// </summary>
            public static readonly RGB Empty = new RGB();

            private int red;
            private int green;
            private int blue;

            [Description("Red component."),]
            public int Red
            {
                get
                {
                    return red;
                }
                set
                {
                    red = (value > 255) ? 255 : ((value < 0) ? 0 : value);
                }
            }

            [Description("Green component."),]
            public int Green
            {
                get
                {
                    return green;
                }
                set
                {
                    green = (value > 255) ? 255 : ((value < 0) ? 0 : value);
                }
            }

            [Description("Blue component."),]
            public int Blue
            {
                get
                {
                    return blue;
                }
                set
                {
                    blue = (value > 255) ? 255 : ((value < 0) ? 0 : value);
                }
            }

            public RGB(int R, int G, int B)
            {
                red = (R > 255) ? 255 : ((R < 0) ? 0 : R);
                green = (G > 255) ? 255 : ((G < 0) ? 0 : G);
                blue = (B > 255) ? 255 : ((B < 0) ? 0 : B);
            }
        }
    } 
}