using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;

namespace Jack.Features
{

    public interface IBodyCreator
    {
        List<IBody2> CreateBodies(ISldWorks app, bool preview = false);

    }
}
