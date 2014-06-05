using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.Projects.Lua
{
    public class LuaProjectWriter : ProjectWriter
    {
        LuaProject project;

        public LuaProjectWriter(LuaProject project, string filename)
            : base(project, filename)
        {
            this.project = base.Project as LuaProject;
        }
    }
}
