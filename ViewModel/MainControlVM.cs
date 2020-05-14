using LinearTransformation.Components;
using LinearTransformation.Model;
using LinearTransformation.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LinearTransformation.ViewModel {
    public class MainControlVM: INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string name = "")
               => this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));

        private UserControl _coordinateSystemControl;

        public UserControl CoordinateSystemControl {
            get { return this._coordinateSystemControl; }
            set {
                this._coordinateSystemControl = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<VectorListItem> Vectors { get; set; } = new ObservableCollection<VectorListItem>();

        public MainControl _mainControl;

        public System.Windows.Forms.ColorDialog ColourDialog = new System.Windows.Forms.ColorDialog();

        public MainControlVM(MainControl mainControl) {
            this._mainControl = mainControl;

            CoordinateSystemData data = new CoordinateSystemData(-3, 3, -3, 3, 1, .5);
            this._coordinateSystemControl = new CoordinateSystem(this, data);
            this.UpdateWindowSettings(data);
        }

        public void Button_Click_AddVector(object sender, RoutedEventArgs e) {
            try {
                if (!double.TryParse(this._mainControl.InputVectorX.Text, out double x))
                    throw new Exception("Invalid X Value");

                if (!double.TryParse(this._mainControl.InputVectorY.Text, out double y))
                    throw new Exception("Invalid Y Value");

                if (double.IsNaN(x) || x == 0)
                    throw new Exception("Invalid X Value");

                if (double.IsNaN(y) || y == 0)
                    throw new Exception("Invalid Y Value");

                Brush b = this._mainControl.InputVectorColour.Background;

                CanvasVector canvasVector = ((CoordinateSystem) this._coordinateSystemControl).AddVector(x, y, b);
                this.Vectors.Add(new VectorListItem(this, canvasVector));

                this._mainControl.InputVectorX.Text = "";
                this._mainControl.InputVectorY.Text = "";

            } catch (Exception exception) {
                Utility.ShowError(exception);
            }
        }

        public void Button_Click_AddRandomVector(object sender, RoutedEventArgs e) {


            CoordinateSystemVM cs = ((CoordinateSystem) this._coordinateSystemControl)._coordinateSystemVM;
            CoordinateSystemData data = cs._dynamicData;
            double x = 0;
            double y = 0;
            while (x == 0) {
                x = Utility.GetRandomDoubleWithinRange(data.MinX, data.MaxX);
            }
            while (y == 0) {
                y = Utility.GetRandomDoubleWithinRange(data.MinY, data.MaxY);
            }

            Brush b = Utility.GetRandomBrush();

            CanvasVector canvasVector = ((CoordinateSystem) this._coordinateSystemControl).AddVector(x, y, b);
            this.Vectors.Add(new VectorListItem(this, canvasVector));

            this._mainControl.InputVectorX.Text = "";
            this._mainControl.InputVectorY.Text = "";
        }

        internal void UpdateWindowSettings(CoordinateSystemData data) {
            this._mainControl.InputMinX.Text = $"{data.MinX}";
            this._mainControl.InputMaxX.Text = $"{data.MaxX}";
            this._mainControl.InputMinY.Text = $"{data.MinY}";
            this._mainControl.InputMaxY.Text = $"{data.MaxY}";
            this._mainControl.InputUnitX.Text = $"{data.UnitX}";
            this._mainControl.InputUnitY.Text = $"{data.UnitY}";
            this._mainControl.InputStepX.Text = $"{data.StepY}";
            this._mainControl.InputStepY.Text = $"{data.StepY}";
        }

        public void Button_Click_Transform(object sender, RoutedEventArgs e) {
            try {
                // IHat
                if (!double.TryParse(this._mainControl.InputIHatX.Text, out double ix))
                    throw new Exception("Invalid Value");
                if (!double.TryParse(this._mainControl.InputIHatY.Text, out double iy))
                    throw new Exception("Invalid Value");

                //JHat
                if (!double.TryParse(this._mainControl.InputJHatX.Text, out double jx))
                    throw new Exception("Invalid Value");
                if (!double.TryParse(this._mainControl.InputJHatY.Text, out double jy))
                    throw new Exception("Invalid Value");

                Vector iHat = new Vector(ix, iy);
                Vector jHat = new Vector(jx, jy);
                ((CoordinateSystem) this.CoordinateSystemControl)._coordinateSystemVM.NewTransformation(iHat, jHat);
            } catch (Exception exception) {
                Utility.ShowError(exception);
            }

        }
        public void Button_Click_UndoTransform(object sender, RoutedEventArgs e) {
            ((CoordinateSystem) this.CoordinateSystemControl)._coordinateSystemVM.UndoTransformation(sender, e);
        }

        public void ToggleButtons_StateChanged(object sender, RoutedEventArgs e) {
            this.UpdateCoordinateSystem();
        }

        public void UpdateCoordinateSystem() {
            ((CoordinateSystem) this._coordinateSystemControl)._coordinateSystemVM.Update();
        }

        public void Canvas_MouseLeftButtonDown_SelectVectorColour(object sender, MouseButtonEventArgs e) {
            System.Windows.Media.Brush mediaBrush = ((Canvas) sender).Background;
            System.Drawing.SolidBrush drawingSolidBrush = new System.Drawing.SolidBrush(
                (System.Drawing.Color) new System.Drawing.ColorConverter().ConvertFromString(new BrushConverter().ConvertToString(mediaBrush)));
            //double a = drawingSolidBrush.Color.A;
            //double r = drawingSolidBrush.Color.R;
            //double g = drawingSolidBrush.Color.G;
            //double b = drawingSolidBrush.Color.B;

            this.ColourDialog.Color = drawingSolidBrush.Color;
            if (this.ColourDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                ((Canvas) sender).Background = new SolidColorBrush(Color.FromArgb(this.ColourDialog.Color.A,
                                                                                  this.ColourDialog.Color.R,
                                                                                  this.ColourDialog.Color.G,
                                                                                  this.ColourDialog.Color.B));
            }
        }

        public void Button_Click_ApplyChanges(object sender, RoutedEventArgs e) {
            // TODO:
            try {

                if (!double.TryParse(this._mainControl.InputMinX.Text, out double minX))
                    throw new Exception("Invalid Value");
                if (!double.TryParse(this._mainControl.InputMaxX.Text, out double maxX))
                    throw new Exception("Invalid Value");
                if (!double.TryParse(this._mainControl.InputMinY.Text, out double minY))
                    throw new Exception("Invalid Value");
                if (!double.TryParse(this._mainControl.InputMaxY.Text, out double maxY))
                    throw new Exception("Invalid Value");
                if (!double.TryParse(this._mainControl.InputUnitX.Text, out double unitX))
                    throw new Exception("Invalid Value");
                if (!double.TryParse(this._mainControl.InputUnitY.Text, out double unitY))
                    throw new Exception("Invalid Value");
                if (!double.TryParse(this._mainControl.InputStepX.Text, out double stepX))
                    throw new Exception("Invalid Value");
                if (!double.TryParse(this._mainControl.InputStepY.Text, out double stepY))
                    throw new Exception("Invalid Value");

                var temp = (CoordinateSystem) this.CoordinateSystemControl;
                temp._coordinateSystemVM._data = new CoordinateSystemData(minX, maxX, minY, maxY, unitX, unitY, stepX, stepY);
                temp._coordinateSystemVM._dynamicData = new CoordinateSystemData(minX, maxX, minY, maxY, unitX, unitY, stepX, stepY) {
                    IHat = temp._coordinateSystemVM._dynamicData.IHat,
                    JHat = temp._coordinateSystemVM._dynamicData.JHat,
                };
                temp._coordinateSystemVM.Update();

            } catch (Exception exception) {
                Utility.ShowError(exception);
            }
        }

        public void Button_Click_RevertChanges(object sender, RoutedEventArgs e) {
            var temp = (CoordinateSystem) this.CoordinateSystemControl;
            this._mainControl.InputMinX.Text = $"{temp._coordinateSystemVM._data.MinX}";
            this._mainControl.InputMaxX.Text = $"{temp._coordinateSystemVM._data.MaxX}";
            this._mainControl.InputMinY.Text = $"{temp._coordinateSystemVM._data.MinY}";
            this._mainControl.InputMaxY.Text = $"{temp._coordinateSystemVM._data.MaxY}";
            this._mainControl.InputUnitX.Text = $"{temp._coordinateSystemVM._data.UnitX}";
            this._mainControl.InputUnitY.Text = $"{temp._coordinateSystemVM._data.UnitY}";
            this._mainControl.InputStepX.Text = $"{temp._coordinateSystemVM._data.StepY}";
            this._mainControl.InputStepY.Text = $"{temp._coordinateSystemVM._data.StepY}";
        }

        public void DeleteVector(CanvasVector canvasVector) {
            ((CoordinateSystem) this.CoordinateSystemControl).DeleteVector(canvasVector);
        }
    }
}
