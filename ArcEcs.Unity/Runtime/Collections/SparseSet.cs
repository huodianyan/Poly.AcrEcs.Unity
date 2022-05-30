using System.Collections;
using System.Collections.Generic;

namespace Poly.Ecs
{
    public struct SparseSet : IEnumerable<int>
    {
        private readonly int _max;      // maximal value the set can contain
                                        // _max = 100; implies a range of [0..99]
        private int _n;                 // current size of the set
        private readonly int[] _d;      // dense array
        private readonly int[] _s;      // sparse array

        /// <summary>
        /// Initializes a new instance of the <see cref="SparseSet"/> class.
        /// </summary>
        /// <param name="maxValue">The maximal value the set can contain.</param>
        public SparseSet(int maxValue)
        {
            _max = maxValue + 1;
            _n = 0;
            _d = new int[_max];
            _s = new int[_max];
        }

        /// <summary>
        /// Adds the given value.
        /// If the value already exists in the set it will be ignored.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(int value)
        {
            if (value >= 0 && value < _max && !Contains(value))
            {
                _d[_n] = value;     // insert new value in the dense array...
                _s[value] = _n;     // ...and link it to the sparse array
                _n++;
            }
        }

        /// <summary>
        /// Removes the given value in case it exists.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(int value)
        {
            if (Contains(value))
            {
                _d[_s[value]] = _d[_n - 1];     // put the value at the end of the dense array
                                                // into the slot of the removed value
                _s[_d[_n - 1]] = _s[value];     // put the link to the removed value in the slot
                                                // of the replaced value
                _n--;
            }
        }

        /// <summary>
        /// Determines whether the set contains the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the set contains the given value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int value)
        {
            if (value >= _max || value < 0)
                return false;
            else
                return _s[value] < _n && _d[_s[value]] == value;    // value must meet two conditions:
                                                                    // 1. link value from the sparse array
                                                                    // must point to the current used range
                                                                    // in the dense array
                                                                    // 2. there must be a valid two-way link
        }

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        public void Clear()
        {
            _n = 0;     // simply set n to 0 to clear the set; no re-initialization is required
        }

        /// <summary>
        /// Gets the number of elements in the set.
        /// </summary>
        public int Count
        {
            get { return _n; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through all elements in the set.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<int> GetEnumerator()
        {
            var i = 0;
            while (i < _n)
            {
                yield return _d[i];
                i++;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through all elements in the set.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
