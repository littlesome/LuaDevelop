using System;
using Aga.Controls.Tree;
using PluginCore.Utilities;

namespace LuaDebugger.Controls
{
	public class DataNode : Node, IComparable<DataNode>
    {
        public override string Text
        {
            get
			{
				return base.Text;
			}
        }

		private int m_ChildrenShowLimit = 500;
		public int ChildrenShowLimit
		{
			get { return m_ChildrenShowLimit; }
			set { m_ChildrenShowLimit = value; }
		}

        private Variable m_Value;
		private bool m_bEditing = false;

		public int CompareTo(DataNode otherNode)
		{
			String thisName = Text;
			String otherName = otherNode.Text;
			if (thisName == otherName)
			{
				return 0;
			}
			if (thisName.Length>0 && thisName[0] == '_')
			{
				thisName = thisName.Substring(1);
			}
			if (otherName.Length>0 && otherName[0] == '_')
			{
				otherName = otherName.Substring(1);
			}
			int result = LogicalComparer.Compare(thisName, otherName);
			if (result != 0)
			{
				return result;
			}
			return m_Value.Name.Length > 0 && m_Value.Name.StartsWith("_") ? 1 : -1;
		}

		public string Value
        {
            get
			{
				if (m_Value == null)
				{
					return string.Empty;
				}
                
                return m_Value.ToString();
			}
			set
			{
				if (m_Value == null)
				{
					return;
				}
                throw new NotImplementedException();
			}
        }

		public Variable Variable
		{
			get
			{
				return m_Value;
			}
		}

        public override bool IsLeaf
        {
            get
            {
				if (m_Value == null)
				{
					return (this.Nodes.Count == 0);
				}

                return !m_Value.HasChild;
            }
        }

		public bool IsEditing
		{
			get
			{
				return m_bEditing;
			}
			set
			{
				m_bEditing = value;
			}
		}

        public DataNode(Variable value) : base(value.Name)
        {
            m_Value = value;
        }

		public DataNode(string value) : base(value)
		{
			m_Value = null;
		}

	}

}
