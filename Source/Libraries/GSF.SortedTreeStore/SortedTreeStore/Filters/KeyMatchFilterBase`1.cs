﻿//******************************************************************************************************
//  KeyMatchFilterBase`1.cs - Gbtc
//
//  Copyright © 2013, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/9/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

using System;
using GSF.IO;

namespace GSF.SortedTreeStore.Filters
{
    /// <summary>
    /// Represents some kind of filter that does a match based on the key.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class KeyMatchFilterBase<TKey>
    {
        public long PointCount;

        /// <summary>
        /// The filter guid 
        /// </summary>
        public abstract Guid FilterType { get; }

        /// <summary>
        /// Loads a filter from the provided <see cref="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to load the filter from</param>
        /// <returns></returns>
        public abstract void Load(BinaryStreamBase stream);

        /// <summary>
        /// Serializes the filter to a stream
        /// </summary>
        /// <param name="stream">the stream to write to</param>
        public abstract void Save(BinaryStreamBase stream);

        /// <summary>
        /// Determines if a key is contained in the filter
        /// </summary>
        /// <param name="key">the key to check</param>
        /// <returns></returns>
        public abstract bool Contains(TKey key);


         /// <summary>
         /// The boundaries of the page.
         /// </summary>
         /// <param name="lowerBounds">the lower inclusive bounds of the page</param>
         /// <param name="upperBounds">the upper exclusive bounds of the page</param>
         /// <returns></returns>
         public virtual bool PageCannotContainPoints(TKey lowerBounds, TKey upperBounds)
         {
             return true;
         }
    }
}
