using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace LinearTransformation.Model {
    public struct CoordinateSystemData {
        private double _minX;
        private double _maxX;
        private double _minY;
        private double _maxY;
        private double _unitX;
        private double _unitY;
        private double _stepX;
        private double _stepY;

        public Vector IHat;
        public Vector JHat;

        public double MinX {
            get => this._minX;
            set {
                if (value >= this.MaxX)
                    throw new Exception("Invalid range");
                this._minX = value;
            }
        }
        public double MaxX {
            get => this._maxX;
            set {
                if (value <= this.MinX)
                    throw new Exception("Invalid range");
                this._maxX = value;
            }
        }
        public double MinY {
            get => this._minY;
            set {
                if (value >= this.MaxY)
                    throw new Exception("Invalid range");
                this._minY = value;
            }
        }
        public double MaxY {
            get => this._maxY;
            set {
                if (value <= this.MinY)
                    throw new Exception("Invalid range");
                this._maxY = value;
            }
        }
        public double UnitX {
            get => this._unitX;
            set {
                if (value <= 0)
                    throw new Exception("Value can not be smaller than 0");
                this._unitX = value;
            }
        }
        public double UnitY {
            get => this._unitY;
            set {
                if (value <= 0)
                    throw new Exception("Value can not be smaller than 0");
                this._unitY = value;
            }
        }
        public double StepX {
            get => this._stepX;
            set {
                if (value < 0)
                    throw new Exception("Value can not be smaller than 0");
                this._stepX = value;
            }
        }
        public double StepY {
            get => this._stepY;
            set {
                if (value < 0)
                    throw new Exception("Value can not be smaller than 0");
                this._stepY = value;
            }
        }

        public CoordinateSystemData(double minX, double maxX,
                                    double minY, double maxY,
                                    double unit, double step) {

            if (minX > maxX || minY > maxY || minX == maxX || minY == maxY) {
                throw new Exception("Invalid boundaries");
            }

            if (unit <= 0) {
                throw new Exception("Unit can not be smaller than or equal to 0");
            }

            if (step < 0) {
                throw new Exception("Step can not be smaller than 0");
            }

            this._minX = minX;
            this._maxX = maxX;
            this._minY = minY;
            this._maxY = maxY;
            this._unitX = unit;
            this._unitY = unit;
            this._stepX = step;
            this._stepY = step;

            this.IHat = new Vector(1, 0);
            this.JHat = new Vector(0, 1);
        }

        public CoordinateSystemData(double minX, double maxX,
                                    double minY, double maxY,
                                    double unitX, double unitY,
                                    double stepX, double stepY) {

            if (minX > maxX || minY > maxY || minX == maxX || minY == maxY) {
                throw new Exception("Invalid boundaries");
            }

            if (unitX <= 0 || unitY <= 0) {
                throw new Exception("Unit can not be smaller than or equal to 0");
            }

            if (stepX < 0 || stepY < 0) {
                throw new Exception("Step can not be smaller than 0");
            }

            this._minX = minX;
            this._maxX = maxX;
            this._minY = minY;
            this._maxY = maxY;
            this._unitX = unitX;
            this._unitY = unitY;
            this._stepX = stepX;
            this._stepY = stepY;

            this.IHat = new Vector(1, 0);
            this.JHat = new Vector(0, 1);
        }

        private static double CalculateCellAmount(double min, double max, double unit) {
            // Calculate the amount of units which fit into the given range

            //TODO: I think this can be turned into a single return
            // return (Math.Abs(max) - min) * temp;

            // Variable used to adjusting the cell amount based on the unit value
            double temp = ((unit < 1) ? (1 / unit * .5) : 1);

            // case 1: Min < 0 < Max
            if (min < 0 && 0 < max) {
                return (Math.Abs(min) + Math.Abs(max)) * temp;
            }

            // case 2: 0 < Min < Max
            if (0 < min) {
                return (max - min) * temp;
            }

            // case 3: Min < Max < 0
            if (0 > max) {
                return (Math.Abs(min) - Math.Abs(max)) * temp;
            }

            // case 4: (Min = 0) < Max
            if (min == 0) {
                return max * temp;
            }

            // case 5: Min < (Max = 0)
            if (max == 0) {
                return Math.Abs(min) * temp;
            }

            throw new Exception("invalid range");
        }

        public Size GetCellSize() {
            // Returns the cell size while considering the value for one unit
            return new Size {
                Width = CoordinateSystemData.CalculateCellAmount(this.MinX, this.MaxX, this.UnitX),
                Height = CoordinateSystemData.CalculateCellAmount(this.MinY, this.MaxY, this.UnitY),
            };
        }

        internal void SetUnitAndStepDynamically(Size canvasSize) {
            // TODO: Implement this

            // UnitX


            // UnitY

            // Step
            //this.StepX = this.UnitX * .5;
            //this.StepY = this.UnitY * .5;
        }

        public static CoordinateSystemData CalculateBoundaries(Size canvasSize, CoordinateSystemData data, Tuple<Vector, Vector> basisVectors) {
            double minX = data.MinX, maxX = data.MaxX, minY = data.MinY, maxY = data.MaxY;
            double w = canvasSize.Width;
            double h = canvasSize.Height;
            double step = 100;



            #region Corner Approach

            Vector iHat = basisVectors.Item1;
            Vector jHat = basisVectors.Item2;

            Vector[] corners = new Vector[4];
            // top left
            corners[0] = CoordinateConverter.FromStaticToDynamic(iHat, jHat, new Vector(minX, maxY));

            // top right
            corners[1] = CoordinateConverter.FromStaticToDynamic(iHat, jHat, new Vector(maxX, maxY));

            // bottom left
            corners[2] = CoordinateConverter.FromStaticToDynamic(iHat, jHat, new Vector(minX, minY));

            // boottom right
            corners[3] = CoordinateConverter.FromStaticToDynamic(iHat, jHat, new Vector(maxX, minY));

            minX = double.PositiveInfinity;
            maxX = double.NegativeInfinity;
            minY = double.PositiveInfinity;
            maxY = double.NegativeInfinity;

            foreach (Vector v in corners) {
                if (v.X < minX)
                    minX = v.X;
                if (v.X > maxX)
                    maxX = v.X;
                if (v.Y < minY)
                    minY = v.Y;
                if (v.Y > maxY)
                    maxY = v.Y;
            }

            #endregion

            #region pixelapproach
            /*
            // Upper canvas border
            for (double x = 0, y = 0; x <= w; x += step) {
                Vector p = CoordinateConverter.FromPointToCoordinate(canvasSize, data, new Vector(x, y));
                if (p.X < minX)
                    minX = p.X;
                else if (p.X > maxX)
                    maxX = p.X;
                if (p.Y < minY)
                    minY = p.Y;
                else if (p.Y > maxY)
                    maxY = p.Y;
            }

            // Lower canvas border
            for (double x = 0, y = h; x <= w; x += step) {
                Vector p = CoordinateConverter.FromPointToCoordinate(canvasSize, data, new Vector(x, y));
                if (p.X < minX)
                    minX = p.X;
                else if (p.X > maxX)
                    maxX = p.X;
                if (p.Y < minY)
                    minY = p.Y;
                else if (p.Y > maxY)
                    maxY = p.Y;
            }

            // Right canvas border
            for (double y = 0, x = w; y <= h; y += step) {
                Vector p = CoordinateConverter.FromPointToCoordinate(canvasSize, data, new Vector(x, y));
                if (p.X < minX)
                    minX = p.X;
                else if (p.X > maxX)
                    maxX = p.X;
                if (p.Y < minY)
                    minY = p.Y;
                else if (p.Y > maxY)
                    maxY = p.Y;
            }

            // Left canvas border
            for (double y = 0, x = 0; y <= h; y += step) {
                Vector p = CoordinateConverter.FromPointToCoordinate(canvasSize, data, new Vector(x, y));
                if (p.X < minX)
                    minX = p.X;
                else if (p.X > maxX)
                    maxX = p.X;
                if (p.Y < minY)
                    minY = p.Y;
                else if (p.Y > maxY)
                    maxY = p.Y;
            }
            */
            #endregion

            data.MinX = minX;
            data.MaxX = maxX;
            data.MinY = minY;
            data.MaxY = maxY;

            data.IHat.X = basisVectors.Item1.X;
            data.IHat.Y = basisVectors.Item1.Y;
            data.JHat.X = basisVectors.Item2.X;
            data.JHat.Y = basisVectors.Item2.Y;

            return data;
        }
    }
}
