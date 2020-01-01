# Accelbuffer - v1.2
`Accelbuffer` 是一个快速，高效的序列化系统，可以用于数据持久化、网络数据传输等。

## 运行时支持
* `.NET Framework 4.6+`
* `/unsafe`

## 特点
* 时间消耗低
* 托管堆内存分配接近于对象的真实大小，减少GC带来的性能问题
* 对于值类型，无装箱、拆箱
* 可以完全自定义的序列化流程
* 自动的运行时代理注入
* 可以运行时绑定的序列化代理
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
|方法|功能|
|:-:|:-:|
|`SerializeProxyInjector.AddBinding<TObject, TProxy>()`|代理绑定（`更加高效`）|
|`SerializeProxyInjector.AddBinding(Type objectType, Type proxyType)`|代理绑定|
|`SerializeProxyInjector.HasBinding<TObject>()`|获取是否存在绑定|
|`SerializeProxyInjector.HasBinding(Type objectType)`|获取是否存在绑定|

## 运行时代理注入
> 这种方法应该尽量在测试时使用，如果条件允许，应该尽可能实现序列化代理，这样可以进一步优化性能
* 生成代理的过程性能消耗非常大，如果使用该方案，
应该尽量在加载场景等位置进行代理初始化(调用`Serializer<T>.Initialize()`)

## 支持的序列化类型
* 基元类型（不包括 `decimal`）
* 一维数组，交错数组
* `List<T>`, `IList<T>`
* `Dictionary<TKey, TValue>`, `IDictionary<TKey, TValue>`
* `ISerializableEnumerable<T>`
* `ICollection<T>`
* 自定义 `class`, `struct`

##### Unity拓展 `AccelbufferUnityExtension`
> 添加类型支持
* `Vector2`, `Vector3`, `Vector4`, `Vector2Int`, `Vector3Int`, `Rect`, `RectInt`, `Quaternion`, `Matrix4x4`, `Color`, `Color32`,
`LayerMask`, `Bounds`, `BoundsInt`

## 基本用法
### 1.使用特性标记类型
#### 方案一，通过`运行时代理注入`
```csharp
[MemoryAllocator(20L, true, RuntimeReadOnly = true)]
public struct UserInput
{
  [FieldIndex(0), NumberType(Number.Var)]
  public int CarId;
  [FieldIndex(1), NumberType(Number.Var)]
  public float Horizontal;
  [FieldIndex(2), NumberType(Number.Var)]
  public float Vertical;
  [FieldIndex(3), NumberType(Number.Var)]
  public float HandBrake;
}
```

#### 方案二，手动实现代理
```csharp
[SerializeBy(typeof(UserInputSerializeProxy))]
[MemoryAllocator(20L, true, RuntimeReadOnly = true)]
public struct UserInput
{
  public int CarId;
  public float Horizontal;
  public float Vertical;
  public float HandBrake;
}

internal sealed class UserInputSerializeProxy : ISerializeProxy<UserInput>
{
  unsafe void ISerializeProxy<UserInput>.Serialize(in UserInput obj, in UnmanagedWriter* writer)
  {
    writer->WriteValue(0, obj.CarId, Number.Var);
    writer->WriteValue(1, obj.Horizontal, NumberOption.VariableLength);
    writer->WriteValue(2, obj.Vertical, Number.Var);
    writer->WriteValue(3, obj.HandBrake, Number.Var);
  }

  unsafe UserInput ISerializeProxy<UserInput>.Deserialize(in UnmanagedReader* reader)
  {
    return new UserInput
    {
      CarId = reader->ReadVariableInt32(0),
      Horizontal = reader->ReadVariableFloat32(1),
      Vertical = reader->ReadVariableFloat32(2),
      HandBrake = reader->ReadVariableFloat32(3)
    };
  }
}
```

### 2.序列化对象
```csharp
UserInput input = new UserInput { CarId = 1, Horizontal = 0, Vertical = 0, HandBrake = 0 };
byte[] data = Serializer<UserInput>.Serialize(input);
```

### 3.反序列化对象
```csharp
byte[] data = File.ReadAllBytes(someFile);
UserInput input = Serializer<UserInput>.Deserialize(data, 0, data.LongLength);
```

### 4.释放缓冲区内存
> 如果一个类型经常被序列化，可以选择不释放内存，以减少内存的频繁分配

