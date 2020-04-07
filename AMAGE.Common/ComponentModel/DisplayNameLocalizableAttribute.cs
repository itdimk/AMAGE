using System;
using System.ComponentModel;
using System.Resources;

namespace AMAGE.Common.ComponentModel
{
    /// <summary> DisplayName attribute with localization by resource class </summary>
    public class DisplayNameLocalizableAttribute : DisplayNameAttribute
    {
        public DisplayNameLocalizableAttribute(Type resourceClassType, string propertyName)
        {
            DisplayNameValue = new ResourceManager(resourceClassType)
                .GetString(propertyName);
        }
    }
}
