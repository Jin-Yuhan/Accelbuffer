# Accelbuffer - v1.3
`Accelbuffer` 是一个快速，高效的序列化系统，可以用于数据持久化、网络数据传输等。

## 运行时支持
`.NET Framework 4.6+`

## 特点
* 时间消耗低
* 托管堆内存分配接近于对象的真实大小
* 无装箱、拆箱
* 自定义序列化流程
* 运行时代理注入
* 运行时代理绑定
* 没有序列化深度限制

## 部分功能
|功能名称|当前是否支持|
|:-:|:-:|
|字符编码设置|支持|
|动态长度数字，固定长度整数|支持|
|自定义可序列化集合|支持|
|序列化事件回调|支持|
|序列化数据损坏检查|支持|
|运行时代理注入|支持|
|运行时代理绑定|支持|
|Unity拓展|支持|

## 运行时代理绑定
> 对于在其他程序集中实现的类型，既无法自动生成代理，又无法使用特性指定代理，通过 `Accelbuffer.Runtime.Injection.SerializeProxyInjector` 可以为这些类型指定代理，并且性能与使用特性指定几乎相同

## 运行时代理注入
> 生成代理的过程性能消耗非常大，这种方法应该尽量在测试时使用，如果条件允许，应该尽可能实现序列化代理，这样可以进一步优化性能

## 支持的序列化类型
* 基元类型（不包括 `decimal`）
* 一维数组，交错数组
* `List<T>`, `IList<T>`
* `Dictionary<TKey, TValue>`, `IDictionary<TKey, TValue>`
* `ISerializableEnumerable<T>`
* `ICollection<T>`
* 自定义 `class`, `struct`

##### Unity拓展 `AccelbufferUnityExtension`
`Vector2`, `Vector3`, `Vector4`, `Vector2Int`, `Vector3Int`, `Rect`, `RectInt`, `Quaternion`, `Matrix4x4`, `Color`, `Color32`,
`LayerMask`, `Bounds`, `BoundsInt`

> 新增以上类型的序列化支持

## 基本用法
### 1.使用特性标记类型
#### 方案一，自动注入代理
```csharp
public struct TestStruct
{
  [FieldIndex(0), NumberType(Number.Var)]
  public List<int> IntList;

  [FieldIndex(1), Encoding(CharEncoding.UTF8)]
  public Dictionary<string, string> StringMap;

  [FieldIndex(2), NumberType(Number.Var)]
  public float Float;
}
```

#### 方案二，手动实现代理
```csharp
[SerializeBy(typeof(TestStructSerializeProxy))]
public struct TestStruct
{
  public List<int> IntList;
  public Dictionary<string, string> StringMap;
  public float Float;
}

public sealed class TestStructSerializeProxy : ISerializeProxy<TestStruct>
{
  TestStruct ISerializeProxy<TestStruct>.Deserialize(ref UnmanagedReader reader, SerializationContext context)
  {
    return new TestStruct
    {
      IntList = Serializer<List<int>>.Deserialize(ref reader, context),
      StringMap = Serializer<Dictionary<string, string>>.Deserialize(ref reader, context),
      Float = reader.ReadFloat32(2, context.DefaultNumberType)
    };
  }

  void ISerializeProxy<TestStruct>.Serialize(TestStruct obj, ref UnmanagedWriter writer, SerializationContext context)
  {
    Serializer<List<int>>.Serialize(obj.IntList, ref writer, context);
    Serializer<Dictionary<string, string>>.Serialize(obj.StringMap, ref writer, context);
    writer.WriteValue(2, obj.Float, context.DefaultNumberType);
  }
}
```

### 2.序列化对象
```csharp
TestStruct test = new TestStruct { ... };
byte[] data = Serializer<TestStruct>.Serialize(test);
```

### 3.反序列化对象
```csharp
byte[] data = File.ReadAllBytes(someFile);
TestStruct test = Serializer<TestStruct>.Deserialize(data, 0, data.LongLength);
```

### 4.释放序列化缓冲区内存
> 如果内存正在被使用，会导致释放失败，但不会抛出异常
> 如果一个类型经常被序列化，可以选择不释放内存，以减少内存的频繁分配

##### 方法一、 释放指定一个类型的缓冲区内存
```csharp
Serializer<TestStruct>.Allocator.TryFreeMemory();
```

##### 方法二、 释放所有序列化器分配的缓冲区内存
```csharp
UnmanagedMemoryAllocator.FreeAllAvailableMemory();
```

### 5.序列化事件回调(`SerializeMessage`)
> 这个方法不会造成值类型的装箱，且可以存在多个回调方法，这些方法可以通过`SerializationCallbackAttribute.Priority`指定调用顺序

```csharp
public struct TestStruct
{
  [FieldIndex(0), NumberType(Number.Var)]
  public List<int> IntList;

  [FieldIndex(1), Encoding(CharEncoding.UTF8)]
  public Dictionary<string, string> StringMap;

  [FieldIndex(2), NumberType(Number.Var)]
  public float Float;

  [SerializationCallback(SerializationCallbackMethod.OnBeforeSerialize)]
  public void Before()
  {
    Console.WriteLine("Before Serialization");
  }

  [SerializationCallback(SerializationCallbackMethod.OnAfterDeserialize)]
  public void After()
  {
    Console.WriteLine("After Deserialization");
  }
}
```

## 更多优化选项
```csharp
/// <summary>
/// 设置初始的序列化缓冲区内存大小，合理设置这个值，可以大幅提升性能
/// </summary>
public sealed class InitialMemorySizeAttribute : Attribute
```

```csharp
/// <summary>
/// 指示对类型使用严格的序列化模式，这个特性会开启
/// 对序列化字段索引的严格匹配，不允许存在数据丢失的情况
/// </summary>
public sealed class StrictSerializationAttribute : Attribute
```


## 支持
* 作者正在努力更新部分新的功能，这个序列化系统原本只是作者开发的Unity开源框架(在~~很久~~不久后也会开源)的一部分，由于没有对Unity的依赖而被单独分离，在这个序列化系统的大部分功能完善后，会继续着手开发Unity框架，同时不定期维护这个项目，更多细节可以参考源码。

* 作者联系方式 QQ：1024751595
