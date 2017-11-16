/*
  int64 API.
  Copyright (C) 2017 Ariel Ortiz, ITESM CEM

  To compile this module as a DLL:

                mcs /t:library int64lib.cs

  To link this DLL to a program written in C#:

                mcs /r:int64lib.dll someprogram.cs

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using System.Collections.Generic;

namespace Int64 {

    public class Utils {

        private static int currentHandleID = 0;

        private static Dictionary<long, List<long>> handles =
            new Dictionary<long, List<long>>();

        //----------------------------------------------------------------------
        // Prints i to stdout as a decimal integer. Does not print a new line
        // at the end. Returns 0.
        public static long Printi(long i) {
            Console.Write(i);
            return 0;
        }

        //----------------------------------------------------------------------
        // Prints a character to stdout, where c is its Unicode code point.
        // Does not print a new line at the end. Returns 0.
        public static long Printc(long c) {
            Console.Write(char.ConvertFromUtf32((int) c));
            return 0;
        }

        //----------------------------------------------------------------------
        // Prints s to stdout as a string. s must be a handle to an array list
        // containing zero or more Unicode code points. Does not print a new
        // line at the end. Returns 0.
        public static long Prints(long s) {
            CheckHandle(s);
            StringBuilder builder = new StringBuilder();
            for (int i = 0, n = (int) Size(s); i < n; i++) {
                builder.Append(char.ConvertFromUtf32((int) Get(s, i)));
            }
            Console.Write(builder.ToString());
            return 0;
        }

        //----------------------------------------------------------------------
        // Prints a newline character to stdout. Returns 0.
        public static long Println() {
            Console.WriteLine();
            return 0;
        }

        //----------------------------------------------------------------------
        // Reads from stdin a signed decimal integer and return its value. Does
        // not return until a valid integer has been read.
        public static long Readi() {
            string input;
            long result;
            do {
                input = Console.ReadLine();
            } while (!long.TryParse(input, out result));

            return result;
        }

        //----------------------------------------------------------------------
        // Reads from stdin a string (until the end of line) and returns a
        // handle to a newly created array list containing the Unicode code
        // points of all the characters read, excluding the end of line.
        public static long Reads() {
            string input = Console.ReadLine();
            long handle = New(0);
            foreach (long i in AsCodePoints(input)) {
                Add(handle, i);
            }
            return handle;
        }

        //----------------------------------------------------------------------
        // Creates a new array list object with n elements and returns its
        // handle. All the elements of the array list are set to zero. Throws
        // an exception if n is less than zero.
        public static long New(long n) {
            if (n < 0) {
                throw new Exception("Can't create a negative size array.");
            }
            long handle = currentHandleID++;
            handles.Add(handle, new List<long>());
            for (int i = 0; i < n; i++) {
                Add(handle, 0);
            }
            return handle;
        }

        //----------------------------------------------------------------------
        // Returns the size (number of elements) of the array list referenced
        // by handle h. Throws an exception if h is not a valid handle.
        public static long Size(long h) {
            CheckHandle(h);
            return handles[h].Count;
        }

        //----------------------------------------------------------------------
        // Adds x at the end of the array list referenced by handle h.
        // Returns 0. Throws an exception if h is not a valid handle.
        public static long Add(long h, long x) {
            CheckHandle(h);
            handles[h].Add(x);
            return 0;
        }

        //----------------------------------------------------------------------
        // Returns the value at index i from the array list referenced by
        // handle h. Throws an exception if i is out of bounds or if h is not
        // a valid handle.
        public static long Get(long h, long i) {
            CheckHandle(h);
            return handles[h][(int) i];
        }

        //----------------------------------------------------------------------
        // Sets to x the element at index i of the array list referenced by
        // handle h. Returns 0. Throws an exception if i is out of bounds or
        // if h is not a valid handle.
        public static long Set(long h, long i, long x) {
            CheckHandle(h);
            handles[h][(int) i] = x;
            return 0;
        }
        
        //----------------------------------------------------------------------
        // Returns the result of raising b to the power of e. Throws an
        // exception if the result doesn't fit into a 64-bit signed integer. 
        public static long Pow(long b, long e) {
            long result = 1;
            for (long i = 0; i < e; i++) {
                checked {
                    result *= b;
                }
            }
            return result;
        }

        //----------------------------------------------------------------------
        // Local function that checks if h is a valid array list handle.
        private static void CheckHandle(long h) {
            if (!handles.ContainsKey(h)) {
                throw new Exception("Invalid array handle.");
            }
        }

        //----------------------------------------------------------------------
        // Local function that allows obtaining all the individual Unicode code
        // points of a given string.
        private static IEnumerable<long> AsCodePoints(string str) {
            for(int i = 0; i < str.Length; i++) {
                yield return char.ConvertToUtf32(str, i);
                if (char.IsHighSurrogate(str, i)) {
                    i++;
                }
            }
        }
    }
}
