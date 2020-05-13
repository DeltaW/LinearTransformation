using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LinearTransformation.Model {
    public static class CoordinateConverter {
        
        public static Vector FromPointToCoordinate(Size canvasSize, CoordinateSystemData data, Vector point) {
            double w = canvasSize.Width;
            double h = canvasSize.Height;

            if (h == 0 || w == 0 || double.IsNaN(w) || double.IsNaN(h)) {
                throw new Exception("Invalid canvas size");
            }

            // Calculate the size of a single (unit) cell while considering the canvas size
            Size cellSize = new Size {
                Width  = w / data.GetCellSize().Width,
                Height = h / data.GetCellSize().Height,
            };

            // TODO: Make sure that this makes sense
            return new Vector(CalculateCoordinateX(data.MinX, data.MaxX,
                                            (point.X / cellSize.Width)  * data.IHat.X +
                                            (point.Y / cellSize.Height) * data.JHat.X),
                              CalculateCoordinateY(data.MinY, data.MaxY,
                                            (point.Y / cellSize.Height) * data.JHat.Y +
                                            (point.X / cellSize.Width)  * data.IHat.Y));

        }
        private static double CalculateCoordinateX(double min, double max, double pointX) {
            // Calculates the corresponding x coordinate through a coordinate (canvas) within a range
            // Note that this does not incorporate the scaling based on the canvas size
            // Examples are located within the corresponding case

            // case 1: Min < 0 < Max
            if (min < 0 && 0 < max) {
                #region Example
                // Min = -2 | Max = 3
                // Coordinate
                // ... -4 -3 [-2 -1  0  1  2  3] 4 ...
                // Corresponding Canvas coordinate:
                // ... -2 -1 [ 0  1  2  3  4  5] 6 ...
                #endregion
                if (pointX == 0) {
                    // Ursprung
                    return min;
                } else if (pointX > 0) {
                    //Ursprung + coordinate
                    return min + pointX;
                } else if (pointX < 0) {
                    // Ursprung + (negative) coordinate
                    return min + pointX;
                }
            }

            // case 2: 0 < Min < Max
            if (0 < min) {
                #region Example
                // Min = 1 | Max = 3
                // Coordinate
                // ... -4 -3 -2 -1  0 [1  2  3] 4 ...
                // Corresponding Canvas coordinate:
                // ... -5 -4 -3 -2 -1 [0  1  2] 3 ...
                #endregion
                if (pointX == 0) {
                    // Ursprung
                    return min;
                } else if (pointX > 0) {
                    // Ursprung + coordinate
                    return min + pointX;
                } else if (pointX < 0) {
                    // Ursprung + (negative) coordinate)
                    return min + pointX;
                }
            }

            // case 3: Min < Max < 0
            if (0 > max) {
                #region Example
                // Min = -3 | Max = -1
                // Coordinate
                // ... -4 [-3 -2 -1]  0  1  2  3  4 ...
                // Corresponding Canvas coordinate:
                // ... -1 [ 0  1  2]  3  4  5  6  7 ...
                #endregion
                if (pointX == 0) {
                    // Ursprung
                    return min;
                } else if (pointX > 0) {
                    // Ursprung + coordinate
                    return min + pointX;
                } else if (pointX < 0) {
                    // Ursprung + (negative) coordinate
                    return min + pointX;
                }
            }

            // case 4: (Min = 0) < Max
            if (min == 0) {
                #region Example
                // Min = 0 | Max = 3
                // Coordinate
                // ... -4 -3 -2 -1  [0  1  2  3] 4 ...
                // Corresponding Canvas coordinate:
                // ... -4 -3 -2 -1  [0  1  2  3] 4 ...
                #endregion
                if (pointX == 0) {
                    // Ursprung
                    return min;
                } else if (pointX > 0) {
                    // Ursprung + coordinate
                    return min + pointX;
                } else if (pointX < 0) {
                    // Urpsrung + (negative coordinate)
                    return min + pointX;
                }
            }

            // case 5: Min < (Max = 0)
            if (max == 0) {
                #region Example
                // Min = -3 | Max = 0
                // Coordinate
                // ... -4 [-3 -2 -1  0]  1  2  3  4 ...
                // Corresponding Canvas coordinate:
                // ... -1 [ 0  1  2  3]  4  5  6  7 ...
                #endregion
                if (pointX == 0) {
                    // Ursprung
                    return min;
                } else if (pointX > 0) {
                    // Ursprung + coordinate
                    return min + pointX;
                } else if (pointX < 0) {
                    // Ursprung + (negative) coordinate;
                    return min + pointX;
                }
            }

            throw new Exception("invalid range");
        }
        private static double CalculateCoordinateY(double min, double max, double pointY) {
            // Calculates the corresponding y coordinate through a coordinate (canvas) within a range
            // Note that this does not incorporate the scaling based on the canvas size
            // Examples are located within the corresponding case      
            
            // case 1: Min < 0 < Max
            if (min < 0 && 0 < max) {
                #region Example
                // Min = -2 | Max = 3
                // Coordinate
                // ... -4 -3 [-2 -1  0  1  2  3]  4 ...
                // Corresponding Canvas coordinate:
                // ...  7  6 [ 5  4  3  2  1  0] -1 ...
                #endregion
                if (pointY == 0) {
                    // Ursprung
                    return max;
                } else if (pointY > 0) {
                    //Ursprung - coordinate
                    return max - pointY;
                } else if (pointY < 0) {
                    // Ursprung - (negative) coordinate
                    return max - pointY;
                }
            }

            // case 2: 0 < Min < Max
            if (0 < min) {
                #region Example
                // Min = 1 | Max = 3
                // Coordinate
                // ... -4 -3 -2 -1  0  [1  2  3]  4 ...
                // Corresponding Canvas coordinate:
                // ...  7  6  5  4  3  [2  1  0] -1 ...
                #endregion
                if (pointY == 0) {
                    // Ursprung
                    return max;
                } else if (pointY > 0) {
                    // Ursprung - coordinate
                    return max - pointY;
                } else if (pointY < 0) {
                    // Ursprung - (negative) coordinate)
                    return max - pointY;
                }
            }

            // case 3: Min < Max < 0
            if (0 > max) {
                #region Example
                // Min = -3 | Max = -1
                // Coordinate
                // ... -4 [-3 -2 -1]  0  1  2  3  4 ...
                // Corresponding Canvas coordinate:
                // ...  3 [ 2  1  0] -1 -2 -3 -4 -5 ...
                #endregion
                if (pointY == 0) {
                    // Ursprung
                    return max;
                } else if (pointY > 0) {
                    // Ursprung - coordinate
                    return max - pointY;
                } else if (pointY < 0) {
                    // Ursprung - (negative) coordinate
                    return max - pointY;
                }
            }

            // case 4: (Min=0) < Max
            if (min == 0) {
                #region Example
                // Min = 0 | Max = 3
                // Coordinate
                // ... -4 -3 -2 -1 [ 0  1  2  3]  4 ...
                // Corresponding Canvas coordinate:
                // ...  7  6  5  4 [ 3  2  1  0] -1 ...
                #endregion
                if (pointY == 0) {
                    // Ursprung
                    return max;
                } else if (pointY > 0) {
                    // Ursprung - coordinate
                    return max - pointY;
                } else if (pointY < 0) {
                    // Urpsrung - (negative coordinate)
                    return max - pointY;
                }
            }

            // case 5: Min < (Max = 0)
            if (max == 0) {
                #region Example
                // Min = -3 | Max = 0
                // Coordinate
                // ... -4 [-3 -2 -1  0]  1  2  3  4 ...
                // Corresponding Canvas coordinate:
                // ...  4 [ 3  2  1  0] -1 -2 -3 -4 ...
                #endregion
                if (pointY == 0) {
                    // Ursprung
                    return max;
                } else if (pointY > 0) {
                    // Ursprung - coordinate
                    return max - pointY;
                } else if (pointY < 0) {
                    // Ursprung + (negative) coordinate;
                    return max - pointY;
                }
            }

            throw new Exception("invalid range");
        }

        public static Vector FromCoordinateToPoint(Size canvasSize, CoordinateSystemData data, Vector coordinate) {
            // Converts a Coordinate into a Point on a canvas considering the canvas size

            double w = canvasSize.Width;
            double h = canvasSize.Height;

            if (h == 0 || w == 0 || double.IsNaN(w) || double.IsNaN(h)) {
                throw new Exception("Invalid canvas size");
            }

            // Calculate the size of a single (unit) cell while considering the canvas size
            Size cellSize = new Size {
                Width  = w / data.GetCellSize().Width,
                Height = h / data.GetCellSize().Height,
            };

            return new Vector(CalculatePointX(data.MinX, data.MaxX,
                                            coordinate.X * data.IHat.X +
                                            coordinate.Y * data.JHat.X) * cellSize.Width,
                              CalculatePointY(data.MinY, data.MaxY,
                                            coordinate.Y * data.JHat.Y +
                                            coordinate.X * data.IHat.Y) * cellSize.Height);
        }
        private static double CalculatePointX(double min, double max, double coordinate) {
            // Calculates the corresponding x (canvas) coordinate through a coordinate within a range
            // Note that this does not incorporate the scaling based on the canvas size
            // Examples are located within the corresponding case

            // case 1: Min < 0 < Max
            if (min < 0 && 0 < max) {
                #region Example
                // Min = -2 | Max = 3
                // Coordinate
                // ... -4 -3 [-2 -1  0  1  2  3] 4 ...
                // Corresponding Canvas coordinate:
                // ... -2 -1 [ 0  1  2  3  4  5] 6 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return -min;
                } else if (coordinate > 0) {
                    //Ursprung + coordinate
                    return -min + coordinate;
                } else if (coordinate < 0) {
                    // Ursprung + (negative) coordinate
                    return -min + coordinate;
                }
            }

            // case 2: 0 < Min < Max
            if (0 < min) {
                #region Example
                // Min = 1 | Max = 3
                // Coordinate
                // ... -4 -3 -2 -1  0 [1  2  3] 4 ...
                // Corresponding Canvas coordinate:
                // ... -5 -4 -3 -2 -1 [0  1  2] 3 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return -min;
                } else if (coordinate > 0) {
                    // Ursprung + coordinate
                    return -min + coordinate;
                } else if (coordinate < 0) {
                    // Ursprung + (negative) coordinate)
                    return -min + coordinate;
                }
            }

            // case 3: Min < Max < 0
            if (0 > max) {
                #region Example
                // Min = -3 | Max = -1
                // Coordinate
                // ... -4 [-3 -2 -1]  0  1  2  3  4 ...
                // Corresponding Canvas coordinate:
                // ... -1 [ 0  1  2]  3  4  5  6  7 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return -min;
                } else if (coordinate > 0) {
                    // Ursprung + coordinate
                    return -min + coordinate;
                } else if (coordinate < 0) {
                    // Ursprung + (negative) coordinate
                    return -min + coordinate;
                }
            }

            // case 4: (Min = 0) < Max
            if (min == 0) {
                #region Example
                // Min = 0 | Max = 3
                // Coordinate
                // ... -4 -3 -2 -1  [0  1  2  3] 4 ...
                // Corresponding Canvas coordinate:
                // ... -4 -3 -2 -1  [0  1  2  3] 4 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return -min;
                } else if (coordinate > 0) {
                    // Ursprung + coordinate
                    return -min + coordinate;
                } else if (coordinate < 0) {
                    // Urpsrung + (negative coordinate)
                    return -min + coordinate;
                }
            }

            // case 5: Min < (Max = 0)
            if (max == 0) {
                #region Example
                // Min = -3 | Max = 0
                // Coordinate
                // ... -4 [-3 -2 -1  0]  1  2  3  4 ...
                // Corresponding Canvas coordinate:
                // ... -1 [ 0  1  2  3]  4  5  6  7 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return -min;
                } else if (coordinate > 0) {
                    // Ursprung + coordinate
                    return -min + coordinate;
                } else if (coordinate < 0) {
                    // Ursprung + (negative) coordinate;
                    return -min + coordinate;
                }
            }

            throw new Exception("invalid range");
        }
        private static double CalculatePointY(double min, double max, double coordinate) {
            // Calculates the corresponding y (canvas) coordinate through a coordinate within a range
            // Note that this does not incorporate the scaling based on the canvas size
            // Examples are located within the corresponding case
            
            // case 1: Min < 0 < Max
            if (min < 0 && 0 < max) {
                #region Example
                // Min = -2 | Max = 3
                // Coordinate
                // ... -4 -3 [-2 -1  0  1  2  3]  4 ...
                // Corresponding Canvas coordinate:
                // ...  7  6 [ 5  4  3  2  1  0] -1 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return max;
                } else if (coordinate > 0) {
                    //Ursprung - coordinate
                    return max - coordinate;
                } else if (coordinate < 0) {
                    // Ursprung - (negative) coordinate
                    return max - coordinate;
                }
            }

            // case 2: 0 < Min < Max
            if (0 < min) {
                #region Example
                // Min = 1 | Max = 3
                // Coordinate
                // ... -4 -3 -2 -1  0  [1  2  3]  4 ...
                // Corresponding Canvas coordinate:
                // ...  7  6  5  4  3  [2  1  0] -1 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return max;
                } else if (coordinate > 0) {
                    // Ursprung - coordinate
                    return max - coordinate;
                } else if (coordinate < 0) {
                    // Ursprung - (negative) coordinate)
                    return max - coordinate;
                }
            }

            // case 3: Min < Max < 0
            if (0 > max) {
                #region Example
                // Min = -3 | Max = -1
                // Coordinate
                // ... -4 [-3 -2 -1]  0  1  2  3  4 ...
                // Corresponding Canvas coordinate:
                // ...  3 [ 2  1  0] -1 -2 -3 -4 -5 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return max;
                } else if (coordinate > 0) {
                    // Ursprung - coordinate
                    return max - coordinate;
                } else if (coordinate < 0) {
                    // Ursprung - (negative) coordinate
                    return max - coordinate;
                }
            }

            // case 4: (Min=0) < Max
            if (min == 0) {
                #region Example
                // Min = 0 | Max = 3
                // Coordinate
                // ... -4 -3 -2 -1 [ 0  1  2  3]  4 ...
                // Corresponding Canvas coordinate:
                // ...  7  6  5  4 [ 3  2  1  0] -1 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return max;
                } else if (coordinate > 0) {
                    // Ursprung - coordinate
                    return max - coordinate;
                } else if (coordinate < 0) {
                    // Urpsrung - (negative coordinate)
                    return max - coordinate;
                }
            }

            // case 5: Min < (Max = 0)
            if (max == 0) {
                #region Example
                // Min = -3 | Max = 0
                // Coordinate
                // ... -4 [-3 -2 -1  0]  1  2  3  4 ...
                // Corresponding Canvas coordinate:
                // ...  4 [ 3  2  1  0] -1 -2 -3 -4 ...
                #endregion
                if (coordinate == 0) {
                    // Ursprung
                    return max;
                } else if (coordinate > 0) {
                    // Ursprung - coordinate
                    return max - coordinate;
                } else if (coordinate < 0) {
                    // Ursprung + (negative) coordinate;
                    return max - coordinate;
                }
            }

            throw new Exception("invalid range");
        }
    }
}