##### 方法一、 释放指定一个类型的缓冲区内存
```csharp
Serializer<UserInput>.Allocator.FreeMemory();
```

##### 方法二、 释放所有序列化器分配的缓冲区内存
```csharp
UnmanagedMemoryAllocator.FreeAllMemory();
```

### 5.序列化事件回调(`SerializeMessage`)
通过定义公开的 `OnBeforeSerialize` 或 `OnAfterDeserialize` 实例方法，实现对序列化事件的关注

> 这个方法不会造成值类型的装箱

```c#
public struct UserInput
{
  [FieldIndex(0), NumberType(Number.Var)] 
  public int CarId;
  [FieldIndex(1), NumberType(Number.Var)] 
  public float Horizontal;
  [FieldIndex(2), NumberType(Number.Var)] 
  public float Vertical;
  [FieldIndex(3), NumberType(Number.Var)] 
  public float HandBrake;
  
  public void OnBeforeSerialize()
  {
    Console.WriteLine("OnBeforeSerialize");
  }
  
  public void OnAfterDeserialize()
  {
    Console.WriteLine("OnAfterDeserialize");
  }
}
```

## 性能对比
> 数据没有MonoJIT的影响，但可能存在允许范围内的误差

#### 测试类型

```C#
[Serializable, ProtoContract]
[MemoryAllocator(50L, true, RuntimeReadOnly = true)]
public struct SerializeTest
{
  [ProtoMember(1), FieldIndex(0), Encoding(CharEncoding.ASCII)] 
  public string String;
  [ProtoMember(2), FieldIndex(1), Encoding(CharEncoding.ASCII)] 
  public char Char;
  [ProtoMember(3), FieldIndex(2)]
  public byte Integer0;
  [ProtoMember(4), FieldIndex(3)]
  public sbyte Integer1;
  [ProtoMember(5), FieldIndex(4), NumberType(Number.Var)]
  public ushort Integer2;
  [ProtoMember(6), FieldIndex(5), NumberType(Number.Var)]
  public short Integer3;
  [ProtoMember(7), FieldIndex(6), NumberType(Number.Var)]
  public uint Integer4;
  [ProtoMember(8), FieldIndex(7), NumberType(Number.Var)]
  public int Integer5;
  [ProtoMember(9), FieldIndex(8), NumberType(Number.Var)]
  public ulong Integer6;
  [ProtoMember(10), FieldIndex(9), NumberType(Number.Var)]
  public long Integer7;
  [ProtoMember(11), FieldIndex(10), NumberType(Number.Var)]
  public float Float0;
  [ProtoMember(12), FieldIndex(11), NumberType(Number.Var)]
  public double Float1;
  [ProtoMember(13), FieldIndex(12)]
  public bool Bool;
}

SerializeTest test = new SerializeTest
{
  String = "hello world!",
  Char = 'A',
  Integer0 = 0,
  Integer1 = -1,
  Integer2 = 10,
  Integer3 = -10,
  Integer4 = 100,
  Integer5 = -100,
  Integer6 = 1000,
  Integer7 = -1000,
  Float0 = 10.666f,
  Float1 = 10.666,
  Bool = true
};
```

#### 对比
![performance_cmp1](README_RES/cmp3.png)
![performance_cmp2](README_RES/cmp1.png)
![performance_cmp3](README_RES/cmp2.png)

|序列化器名称|序列化 GC Alloc/字节|反序列化 GC Alloc/字节|序列化时间/纳秒|反序列化时间/纳秒|序列化文件大小/字节|
|:-:|:-:|:-:|:-:|:-:|:-:|
|Accelbuffer|180|130|3282|2028|68|
|Protobuf-net|614.4|512|5254|7345|83|
|UnityJsonSerializer|512|130|6308|9736|211|
|.NET BinarySerializer|6963.2|6656|74990|70290|311|
|.NET XmlSerializer|9113.6|17408|123171|155656|524|

## 支持
* 作者正在努力更新部分新的功能，这个序列化系统原本只是作者开发的Unity开源框架(在~~很久~~不久后也会开源)的一部分，由于没有对Unity的依赖而被单独分离，在这个序列化系统的大部分功能完善后，会继续着手开发Unity框架，同时不定期维护这个项目，更多细节可以参考源码。

* 作者联系方式 QQ：1024751595
