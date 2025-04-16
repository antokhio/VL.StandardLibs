﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace VL.Core.EditorAttributes
{
    /// <summary>
    /// A host reflecting over the properties of a hosted object can create bindings to the outside world depending on this attribute.
    /// For more granular decisions on where to expose to the tag attribute can be used.
    /// </summary>
    [Obsolete]
    public class ExposedAttribute : Attribute
    {
        public ExposedAttribute()
        {
        }

        public override string ToString()
        {
            return $"Exposed";
        }
    }
}
