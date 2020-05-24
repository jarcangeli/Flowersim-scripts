using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITest<T>
{
    T value { get; set; }
    void SayHello();
    void SetValue(T t);
    T GetValue();
}

public class TestFloat : ITest<float>
{
    public float value { get; set; } = 0f;
    public void SayHello() { Debug.Log("Hello"); }
    public void SetValue(float val) { value = val; }
    public float GetValue() { return value; }
}
public class TestInt : ITest<int>
{
    public int value { get; set; } = 0;
    public void SayHello() { Debug.Log("Hello"); }
    public void SetValue(int val) { value = val; }
    public int GetValue() { return value; }
}

public class TestCasting : MonoBehaviour
{
    TestFloat f1 = new TestFloat();
    TestFloat f2 = new TestFloat();
    TestInt i1 = new TestInt();

    List<object> testList = new List<object>();
    List<Type> typeList = new List<Type>() 
        { typeof(TestFloat), typeof(TestFloat), typeof(TestInt) };

    /*
    // Start is called before the first frame update
    void Start()
    {
        testList.Add(f1);
        testList.Add(f2);
        testList.Add(i1);
        int i = 0;
        foreach (Type type in typeList)
        {
            object obj = testList[i];
            ITest test = obj as ITest;
            ++i;
        }
    }
    */
}
