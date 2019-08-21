using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame;
using TSFrame.MVVM;
using TSFrame.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TSFrame.Test
{
    class Program
    {
        static BindableProperty<string> str1 = new BindableProperty<string>();
        static void Main(string[] args)
        {
            Type enumType = typeof(UILayerEnum);
            string[] strs = Enum.GetNames(enumType);
            foreach (var item in strs)
            {
                UILayerEnum uILayerEnum = (UILayerEnum)Enum.Parse(enumType, item);
                Console.WriteLine(uILayerEnum);
            }
            //string str = "btn_aaa_";
            //int index = str.IndexOf("_");
            //index++;
            //string s = str.Substring(index, str.Length - index);
            //Console.WriteLine(s);
            //BindableList<int> list = new BindableList<int>();
            //int[] text = new int[] { 1, 2, 3 };
            //BindableList<int> list1 = new BindableList<int>(text);
            //foreach (var item in list1)
            //{
            //    Console.WriteLine(item);
            //}
            //Type type = typeof(TSFrame.MVVM.BindableProperty<>);
            //Type fieldType = str1.GetType();
            //Type[] types = type.GetInterfaces();
            //foreach (var item in types)
            //{
            //    Console.WriteLine(item.Name);
            //}
            //Console.WriteLine(types[0].IsAssignableFrom(fieldType));
            //List<int> list = new List<int>();
            //list.Add(1);
            //list.Add(1);
            //list.Add(1);
            //foreach (var item in list)
            //{
            //    Console.WriteLine(item);
            //}
            //UIButton uIButton = new UIButton(new Button());
            //a<UIButton,Button>(null,out uIButton);

            //Panel p = new Panel();
            //p.BindingContext.Bind("", null);
            //p.BindingContext.SourceData = new TestModel();
            Console.ReadKey();
        }
        //static void a<T1>(T1 uI, out UIElement<T1> t) where T1 : UIBehaviour 
        //{
        //    t = new UIElement<T1>(uI);
        //}
    }

    //public class TestModel : BindingModel
    //{
    //    public BindableProperty<string> str = new BindableProperty<string>("str");
    //}
    //public class TestModel : IBindingModel
    //{
    //    public BindableProperty<string> str = new BindableProperty<string>("str");
    //}
    //public partial class TestPanel : UIPanel<TestModel>
    //{
    //    public override string UIPath => "";
    //}
    //public class p : IBindingElement
    //{
    //    public event ValueChangedEvent ValueChanged;

    //    public void a()
    //    {
    //       // PropertyChanged.Invoke("a");
    //    }

    //    public void SetValue(object value)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    public partial class Panel : UIPanel
    {
        public override string UIPath => "";
    }

    //public partial class TestPanel
    //{
    //    protected override void OnCreate()
    //    {
    //        base.OnCreate();
    //        TestModel testModel = new TestModel();
    //        Dictionary<int, int> tempDic = new Dictionary<int, int>();
    //        for (int i = 0; i < 100000; i++)
    //        {
    //            testModel.str = new BindableProperty<string>();
    //            tempDic.Add(testModel.str.GetHashCode(), 1);
    //        }
    //        //Console.WriteLine(testModel.str.GetHashCode());
    //        //Console.WriteLine(testModel.str.GetHashCode());
    //        //this.BindingContext.Bind(testModel.str, null);
    //    }
    //}
}
