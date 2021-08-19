using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace StrokeForEgypt.BaseRepository
{
    public interface ILookUp<Entity>
    {
        Dictionary<string, string> GetLookUp(Expression<Func<Entity, bool>> expression);
    }
}
