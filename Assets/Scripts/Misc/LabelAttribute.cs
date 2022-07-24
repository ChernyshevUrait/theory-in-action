using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// INHERITANCE
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
sealed class LabelAttribute : Attribute
{
    private readonly string label;

    // ENCAPSULATION
    public string Label
    {
        get { return label; }
    }

    public LabelAttribute(string label)
    {
        this.label = label;
    }
}
