using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MonkeyArtInventory.Core.Utilities;

public static class EnumHelper
{
    public static string GetDisplayName(Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        if (member is null)
        {
            return value.ToString();
        }

        var attribute = member.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }
}
