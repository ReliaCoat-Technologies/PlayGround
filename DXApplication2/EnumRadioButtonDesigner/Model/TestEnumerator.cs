using System.ComponentModel;
using DevExpress.Xpf.Core.ReflectionExtensions.Attributes;

namespace EnumRadioButtonDesigner.Model
{
    public enum TestEnumerator
    {
        [Description("My name is Foo")]
        Foo,
        [Description("My name is Bar")]
        Bar,
        All,
        Day,
        Long
    }
}