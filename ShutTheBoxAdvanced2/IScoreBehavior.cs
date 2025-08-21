using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShutTheBoxAdvanced2
{
    public interface IScoreBehavior<T>
    {
        T DisplayScore(IBox box);
    }
}