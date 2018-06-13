using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AsciiArt
{
    /// <summary>
    /// An histogram storing RGB values in 4 bins per channel
    /// </summary>
    internal class Histogram
    {
        private int[,,] _values;

        public Histogram()
        {
            _values = new int[4, 4, 4];
        }

        internal double Variance { get; private set; }

        internal void AddValue(Color c)
        {
            int r = c.R / 64;
            int g = c.G / 64;
            int b = c.B / 64;
            _values[r, g, b]++;
        }

        /// <summary>
        /// Calculates Bhattacharyya distance between two histograms. See <see cref="https://en.wikipedia.org/wiki/Bhattacharyya_distance"/> for more information.
        /// </summary>
        /// <param name="other">The histogram to calculate the distance to</param>
        /// <returns>A score representing the distance</returns>
        internal double BhattacharyyaDistance(Histogram other)
        {
            double score = 0;

            for (int r = 0; r < _values.GetLength(0); r += 1)
            {
                for (int g = 0; g < _values.GetLength(1); g += 1)
                {
                    for (int b = 0; b < _values.GetLength(2); b++)
                    {
                        score += Math.Sqrt(_values[r, g, b] * other._values[r, g, b]);
                    }
                }
            }

            return -Math.Log(score);
        }

        /// <summary>
        /// Calculates Chi-squared distance between two histograms. See <see cref="https://en.wikipedia.org/wiki/Chi-squared_distribution"/> for more information.
        /// </summary>
        /// <param name="other">The histogram to calculate the distance to</param>
        /// <returns>A score representing the distance</returns>
        internal double ChiSquaredDistance(Histogram other)
        {
            double score = 0;

            for (int r = 0; r < _values.GetLength(0); r += 1)
            {
                for (int g = 0; g < _values.GetLength(1); g += 1)
                {
                    for (int b = 0; b < _values.GetLength(2); b++)
                    {
                        int divisor = _values[r, g, b];

                        if (divisor == 0)
                        {
                            divisor = 1;
                        }

                        score += Math.Pow(_values[r, g, b] - other._values[r, g, b], 2) / divisor;
                    }
                }
            }

            return score;
        }

        /// <summary>
        /// Calculates Earth mover's distance distance between two histograms. See <see cref="https://en.wikipedia.org/wiki/Earth_mover%27s_distance"/> for more information.
        /// </summary>
        /// <param name="other">The histogram to calculate the distance to</param>
        /// <returns>A score representing the distance</returns>
        internal double EarthMoversDistance(Histogram other)
        {
            double score = 0;

            for (int r = 0; r < _values.GetLength(0); r += 1)
            {
                for (int g = 0; g < _values.GetLength(1); g += 1)
                {
                    for (int b = 0; b < _values.GetLength(2); b++)
                    {
                        score += _values[r, g, b] * other._values[r, g, b];
                    }
                }
            }

            return score;
        }

        /// <summary>
        /// Calculates Euclidean distance between two histograms. See <see cref="https://en.wikipedia.org/wiki/Euclidean_distance"/> for more information.
        /// </summary>
        /// <param name="other">The histogram to calculate the distance to</param>
        /// <returns>A score representing the distance</returns>
        internal double EuclideanDistance(Histogram other)
        {
            double score = 0;

            for (int r = 0; r < _values.GetLength(0); r += 1)
            {
                for (int g = 0; g < _values.GetLength(1); g += 1)
                {
                    for (int b = 0; b < _values.GetLength(2); b++)
                    {
                        score += Math.Pow(_values[r, g, b] - other._values[r, g, b], 2);
                    }
                }
            }

            return Math.Sqrt(score);
        }

        /// <summary>
        /// Calculates intersection between two histograms. See <see cref="https://pdfs.semanticscholar.org/75d0/f2a4a2dff5cce4eb4097c2e68e1ff75ae569.pdf"/> for more information.
        /// </summary>
        /// <param name="other">The histogram to calculate the distance to</param>
        /// <returns>A score representing the distance</returns>
        internal double Intersection(Histogram other)
        {
            int score = 0;
            int divisor = 0;

            for (int r = 0; r < _values.GetLength(0); r += 1)
            {
                for (int g = 0; g < _values.GetLength(1); g += 1)
                {
                    for (int b = 0; b < _values.GetLength(2); b++)
                    {
                        score += Math.Min(_values[r, g, b], other._values[r, g, b]);
                        divisor += _values[r, g, b];
                    }
                }
            }

            return score / divisor;
        }

        /// <summary>
        /// Calculates Manhattan distance between two histograms. See <see cref="https://en.wikipedia.org/wiki/Taxicab_geometry"/> for more information.
        /// </summary>
        /// <param name="other">The histogram to calculate the distance to</param>
        /// <returns>A score representing the distance</returns>
        internal double ManhattanDistance(Histogram other)
        {
            double score = 0;

            for (int r = 0; r < _values.GetLength(0); r += 1)
            {
                for (int g = 0; g < _values.GetLength(1); g += 1)
                {
                    for (int b = 0; b < _values.GetLength(2); b++)
                    {
                        score += Math.Abs(_values[r, g, b] - other._values[r, g, b]);
                    }
                }
            }

            return score;
        }

        /// <summary>
        /// Calculates Matusita distance between two histograms. See <see cref="https://pdfs.semanticscholar.org/75d0/f2a4a2dff5cce4eb4097c2e68e1ff75ae569.pdf"/> for more information.
        /// </summary>
        /// <param name="other">The histogram to calculate the distance to</param>
        /// <returns>A score representing the distance</returns>
        internal double MatusitaDistance(Histogram other)
        {
            double score = 0;

            for (int r = 0; r < _values.GetLength(0); r += 1)
            {
                for (int g = 0; g < _values.GetLength(1); g += 1)
                {
                    for (int b = 0; b < _values.GetLength(2); b++)
                    {
                        score += Math.Pow(Math.Sqrt(_values[r, g, b]) - Math.Sqrt(other._values[r, g, b]), 2);
                    }
                }
            }

            return Math.Sqrt(score);
        }
    }
}
