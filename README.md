# Accelbuffer - v1.1 - preview
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
* 自动的C#代理代码生成 [`accelc`] (正在开发中)

## 部分功能
|功能名称|当前是否支持|
|:-:|:-:|
|字符编码设置(`ASCII`, `Unicode`, `UTF-8`)|支持|
|动态长度数字(`VariableNumber`)，固定长度整数(`FixedNumber`)|支持|
|序列化事件回调(`SerializeMessage`)|支持|
|序列化数据损坏检查(`StrictMode`)|支持|
|运行时代理注入(`RuntimeSerializeProxyInjection`)|支持|
|运行时代理绑定(`RuntimeSerializeProxyBinding`)|支持|
|Unity拓展(`AccelbufferUnityExtension`)|支持|
|C#代理脚本自动生成(`accelc`)|暂时不受支持|

## 运行时代理绑定`RuntimeSerializeProxyBinding`
|方法|功能|
|:-:|:-:|
|`SerializeProxyInjector.AddSerializeProxyBinding<TObject, TProxy>()`|代理绑定（`更加高效`）|
|`SerializeProxyInjector.AddSerializeProxyBinding(Type objectType, Type proxyType)`|代理绑定|
|`SerializeProxyInjector.RemoveSerializeProxyBinding<TObject>()`|取消代理绑定|
|`SerializeProxyInjector.RemoveSerializeProxyBinding(Type objectType)`|取消代理绑定|

## 运行时代理注入`RuntimeSerializeProxyInjection`
* 通过`System.Reflection.Emit`向运行时注入`IL`代码，生成默认的序列化代理，这个过程性能消耗非常大，如果使用该方案，
应该尽量在加载场景等位置进行代理初始化(调用`Serializer<T>.Initialize()`)

#### `RuntimeSerializeProxyInjection`支持的字段类型
* `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `string`, `float`, `double`, `bool` 

* `sbyte[]`, `byte[]`, `short[]`, `ushort[]`, `int[]`, `uint[]`, `long[]`, `ulong[]`, `char[]`, `string[]`, `float[]`, `double[]`, `bool[]` 

* 包括 （以上类型字段、标记了 `SerializeContractAttribute` 的任意类型字段 和 运行时绑定了代理的任意类型字段） 的 `class`, `struct`

* `List<T>`, `Dictionary<T>`（即将支持）

* 拥有公共 `Add(T value)` 方法的 `IEnumerable<T>` 类型（即将支持）

## 可以直接使用`Serializer<T>`进行序列化的类型
* `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `string`, `float`, `double`, `bool` 

* `sbyte[]`, `byte[]`, `short[]`, `ushort[]`, `int[]`, `uint[]`, `long[]`, `ulong[]`, `char[]`, `string[]`, `float[]`, `double[]`, `bool[]` 

* 定义了对应的`ISerializeProxy<T>`并且标记了 `SerializeContractAttribute` 指定代理的任意类型

* 运行时绑定了对应的 `ISerializeProxy<T>` 的任意类型

* 标记了`SerializeContractAttribute`拥有默认无参构造函数并且字段类型被`RuntimeSerializeProxyInjection`支持的任意类型

##### Unity拓展 `AccelbufferUnityExtension`
> 添加类型支持
* `Vector2`, `Vector3`, `Vector4`, `Vector2Int`, `Vector3Int`, `Rect`, `RectInt`, `Quaternion`, `Matrix4x4`, `Color`, `Color32`,
`LayerMask`, `Bounds`, `BoundsInt`

## 基本用法
### 1.使用特性标记类型
#### 方案一，通过`RuntimeSerializeProxyInjection`
```c#
[SerializeContract]
[MemoryAllocatorSettings(20L, true, RuntimeReadOnly = true)]
public struct UserInput
{
  [SerializeInclude(0), Number(NumberOption.VariableLength)] 
  public int CarId;
  [SerializeInclude(1), Number(NumberOption.VariableLength)] 
  public float Horizontal;
  [SerializeInclude(2), Number(NumberOption.VariableLength)] 
  public float Vertical;
  [SerializeInclude(3), Number(NumberOption.VariableLength)] 
  public float HandBrake;
}
```

