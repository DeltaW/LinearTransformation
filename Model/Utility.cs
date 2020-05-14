using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LinearTransformation.Model {
    public class Utility {

        #region Round
        public static double Round(double value, double step) {
            // TODO: adjust this algorithm

            if (((double) ((int) (value / step))) == value / step)
                return value;

            double a = RoundUp(value, step) - value;
            double b = value - RoundDown(value, step);

            if (b < a) {
                Console.WriteLine("rounded " + value + " down");
                return RoundDown(value, step);
            }
            //if (a < b)
            Console.WriteLine("rounded " + value + " up");
            return RoundUp(value, step);
            //throw new Exception("aaa");
        }
        public static double RoundUp(double value, double step) {

            if (((double) ((int) (value / step))) == value / step)
                return value;

            return (step - value % step) + value;
        }
        public static double RoundDown(double value, double step) {
            return value - value % step;
        }
        #endregion

        public static System.Windows.Size GetTextSize(string text, double fontSize) {
            // Create a temporary label with the given content
            Label label = new Label {
                Content = text,
                FontSize = fontSize,
            };

            // Resize the label based on its content
            label.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            label.Arrange(new Rect(label.DesiredSize));

            // Return the size
            return new System.Windows.Size(label.ActualWidth, label.ActualHeight);
        }

        public static double Lerp(double from, double to, double by) {
            return from + (to - from) * by;
        }
        public static Vector Lerp(Vector from, Vector to, double by) {
            return new Vector(Utility.Lerp(from.X, to.X, by),
                              Utility.Lerp(from.Y, to.Y, by));
        }

        public static Random Random = new Random();
        public static double GetRandomDoubleWithinRange(double min, double max) {
            return Utility.Random.NextDouble() * (max - min) + min;
        }
        public static System.Windows.Media.Brush GetRandomBrush() {
            PropertyInfo[] properties = (typeof(System.Windows.Media.Brushes)).GetProperties();
            return (System.Windows.Media.Brush) properties[Utility.Random.Next(properties.Length)].GetValue(null, null);
        }

        internal static void ShowError(Exception e) {
            MessageBox.Show(e.Message, "Storch", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        public static void SetCustomColours(System.Windows.Forms.ColorDialog colorDialog) {

            for (int i = 0; i < colorDialog.CustomColors.Length; i++) {
                colorDialog.CustomColors[i] = ((int) Properties.Settings.Default[$"CustomColour{i + 1}"]);
            }
        }

        public static void SaveCustomColours(System.Windows.Forms.ColorDialog colorDialog) {
            for (int i = 0; i < colorDialog.CustomColors.Length; i++) {
                Properties.Settings.Default[$"CustomColour{i + 1}"] = colorDialog.CustomColors[i];
            }
            int a = (int) Properties.Settings.Default[$"CustomColour1"];
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

    }
}
