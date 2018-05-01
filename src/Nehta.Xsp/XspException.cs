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
    /// Base Xsp exception.
    /// </summary>
    [Serializable]
    public class XspException : Exception
    {
        /// <summary>
        /// Create XspException
        /// </summary>
        public XspException()
            : base()
        {
        }

        /// <summary>
        /// Create XspException
        /// </summary>
        /// <param name="errMsg">Error message</param>
        public XspException(string errMsg)
            : base(errMsg)
        {
        }

        /// <summary>
        /// Create XspException
        /// </summary>
        /// <param name="errMsg">Error message</param>
        /// <param name="innerEx">Inner exception</param>
        public XspException(string errMsg, Exception innerEx)
            : base(errMsg, innerEx)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XspException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected XspException(SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        {
        }
    }
}
