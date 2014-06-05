using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace ProjectManager.Projects
{
	public class ProjectWriter : XmlTextWriter
	{
		Project project;

		public ProjectWriter(Project project, string filename) : base(filename,Encoding.UTF8)
		{
			this.project = project;
			this.Formatting = Formatting.Indented;
		}

        protected Project Project { get { return project; } }

		public void WriteProject()
		{
			WriteStartDocument();
			WriteStartElement("project");
            WriteAttributeString("version", "1");
            OnAfterBeginProject();
			WriteHiddenPaths();
			WriteProjectOptions();
            WriteStorage();
            OnBeforeEndProject();
			WriteEndElement();
			WriteEndDocument();
		}

        protected virtual void OnAfterBeginProject() { }
        protected virtual void OnAfterWriteClasspaths() { }
        protected virtual void OnAfterWriteCompileTargets() { }
        protected virtual void OnBeforeEndProject() { }

		public void WriteHiddenPaths()
		{
			WriteComment(" Paths to exclude from the Project Explorer tree ");
			WriteStartElement("hiddenPaths");
			WritePaths(project.HiddenPaths,"hidden");
			WriteEndElement();			
		}

		public void WriteProjectOptions()
		{
			WriteComment(" Other project options ");
			WriteStartElement("options");
			WriteOption("showHiddenPaths",project.ShowHiddenPaths);
            WriteOption("command", project.StartupCommand);
            WriteOption("argument", project.StartupArgument);
            WriteOption("workingdir", project.WorkingDir);
            WriteOption("symbolpath", project.SymbolPath);
			WriteEndElement();
        }

        private void WriteStorage()
        {
            WriteComment(" Plugin storage ");
            WriteStartElement("storage");
            foreach (string key in project.storage.Keys)
            {
                string value = project.storage[key];
                if (value == null) continue;
                WriteStartElement("entry");
                WriteAttributeString("key", key);
                WriteCData(value);
                WriteEndElement();
            }
            WriteEndElement();
        }

		public void WriteOption(string optionName, object optionValue)
		{
			WriteOption("option",optionName,optionValue);
		}

		public void WriteOption(string nodeName, string optionName, object optionValue)
		{
			WriteStartElement(nodeName);
			WriteAttributeString(optionName,optionValue.ToString());
			WriteEndElement();
		}

		public void WritePaths(ICollection paths, string pathNodeName)
		{
			if (paths.Count > 0)
			{
				foreach (string path in paths)
				{
					WriteStartElement(pathNodeName);
					WriteAttributeString("path",path);
					WriteEndElement();
				}
			}
			else WriteExample(pathNodeName,"path");
		}

		public void WriteExample(string nodeName, params string[] attributes)
		{
			StringBuilder example = new StringBuilder();
			example.Append(" example: <"+nodeName);
			foreach (string attribute in attributes)
				example.Append(" " + attribute + "=\"...\"");
			example.Append(" /> ");
			WriteComment(example.ToString());
		}
	}
}
