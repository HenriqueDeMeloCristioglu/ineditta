using System;

namespace Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class NotSearchableDataTableAttribute : Attribute
    {
    }
}
