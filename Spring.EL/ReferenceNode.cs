#region License

/*
 * Copyright ?2002-2011 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Linq;
using System.Runtime.Serialization;
using Spring.Core;
using Spring.Expressions;

namespace Spring.Context.Support
{
    

    /// <summary>
    /// Represents a reference to a Spring-managed object.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    [Serializable]
    public class ReferenceNode : BaseNode
    {
         
        /// <summary>
        /// Create a new instance
        /// </summary>
        public ReferenceNode():base()
        {
        }

        /// <summary>
        /// Create a new instance from SerializationInfo
        /// </summary>
        protected ReferenceNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
        /// <summary>
        /// Returns a value for the integer literal node.
        /// </summary>
        /// <param name="context">Context to evaluate expressions against.</param>
        /// <param name="evalContext">Current expression evaluation context.</param>
        /// <returns>Node's value.</returns>
        protected override object Get(object context, EvaluationContext evalContext)
        {
            if (!evalContext.Variables.TryGetValue("_sprint_context_resove_", out object Resove))
            {
                return null;
            }

            string objectName;
            if (this.getNumberOfChildren() == 2)
            {
                objectName = this.getFirstChild().getNextSibling().getText();
            }
            else
            {
                objectName = this.getFirstChild().getText();
            }

            SprintContextResove deResove = (SprintContextResove) Resove;

            var typeName = objectName.Split('@');

            if (typeName.Length > 2)
            {
                throw new TypeMismatchException($"`{objectName}` formatter invalid");
            }

            Type type = System.Type.GetType(typeName[0],false);

            if (type == null)
            {
                throw new TypeMismatchException($"`{objectName}` invalid ,can not parse `{typeName[0]}` to Csharp Type");
            }

            return deResove.Invoke(type, typeName.Length>1?typeName.Last():null);
        }
    }
}