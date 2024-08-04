using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
namespace WebApi.Helpers;
public static class GenericConverter
{
    public static T ConvertObject<T>(object src)
    {
        var dest = (T?) Activator.CreateInstance(typeof(T));
        if (src == null || dest == null) {
          throw new SystemException("Failed to Convert Object");
        };
        var srcProperties = src.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);
        var destProperties = dest.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);
        var properties = srcProperties.Join(destProperties, p => new { p.Name,p.PropertyType}, p => new { p.Name, p.PropertyType }, (p1, p2) => new { p1, p2 });
        foreach (var property in properties)
            property.p2.SetValue(dest, property.p1.GetValue(src));
        return dest;
    }
    public static List<T> ConvertList<T>(object src)
    {
      var _src = (List<object>)src;
      var dest = new List<T>();
      foreach(var item in _src){
        var destItem = ConvertObject<T>(item);
        if(destItem != null){
          dest.Add(destItem);
        }
      }
      return dest;
    }
}
