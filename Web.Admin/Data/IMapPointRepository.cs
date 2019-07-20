using System;
using System.Collections.Generic;
using Web.Admin.Models;

namespace Web.Admin.Data
{

    public interface IMapPointRepository
    {
        List<MapPoint> GetAll(bool includeHidden);
        MapPoint Get(int id);
        void AddNew(MapPoint mapPoint);
        int Update(MapPoint mapPoint);
    }
}
