using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyWebServer
{
    /// <summary>
    /// A custom Mutex for the Navigation Plugin which returns returns if access was granted or not 
    /// </summary>
    public class SoftLock
    {
        private bool _LockStatus = false;
        private Mutex _InternalLock = new Mutex();
        private Mutex _ExternalLock = new Mutex();

        /// <summary>
        /// Constructor for the SoftLock
        /// </summary>
        public SoftLock() { }

        /// <summary>
        /// Checks if the Mutex is locked or not
        /// </summary>
        public bool IsLocked()
        {
            return _LockStatus;
        }

        /// <summary>
        /// Tries locking and returns if it was successful or not
        /// </summary>
        public bool TryWait()
        {
            _InternalLock.WaitOne();

            if (_LockStatus == false)
            {
                _LockStatus = true;
                _ExternalLock.WaitOne();
                _InternalLock.ReleaseMutex();
                return true;
            }

            _InternalLock.ReleaseMutex();
            return false;
        }

        /// <summary>
        /// Releases the Lock
        /// </summary>
        public void Release()
        {
            _InternalLock.WaitOne();
            _ExternalLock.ReleaseMutex();
            _LockStatus = false;
            _InternalLock.ReleaseMutex();
        }
    }
}
