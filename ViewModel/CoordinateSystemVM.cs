using LinearTransformation.Components;
using LinearTransformation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LinearTransformation.ViewModel {
    public class CoordinateSystemVM {
        public List<CanvasVector> Vectors { get; set; }

        private readonly MainControlVM _mainControlVM;
        private readonly Canvas _canvas;
        public CoordinateSystemData _data;
        public CoordinateSystemData _dynamicData;

        private readonly List<Tuple<Vector, Vector>> _previousTransformations = new List<Tuple<Vector, Vector>>();

        // movement Variables
        private bool _isDragging;
        private Vector _mouseLocationWithinCanvas;


        private void Control_MouseLeftButtonDown(object sender, MouseEventArgs e) {
            this._isDragging = true;
            Point m = e.GetPosition(this._canvas);
            this._mouseLocationWithinCanvas = new Vector(m.X, m.Y);
            this._canvas.CaptureMouse();
        }
        private void Control_MouseLeftButtonUp(object sender, MouseEventArgs e) {
            this._isDragging = false;
            this._canvas.ReleaseMouseCapture();
        }
        private void Control_MouseMove(object sender, MouseEventArgs e) {
            Point p = e.GetPosition(this._canvas);
            Vector m = new Vector(p.X, p.Y);
            Size canvasSize = new Size(this._canvas.ActualWidth, this._canvas.ActualHeight);
            Vector fromMousePosition = CoordinateConverter.FromPointToCoordinate(canvasSize,
                                                                                   this._data,
                                                                                   this._mouseLocationWithinCanvas);
                Vector toMousePosition = CoordinateConverter.FromPointToCoordinate(canvasSize,
                                                                                   this._data,
                                                                                   m);
            if (this._isDragging) {




                double scrollspeed = (this._data.UnitX + this._data.UnitY) / 2;
                Vector distance = (fromMousePosition - toMousePosition) * scrollspeed;


                this._data.MinX += distance.X;
                this._data.MaxX += distance.X;
                this._data.MinY += distance.Y;
                this._data.MaxY += distance.Y;

                this._dynamicData.MinX += distance.X;
                this._dynamicData.MaxX += distance.X;
                this._dynamicData.MinY += distance.Y;
                this._dynamicData.MaxY += distance.Y;

                this._mouseLocationWithinCanvas = m;

                this._mainControlVM.UpdateWindowSettings(this._data);


                this.Update();
            }
            Vector temp2 = CoordinateConverter.FromPointToCoordinate(canvasSize,
                                                                        this._dynamicData,
                                                                        new Vector(0,0)/*m*/);
            Vector temp = CoordinateConverter.FromStaticToDynamic(this._dynamicData.IHat, this._dynamicData.JHat, toMousePosition);
            this._mainControlVM._mainControl.Label_DynamicCoordinate.Content = $"dynamic: ({temp.X} | {temp.Y})";
            this._mainControlVM._mainControl.Label_StaticCoordinate.Content = $"static:  ({toMousePosition.X} | {toMousePosition.Y})";
            this._mainControlVM._mainControl.Label_test.Content = $"dynamic (0|0): ({temp2.X} | {temp2.Y})";

            this._mainControlVM._mainControl.Label_test2.Content = $"dynamic (ihat): ({this._dynamicData.IHat.X} | {this._dynamicData.IHat.Y})";
            this._mainControlVM._mainControl.Label_test3.Content = $"dynamic (jhat): ({this._dynamicData.JHat.X} | {this._dynamicData.JHat.Y})";


        }
        private void Control_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (e.Delta > 0) {
                // Zoom in
                this._data.MinX *= .9;
                this._data.MaxX *= .9;
                this._data.MinY *= .9;
                this._data.MaxY *= .9;
                this._dynamicData.MinX *= .9;
                this._dynamicData.MaxX *= .9;
                this._dynamicData.MinY *= .9;
                this._dynamicData.MaxY *= .9;
                this._data.SetUnitAndStepDynamically(new Size(this._canvas.ActualWidth, this._canvas.ActualHeight));
                this._mainControlVM.UpdateWindowSettings(this._data);
                this.Update();
            } else if (e.Delta < 0) {
                // Zoom out
                this._data.MinX *= 1.1;
                this._data.MaxX *= 1.1;
                this._data.MinY *= 1.1;
                this._data.MaxY *= 1.1;
                this._dynamicData.MinX *= 1.1;
                this._dynamicData.MaxX *= 1.1;
                this._dynamicData.MinY *= 1.1;
                this._dynamicData.MaxY *= 1.1;
                this._data.SetUnitAndStepDynamically(new Size(this._canvas.ActualWidth, this._canvas.ActualHeight));
                this._mainControlVM.UpdateWindowSettings(this._data);
                this.Update();
            }
        }

        public void Control_KeyboardMove(object sender, KeyEventArgs e) {
            Key key = (Key) e.Key;

            bool canvasNeedsRedraw = false;


            if (key == Key.Up && !Keyboard.IsKeyDown(Key.Down)) {
                double distance = (this._data.StepY == 0) ? this._data.UnitY
                                                          : this._data.StepY;
                this._data.MinY += distance;
                this._data.MaxY += distance;
                this._dynamicData.MinY += distance;
                this._dynamicData.MaxY += distance;
                canvasNeedsRedraw = true;
            }

            if (key == Key.Down && !Keyboard.IsKeyDown(Key.Up)) {
                double distance = (this._data.StepY == 0) ? this._data.UnitY
                                                          : this._data.StepY;
                this._data.MinY -= distance;
                this._data.MaxY -= distance;
                this._dynamicData.MinY -= distance;
                this._dynamicData.MaxY -= distance;
                canvasNeedsRedraw = true;
            }

            if (key == Key.Left && !Keyboard.IsKeyDown(Key.Right)) {
                double distance = (this._data.StepY == 0) ? this._data.UnitY
                                                          : this._data.StepY;
                this._data.MinX -= distance;
                this._data.MaxX -= distance;
                this._dynamicData.MinX -= distance;
                this._dynamicData.MaxX -= distance;
                canvasNeedsRedraw = true;
            }

            if (key == Key.Right && !Keyboard.IsKeyDown(Key.Left)) {
                double distance = (this._data.StepY == 0) ? this._data.UnitY
                                                          : this._data.StepY;
                this._data.MinX += distance;
                this._data.MaxX += distance;
                this._dynamicData.MinX += distance;
                this._dynamicData.MaxX += distance;
                canvasNeedsRedraw = true;
            }

            if (canvasNeedsRedraw) {
                this._mainControlVM.UpdateWindowSettings(this._data);
                this.Update();
            }
        }

        public CoordinateSystemVM(MainControlVM mainControlVM, Canvas canvas) {
            this._mainControlVM = mainControlVM;
            this._canvas = canvas;
            this.InstantiateViewSettings();
            this.AddMovementFunctionality();
            this.Vectors = new List<CanvasVector>();
        }

        public CoordinateSystemVM(MainControlVM mainControlVM, Canvas canvas, CoordinateSystemData data, List<CanvasVector> vectors) {
            this._mainControlVM = mainControlVM;
            this._canvas = canvas;
            this._data = data;
            this._dynamicData = data;
            this.AddMovementFunctionality();
            if (vectors == null) {
                this.Vectors = new List<CanvasVector>();
            } else {
                this.Vectors = vectors;
            }
        }

        private void AddMovementFunctionality() {
            // Adding mouse movement
            this._canvas.MouseLeftButtonDown += new MouseButtonEventHandler(this.Control_MouseLeftButtonDown);
            this._canvas.MouseLeftButtonUp += new MouseButtonEventHandler(this.Control_MouseLeftButtonUp);
            this._canvas.MouseMove += new MouseEventHandler(this.Control_MouseMove);

            // Adding keyboard movement
            this._canvas.KeyDown += new KeyEventHandler(this.Control_KeyboardMove);

            // Adding scroll wheel zoom
            this._canvas.MouseWheel += new MouseWheelEventHandler(this.Control_MouseWheel);
        }

        public void Update() {
            bool showStaticGrid = (bool) this._mainControlVM._mainControl.ToggleButton_StaticGrid.IsChecked;
            bool showVectors = (bool) this._mainControlVM._mainControl.ToggleButton_Vectors.IsChecked;
            bool showDynamicGrid = (bool) this._mainControlVM._mainControl.ToggleButton_DynamicGrid.IsChecked;
            bool showBasisVectors = (bool) this._mainControlVM._mainControl.ToggleButton_BasisVectors.IsChecked;

            this._dynamicData = CoordinateSystemData.CalculateBoundaries(new Size(this._canvas.ActualWidth,
                                                                                  this._canvas.ActualHeight),
                                                                         this._data,
                                                                         new Tuple<Vector, Vector>(this._dynamicData.IHat,
                                                                                                   this._dynamicData.JHat));
            //this._dynamicData = new CoordinateSystemData(-1, 1, -1, 1, 1, .5);
            //this._dynamicData.IHat = this._data.IHat;
            //this._dynamicData.JHat = this._data.JHat;

            this._canvas.Children.Clear();

            if (showStaticGrid)
                CoordinateSystemDrawer.Draw(this._canvas, this._data);
            if (showDynamicGrid)
                CoordinateSystemDrawer.Draw(this._canvas, this._dynamicData);
            if (showVectors)
                this.InstantiateVectors(this._dynamicData, this.Vectors);
            if (showBasisVectors)
                CoordinateSystemDrawer.DrawBasisVectors(this._canvas, this._dynamicData);

        }

        public CanvasVector AddVector(double x, double y, Brush b) {
            CanvasVector canvasVector = new CanvasVector(new Size(this._canvas.ActualWidth, this._canvas.ActualHeight),
                                                         b, this._data, new Vector(x, y), new Vector(0, 0));
            this.Vectors.Add(canvasVector);
            this.Update();
            return canvasVector;
        }

        public void DeleteVector(CanvasVector canvasVector) {
            this._canvas.Children.Remove(canvasVector);
            this.Vectors.Remove(canvasVector);
        }

        public void NewTransformation(Vector iHat, Vector jHat) {
            //this._dynamicData = this._data;
            //this._dynamicData.IHat = iHat;
            //this._dynamicData.JHat = jHat;

            // Store the current Basis Vectors
            this._previousTransformations.Add(new Tuple<Vector, Vector>(this._dynamicData.IHat, this._dynamicData.JHat));

            this._mainControlVM._mainControl.Button_Undo.IsEnabled = true;

            // Start the transformation
            this.StartAnimation(new Vector(this._data.IHat.X * iHat.X + this._data.JHat.X * iHat.Y,
                                           this._data.IHat.X * jHat.X + this._data.JHat.X * jHat.Y),
                                new Vector(this._data.IHat.Y * iHat.X + this._data.JHat.Y * iHat.Y,
                                           this._data.IHat.Y * jHat.X + this._data.JHat.Y * jHat.Y));

            //this.Update();
        }

        public void UndoTransformation(object sender, RoutedEventArgs e) {
            if (this._previousTransformations.Count > 0) {
                Vector iHat = this._previousTransformations[this._previousTransformations.Count - 1].Item1;
                Vector jHat = this._previousTransformations[this._previousTransformations.Count - 1].Item2;

                this.StartAnimation(iHat, jHat);
                this._previousTransformations.RemoveAt(this._previousTransformations.Count - 1);
                if (this._previousTransformations.Count == 0)
                    this._mainControlVM._mainControl.Button_Undo.IsEnabled = false;
            }
        }

        private Task StartAnimation(Vector iHat, Vector jHat, int fps = 60) {
            return Task.Run(() => Application.Current.Dispatcher.Invoke(async () => {
                //this._dynamicData.IHat.X
                //double iHatStepX = /*Math.Abs*/(iHat.X - this._dynamicData.IHat.X ) / duration / fps;
                //double iHatStepY = /*Math.Abs*/(iHat.Y - this._dynamicData.IHat.Y ) / duration / fps;
                //double jHatStepX = /*Math.Abs*/(jHat.X - this._dynamicData.JHat.X ) / duration / fps;
                //double jHatStepY = /*Math.Abs*/(jHat.Y - this._dynamicData.JHat.Y ) / duration / fps;
                int max = 1/*duration / 1000 * fps*/;
                for (int i = 0; i <= max; i++) {
                    double by = ((double) i) / ((double) max);
                    this._dynamicData.IHat = Utility.Lerp(this._dynamicData.IHat, iHat, by);
                    this._dynamicData.JHat = Utility.Lerp(this._dynamicData.JHat, jHat, by);

                    await Task.Delay(1000 / fps);
                    this.Update();
                    Application.Current.Dispatcher.Invoke(delegate { }, System.Windows.Threading.DispatcherPriority.Render);
                }

                //while (iHat != this._dynamicData.IHat && jHat != this._dynamicData.JHat) {
                //for (int i = 0; i < (duration * 1000) - (1000 / fps); i += (1000) / fps) {
                //    // Set iHat and jHat accordingly
                //    //this._dynamicData.IHat.X = this._dynamicData.IHat.X + iHatStepX;
                //    //this._dynamicData.IHat.Y = this._dynamicData.IHat.Y + iHatStepY;
                //    //this._dynamicData.JHat.X = this._dynamicData.JHat.X + jHatStepX;
                //    //this._dynamicData.JHat.Y = this._dynamicData.JHat.Y + jHatStepY;

                //    this._dynamicData.IHat = Utility.Lerp(this._dynamicData.IHat, iHat, i / (duration * 1000));
                //    this._dynamicData.JHat = Utility.Lerp(this._dynamicData.JHat, jHat, i / (duration * 1000));

                //    // Wait
                //    await Task.Delay((1000 / fps));
                //    // Update UI
                //    this.Update();
                //    Application.Current.Dispatcher.Invoke(delegate { }, System.Windows.Threading.DispatcherPriority.Render);

                //}

                //// Just set them directly afterwards to compensate for small differences
                //this._dynamicData.IHat = iHat;
                //this._dynamicData.JHat = jHat;


                // Wait
                //await Task.Delay((1000 / fps));

                //// Update UI
                //this.Update();
                //Application.Current.Dispatcher.Invoke(delegate { }, System.Windows.Threading.DispatcherPriority.Render);
            }));
        }


        private void InstantiateVectors(CoordinateSystemData data, List<CanvasVector> vectors) {
            foreach (CanvasVector vector in vectors) {
                vector.CanvasSize = new Size(this._canvas.ActualWidth, this._canvas.ActualHeight);
                vector.Data = data;
                vector.UpdateCoordinates();
                this._canvas.Children.Add(vector);
            }
        }

        private void InstantiateViewSettings() {

            this._data = new CoordinateSystemData {
                MinX = -3,
                MaxX = 3,
                MinY = -3,
                MaxY = 3,
                UnitX = 1,
                UnitY = 1,
                StepX = .5,
                StepY = .5,
            };

            //this._data = new CoordinateSystemData(
            //    minX: -4,
            //    minY: -4,
            //    maxX:  4,
            //    maxY:  4,
            //    unit:  1,
            //    step: .5
            //);
        }

    }
}
