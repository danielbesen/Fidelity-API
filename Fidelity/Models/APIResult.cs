using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Models
{
    public class APIResult<T>
    {
        /// <summary>
        /// Operation Success
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Error Message
        /// </summary>
        public string Message { get; set; }

        //public int Status { get; set; }

        /// <summary>
        /// Http Count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Object Return
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Object User Return
        /// </summary>
        public T ResultUser { get; set; }
    }
}