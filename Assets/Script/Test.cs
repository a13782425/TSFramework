using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TSFrame.MVVM;
using TSFrame.UI;
using UnityEngine;
using UnityEngine.UI;

public class Model
{
    public readonly BindableProperty<int> Str = new BindableProperty<int>();
    public readonly BindableProperty<string> Str1 = new BindableProperty<string>();
}

public class Test : MonoBehaviour
{
    public InputField field;
    public Text text;

    Model model;
    // Start is called before the first frame update
    void Start()
    {
        model = new Model();
        UIInputField uIInput = new UIInputField(field);
        UIText uIText = new UIText(text);

        uIInput.OneWayToSource(model.Str1);
        uIText.OneWay(model.Str1);

    }
    float time = 0;
    // Update is called once per frame
    void Update()
    {
        //time += Time.deltaTime;
        //if (time > 1f)
        //{
        //    model.Str1.Value = Guid.NewGuid().ToString();
        //}
    }


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
