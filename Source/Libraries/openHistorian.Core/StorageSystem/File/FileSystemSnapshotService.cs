﻿//******************************************************************************************************
//  FileSystemSnapshotService.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
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
//  10/14/2011 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace openHistorian.Core.StorageSystem.File
{
    /// <summary>
    /// This class is responsible for managing the transactions that occur on the file system.
    /// Therefore, it keeps up with the latest snapshot of the file allocation table, 
    /// permits only a single concurrent edit of the archive system, and determines when a file
    /// can be deleted when there are no read or write transactions. It also containst the IO system.
    /// </summary>
    internal class FileSystemSnapshotService : IDisposable
    {
        #region [ Members ]

        /// <summary>
        /// Determines if this object has been disposed.
        /// </summary>
        private bool m_disposed;

        /// <summary>
        /// Constains the disk IO subsystem for accessing the file.
        /// </summary>
        DiskIOUnbuffered m_DiskIO;

        /// <summary>
        /// Contains the current snapshot of the file system.
        /// </summary>
        FileAllocationTable m_FileAllocationTable;

        /// <summary>
        /// This object will be used to synchronize access to aquire a transaction.
        /// </summary>
        object m_EditTransactionSynchronizationObject = new object();

        /// <summary>
        /// Contains the current active transaction.  If this null, there is no active transaction.
        /// </summary>
        TransactionalEdit m_CurrentTransaction;

        /// <summary>
        /// Contains all of the transactions that are currently reading.
        /// </summary>
        List<TransactionalRead> m_ReadTransactions;

        #endregion

        #region [ Constructors ]
        /// <summary>
        /// Opens an existing archive file system
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="isReadOnly">Determines if the file will be opened in read only mode.</param>
        private FileSystemSnapshotService(string fileName, bool isReadOnly)
        {
            m_DiskIO = DiskIOUnbuffered.OpenFile(fileName, isReadOnly);
            m_FileAllocationTable = FileAllocationTable.OpenHeader(m_DiskIO);
            m_ReadTransactions = new List<TransactionalRead>();
        }

        /// <summary>
        /// Creates a new archive file for editing
        /// </summary>
        /// <param name="fileName">the file name</param>
        private FileSystemSnapshotService(string fileName)
        {
            m_DiskIO = DiskIOUnbuffered.CreateFile(fileName);
            FileAllocationTable.CreateFileAllocationTable(m_DiskIO);
            m_FileAllocationTable = FileAllocationTable.OpenHeader(m_DiskIO);
            m_ReadTransactions = new List<TransactionalRead>();
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="FileSystemSnapshotService"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~FileSystemSnapshotService()
        {
            Dispose(false);
        }
        #endregion

        #region [ Properties ].
        #endregion
        #region [ Methods ]

        /// <summary>
        /// This will start a transactional edit on the file. 
        /// </summary>
        /// <param name="timeout">the amout of time in milliseconds to wait for a transaction before timing out and returning null.</param>
        /// <returns></returns>
        public TransactionalEdit BeginEditTransaction(int timeout = -1)
        {
            if (m_DiskIO.IsReadOnly)
                throw new Exception("File has been opened in readonly mode");
            if (Monitor.TryEnter(m_EditTransactionSynchronizationObject, timeout))
            {
                if (m_CurrentTransaction != null)
                    throw new Exception("A transaction has already been started");
                m_CurrentTransaction = new TransactionalEdit(m_DiskIO, m_FileAllocationTable);
                m_CurrentTransaction.HasBeenCommitted += new EventHandler(OnTransactionCommitted);
                m_CurrentTransaction.HasBeenRolledBack += new EventHandler(OnTransactionRolledBack);
                m_CurrentTransaction.HasBeenDisposed += new EventHandler(OnEditTransactionDisposed);
                return m_CurrentTransaction;
            }
            return null;
        }

        /// <summary>
        /// This will start a transactional read on the file. 
        /// </summary>
        /// <returns></returns>
        public TransactionalRead BeginReadTransaction()
        {
            TransactionalRead readTransaction = new TransactionalRead(m_DiskIO, m_FileAllocationTable);
            readTransaction.HasBeenDisposed += new EventHandler(OnReadTransactionDisposed);
            m_ReadTransactions.Add(readTransaction);
            return readTransaction;
        }

        void OnReadTransactionDisposed(object sender, EventArgs e)
        {
            m_ReadTransactions.Remove((TransactionalRead)sender);
        }

        void OnEditTransactionDisposed(object sender, EventArgs e)
        {
            if (m_CurrentTransaction != sender)
                throw new Exception("Only the current transaction can raise this.");
            m_CurrentTransaction = null;
            Monitor.Exit(m_EditTransactionSynchronizationObject);
        }

        void OnTransactionCommitted(object sender, EventArgs e)
        {
            if (m_CurrentTransaction != sender)
                throw new Exception("Only the current transaction can raise this.");
            m_FileAllocationTable = FileAllocationTable.OpenHeader(m_DiskIO);
        }

        void OnTransactionRolledBack(object sender, EventArgs e)
        {
            if (m_CurrentTransaction != sender)
                throw new Exception("Only the current transaction can raise this.");
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="FileSystemSnapshotService"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FileSystemSnapshotService"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    // This will be done regardless of whether the object is finalized or disposed.
                    if (m_DiskIO != null)
                        m_DiskIO.Dispose();
                    m_DiskIO = null;
                    if (disposing)
                    {
                        // This will be done only when the object is disposed by calling Dispose().
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }


        #endregion
        #region [ Static ]
        #region [ Methods ]

        /// <summary>
        /// Opens an existing file archive for unbuffered read/write operations.
        /// </summary>
        /// <param name="fileName">The path to the archive file.</param>
        /// <param name="IsReadOnly">Determines if the file is going to be opened in readonly mode.</param>
        /// <remarks>
        /// Since buffering the data will be the responsibility of another layer, 
        /// allowing the OS to buffer the data decreases performance (slightly) and
        /// wastes system memory that can be better used by the historian's buffer.
        /// </remarks>
        public static FileSystemSnapshotService OpenFile(string fileName, bool IsReadOnly)
        {
            return new FileSystemSnapshotService(fileName, IsReadOnly);
        }

        /// <summary>
        /// Creates a new archive file at the given path
        /// </summary>
        /// <param name="fileName">The file name of the archive to write.  This file must not already exist.</param>
        /// <returns></returns>
        public static FileSystemSnapshotService CreateFile(string fileName)
        {
            return new FileSystemSnapshotService(fileName);
        }

        #endregion
        #endregion
    }
}