#### 方案二，手动实现代理
```c#
[SerializeContract(typeof(UserInputSerializeProxy))]
[MemoryAllocatorSettings(20L, true, RuntimeReadOnly = true)]
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
    writer->WriteValue(0, obj.CarId, NumberOption.VariableLength);
    writer->WriteValue(1, obj.Horizontal, NumberOption.VariableLength);
    writer->WriteValue(2, obj.Vertical, NumberOption.VariableLength);
    writer->WriteValue(3, obj.HandBrake, NumberOption.VariableLength);
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

#### 方案三，利用C#代理脚本生成(`accelc`)
> 即将被支持

### 2.序列化对象
```c#
UserInput input = new UserInput { CarId = 1, Horizontal = 0, Vertical = 0, HandBrake = 0 };
byte[] data = Serializer<UserInput>.Serialize(input);
```

### 3.反序列化对象
```c#
byte[] data = File.ReadAllBytes(someFile);
UserInput input = Serializer<UserInput>.Deserialize(data, 0, data.LongLength);
```

### 4.释放缓冲区内存
> 如果一个类型经常被序列化，可以选择不释放内存，以减少内存的频繁分配

##### 方法一、 释放指定一个类型的缓冲区内存
```c#
Serializer<UserInput>.Allocator.FreeUsedMemory();
```

##### 方法二、 释放所有序列化器分配的缓冲区内存
```c#
UnmanagedMemoryAllocator.FreeAll();
```

### 5.序列化事件回调(`SerializeMessage`)
通过定义 `公开的(public) ` `OnBeforeSerialize` 或 `OnAfterDeserialize` `实例(instance)` 方法实现对序列化事件的关注

> 这个方法不会造成值类型的装箱！

```c#
[SerializeContract(InitialBufferSize = 20L, StrictMode = true)]
public struct UserInput
{
  [SerializeInclude(0), Number(NumberOption.VariableLength)] 
  public int CarId;
  [SerializeInclude(1), Number(NumberOption.VariableLength)] 
  public float Horizontal;
  [SerializeInclude(2), Number(NumberOption.VariableLength)] 
  public float Vertical;
  [SerializeInclude(3), Number(NumberOption.VariableLength)] 
  public float HandBrake;
  
  public void OnBeforeSerialize()
  {
    UnityEngine.Debug.Log("OnBeforeSerialize");
  }
  
  public void OnAfterDeserialize()
  {
    UnityEngine.Debug.Log("OnAfterDeserialize");
  }
}
```

## 性能对比
> 数据没有MonoJIT的影响，但可能存在允许范围内的误差

#### 测试类型

```C#
[Serializable, ProtoContract, SerializeContract(InitialBufferSize = 50L, StrictMode = true)]
public struct SerializeTest
{
  [ProtoMember(1), SerializeInclude(0), Encoding(CharEncoding.ASCII)] 
  public string String;
  [ProtoMember(2), SerializeInclude(1), Encoding(CharEncoding.ASCII)] 
  public char Char;
  [ProtoMember(3), SerializeInclude(2)]
  public byte Integer0;
  [ProtoMember(4), SerializeInclude(3)]
  public sbyte Integer1;
  [ProtoMember(5), SerializeInclude(4), Number(NumberOption.VariableLength)]
  public ushort Integer2;
  [ProtoMember(6), SerializeInclude(5), Number(NumberOption.VariableLength)]
  public short Integer3;
  [ProtoMember(7), SerializeInclude(6), Number(NumberOption.VariableLength)]
  public uint Integer4;
  [ProtoMember(8), SerializeInclude(7), Number(NumberOption.VariableLength)]
  public int Integer5;
  [ProtoMember(9), SerializeInclude(8), Number(NumberOption.VariableLength)]
  public ulong Integer6;
  [ProtoMember(10), SerializeInclude(9), Number(NumberOption.VariableLength)]
  public long Integer7;
  [ProtoMember(11), SerializeInclude(10), Number(NumberOption.VariableLength)]
  public float Float0;
  [ProtoMember(12), SerializeInclude(11), Number(NumberOption.VariableLength)]
  public double Float1;
  [ProtoMember(13), SerializeInclude(12)]
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
