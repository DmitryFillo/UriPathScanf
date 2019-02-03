﻿using System;
using System.Collections.Generic;
using System.Linq;
using UriPathScanf.Internal;

namespace UriPathScanf
{
    /// <summary>
    /// Marks model as fetchable from URI path
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UriPathAttribute : Attribute
    {
        /// <summary>
        /// URI path format
        /// </summary>
        public readonly string Format;

        /// <summary>
        /// Names of variable, should be the same as prop names in the model
        /// </summary>
        public IEnumerable<string> Names { get; }

        /// <summary>
        /// <inheritdoc cref="UriPathAttribute"/>
        /// </summary>
        /// <param name="uriPathFormat">Format string, e.g. "/some/path/{}/xxx"</param>
        /// <param name="names">Names params in the same order as in the format string</param>
        public UriPathAttribute(string uriPathFormat, params string[] names)
        {
            Format = uriPathFormat;
            Names = names;
        }
    }
}
