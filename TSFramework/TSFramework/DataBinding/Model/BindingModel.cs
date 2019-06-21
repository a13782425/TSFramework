using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    public abstract class BindingModel : IBindingModel
    {

        public BindingModel()
        {
            FieldInfo[] fieldInfos= this.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        }
    }
}
