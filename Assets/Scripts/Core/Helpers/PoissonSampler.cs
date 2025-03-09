using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoJam.Core
{
    /// <summary>
    /// Info:
    ///     Implemation of Possion-Disc sampling using the Bridson's algorithm. 
    ///     For more infomation on this alogrthum please refer to the reference(s) below:
    ///         [1] http://devmag.org.za/2009/05/03/poisson-disk-sampling/
    /// 
    /// Usgae:
    ///     PoissonDiskSampler sampler = new PoissonDiskSampler(20, 10, 0.2f);
    ///     
    ///     // Method 1: Generates upto N random Vector2 and store in list
    ///     List<Vector2> samples = sampler.Sample(20)
    /// 
    ///     // Method 2: Iterate through as many samples as you need 
    ///     foreach (Vector2 sample in sampler.Sample()) {
    ///         // Excute logic with as many samples as you need.
    ///     }
    ///     
    /// Author(s): Colby O'Keefe
    /// </summary>
    public class PoissonSampler
    {
        private const int DIMENSION = 2; /* Spatial dimesnion. */

        private Grid _grid; /* Stores the internal grid. */

        private readonly Rect _rect; /* Bounds that the points can exist in. */

        // Generation paramters
        private int? _seed;
        private readonly int _k; /* maximum number of attempts a sample can be tried. */
        private readonly float _radius; /* minimum distance each point has to be frome ach other. */
        private readonly float _radius2; /* radius squared. */
        private List<Vector2> _activeSamples; /* A list of active samples in the generator. */

        // Random states
        private Random.State _originalState; /*  The random state upon allocating the class instance */
        private Random.State _oldState; /* The random state upon enter a function in this class */
        private Random.State _generatorState; /* The internal random state of the class*/

        /// <summary>
        /// Creates a sampler.
        /// </summary>
        /// <param name="width">Each sample x-coord will be between [0, <paramref name="width"/>).</param>
        /// <param name="height">Each sample y-coord will be between [0, <paramref name="height"/>).</param>
        /// <param name="radius">Each sample will be at least <paramref name="radius"/> apart.</param>
        /// <param name="k">Maximum number of attempts before giving up on a sample. Recommend you leave at 30.</param>
        public PoissonSampler(float width, float height, float radius, int? seed = null, int k = 30)
        {
            _originalState = Random.state;
            _seed = seed;
            InitGeneratorSeed();
            _rect = new Rect(0, 0, width, height);
            _radius = radius;
            _radius2 = Mathf.Pow(radius, 2f);
            _k = k;
            _grid = InitializeGrid(width, height);
            _activeSamples = new List<Vector2>();
        }

        /// <summary>
        /// Initalizes the grid for the generator.
        /// </summary>
        /// <param name="width">The regions width.</param>
        /// <param name="height">The regions height.</param>
        /// <returns></returns>
        private Grid InitializeGrid(float width, float height)
        {
            Grid grid = new Grid();

            grid.cellSize = _radius / Mathf.Sqrt(DIMENSION);

            grid.width = Mathf.CeilToInt(width / grid.cellSize) + 1;
            grid.height = Mathf.CeilToInt(height / grid.cellSize) + 1;

            grid.map = new Vector2[grid.width, grid.height];

            ResetGrid(ref grid);

            return grid;
        }

        /// <summary>
        /// Resets the grid to it's inital state.
        /// </summary>
        /// <param name="grid">The internal grid of points added to the set.</param>
        private void ResetGrid(ref Grid grid)
        {
            if (grid.map == null) return;

            for (int i = 0; i < grid.width; i++)
                for (int j = 0; j < grid.height; j++)
                    grid[i, j] = Vector2.zero;
        }

        /// <summary>
        /// Checks if a point is too close or not.
        /// </summary>
        /// <param name="grid">The internal grid of points added to the set.</param>
        /// <param name="pt">The potential point to be added</param>
        /// <param name="radius">The minimum distance between points</param>
        /// <returns>A boolean that states if the point is vaild or not.</returns>
        private bool IsValidPoint(Grid grid, Vector2 pt, float radius)
        {
            int xindex = Mathf.FloorToInt(pt.x / grid.cellSize);
            int yindex = Mathf.FloorToInt(pt.y / grid.cellSize);
            int i0 = Mathf.Max(xindex - 1, 0);
            int i1 = Mathf.Min(xindex + 1, grid.width - 1);
            int j0 = Mathf.Max(yindex - 1, 0);
            int j1 = Mathf.Min(yindex + 1, grid.height - 1);

            for (int i = i0; i <= i1; i++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    if (grid[i, j] != Vector2.zero)
                    {
                        Vector2 pos = grid[i, j];
                        if (Vector2.Distance(pos, pt) < radius)
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Add a point the the generators samples.
        /// </summary>
        /// <param name="pt">The sample to be added</param>
        /// <returns></returns>
        private Vector2 AddSample(Vector2 pt)
        {
            _grid.Insert(pt);
            _activeSamples.Add(pt);
            return pt;
        }

        /// <summary>
        /// Initalizes the generator internal Ramdom states
        /// </summary>
        private void InitGeneratorSeed()
        {
            if (_seed != null)
            {
                _oldState = Random.state;
                Random.InitState(_seed ?? 0);
                _generatorState = Random.state;
                Random.state = _oldState;
            }
            else
            {
                _generatorState = _originalState;
            }
        }

        /// <summary>
        /// Reset unity internal Random class to it state inside the 
        /// generator.
        /// </summary>
        private void SetGeneratorRandomState()
        {
            if (_seed != null)
            {
                _oldState = Random.state;
                Random.state = _generatorState;
            }
        }

        /// <summary>
        /// Reset unity internal Random class to it state outside the 
        /// generator.
        /// </summary>
        private void SetOriginalRandomState()
        {
            if (_seed != null)
            {
                _generatorState = Random.state;
                Random.state = _oldState;
            }
        }

        /// <summary>
        /// Reset the samplier to it's inital state.
        /// The next sample set generated will not be the same
        /// unless the <paramref name="incrementSeed"/> flag is set.
        /// </summary>
        /// <param name="incrementSeed">A flag that indicates if the seed should be chnaged or not. Off by default.</param>
        public void Reset(bool incrementSeed = false)
        {
            if (!incrementSeed)
                InitGeneratorSeed();

            ResetGrid(ref _grid);
        }

        /// <summary>
        /// Generates upto <paramref name="N"/> samples. If The maximum number a points that can 
        /// exist in a bounds is greater than <paramref name="N"/> only the maximum number of points 
        /// well be taken.
        /// </summary>
        /// <param name="N">The number of points requested.</param>
        /// <param name="reset">A optional flag that determines if the Enumerable should resets itself 
        /// upon generating the maximum number of points. Turned off by default.</param>
        /// <returns></returns>
        public List<Vector2> Sample(int N, bool reset = false)
        {
            List<Vector2> samples = new List<Vector2>();

            foreach (Vector2 sample in Sample(reset))
                samples.Add(sample);

            if (N >= samples.Count)
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"<color=red>Warning in PoissonDiskSampler:</color> " +
                    $"Requested {N} points but can only generate " +
                    $"{samples.Count} with the current parameters."
                );
#endif
                return samples;
            }

            SetGeneratorRandomState();
            List<Vector2> randomSamples = samples.OrderBy(x => Random.value).Take(N).ToList();
            SetOriginalRandomState();

            return randomSamples;
        }

        /// <summary>
        /// A Enumerable that generate new samples within the bounds limits.
        /// An important note is when the <paramref name="reset"/> flag is set the IEnumerable will go back to
        /// it's inital state. The <paramref name="reset"/> flag is off by default. Turned off by default.
        /// </summary>
        /// /// <param name="reset">A optional flag that determines if the Enumerable resets itself upon completion.</param>
        /// <returns></returns>
        public IEnumerable<Vector2> Sample(bool reset = false)
        {
            SetGeneratorRandomState();
            Vector2 sample = AddSample(new Vector2(Random.value * _rect.width, Random.value * _rect.height));
            SetOriginalRandomState();
            yield return sample;

            while (_activeSamples.Count > 0)
            {
                SetGeneratorRandomState();
                int nextIndex = Random.Range(0, _activeSamples.Count);
                Vector2 pt = _activeSamples[nextIndex];

                bool hasFoundPoint = false;

                for (int i = 0; i < _k; i++)
                {
                    float theta = 2.0f * Mathf.PI * Random.value;
                    float newRadius = Mathf.Sqrt(Random.value * 3.0f * _radius2 + _radius2);
                    Vector2 newPt = pt + newRadius * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));

                    if (_rect.Contains(newPt) && IsValidPoint(_grid, newPt, _radius))
                    {
                        hasFoundPoint = true;
                        SetOriginalRandomState();
                        yield return AddSample(newPt);
                        break;
                    }
                }

                if (!hasFoundPoint)
                {
                    _activeSamples[nextIndex] = _activeSamples[_activeSamples.Count - 1];
                    _activeSamples.RemoveAt(_activeSamples.Count - 1);
                }

                SetOriginalRandomState();
            }

            if (reset) Reset(true);
        }

        /// <summary>
        /// Struct that stores the internal grid state of the samplier. 
        /// </summary>
        private struct Grid
        {
            public Vector2[,] map;
            public float cellSize;
            public int width;
            public int height;

            public void Insert(Vector2 pt)
            {
                int x = Mathf.FloorToInt(pt.x / cellSize);
                int y = Mathf.FloorToInt(pt.y / cellSize);
                this[x, y] = pt;
            }

            public Vector2 this[int x, int y]
            {
                get => (Vector2)map.GetValue(x, y);
                set => map.SetValue(value, x, y);
            }
        }
    }
}