﻿//******************************************************************************************************
//  ISortedTreeKey`1.cs - Gbtc
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
//  11/1/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//     
//******************************************************************************************************

namespace openHistorian.Collections.Generic
{
    /// <summary>
    /// The interface that is required to use as a key in <see cref="SortedTree"/> 
    /// </summary>
    /// <typeparam name="TKey">A class that has a default constructor</typeparam>
    public interface ISortedTreeKey<TKey>
        where TKey : class, new()
    {
        /// <summary>
        /// Creates a class that contains the necessary methods for the SortedTree.
        /// </summary>
        /// <returns></returns>
        TreeKeyMethodsBase<TKey> CreateKeyMethods();
    }
}
