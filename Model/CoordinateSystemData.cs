﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LinearTransformation.Model {
    public struct CoordinateSystemData {
        public double MinX, MaxX, MinY, MaxY, Unit, Step, PossibleAmountOfCells;

        public CoordinateSystemData(double minX, double maxX, double minY, double maxY,
                                    double unit, double step,
                                    double possibleAmountOfCells = double.PositiveInfinity) {

            if (minX > maxX || minY > maxY) {
                throw new Exception("Invalid boundaries");
            }

            this.MinX = minX;
            this.MaxX = maxX;
            this.MinY = minY;
            this.MaxY = maxY;
            this.Unit = unit;
            this.Step = step;

            this.PossibleAmountOfCells = possibleAmountOfCells;
        }

        //public CoordinateSystemData(double minX, double maxX, double minY, double maxY,
        //                    double unit, double step,
        //                    double possibleAmountOfCells = double.PositiveInfinity) {

        //    if (minX > maxX || minY > maxY) {
        //        throw new Exception("Invalid boundaries");
        //    }

        //    this.MinX = minX;
        //    this.MaxX = maxX;
        //    this.MinY = minY;
        //    this.MaxY = maxY;
        //    this.Unit = unit;
        //    this.Step = step;

        //    this.PossibleAmountOfCells = possibleAmountOfCells;
        //}

        public void SetUnitAndStepDynamically() {
            // TODO: Add user property to enable or disable this feature
            if (double.IsInfinity(this.PossibleAmountOfCells)) {
                // just do whatever
            } else {

            }
        }

        //private static double CalculateUnit () {

        //}

        private static double CalculateCellAmount(double min, double max, double unit) {
            // Calculate the amount of units which fit into the given range

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
                Width = CoordinateSystemData.CalculateCellAmount(this.MinX, this.MaxX, this.Unit),
                Height = CoordinateSystemData.CalculateCellAmount(this.MinY, this.MaxY, this.Unit),
            };
        }
    }
}
