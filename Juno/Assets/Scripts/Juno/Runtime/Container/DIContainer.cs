using System;
using System.Collections.Generic;

namespace Juno
{
    public class DIContainer
    {
        #region public
        #endregion // public

        #region private
        private Dictionary<Type, Dictionary<int, List<object>>> m_bindings;
        #endregion // private
    }
}