using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nehta.Common.Utils
{
  public static class ArgumentUtils
  {
    /// <summary>
    /// Throws an exception if an argument value is null.
    /// </summary>
    /// <param name="value">Value of the argument to check</param>
    /// <param name="name">Name of the argument</param>
    /// <exception cref="ArgumentException">Thrown when the argument is null</exception>
    public static void CheckNotNull(object value, string name)
    {
      if (value == null)
      {
        throw new ArgumentException(name + " cannot be null.");
      }
    }
    
    /// <summary>
    /// Throws an exception if a collection argument is null or is empty.
    /// </summary>
    /// <param name="collection">Collection to check</param>
    /// <param name="name">Name of the argument</param>
    /// <exception cref="ArgumentException">Thrown if the collection argument
    /// is null or does not contain at least one item.</exception>
    public static void CheckNotNullNorEmpty<T>(IList<T> collection, string name)
    {
      CheckNotNull(collection, name);
      if (collection.Count == 0)
      {
        throw new ArgumentException(name + " must contain at least one item.");
      }      
    }
    
    /// <summary>
    /// Throws an exception if a string argument is null, is an empty string
    /// or is made of whitespace characters only.
    /// </summary>
    /// <param name="value">String value to check</param>
    /// <param name="name">Name of the argument</param>
    /// <exception cref="ArgumentException">Thrown if the string argument
    /// is null, is empty or is made of whitespace characters only.</exception>
    public static void CheckNotNullNorBlank(string value, string name)
    {
      CheckNotNull(value, name);
      if ((value.Length == 0) || (value.Trim().Length == 0))
      {
        throw new ArgumentException(name + " cannot be a blank string.");
      }
    }
    
    /// <summary>
    /// Throws an exception if the argument is not null or contain any
    /// characters.
    /// </summary>
    /// <param name="value">String value to check</param>
    /// <param name="name">Name of the argument</param>
    public static void CheckNullOrBlank(string value, string name)
    {
      if ((value != null) && (value.Trim().Length > 0))
      {
        throw new ArgumentException(name + " cannot have a value.");
      }
    }
   
    /// <summary>
    /// Checks that the length of a string does not exceed the max length.
    /// </summary>
    /// <param name="value">Value of the string argument to check</param>
    /// <param name="maxLen">Maximum length of the string argument</param>
    /// <param name="name">Name of the string argument</param>
    public static void CheckMaxLength(string value, int maxLen, string name)
    {
      CheckNotNull(value, name);
      if (value.Length > maxLen)
      {
        throw new ArgumentException(name +
          " exceeds the maximum permitted length of " + maxLen +
          " characters.");
      }
    }
    
    /// <summary>
    /// Returns true if the collection is null or empty.
    /// </summary>
    /// <param name="collection">Collection to check</param>
    /// <returns>True when the collection is null or empty otherwise false</returns>
    public static bool IsNullOrEmpty<T>(IList<T> collection)
    {
      return ((collection == null) || (collection.Count == 0));
    }
    
    /// <summary>
    /// Returns true when the string null, empty or only has whitespace characters.
    /// </summary>
    /// <param name="value">String value to check</param>
    /// <returns>True when the string is null, is empty or only contains
    /// whitespace characters</returns>
    public static bool IsNullOrBlank(string value)
    {
      return ((value == null) || (value.Length == 0) || (value.Trim().Length == 0));
    }        
    
    /// <summary>
    /// Returns true when the value is larger than or equal to min and 
    /// smaller than or equal to max. Minimum must be smaller than
    /// maximum.
    /// </summary>
    /// <param name="val">Value to check</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    /// <returns></returns>
    public static bool IsInRange(int val, int min, int max)
    {
      if (min >= max)
      {
        throw new ArgumentException("Minimum '" + min + 
          "' is equal to or larger than maximum '" + max + "'");
      }
      
      return (val >= min && val <= max);
    }
    
    /// <summary>
    /// Checks if a value is within a range.
    /// </summary>
    /// <param name="val">Value to check</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    public static void CheckIsInRange(int val, int min, int max)
    {
      bool result = IsInRange(val, min, max);
      
      if (!result)
      {
        throw new ArgumentException("Value '" + val + 
          "' is not within the range '" + min + "-" + max + "'");
      }
    }
  }
}
