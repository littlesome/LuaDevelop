using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.Projects.Lua
{
    public class LuaProjectReader : ProjectReader
    {
        LuaProject project;

        public LuaProjectReader(string filename)
            : base(filename, new LuaProject(filename))
        {
            this.project = base.Project as LuaProject;
        }

        public new LuaProject ReadProject()
        {
            return base.ReadProject() as LuaProject;
        }
    }
}
