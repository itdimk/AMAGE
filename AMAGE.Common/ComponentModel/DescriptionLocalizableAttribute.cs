using System;
using System.ComponentModel;
using System.Resources;

namespace AMAGE.Common.ComponentModel
{
    /// <summary> Description attribute with localization by resource class </summary>
    public class DescriptionLocalizableAttribute : DescriptionAttribute
    {
        public DescriptionLocalizableAttribute(Type resourceClassType, string propertyName)
        {
            DescriptionValue = new ResourceManager(resourceClassType)
                .GetString(propertyName);
        }
    }
}
