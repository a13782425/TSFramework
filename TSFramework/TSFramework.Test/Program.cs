using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame;
using TSFrame.MVVM;
using TSFrame.UI;


namespace TSFrame.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(1);
            list.Add(1);
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

            Panel p = new Panel();
            p.BindingContext.Bind("", null);
            p.BindingContext.SourceData = new TestModel();
            Console.ReadKey();
        }
    }

    public class TestModel : BindingModel
    {
        public BindableProperty<string> str = new BindableProperty<string>("str");
    }
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
