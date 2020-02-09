# Accelbuffer - v2.0
`Accelbuffer` 是一个快速，高效的序列化系统。

## 支持
* 需要.NET Framework 4.5+
* 提供unity类型支持和unitypackage（自动编译，模板代码，编辑器拓展，etc）
* 提供Sublime，VSCode的语法高亮文件
* 语言支持C#，Visual Basic.NET

## 特点
* 时间消耗低
* 托管堆内存分配少
* 完全避免装箱、拆箱
* 可以自定义序列化流程
* 没有序列化深度限制
* 可以控制序列化使用的字节序和字符编码

## 支持的序列化类型
* 基元类型，可空类型，枚举
* 一维数组，交错数组
* `List<T>`, `IList<T>`, `ICollection<T>`
* `Dictionary<TKey, TValue>`, `IDictionary<TKey, TValue>`
* `KeyValuePair<TKey, TValue>`
* `Intptr`, `UIntptr`, `VInt`, `VUInt`
* `Type`, `Guid`, `TimeSpan`, `DateTime`, `DateTimeOffset`
* `class`, `struct`

> Unity配置下，新增类型
* `Vector2`, `Vector3`, `Vector4`, `Vector2Int`, `Vector3Int`, `Quaternion`, `Color`, `Color32`

## 基本用法
> 以C#代码举例

### 1.使用特性标记类型
#### 方案一，在运行时自动生成代理类型
```csharp
namespace Accelbuffer.Test
{
    [MemorySize(10)]
    public struct Example
    {
        [FieldIndex(1)]
        public string Name;
        
        [FieldIndex(2)]
        public int Id;
    }
}
```

#### 方案二，手动实现代理
```csharp
namespace Accelbuffer.Test
{
    [MemorySize(10)]
    [SerializeBy(typeof(Example.ExampleSerializer))]
    public partial struct Example
    {
        [FieldIndex(1)]
        public string Name;
        
        [FieldIndex(2)]
        public int Id;
        
        public sealed class ExampleSerializer : ITypeSerializer<Example>
        {
            public void Serialize(Example obj, ref AccelWriter writer)
            {
                if ((obj.m_Name != null))
                {
                    writer.WriteValue(1, obj.Name);
                }
                writer.WriteValue(2, obj.Id);
            }
            
            public Example Deserialize(ref AccelReader reader)
            {
                Example result = new Example();

                while (reader.HasNext(out int index))
                {
                    switch (index)
                    {
                        case 1:
                            result.Name = reader.ReadString();
                            break;
                        case 2:
                            result.Id = reader.ReadInt32();
                            break;
                        default:
                            reader.SkipNext();
                            break;
                    }
                }

                return result;
            }
        }
    }
}
```

#### 方案三，使用`Accelbuffer Script`
> Accelbuffer Script语法高度重视类型结构的清晰与直观性，学习起来也很简单轻松。Accelbuffer Script 语法简化了 C#， Visual Basic.NET 的许多复杂操作，并提供强大功能。通常使用Accelbuffer Script可以节省许多时间

> 如下声明方法与上文代码等价，使用Accelbuffer Script编写
可以使类型的结构更加清晰，并且让类型的声明与目标语言无关

```csharp
package Accelbuffer.Test;

public struct Example about 10
{
    var Name : string;
    var Id : *int32;
}
```

命令行中输入
> asc -c -cs C:\Users\Administrator\Desktop\Example.accel

如果输出以下内容，则编译成功
> output：C:\Users\Administrator\Desktop\Example.cs

### 2.序列化对象
```csharp
Example example = new Example { ... };
NativeBuffer buffer = Serializer.Serialize<Example>(example);
//...
buffer.Dispose();
```

### 3.反序列化对象
```csharp
Example example = Serializer.Deserialize<Example>(buffer, 0, buffer.Length);
buffer.Dispose();
```

### 4.释放序列化缓冲区内存
可以通过调用下面的方法释放多余的内存或全部内存
```csharp
MemoryAllocator.Shared.Trim();
```

> MemoryAllocator是Accelbuffer实现的一个非托管内存管理器，是一个线程安全的单例类型

### 5.序列化事件回调
> 这个方法不会造成值类型的装箱，方法必须是公共的实例方法

```csharp
public class ...
{
    [OnBeforeSerialization]
    public void Before()
    {
        //在序列化前调用
        Console.WriteLine("Before Serialization");
    }

    [OnAfterDeserialization]
    public void After()
    {
        //在反序列化后调用
        Console.WriteLine("After Deserialization");
    }
}
```

## 使用BenchmarkDotNet的测试结果
* 结果可能存在误差
> BenchmarkDotNet=v0.12.0, OS=Windows 7 SP1 (6.1.7601.0)

> Intel Pentium CPU G4560T 2.90GHz, 1 CPU, 4 logical and 2 physical cores

> Frequency=2836025 Hz, Resolution=352.6062 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT  [AttachedDebugger]
  Job-SWSWQT : .NET Framework 4.7.2 (4.7.3324.0), X86 LegacyJIT

> Runtime=.NET 4.7.2  

|Method|Mean|Error|StdDev|StdErr|Allocated|
|:-:|:-:|:-:|:-:|:-:|:-:|
|AccelbufferSerialize|1,049.0 ns|8.39 ns|7.44 ns|1.99 ns|-|
|AccelbufferDeserialize|897.3 ns|6.39 ns|5.98 ns|1.54 ns|244 B|
|ProtobufSerialize|1,324.1 ns|11.47 ns|10.17 ns|2.72 ns|504 B|
|ProtobufDeserialize|2,550.1 ns|24.72 ns|23.12 ns|5.97 ns|748 B|

## Accelbuffer Script的保留关键字
|1|2|3|4|5|6|
|:-:|:-:|:-:|:-:|:-:|:-:|
|package|using|struct|about|var|obsolete|
|public|internal|private|protected|final|ref|
|boolean|int8|uint8|int16|uint16|int32|
|uint32|int64|uint64|float32|float64|float128|
|char|string|intptr|uintptr|vint|vuint|

* Unity下增加关键字

|1|2|3|4|
|:-:|:-:|:-:|:-:|
|vector2|vector3|vector4|vector2int|
|vector3int|quaternion|color|color32|

* 作者联系方式 QQ：1024751595