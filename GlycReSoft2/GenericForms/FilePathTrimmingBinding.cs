using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlycReSoft.Generics.GenericForms
{
    public class FilePathTrimmingBinding : Binding
    {
        public FilePathTrimmingBinding(String propertyName, object dataSource, String dataMember)
            : base(propertyName, dataSource, dataMember, true)
        {
            this.Format += FilePathTrimmingBinding_Format;
        }

        void FilePathTrimmingBinding_Format(object sender, ConvertEventArgs e)
        {
            e.Value = (Path.GetFileName(e.Value as string));
        }
    }
}
