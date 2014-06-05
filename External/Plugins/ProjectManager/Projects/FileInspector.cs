using System;
using System.Collections;
using System.IO;
using System.Text;
using ProjectManager.Helpers;

namespace ProjectManager.Projects
{
	public class FileInspector
	{
        public static string[] ExecutableFileTypes = null;

		public static bool IsLua(string path, string ext)
		{
			return ext == ".lua";
		}

        public static bool IsCss(string path, string ext)
        {
            return ext == ".css";
        }

        public static bool IsImage(string path, string ext)
		{
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif";
		}

        public static bool IsFont(string path, string ext)
		{
            return ext == ".ttf" || ext == ".otf";
        }

        public static bool IsSound(string path, string ext)
        {
            return ext == ".mp3";
        }

        public static bool IsResource(string path, string ext)
		{
            return IsImage(path, ext) || IsFont(path, ext) || IsSound(path, ext);
		}

		public static bool IsResource(ICollection paths)
		{
            foreach (string path in paths)
            {
                if (!IsResource(path, Path.GetExtension(path).ToLower())) return false;
            }
			return true;
		}

		public static bool ShouldUseShellExecute(string path)
		{
            string ext = Path.GetExtension(path).ToLower();
            if (ExecutableFileTypes != null)
            foreach (string type in ExecutableFileTypes)
            {
                if (type == ext) return true;
            }
			return false;
		}

		public static bool IsHtml(string path, string ext)
		{
            return ext == ".html" || ext == ".htm" || ext == ".mtt"/*haxe templo*/;
		}

		public static bool IsXml(string path, string ext)
		{
			// allow for mxml, sxml, asml, etc
            return (ext == ".xml" || (ext.Length == 5 && ext.EndsWith("ml")));
		}

		public static bool IsText(string path, string ext)
		{
            return ext == ".txt" || Path.GetFileName(path).StartsWith(".");
		}

        public static bool IsLuaProject(string path, string ext)
        {
            return ext == ".luaproj";
        }

        public static bool IsProject(string path)
        {
            return IsProject(path, Path.GetExtension(path).ToLower());
        }

        public static bool IsProject(string path, string ext)
        {
            return ProjectCreator.IsKnownProject(ext) || IsLuaProject(path, ext);
        }

        public static bool IsCustomProject(string path, string ext)
        {
            return (!IsLuaProject(path, ext)) && ProjectCreator.IsKnownProject(ext);
        }

        public static bool IsTemplate(string path, string ext)
        {
            return ext == ".template";
        }

    }

}
