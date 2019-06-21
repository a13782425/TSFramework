using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TSFrame.MVVM;
using TSFrame.UI;
using TSFrame;
using UnityEngine;
using UnityEngine.UI;

public class Model
{
    public readonly BindableProperty<int> Str = new BindableProperty<int>("Str");
    public readonly BindableProperty<string> Str1 = new BindableProperty<string>("Str1");
    public readonly BindableProperty<Sprite> Sp = new BindableProperty<Sprite>("Sp");
    public readonly BindableProperty<float> Float = new BindableProperty<float>("Float");
    public readonly BindableProperty<bool> B = new BindableProperty<bool>("B");
}

public class Model1 : ViewModelBase
{

    public readonly BindableProperty<string> Str = new BindableProperty<string>();
}


public class TestPanel : UIPanel
{
    public override string UIPath => "aaa";

    public Model model;

    public Image sprite;

    public UIImage uIImage;

    protected override void OnCreate()
    {
        base.OnCreate();
        sprite = GameObject.Find("Image").GetComponent<Image>();
        uIImage = new UIImage(sprite);
    }
    protected override void Binding()
    {
        base.Binding();
        model = new Model();
        this.BindingContext.Bind(model.Sp, uIImage);
        this.BindingContext.SourceData = model;
    }
}


public class Test : MonoBehaviour
{
    public Slider slider;
    public Text text;
    public Toggle toggle;
    public Toggle toggle2;
    Model model;
    public Sprite sprite;
    private TestPanel panel;
    public Test2 test2;
    private Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
        coroutine = test2.StartCoroutine(TestCoroutine());

        //panel = UIView.CreateView<TestPanel>();
        //model = new Model();
        //uIToggle = new UIToggle(toggle);
        //uIToggle2 = new UIToggle(toggle2);
        //uISlider = new UISlider(slider);
        //uIText = new UIText(text);

        //uIText.Binding(model.B);

        //model.B.Binding(uIToggle);
        //model.Float.Binding(uIText);
        //model.Float.Binding(uISlider, BindingMode.OneWayToSource);
        //model.B.Binding(uIToggle2, BindingMode.OneWayToSource);
        //UIInputField uIInput = new UIInputField(field);
        //UISlider uISlider = new UISlider(slider);
        //uIText = new UIText(text);
        ////uIImage = new UIImage(image);
        ////uIImage1 = new UIImage(image1);
        ////model.Sp.Binding(uIImage1);
        ////uIImage.Binding(model.Sp);
        //model.Float.Binding(uIText);
        //uISlider.Binding(model.Float);


        //model.Str1.Binding(uIText);
        //model.Str1.Binding(uIInput, BindingMode.OneWayToSource);
        //uIInput.OneWayToSource(model.Str1);
        //uIText.OneWay(model.Str1);

    }

    private IEnumerator TestCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Debug.LogError("dasd");
        }
    }
    //float time = 0;
    // Update is called once per frame
    void Update()
    {
        //time += Time.deltaTime;
        //if (time > 1f)
        //{
        //    model.Str1.Value = Guid.NewGuid().ToString();
        //}
        if (Input.GetKeyUp(KeyCode.A))
        {
            //panel.model.Sp.Value = sprite;
            test2.StopCoroutine(coroutine);
            //model.Sp.Value = sp;
        }
    }

    //private void OnGUI()
    //{
    //    if (Input.GetKeyUp(KeyCode.A))
    //    {
    //        model.Str.Unbinding(uIText);
    //    }
    //}

    void TestObj<T>(Expression<Func<T, object>> expression)
    {
        Expression body = expression.Body;
        switch (body)
        {
            case NewExpression newExpression:
                {
                    Debug.LogError(newExpression.Arguments.Count);
                    var type = newExpression.Constructor.DeclaringType;
                    var i = 0;
                    foreach (var item in newExpression.Arguments)
                    {
                        var aliasName = newExpression.Members[i].Name;
                        Debug.LogError(aliasName);
                        if (item is MemberExpression)
                        {
                            var member = (MemberExpression)item;
                        }
                        else
                        {
                            throw new Exception($"暂时不支持的{item.GetType().Name}写法！请使用经典写法！");
                            //var member = (MethodCallExpression)item;
                            //result[i] = ConvertFun(member, aliasName).FirstOrDefault();
                        }
                        i++;
                    }
                }
                break;
            case UnaryExpression unaryExpression:
                {
                    Expression ex = unaryExpression.Operand;
                    //ex.
                    Debug.LogError(ex.GetType());
                    //Debug.LogError(newExpression.Arguments.Count);  
                    //var type = newExpression.Constructor.DeclaringType;
                    //var i = 0;
                    //foreach (var item in newExpression.Arguments)
                    //{
                    //    var aliasName = newExpression.Members[i].Name;
                    //    if (item is MemberExpression)
                    //    {
                    //        var member = (MemberExpression)item;
                    //    }
                    //    else
                    //    {
                    //        throw new Exception($"暂时不支持的{item.GetType().Name}写法！请使用经典写法！");
                    //        //var member = (MethodCallExpression)item;
                    //        //result[i] = ConvertFun(member, aliasName).FirstOrDefault();
                    //    }
                    //    i++;
                    //}
                }
                break;
            default:
                throw new Exception("暂时不支持的Select lambda写法！请使用经典写法！");
        }
    }
}
