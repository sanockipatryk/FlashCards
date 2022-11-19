using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FlashCards.Helpers
{
    public static class EnumExtensionMethods
    {
        public static string GetDisplayName(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs2 = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                if ((_Attribs2.Any()))
                {
                    return ((DisplayAttribute)_Attribs2[0]).Name;
                }
            }
            return GenericEnum.ToString();
        }
    }
}