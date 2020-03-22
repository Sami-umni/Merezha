using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphX.PCL.Common.Models;

namespace Merezha.Moduls
{
    public class DataVertex : VertexBase
    {
        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Text { get; set; }

        #region Calculated or static props

        public override string ToString()
        {
            return Text;
        }

        #endregion

        /// <summary>
        /// Default parameterless constructor for this class
        /// (required for YAXLib serialization)
        /// </summary>
        public DataVertex() : this(string.Empty)
        {
        }

        public DataVertex(string text = "")
        {
            Text = text;
        }
    }
}
