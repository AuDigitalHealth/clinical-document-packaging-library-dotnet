/*
 * Copyright 2010 NEHTA
 *
 * Licensed under the NEHTA Open Source (Apache) License; you may not use this
 * file except in compliance with the License. A copy of the License is in the
 * 'license.txt' file, which should be provided with this work.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Runtime.Serialization;


namespace Nehta.Xsp
{ 
  /// <summary>
  /// Thrown when trying to match a certificate to an encrypted key.
  /// </summary>
  [Serializable]
  public class KeyMismatchException : Exception
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    public KeyMismatchException()
      : base()
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="errMsg"></param>
    public KeyMismatchException(string errMsg)
      : base(errMsg)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="errMsg"></param>
    /// <param name="innerEx"></param>
    public KeyMismatchException(string errMsg, Exception innerEx)
      : base(errMsg, innerEx)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected KeyMismatchException(SerializationInfo info, 
      StreamingContext context) : base(info, context)
    {   
    }    
  }
}
